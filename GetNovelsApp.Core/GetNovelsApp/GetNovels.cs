using System.Collections.Generic;

using GetNovelsApp.Core.Utilidades;
using GetNovelsApp.Core.Modelos;
using System.Diagnostics;
using System.Threading.Tasks;
using System;

using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.Core.Empaquetadores;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.GetNovelsApp;
using System.Linq;
using Testing;


namespace GetNovelsApp.Core
{
    /// <summary>
    /// Script encargado de manejar el flujo de la libreria.
    /// </summary>
    public class GetNovels : IReportero
    {
        #region Constantish

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
        private INovela MyNovela;



        #endregion


        #region Constructores & Setup

        public GetNovels(IFabrica Fabrica, IComunicador comunicador)
        {
            GetNovelsFactory.InicializaFabrica(Fabrica);
            GetNovelsComunicador.InicializaComunicador(comunicador);
            GetNovelsConfig.InicializaConfig();


            Archivador = new Archivador();
            MyScraper = new Scraper();
            MyEmpaquetador = new EmpaquetadorNovelas(Archivador);
            GetNovelsEvents.ImprimeNovela += RecolectaInformacion;
        }

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



        #region New

        private Queue<INovela> NovelasPorDescargar = new Queue<INovela>();
        private bool Descargando = false;

        /// <summary>
        /// Une el reporte con el ID de la novela.
        /// </summary>
        private Dictionary<int, IProgress<Descarga>> ReportePorID = new Dictionary<int, IProgress<Descarga>>();

        public async Task<bool> AgregaAlQueue(INovela novela, IProgress<Descarga> reporte)
        {
            #region Checks
            //no puede estar en la DB al 100%
            if (novela.EstoyCompleta)
            {
                GetNovelsComunicador.ReportaError("La novela ya se encuentra 100% descargada.", this);
                return false;
            }
            //no puede estar ya en el queue
            if (NovelasPorDescargar.Contains(novela))
            {
                GetNovelsComunicador.ReportaError("La novela ya se encuentra en el queue.", this);
                return false;
            }
            #endregion

            
            RegistraNovela(novela, reporte);

            await RevisaSiQuedanNovelasPorDescargarAsync();

            return true;
        }

        /// <summary>
        /// Registra una novela para ser descargada cuando se pueda.
        /// </summary>
        private void RegistraNovela(INovela novela, IProgress<Descarga> reporte)
        {
            NovelasPorDescargar.Enqueue(novela);            
            ReportePorID.Add(novela.ID, reporte);
        }

        /// <summary>
        /// Hace lo necesario cuando una novela ha sido descargada.
        /// </summary>
        private async Task NovelaDescargadaAsync(INovela novelaNueva)
        {
            ReportePorID.Remove(novelaNueva.ID); //ya no es necesario mantener ref al reporte de una novela que ya no se le reportará nada.
            await RevisaSiQuedanNovelasPorDescargarAsync();
        }

        /// <summary>
        /// Revisa si hay alguna novela en el Queue. Si la hay, comienza la descarga.
        /// </summary>
        /// <returns></returns>
        private async Task RevisaSiQuedanNovelasPorDescargarAsync()
        {
            //Espera a que se tome la decision, no a que se descargue la novela... I hope so
           await Task.Run(  
               ()=> {
                   if (Descargando == false & NovelasPorDescargar.Any())
                   {
                       //no la espero porque solo esperamos por el check, no por toda la descarga.
                       INovela novelaADescargar = NovelasPorDescargar.Dequeue();
                       GetNovelAsync(novelaADescargar);
                   }
               });
        }


        #endregion


        /// <summary>
        /// Obtiene capitulos de una novela en el formato establecido y los coloca en la carpeta de la configuracion.
        /// </summary>
        public async Task<INovela> GetNovelAsync(INovela novelaNueva, int ComienzaEn = 0)
        {
            Descargando = true;

            int PorcentajeDeDescarga = novelaNueva.PorcentajeDescarga;

            if (PorcentajeDeDescarga == 100)
            {
                GetNovelsComunicador.ReportaCambioEstado($"La novela {novelaNueva.Titulo} se encuentra en la base de datos.", this);
                RevisaSiQuedanNovelasPorDescargarAsync();
                return novelaNueva;
            }

            //Else, continua normalmente.

            PreparaNovelaNueva(novelaNueva);
            GetNovelsComunicador.ReportaCambioEstado($"La novela {MyNovela.Titulo} tiene un porcentaje de descarga de {MyNovela.PorcentajeDescarga}%.", this);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            IProgress<Descarga> Reporte = ReportePorID[novelaNueva.ID];
            await ScrapMyNovelaAsync(ComienzaEn, Reporte);

            stopwatch.Stop();

            //Reportando:            
            GetNovelsComunicador.ReportaExito($"Se han finalizado todas las iteraciones. Tiempo tomado: {stopwatch.ElapsedMilliseconds / (60 * 1000)}min.", this);

            Descargando = false;
            await NovelaDescargadaAsync(novelaNueva);

            return MyNovela;
        }



        #endregion


        #region Scraping:


        private async Task ScrapMyNovelaAsync(int ComienzaEn, IProgress<Descarga> reporte)
        {
            //Preparaciones:
            int tamañoBatch = GetNovelsConfig.TamañoBatch;
            int cantidadDeLinksAUtilizar = MyNovela.CapitulosPorDescargar.Count - ComienzaEn;
            int batches = DivideYRedondeaUp(cantidadDeLinksAUtilizar, tamañoBatch);

            GetNovelsComunicador.ReportaEspecial($"{MyNovela.Titulo} tiene {MyNovela.CapitulosPorDescargar.Count} capitulos por descargar. Se empezará desde: {MyNovela.CapitulosPorDescargar[ComienzaEn].TituloCapitulo}\n" +
                                                $"Se realizarán {batches+1} iteraciones."
                                                , this);

            //------------------------------------------------------------------------------------------------------------------------------

            GetNovelsComunicador.Reporta("Comenzando Scrap\n", this);

            for (int i = 0; i < batches; i++)
            {
                int factor = (i * tamañoBatch);
                int xi = ComienzaEn + factor;
                int xf = xi + tamañoBatch - 1;

                GetNovelsComunicador.Reporta($"Batch {(i + 1)}/{batches + 1}...", this);

                var capitulosCompletos = await ScrapCapitulosAsync(xi, xf);
                

                GetNovelsComunicador.Reporta($"... Guardando capitulos...", this);

                

                Task.Run(() => MyEmpaquetador.EmpaquetaCapitulo(capitulosCompletos, MyNovela, reporte));

                GetNovelsComunicador.Reporta($"... {((i + 1) * 100) / (batches + 1)}% de las iteraciones completadas...", this);

                //if (i != batches - 1) //Solo espera si no eres el ultimo.
                //{
                //    int segundos = 5;
                //    Comunicador.Reporta($"... Esperando {segundos}s\n", this);
                //    System.Threading.Thread.Sleep(segundos * 1000);
                //}
            }

            GetNovelsComunicador.ReportaExito("\nFinalizados todos los batchs.", this);
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
                    tareas.Add(Task.Run(()=> MyScraper.CompletaCapitulo(CapIncompleto)));

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
        private void PreparaNovelaNueva(INovela novelaNueva)
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
        private void RecolectaInformacion(INovela novela, TiposDocumentos tipo)
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
