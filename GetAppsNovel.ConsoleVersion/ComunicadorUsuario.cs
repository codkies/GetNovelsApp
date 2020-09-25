
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using GetNovelsApp.Core;
using GetNovelsApp.Core.Configuracion;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Reportaje;


namespace GetAppsNovel.ConsoleVersion
{
    /// <summary>
    /// 
    /// </summary>
    public class ComunicadorUsuario : IReportero
    {
        public string Nombre => "Programa";


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
            string Path = AppGlobalMensajero.PideInput("Carpeta: (Se creará una subcarpeta, con el titulo de cada novela, dentro de la dirección que introduzcas).", this);

            while (!InputFinalizado)
            {
                //Campos de input:   
                string LinkNovela = AppGlobalMensajero.PideInput("Link de la página principal de la novelas:", this);
                string _comienzo = AppGlobalMensajero.PideInput("Desde qué capitulo se comenzará? (0 -> Inicio)", this);       
                //hay que parsear el input asi que se va con un try:
                int comienzo = -1;
                while(comienzo < 0)
                {
                    try
                    {
                        comienzo = int.Parse(_comienzo);
                    }
                    catch (FormatException)
                    {
                        AppGlobalMensajero.ReportaError("Debes escribir un número", this);
                        _comienzo = AppGlobalMensajero.PideInput("Desde qué capitulo se comenzará? (0 -> Inicio)", this);
                    }
                }

                AppGlobalMensajero.Reporta("\nObteniendo información de novela...\n", this);
                Path = Path.Replace(@"\\", @"\\\\");
                Novela novela = new Novela(LinkNovela, Path, comienzo);

                ///Confirmando con el usuario:
                ConfirmaUsuario(ref Novelas, ref numeroDeNovelas, LinkNovela, comienzo, novela);

                //Pregunta por otra novela:
                InputFinalizado = PreguntaPorOtraNovela(InputFinalizado);

                if (!InputFinalizado) continue;

                TerminaInput(Novelas);
            }

            return Novelas;
        }

        public void MustraResultado(GetNovels ejecutor, Stopwatch stopwatch)
        {
            TimeSpan ts = stopwatch.Elapsed;
            string report = ts.ToString("hh\\:mm\\:ss\\.ff");

            AppGlobalMensajero.ReportaEspecial($"Finalizado el proceso en {report}. Resumen: \n" +
                        $"  Se han creado {ejecutor.DocumentosCreados} documentos.\n" +
                        $"  Se han ignorado {ejecutor.EntradasIgnoradas} entradas.\n" +
                        $"  Se han saltado {ejecutor.Skips} iteraciones.\n" +
                        $"  Se han obtenido {ejecutor.CapitulosEncontrados} capitulos.\n" +
                        $"  Para un total de {ejecutor.CaracteresVistos} caracteres.",
                        this);
            FinalizaApp();
        }


        #region Interno

        private void FinalizaApp()
        {
            AppGlobalMensajero.ReportaExito("Press (Enter) to exit.", this);
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
            AppGlobalMensajero.PideInput(mensaje, this);
        }


        private bool PreguntaPorOtraNovela(bool InputFinalizado)
        {
            bool decisionTomada = false;
            while (!decisionTomada)
            {
                string decision = AppGlobalMensajero.PideInput("Quieres agregar otra novela? (Y/N)", this);
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
            AppGlobalMensajero.Reporta($"Titulo: {novela.Titulo}\n" +
                                                        $"Link: {LinkNovela}\n" +
                                                        $"Se comenzará desde el capitulo: {novela.LinksDeCapitulos[comienzo]}\n" +
                                                        $"Cantidad de capitulos: {novela.LinksDeCapitulos.Count - comienzo}\n" +
                                                        $"CapitulosPorPdf: {AppGlobalConfig.CapitulosPorPdf}\n" +
                                                        $"Carpeta: {novela.CarpetaPath}",
                                                        this);


            string respuesta = AppGlobalMensajero.PideInput("\nConfirmar (Y/N)", this);

            if (respuesta.Equals("y") | respuesta.Equals("yes"))
            {
                Novelas.Add(novela);
                numeroDeNovelas++;
                AppGlobalMensajero.Reporta($"Confirmada {novela.Titulo}", this);
            }
            else
            {
                AppGlobalMensajero.ReportaError($"Descartada {novela.Titulo}", this);
            }
        } 

        #endregion
    }
}
