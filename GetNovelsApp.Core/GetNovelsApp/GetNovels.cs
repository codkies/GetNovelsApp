using System.Collections.Generic;

using GetNovelsApp.Core.Utilidades;
using GetNovelsApp.Core.Modelos;
using System.Diagnostics;
using System.Threading.Tasks;
using System;

using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.Core.ConfiguracionApp;
using GetNovelsApp.Core.Empaquetadores;
using GetNovelsApp.Core.Conexiones.DB;


/* Ideas:       
   - Agregar variabilidad en la fuente y en el tamaño
   - Uh, cómo colocar imagenes de portada?
*/

namespace GetNovelsApp.Core
{
    /// <summary>
    /// Script encargado de manejar el flujo de la libreria.
    /// </summary>
    public class GetNovels : IReportero
    {
        #region Constructores & Setup

        public GetNovels(IConfig ConfiguracionApp)
        {
            GetNovelsConfig.EstableceConfig(ConfiguracionApp);
            Archivador = new Archivador();
            MyScraper = new Scraper();
            MyEmpaquetador = new EmpaquetadorNovelas(Archivador);
            EventsManager.ImprimeNovela += RecolectaInformacion;
        }

        #endregion


        #region Manejo interno script

        /// <summary>
        /// Referencia a los capitulos por descargar.
        /// </summary>        
        List<Capitulo> DescargaEstosCapitulos = new List<Capitulo>();

        /// <summary>
        /// Sraper que esta instancia del GetNovels está usando.
        /// </summary>
        private Scraper MyScraper;


        /// <summary>
        /// Empaquetador que esta instancia del GetNovels está usando.
        /// </summary>
        private EmpaquetadorNovelas MyEmpaquetador;


        /// <summary>
        /// Novela que esta instancia del GetNovels está obteniendo.
        /// </summary>
        private NovelaRuntimeModel MyNovela;



        #endregion


        #region Publico


        #region Props

        public int DocumentosCreados { get; private set; } = 0;

        public int EntradasIgnoradas { get; private set; } = 0;

        public long CaracteresVistos { get; private set; } = 0;

        public int CapitulosEncontrados { get; private set; } = 0;

        public int Skips { get; private set; } = 0;

        public string Nombre => "GetNovelsApp";

        private readonly Archivador Archivador;

        #endregion
        

        /// <summary>
        /// Obtiene capitulos de una novela en el formato establecido y los coloca en la carpeta de la configuracion.
        /// </summary>
        public async Task<NovelaRuntimeModel> GetNovelAsync(NovelaRuntimeModel novelaNueva, int ComienzaEn)
        {
            int PorcentajeDeDescarga = novelaNueva.PorcentajeDescarga;
            
            if (PorcentajeDeDescarga == 100)
            {
                Comunicador.ReportaCambioEstado($"La novela {novelaNueva.Titulo} se encuentra en la base de datos.", this);
                return novelaNueva;
            }

            //Else, continua normalmente.

            PreparaNovelaNueva(novelaNueva);
            Comunicador.ReportaCambioEstado($"La novela {MyNovela.Titulo} tiene un porcentaje de descarga de {MyNovela.PorcentajeDescarga}%.", this);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            await ScrapMyNovelaAsync(ComienzaEn);

            stopwatch.Stop();

            //Reportando:            
            Comunicador.ReportaExito($"Se han finalizado todas las iteraciones. Tiempo tomado: {stopwatch.ElapsedMilliseconds / (60 * 1000)}min.", this);
            return MyNovela;
        }


        #endregion



        #region Scraping:


        private async Task ScrapMyNovelaAsync(int ComienzaEn)
        {
            //Preparaciones:
            Comunicador.ReportaEspecial($"{MyNovela.Titulo} tiene {MyNovela.CantidadLinks} links. Se empezará desde el link #{ComienzaEn}\n" +
                                                $"Se realizarán {DescargaEstosCapitulos.Count - ComienzaEn} iteraciones."
                                                , this);

            //------------------------------------------------------------------------------------------------------------------------------

            //Scraping:
            int tamañoBatch = GetNovelsConfig.TamañoBatch;
            int cantidadDeLinksAUtilizar = MyNovela.CantidadLinks - ComienzaEn;
            int iteraciones = DivideYRedondeaUp(cantidadDeLinksAUtilizar, tamañoBatch);

            Comunicador.Reporta("Comenzando Scrap\n", this);

            for (int i = 0; i < iteraciones; i++)
            {
                int factor = (i * tamañoBatch);
                int xi = ComienzaEn + factor;
                int xf = xi + tamañoBatch - 1;

                Comunicador.Reporta($"Batch {(i + 1)}/{iteraciones + 1}...", this);

                var capitulosCompletos = await ScrapCapitulosAsync(xi, xf);

                Comunicador.Reporta($"... Guardando capitulos...", this);

                MyEmpaquetador.EmpaquetaCapitulo(capitulosCompletos, MyNovela);

                Comunicador.Reporta($"... {((i + 1) * 100) / (iteraciones + 1)}% completado...", this);

                if (i != iteraciones - 1) //Solo espera si no eres el ultimo.
                {
                    int segundos = 5;
                    Comunicador.Reporta($"... Esperando {segundos}s\n", this);
                    System.Threading.Thread.Sleep(segundos * 1000);
                }
            }

            Comunicador.ReportaExito("\nFinalizados todos los batchs.", this);
        }        


        /// <summary>
        /// Probando hacer que el async no haga todos los caps a la vez, sino solo los que se le digan.
        /// </summary>
        /// <param name="xi">Donde comienza el Loop.</param>
        /// <param name="xf">Donde termina el Loop.</param>
        /// <returns></returns>
        private async Task<List<Capitulo>> ScrapCapitulosAsync(int xi, int xf)
        {
            //Scraping:
            List<Task<Capitulo>> tareas = new List<Task<Capitulo>>();
            for (int i = xi; i <= xf; i++)
            {
                Capitulo CapIncompleto = null;
                try
                {   
                    CapIncompleto = DescargaEstosCapitulos[i];
                    tareas.Add(Task.Run(() => MyScraper.CompletaCapitulo(CapIncompleto)));
                }
                catch (IndexOutOfRangeException)
                {
                    break;
                }
                catch (ArgumentOutOfRangeException)
                {
                    break;
                }
            }
            var resultados = await Task.WhenAll(tareas);

            //Ordenando los capitulos:
            List<Capitulo> CapitulosCompletos = new List<Capitulo>(resultados);
            CapitulosCompletos.Sort(new ComparerOrdenadorCapitulos());
            return CapitulosCompletos;

        }

        #endregion


        #region Helpers:


        /// <summary>
        /// Divide 2 int como decimales y redonda hacia arriba.
        /// </summary>
        /// <param name="num1"></param>
        /// <param name="num2"></param>
        /// <returns></returns>
        private int DivideYRedondeaUp(int num1, int num2)
        {
            decimal x = (decimal)num1 / num2;
            return (int)Math.Ceiling(x);
        }


        /// <summary>
        /// Organiza los capitulos que se deben descargar y toma referencias necesarias.
        /// </summary>
        /// <param name="novelaNueva"></param>
        private void PreparaNovelaNueva(NovelaRuntimeModel novelaNueva)
        {
            MyNovela = novelaNueva;

            DescargaEstosCapitulos = new List<Capitulo>();
            foreach (Capitulo capitulo in MyNovela.CapitulosPorDescargar)
            {
                DescargaEstosCapitulos.Add(capitulo);
            }
           
        }


        /// <summary>
        /// Obteniendo informacion de las iteraciones anteriores.
        /// </summary>
        private void RecolectaInformacion(NovelaRuntimeModel novela, TiposDocumentos tipo)
        {
            if (MyEmpaquetador == null | MyScraper == null) return;
            DocumentosCreados += MyEmpaquetador.DocumentosCreados;
            EntradasIgnoradas += MyScraper.EntradasIgnoradas;
            CaracteresVistos += MyScraper.CaracteresVistos;
            CapitulosEncontrados += MyScraper.CapitulosEncontrados;
        }

        #endregion
    }
}
