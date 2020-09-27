using System;
using System.Collections.Generic;
using System.Diagnostics;
using GetNovelsApp.Core;
using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Conexiones;
using System.Data.SqlTypes;
using System.IO;
using GetNovelsApp.Core.Empaquetador;

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

        public readonly ConfiguracionConsoleUI Configuracion;

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

        /// <summary>
        /// Le pregunta al usuario por campos a llenar referente a la manera en que la app funcionará.
        /// </summary>
        /// <returns></returns>
        internal ConfiguracionConsoleUI PideConfiguracion()
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

                if(!int.TryParse(caps, out _))
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


            return new ConfiguracionConsoleUI(this, Direccion, BatchSize, int.Parse(_CapsPorDoc));
        }


        public string PreguntaSiSeImprime(Novela novela)
        {
            return PideInput($"Imprimir \"{novela.Titulo}\"? (Y/N)", this);
        }

        /// <summary>
        /// Crea un formulario para que el usuario meta la informacion a buscar.
        /// </summary>
        /// <param name="xPaths"></param>
        /// <param name="Novelas"></param>
        public Dictionary<InformacionNovela, int> PideInformacionUsuario(string FolderPathDefined)
        {
            //Preps:
            Dictionary<InformacionNovela, int> InfoDescarga = new Dictionary<InformacionNovela, int>();
            bool InputFinalizado = false;
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

                //Valindando comienzo:
                string _comienzo = PideInput("Desde qué capitulo se comenzará? (0 -> Inicio)", this);
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

                //Obteniendo información de novela:
                Reporta("\nObteniendo información de novela...\n", this);
                InformacionNovela infoNov = ManipuladorDeLinks.EncuentraInformacionNovela(UriNovela);

                ///Confirmando con el usuario:
                ConfirmaInfoNovelaConUsuario(ref InfoDescarga, infoNov, comienzo, FolderPathDefined);

                //Pregunta por otra novela:
                InputFinalizado = PreguntaPorOtraNovela(InputFinalizado);

                if (!InputFinalizado) continue;
                List<InformacionNovela> InfoTodasNovelas = new List<InformacionNovela>(InfoDescarga.Keys);
                TerminaInput(InfoTodasNovelas);
            }

            return InfoDescarga;
        }


        private static bool ValidaLink(string LinkNovela, out Uri Uri)
        {
            bool x = Uri.TryCreate(LinkNovela, UriKind.Absolute, out Uri uriSalida) && (uriSalida.Scheme == Uri.UriSchemeHttp || uriSalida.Scheme == Uri.UriSchemeHttps);
            Uri = uriSalida;
            return x;
        }


        public void MustraResultado(GetNovels ejecutor, Stopwatch stopwatch)
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


        private void FinalizaApp()
        {
            ReportaExito("Press (Enter) to exit.", this);
            EventsManager.DestruyeReferencias();
            Console.ReadLine();
        }


        private void TerminaInput(List<InformacionNovela> InfoNovelas)
        {
            string mensaje = $"\nSe obtendrán {InfoNovelas.Count} novelas:";
            for (int i = 0; i < InfoNovelas.Count; i++)
            {
                InformacionNovela _ = InfoNovelas[i];
                mensaje += $"\n    #{i + 1} {_.Titulo}";
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


        private void ConfirmaInfoNovelaConUsuario(ref Dictionary<InformacionNovela, int> InfoDescarga, InformacionNovela infoNov, int comienzo, string PathCarpeta)
        {
            Reporta($"Titulo: {infoNov.Titulo}\n" +
                                                        $"Link: {infoNov.LinkPrincipal}\n" +
                                                        $"Se comenzará desde el capitulo: {infoNov.LinksDeCapitulos[comienzo]}\n" +
                                                        $"Cantidad de capitulos: {infoNov.LinksDeCapitulos.Count - comienzo}\n" +
                                                        $"CapitulosPorPdf: {GetNovelsConfig.CapitulosPorPdf}\n" +
                                                        $"Carpeta: {PathCarpeta}",
                                                        this);


            string respuesta = PideInput("\nConfirmar (Y/N)", this);

            if (respuesta.Equals("y") | respuesta.Equals("yes"))
            {
                InfoDescarga.Add(infoNov, comienzo);
                Reporta($"Confirmada {infoNov.Titulo}", this);
            }
            else
            {
                ReportaError($"Descartada {infoNov.Titulo}", this);
            }
        }


        #endregion
    }
}
