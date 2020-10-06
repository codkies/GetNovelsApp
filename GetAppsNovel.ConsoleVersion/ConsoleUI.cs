using System;
using System.Collections.Generic;
using System.Diagnostics;
using GetNovelsApp.Core;
using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Conexiones.DB;
using System.IO;
using System.Runtime.InteropServices;
using GetNovelsApp.Core.Conexiones.Internet;
using GetNovelsApp.Core.GetNovelsApp;
using GetNovelsApp.Core.ConfiguracionApp;

namespace GetAppsNovel.ConsoleVersion
{
    /// <summary>
    /// UI de consola.
    /// </summary>
    public class ConsoleUI : IComunicador, IReportero
    {
        public ConsoleUI()
        {
           
        }

        public string Nombre => "Programa";


        #region Interfaz Comunicador

        #region Colores

        private const ConsoleColor ColorError = ConsoleColor.Red;
        private const ConsoleColor ColorReportero = ConsoleColor.Magenta;
        private const ConsoleColor ColorFormulario = ConsoleColor.Cyan;

        private const ConsoleColor ColorNotificacion = ConsoleColor.DarkYellow;
        private const ConsoleColor ColorCambioEstado = ConsoleColor.Yellow;

        private const ConsoleColor ColorEspecial = ConsoleColor.White;
        private const ConsoleColor ColorExito = ConsoleColor.DarkCyan;


        #endregion


        IReportero reporteroActual = null;

        private void EscribeReportero(IReportero reportero)
        {
            Console.ForegroundColor = ColorReportero;
            if (reporteroActual != reportero)
            {
                Console.WriteLine($"\n-----------> {reportero.Nombre} says: <-----------\n");
                reporteroActual = reportero;
            }
        }

        public string PideInput(string enunciado, IReportero reportero)
        {
            EscribeReportero(reportero);
            Console.ForegroundColor = ColorFormulario;
            EscribeEnunciado(enunciado);
            Console.ForegroundColor = ColorEspecial;
            return Console.ReadLine();
        }

        private static void EscribeEnunciado(string enunciado)
        {
            Console.WriteLine(enunciado);
        }

        public void Reporta(string enunciado, IReportero reportero)
        {
            EscribeReportero(reportero);
            Console.ForegroundColor = ColorNotificacion;
            EscribeEnunciado(enunciado);
        }

        public void ReportaCambioEstado(string enunciado, IReportero reportero)
        {
            EscribeReportero(reportero);
            Console.ForegroundColor = ColorCambioEstado;
            EscribeEnunciado(enunciado);
        }

        public void ReportaError(string enunciado, IReportero reportero)
        {
            EscribeReportero(reportero);
            Console.ForegroundColor = ColorError;
            EscribeEnunciado(enunciado);
        }

        public void ReportaErrorMayor(string enunciado, IReportero reportero)
        {
            enunciado += " Presiona enter para cerrar el programa.";
            ReportaError(enunciado, reportero);
            Console.ReadLine();
            Environment.Exit(0);
        }

        public void ReportaEspecial(string enunciado, IReportero reportero)
        {
            EscribeReportero(reportero);
            Console.ForegroundColor = ColorEspecial;
            EscribeEnunciado(enunciado);
        }

        public void ReportaExito(string enunciado, IReportero reportero)
        {
            EscribeReportero(reportero);
            Console.ForegroundColor = ColorExito;
            EscribeEnunciado(enunciado);
        }

        #endregion



        #region Reportero (Llamado desde programa.cs)

        #region Interno


        /// <summary>
        /// Le pregunta al usuario por campos a llenar referente a la manera en que la app funcionará.
        /// </summary>
        /// <returns></returns>
        internal IConfig PideConfiguracion()
        {
            string Direccion = string.Empty;
            int BatchSize = 25;
            string _CapsPorDoc = string.Empty;

            //Obteniendo dir
            while (Direccion == string.Empty)
            {
                string dir = PideInput("Directorio donde se guardaran las novelas:", this);
                Reporta($"Has escrito {dir}", this);
                string decision = PideInput("Escribe (Y) para confirmar.", this).ToLower();
                if (decision.Equals("y") | decision.Equals("yes"))
                {
                    dir = dir.Replace(@"\\", @"\\\\");
                    Direccion = dir;
                }
            }

            //Obteniendo tamaño cap
            while (_CapsPorDoc == string.Empty)
            {
                string caps = PideInput("¿Cuantos capitulos quieres que hayan por documento?", this);
                Reporta($"Has escrito {caps}", this);

                if (!int.TryParse(caps, out _))
                {
                    ReportaError("Tiene que ser numero.", this);
                    continue;
                }

                string decision = PideInput("Escribe (Y) para confirmar.", this).ToLower();
                if (decision.Equals("yes") | decision.Equals("y"))
                {
                    _CapsPorDoc = caps;
                }
            }

            int CapsPorDoc = int.Parse(_CapsPorDoc);
            return new ConfiguracionBasica(BatchSize, CapsPorDoc, Direccion);
        }


        internal Dictionary<INovela, int> PidePathTXTusuario(string folderPath)
        {
            Dictionary<INovela, int> InfoDescarga = new Dictionary<INovela, int>();

            //string path = PideInput($"Introduce path del txt file", this);
            //path = path.Replace(@"\\", @"\\\\");
            //path += ".txt";

            string path = @"C:\Users\Juan\Desktop\LINKS.txt";

            var lineas = File.ReadAllLines(path);
            List<Uri> Links = new List<Uri>();

            foreach (string linea in lineas)
            {
                bool linkValido = ValidaLink(linea, out Uri UriNovela);

                while (linkValido == false)
                {
                    ReportaError($"Link: ({linea}) no valido.", this);
                    string reemplazo = PideInput("Ingrese el link:", this);
                    linkValido = ValidaLink(reemplazo, out Uri u);
                    UriNovela = u;
                }

                Links.Add(UriNovela);
            }
            
             Archivador archivador = new Archivador();

            //1) Pidela a la DB:                
            foreach (Uri uri in Links)
            {
                Reporta($"\nObteniendo información de ({uri})...", this);
                INovela novela = archivador.Legacy_MeteNovelaDB(uri, out _);
                ///Confirmando con el usuario:
                ConfirmaInfoNovelaConUsuario(ref InfoDescarga, novela, folderPath);
               
            }
            TerminaInput(new List<INovela>(InfoDescarga.Keys));
            return InfoDescarga;
        }



        /// <summary>
        /// Crea un formulario para que el usuario meta la informacion a buscar.
        /// </summary>
        /// <param name="xPaths"></param>
        /// <param name="Novelas"></param>
        internal Dictionary<INovela, int> PideInfoUsuario(string FolderPathDefined)
        {
            //Preps:
            Dictionary<INovela, int> InfoDescarga = new Dictionary<INovela, int>();
            bool InputFinalizado = false;
            Archivador archivador = new Archivador();
            ReportaEspecial("\nInformacion de novelas:", this);

            while (!InputFinalizado)
            {
                //Validando link:
                string LinkNovela = PideInput("Link de la página principal de la novela:", this);
                bool linkValido = ValidaLink(LinkNovela, out Uri UriNovela);

                while (linkValido == false)
                {
                    ReportaError("Link no valido. Requiere Https.", this);
                    LinkNovela = PideInput("Link de la página principal de la novelas:", this);
                    linkValido = ValidaLink(LinkNovela, out Uri u);
                    UriNovela = u;
                }

                //Obteniendo información de novela:
                Reporta("\nObteniendo información de novela...\n", this);

                //1) Pidela a la DB:                
                INovela novela = archivador.Legacy_MeteNovelaDB(UriNovela, out _);

                ///Confirmando con el usuario:
                ConfirmaInfoNovelaConUsuario(ref InfoDescarga, novela, FolderPathDefined);

                //Pregunta por otra novela:
                InputFinalizado = PreguntaPorOtraNovela(InputFinalizado);

                if (!InputFinalizado) continue;
                List<INovela> TodasLasNovelasAObtener = new List<INovela>(InfoDescarga.Keys);
                TerminaInput(TodasLasNovelasAObtener);
            }

            return InfoDescarga;
        }


        internal string PreguntaSiSeImprime(INovela novela)
        {
            return PideInput($"Imprimir \"{novela.Titulo}\"? (Y/N)", this);
        }


        internal void MustraResultado(GetNovels ejecutor, Stopwatch stopwatch)
        {
            TimeSpan ts = stopwatch.Elapsed;
            string report = ts.ToString("hh\\:mm\\:ss\\.ff");

            ReportaEspecial($"Finalizado el proceso en {report}. Resumen: \n" +
                        $"  Se han creado {ejecutor.DocumentosCreados} documentos.\n" +
                        $"  Se han ignorado {ejecutor.EntradasIgnoradas} entradas.\n" +
                        $"  Se han saltado {ejecutor.Skips} iteraciones.\n" +
                        $"  Se han obtenido {ejecutor.CapitulosEncontrados} capitulos.\n" +
                        $"  Para un total de {ejecutor.CaracteresVistos} caracteres.",
                        this);
            FinalizaApp();
        }

        #endregion



        #region Privado


        private static bool ValidaLink(string LinkNovela, out Uri Uri)
        {
            bool x = Uri.TryCreate(LinkNovela, UriKind.Absolute, out Uri uriSalida) && (uriSalida.Scheme == Uri.UriSchemeHttp || uriSalida.Scheme == Uri.UriSchemeHttps);
            Uri = uriSalida;
            return x;
        }


        private void FinalizaApp()
        {
            ReportaExito("Press (Enter) to exit.", this);
            GetNovelsEvents.DestruyeReferencias();
            Console.ReadLine();
        }


        private void TerminaInput(List<INovela> novelasRT)
        {
            string mensaje = $"\nSe obtendrán {novelasRT.Count} novelas:";
            for (int i = 0; i < novelasRT.Count; i++)
            {
                mensaje += $"\n    #{i + 1} {novelasRT[i].Titulo}";
            }
            mensaje += "\nPresiona cualquier tecla para comenzar.";
            PideInput(mensaje, this);
        }


        private bool PreguntaPorOtraNovela(bool InputFinalizado)
        {
            bool decisionTomada = false;
            while (!decisionTomada)
            {
                string decision = PideInput("Quieres agregar otra novela? (Y/N)", this);
                if (decision.Equals("n") | decision.Equals("no") | decision.Equals("not"))
                {
                    decisionTomada = true;
                    InputFinalizado = true;
                }
                else if (decision.Equals("y") | decision.Equals("yes") | decision.Equals("ye"))
                {
                    decisionTomada = true;
                }
            }

            return InputFinalizado;
        }


        private void ConfirmaInfoNovelaConUsuario(ref Dictionary<INovela, int> InfoDescarga, INovela nov, string PathCarpeta)
        {
            Reporta($"Titulo: {nov.Titulo}\n" +     
                                                        $"Link: {nov.LinkPrincipal}\n" +
                                                        $"Tiene {nov.CantidadLinks} links.\n" +
                                                        $"Existen {nov.CantidadCapitulosDescargados} capitulos en la base de datos.\n" +
                                                        $"Se tienen que descargar {nov.CapitulosPorDescargar.Count} capitulos.\n" +
                                                        $"Carpeta: {PathCarpeta}",
                                                        this);
            int comienzo = 0;
            if (nov.CapitulosPorDescargar.Count > 0)
            {
                comienzo = PidePorElComienzo();
            }

            string respuesta = PideInput("\nConfirmar (Y/N)", this);

            if (respuesta.Equals("y") | respuesta.Equals("yes"))
            {
                InfoDescarga.Add(nov, comienzo);
                Reporta($"Confirmada {nov.Titulo}", this);
            }
            else
            {
                ReportaError($"Descartada {nov.Titulo}", this);
            }


        }


        /// <summary>
        /// Le pide al usuario por el primer cap a descargar.
        /// </summary>
        /// <returns></returns>
        private int PidePorElComienzo()
        {
            //Valindando comienzo:
            string _comienzo = PideInput("De los capitulos que se tienen que descargar, de qué capitulo se comenzará? (0 es el primero.)", this);
            //hay que parsear el input asi que se va con un try:
            int comienzo = -1;
            while (comienzo < 0)
            {
                try
                {
                    comienzo = int.Parse(_comienzo);
                }
                catch (FormatException)
                {
                    ReportaError("Debes escribir un número", this);
                    _comienzo = PideInput("Desde qué capitulo se comenzará? (0 -> Inicio)", this);
                }
            }

            return comienzo;
        }


        #endregion


        #endregion
    }
}
