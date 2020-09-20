using System;

namespace GetNovelsApp.Core.Utilidades
{
    public struct InformacionCapitulo
    {
        public string TituloCapitulo;
        public int Valor;
        public int NumeroCapitulo;

        public InformacionCapitulo(string tituloCapitulo, int valor, int numeroCapitulo)
        {
            TituloCapitulo = tituloCapitulo;
            Valor = valor;
            NumeroCapitulo = numeroCapitulo;
        }
    }

    public static class ManipuladorDeLinks
    {
        /// <summary>
        /// Encuentra el numero del capitulo según el link.
        /// </summary>
        /// <param name="DireccionAProbar"></param>
        /// <returns></returns>
        public static InformacionCapitulo EncuentraInformacionCapitulo(string DireccionAProbar)
        {
            string CapituloEscrito = string.Empty;

            string TituloCapitulo = string.Empty;

            int gruposDeNumeros = 0;

            for (int i = 0; i < DireccionAProbar.Length; i++)
            {
                //Preparations.
                char letra = DireccionAProbar[i];
                bool EsUnNumero = char.IsDigit(letra);
                CapituloEscrito = string.Empty;

                if(gruposDeNumeros > 0 & !EsUnNumero)
                {
                    //CapituloEscrito += letra.ToString();
                }
                else if (!EsUnNumero) continue;                

                gruposDeNumeros++;
                //Haciendo un check de que hayan mas caracteres
                if (i == DireccionAProbar.Length - 1)
                {
                    CapituloEscrito += letra.ToString();
                    TituloCapitulo += letra.ToString();
                    break;
                }
                //--------------------------------------------------------------

                //Core:
                CapituloEscrito += letra.ToString(); //1  

                //Revisando los siguientes caracteres.
                int salto = i + 1;
                bool subio = false;
                for (int x = salto; x < DireccionAProbar.Length; x++)
                {
                    char letraFutura = DireccionAProbar[x];
                    if (char.IsDigit(letraFutura)) //Solo procede si el caracter es un #
                    {
                        CapituloEscrito += letraFutura.ToString();//2     
                        salto = x;
                        subio = true;
                    }
                    else break;//Apenas halles una letra, rompe este loop.
                }
                TituloCapitulo += CapituloEscrito;
                i = subio ? salto : i; //Si el valor subió, has que la siguiente iteracion continue ahí. Sino, deja que continue normalmente.
            }

            //Posibles errores:
            if(gruposDeNumeros < 1) Mensajero.MuestraErrorMayor("No se pudo determinar el valor del capitulo.");
            //---------------------


            int NumeroCapitulo = Math.Abs(int.Parse(CapituloEscrito));
            int Valor = gruposDeNumeros;
            TituloCapitulo = $"Chapter {TituloCapitulo}";

            InformacionCapitulo infoDelCapituloSegunElLink = new InformacionCapitulo(TituloCapitulo, Valor, NumeroCapitulo);

            return infoDelCapituloSegunElLink;
        }
    }
}
