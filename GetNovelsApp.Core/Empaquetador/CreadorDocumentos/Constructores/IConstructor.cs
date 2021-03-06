﻿using System;
using System.Collections.Generic;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Reportaje;

namespace GetNovelsApp.Core.Empaquetadores.CreadorDocumentos.Constructores
{
    /// <summary>
    /// Interfaz que el Empaquetador necesita.
    /// </summary>
    public interface IConstructor
    {
        void ConstruyeDocumento(List<Capitulo> CapitulosAImprimir, IProgress<IReporte> progress = null);
    }
}