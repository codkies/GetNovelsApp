using System;

namespace GetNovelsApp.Core.Utilidades
{
    /*
       Ideas:
       - Meter un campo de maximos capitulos y una barra de carga con el % de cada uno
       - Agregar variabilidad en la fuente y en el tamaño
       - Casilla para elegir la carpeta
       - Uh, cómo colocar imagenes de portada?
       - Mayor soporte para más páginas
           - Hacer el xPath variable
           - Expandir la manera que se iteran las direcciones de los capitulos
       - Colocar variable las palabras claves de los checks
   */

    /// <summary>
    /// Define de qué manera el mensajero enviará sus mensajes.
    /// </summary>
    public enum TipoMensajero { Console, WPF}

    /// <summary>
    /// Encargado de enviar mensajes al usuario
    /// </summary>
    public static class Mensajero
    {
        private const ConsoleColor ColorError = ConsoleColor.Red;

        private const ConsoleColor ColorNotificacion = ConsoleColor.DarkYellow;
        private const ConsoleColor ColorCambioEstado = ConsoleColor.Yellow;

        private const ConsoleColor ColorEspecial = ConsoleColor.White;
        private const ConsoleColor ColorExito = ConsoleColor.DarkCyan;

        /*To do:
        Cambiar los enums por un State design pattern*/

        /// <summary>
        /// Este mensaje rompe la aplicacion.
        /// </summary>
        /// <param name="mensaje"></param>
        /// <param name="TipoMensajero"></param>
        public static void MuestraErrorMayor(string mensaje, TipoMensajero TipoMensajero = TipoMensajero.Console)
        {
            switch (TipoMensajero)
            {
                case TipoMensajero.Console:
                    mensaje += " Presiona enter para cerrar el programa.";
                    Console.ForegroundColor = ColorError;
                    MuestraError(mensaje);
                    Console.ReadLine();
                    Environment.Exit(0);
                    break;
                case TipoMensajero.WPF:
                    throw new NotImplementedException();
                default:
                    break;
            }
        }

        public static void MuestraCambioEstado(string mensaje, TipoMensajero TipoMensajero = TipoMensajero.Console)
        {
            switch (TipoMensajero)
            {
                case TipoMensajero.Console:
                    Console.ForegroundColor = ColorCambioEstado;
                    Console.WriteLine(mensaje);
                    break;
                case TipoMensajero.WPF:
                    throw new NotImplementedException();
                default:
                    break;
            }
        }

        public static void MuestraError(string mensaje, TipoMensajero TipoMensajero = TipoMensajero.Console)
        {
            switch (TipoMensajero)
            {
                case TipoMensajero.Console:
                    Console.ForegroundColor = ColorError;
                    Console.WriteLine(mensaje);
                    break;
                case TipoMensajero.WPF:
                    throw new NotImplementedException();
                default:
                    break;
            }
        }

        public static void MuestraNotificacion(string mensaje, TipoMensajero TipoMensajero = TipoMensajero.Console)
        {   
            switch (TipoMensajero)
            {
                case TipoMensajero.Console:
                    Console.ForegroundColor = ColorNotificacion;
                    Console.WriteLine(mensaje);
                    break;
                case TipoMensajero.WPF:
                    throw new NotImplementedException();
                default:
                    break;
            }
        }

        public static void MuestraExito(string mensaje, TipoMensajero TipoMensajero = TipoMensajero.Console)
        {   
            switch (TipoMensajero)
            {
                case TipoMensajero.Console:
                    Console.ForegroundColor = ColorExito;
                    Console.WriteLine(mensaje);
                    break;
                case TipoMensajero.WPF:
                    throw new NotImplementedException();
                default:
                    break;
            }
        }

        public static void MuestraEspecial(string mensaje, TipoMensajero TipoMensajero = TipoMensajero.Console)
        {            
            switch (TipoMensajero)
            {
                case TipoMensajero.Console:
                    Console.ForegroundColor = ColorEspecial;
                    Console.WriteLine(mensaje);
                    break;
                case TipoMensajero.WPF:
                    throw new NotImplementedException();
                default:
                    break;
            }
        }
    }
}


//iText7_Test(Test, "Novela X", 2, Path);