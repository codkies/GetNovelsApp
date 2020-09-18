using System;

namespace GetNovelsApp.ConsoleVersion
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

    public static class Mensajero
    {
        private const ConsoleColor ColorError = ConsoleColor.Red;
        private const ConsoleColor ColorNotificacion = ConsoleColor.DarkYellow;
        private const ConsoleColor ColorExito = ConsoleColor.DarkCyan;
        private const ConsoleColor ColorEspecial = ConsoleColor.White;

        public static void MuestraError(string mensaje)
        {
            Console.ForegroundColor = ColorError;
            Console.WriteLine(mensaje);
        }

        public static void MuestraNotificacion(string mensaje)
        {
            Console.ForegroundColor = ColorNotificacion;
            Console.WriteLine(mensaje);
        }

        public static void MuestraExito(string mensaje)
        {
            Console.ForegroundColor = ColorExito;
            Console.WriteLine(mensaje);
        }

        public static void MuestraEspecial(string mensaje)
        {
            Console.ForegroundColor = ColorEspecial;
            Console.WriteLine(mensaje);
        }
    }
}


//iText7_Test(Test, "Novela X", 2, Path);