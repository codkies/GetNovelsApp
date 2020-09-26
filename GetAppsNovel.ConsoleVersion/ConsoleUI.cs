using System;
using System.Collections.Generic;
using System.Diagnostics;
using GetNovelsApp.Core;
using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.Core.Configuracion;
using GetNovelsApp.Core.Modelos;

namespace GetAppsNovel.ConsoleVersion
{
    public class ConsoleUI : IComunicador, IReportero
    {
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

        public string PreguntaSiSeImprime(Novela novela)
        {
            return PideInput($"Imprimir \"{novela.Titulo}\"? (Y/N)", this);
        }

        /// <summary>
        /// Crea un formulario para que el usuario meta la informacion a buscar.
        /// </summary>
        /// <param name="xPaths"></param>
        /// <param name="Novelas"></param>
        public List<Novela> PideInformacionUsuario()
        {
            //Preps:
            List<Novela> Novelas = new List<Novela>();
            bool InputFinalizado = false;
            int numeroDeNovelas = 1;

            //Pidiendo info de la carpeta:
            string Path = PideInput("Carpeta: (Se creará una subcarpeta, con el titulo de cada novela, dentro de la dirección que introduzcas).", this);

            while (!InputFinalizado)
            {
                //Campos de input:   
                string LinkNovela = PideInput("Link de la página principal de la novelas:", this);                
                bool linkValido = ValidaLink(LinkNovela, out Uri UriNovela);

                while (linkValido == false)
                {
                    ReportaError("Link no valido. Requiere Https.", this);
                    LinkNovela = PideInput("Link de la página principal de la novelas:", this);
                    linkValido = ValidaLink(LinkNovela, out Uri u);
                    UriNovela = u;
                }

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

                Reporta("\nObteniendo información de novela...\n", this);
                Path = Path.Replace(@"\\", @"\\\\");


                Novela novela = new Novela(UriNovela, Path, comienzo);

                ///Confirmando con el usuario:
                ConfirmaUsuario(ref Novelas, ref numeroDeNovelas, LinkNovela, comienzo, novela);

                //Pregunta por otra novela:
                InputFinalizado = PreguntaPorOtraNovela(InputFinalizado);

                if (!InputFinalizado) continue;

                TerminaInput(Novelas);
            }

            return Novelas;
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
            Console.ReadLine();
        }

        private void TerminaInput(List<Novela> Novelas)
        {
            string mensaje = $"\nSe obtendrán {Novelas.Count} novelas:";
            for (int i = 0; i < Novelas.Count; i++)
            {
                Novela _ = Novelas[i];
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


        private void ConfirmaUsuario(ref List<Novela> Novelas, ref int numeroDeNovelas, string LinkNovela, int comienzo, Novela novela)
        {
            Reporta($"Titulo: {novela.Titulo}\n" +
                                                        $"Link: {LinkNovela}\n" +
                                                        $"Se comenzará desde el capitulo: {novela.LinksDeCapitulos[comienzo]}\n" +
                                                        $"Cantidad de capitulos: {novela.LinksDeCapitulos.Count - comienzo}\n" +
                                                        $"CapitulosPorPdf: {GetNovelsConfig.CapitulosPorPdf}\n" +
                                                        $"Carpeta: {novela.CarpetaPath}",
                                                        this);


            string respuesta = PideInput("\nConfirmar (Y/N)", this);

            if (respuesta.Equals("y") | respuesta.Equals("yes"))
            {
                Novelas.Add(novela);
                numeroDeNovelas++;
                Reporta($"Confirmada {novela.Titulo}", this);
            }
            else
            {
                ReportaError($"Descartada {novela.Titulo}", this);
            }
        }


        #endregion
    }
}
