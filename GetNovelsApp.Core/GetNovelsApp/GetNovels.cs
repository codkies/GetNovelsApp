using System.Collections.Generic;

using GetNovelsApp.Core.Utilidades;
using GetNovelsApp.Core.Modelos;
using System.Diagnostics;
using System.Threading.Tasks;
using System;

using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.Core.ConfiguracionApp;
using GetNovelsApp.Core.Empaquetadores;
using GetNovelsApp.Core.GetNovelsApp.Empaquetador.BaseDatos;


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
        }

        #endregion


        #region Manejo interno script

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
        private Novela MyNovela;


        /// <summary>
        /// Ultimo capitulo segun la novela.
        /// </summary>
        private float MiUltimoCapitulo => MyNovela.UltimoNumeroCapitulo;


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
        public async Task<Novela> GetNovelAsync(InformacionNovela infoNovela, int ComienzaEn)
        {
            //Revisado si la novela ya está en la base de datos.
            Novela novelaNueva = Archivador.GuardaInfoNovela(infoNovela);

            //Actualizando referencias
            PreparaNovelaNueva(novelaNueva);
            //------------------------------------------

            //Preparaciones:
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Comunicador.ReportaEspecial($"{MyNovela.Titulo} tiene {MiUltimoCapitulo} capitulos. Se empezará desde el link #{ComienzaEn}\n" +
                                                $"Se realizarán {MyNovela.LinksDeCapitulos.Count - ComienzaEn} iteraciones."
                                                , this);
            //------------------------------------------------------------------------------------------------------------------------------

            //Actualiza novela en DB cada vez que se consigue 1 batch.

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

                var resultados = await ScrapCapitulosAsync(xi, xf);

                Comunicador.Reporta($"... Guardando capitulos...", this);

                foreach (Capitulo cap in resultados)
                {
                    MyEmpaquetador.AgregaCapitulo(cap, novelaNueva);
                }

                Comunicador.Reporta($"... {((i + 1) * 100) /(iteraciones + 1)}% completado...", this);

                if (i != iteraciones - 1) //Solo espera si no eres el ultimo.
                {
                    int segundos = 5;
                    Comunicador.Reporta($"... Esperando {segundos}s\n", this);
                    System.Threading.Thread.Sleep(segundos * 1000);
                }
            }

            Comunicador.ReportaExito("\nFinalizados todos los batchs.", this);            
            
            //Reportando:            
            stopwatch.Stop();
            Comunicador.ReportaExito($"Se han finalizado todas las iteraciones. Tiempo tomado: {stopwatch.ElapsedMilliseconds / (60 * 1000)}min.", this);
            RecolectaInformacion();
            return novelaNueva;
        }


        #endregion


        #region Privados


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
                Uri Link = null;
                try
                {   
                    Link = MyNovela.LinksDeCapitulos[i];
                    tareas.Add(Task.Run(() => MyScraper.ObtenCapitulo(Link)));
                }
                catch (IndexOutOfRangeException)
                {
                    break;
                }
                catch (ArgumentOutOfRangeException)
                {
                    break;
                }
                //tareas.Add(Task.Run(() => ScraperActual.ObtenCapitulo(Link)));
            }
            var resultados = await Task.WhenAll(tareas);

            //Ordenando los capitulos:
            List<Capitulo> caps = new List<Capitulo>(resultados);
            caps.Sort(new ComparerOrdenadorCapitulos());
            return caps;

        }


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
        /// Crea scraper y empaquetador para la novela nueva.
        /// </summary>
        /// <param name="novelaNueva"></param>
        private void PreparaNovelaNueva(Novela novelaNueva)
        {
            MyNovela = novelaNueva;

            MyScraper = new Scraper();
            MyEmpaquetador = new EmpaquetadorNovelas(Archivador);
        }


        /// <summary>
        /// Obteniendo informacion de las iteraciones anteriores.
        /// </summary>
        private void RecolectaInformacion()
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
