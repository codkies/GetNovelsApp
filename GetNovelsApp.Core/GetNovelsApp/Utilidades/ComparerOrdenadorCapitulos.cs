using System;
using System.Collections.Generic;
using GetNovelsApp.Core.Modelos;

namespace GetNovelsApp.Core.Utilidades
{
    /// <summary>
    /// Ordena capitulos acorde a sus numeros
    /// </summary>
    public class ComparerOrdenadorCapitulos : Comparer<Capitulo>
    {
        public override int Compare(Capitulo x, Capitulo y)
        {
            if (x.NumeroCapitulo > y.NumeroCapitulo)
                return 1;
            else if (x.NumeroCapitulo < y.NumeroCapitulo)
                return -1;
            else
                throw new NotSupportedException("Ambos capitulos tienen el mismo numero");
        }
    }
}
