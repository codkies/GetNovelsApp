using System.Collections.Generic;

using GetNovelsApp.Core.Modelos;
using System.Threading.Tasks;
using System;

using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.Core.Empaquetadores;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.GetNovelsApp;
using System.Linq;

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
        private INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>> MyNovela;



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

        

        #region Props

        public int DocumentosCreados { get; private set; } = 0;

        public int EntradasIgnoradas { get; private set; } = 0;

        public long CaracteresVistos { get; private set; } = 0;

        public int CapitulosEncontrados { get; private set; } = 0;

        public int Skips { get; private set; } = 0;

        public string Nombre => "GetNovelsApp";


        private readonly Archivador Archivador;

        #endregion



        #region Queueing a novel


        #region queueing fields
        private Queue<INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>>> NovelasPorDescargar = new Queue<INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>>>();


        private bool Descargando = false; 


        /// <summary>
        /// Une el reporte con el ID de la novela.
        /// </summary>
        /// 
        private Dictionary<int, IProgress<IReporte>> ReportePorID = new Dictionary<int, IProgress<IReporte>>();
        #endregion


        public async Task<bool> AgregaAlQueue(INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>> novela, IProgress<IReporte> progreso)
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

            NovelasPorDescargar.Enqueue(novela);
            ReportePorID.Add(novela.ID, progreso);

            await RevisaSiQuedanNovelasPorDescargarAsync(); //Comienza el trigger de las descargas

            return true;
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
                       INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>> novelaADescargar = NovelasPorDescargar.Dequeue();
                       GetNovelAsync(novelaADescargar);
                   }
               });
        }


        #endregion
      

        #region Bajando los capitulos de novela.


        /// <summary>
        /// Obtiene capitulos de una novela en el formato establecido y los coloca en la carpeta de la configuracion.
        /// </summary>
        public async Task<INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>>> GetNovelAsync(
            INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>> novelaNueva, 
            int ComienzaEn = 0)
        {
            Descargando = true;

            if (novelaNueva.PorcentajeDescarga >= 100) //Si ya la novela está descargada...
            {
                Descargando = false;
                await RevisaSiQuedanNovelasPorDescargarAsync();
                return novelaNueva;
            }

            //Else, continua normalmente:

            PreparaNovelaNueva(novelaNueva);

            IProgress<IReporte> progresoDeNovela = ReportePorID[novelaNueva.ID];

            List<Task> Scrappers = new List<Task>();
            
            int tamañoBatch = GetNovelsConfig.TamañoBatch;
            
            foreach (var capitulo in DescargaEstosCapitulos)
            {
                Scrappers.Add(Task.Run(() => Scrap(capitulo, progresoDeNovela)));
                if(Scrappers.Count >= tamañoBatch) //Solo haz X tareas a la vez
                {
                    await Task.Run(async () =>
                    {
                        //Espera que las tareas acaben
                        await Task.WhenAll(Scrappers);
                        
                        //resetea                         
                        Scrappers.Clear();
                    });
                }
            }   

            if(Scrappers.Any()) await Task.WhenAll(Scrappers);

            Descargando = false;
            await NovelaFueDescargadAsync(novelaNueva);

            return MyNovela;
        }


        /// <summary>
        /// Hace lo necesario cuando una novela ha sido descargada.
        /// </summary>
        private async Task NovelaFueDescargadAsync(INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>> novelaNueva)
        {
            ReportePorID.Remove(novelaNueva.ID); //ya no es necesario mantener ref al reporte de una novela que ya no se le reportará nada.
            await RevisaSiQuedanNovelasPorDescargarAsync();
        }


        private void Scrap(Capitulo capituloPorDescargar, IProgress<IReporte> progress)
        {
            //1) baja capitulo
            Capitulo capituloDescargado = MyScraper.CompletaCapitulo(capituloPorDescargar);

            //2) guarda capitulo
            MyEmpaquetador.EmpaquetaCapitulo(capituloDescargado, MyNovela, progress);
        }



        #endregion



        #region Helpers


        /// <summary>
        /// Organiza los capitulos que se deben descargar y toma referencias necesarias.
        /// </summary>
        /// <param name="novelaNueva"></param>
        private void PreparaNovelaNueva(INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>> novelaNueva)
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
        private void RecolectaInformacion(INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>> novela, TiposDocumentos tipo)
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
