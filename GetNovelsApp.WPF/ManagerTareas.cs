using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Resources;
using System.Runtime;
using System.Threading;
using System.Windows.Documents;
using GetNovelsApp.Core;
using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.WPF.Models;
using GetNovelsApp.WPF.Utilidades;

namespace GetNovelsApp.WPF
{
    public static class ManagerTareas
    {
        private static List<ITarea> Tareas = new List<ITarea>();


        public static Tarea MuestraReporte(IReporte reporte)
        {
            Tarea t = new Tarea(reporte.Estado, reporte.NombreItem, reporte.PorcentajeDeCompletado);
            Tareas.Add(t);
            if(TareaEnPantalla == null)
            {
                TareaEnPantalla = Tareas.First();
                ActualizaMensaje();
            }
            return t;
        }


        public static void ActualizaReporte(IReporte reporte)
        {
            //bool found = false; 
            foreach (ITarea tarea in Tareas)
            {
                if(tarea.NombreItem.ToLower().Trim() == reporte.NombreItem.ToLower().Trim())
                {
                    //found = true;
                    tarea.ActualizaProgreso(reporte.PorcentajeDeCompletado, reporte.Estado);
                    TareaEnPantalla = tarea;
                    ActualizaMensaje();
                    RevisaSiEsUltimaTarea();                    
                    return;
                }
            }
            //if(found == false) throw new Exception("Se actualizó un reporte que no existia en el manager.");
        }

        private static void RevisaSiEsUltimaTarea()
        {
            bool esUltima = Tareas.IndexOf(TareaEnPantalla) == Tareas.Count - 1;
            if (esUltima & TareaEnPantalla.PorcentajeTarea >= 100)
            {                
                //Muestra por 5s el mensaje
                ActualizaMensaje($"Todas las tareas terminadas.");                
                Thread.Sleep(5 * 1000);
                //Borra el mensaje en pantalla
                ActualizaMensaje($"");        
                //Elimina referencia a tarea en pantalla.
                TareaEnPantalla = null;
                Tareas.Clear();
                return;
            }
        }


        #region Mensaje de estado

        public static string Mensaje;

        private static void ActualizaMensaje(string mensajeEspecial = null)
        {
            if (TareaEnPantalla == null) return;
            if(mensajeEspecial == null)
            {
                Mensaje = $"{EstadoTareas} '{Item}' ({PorcentajeTareaCorriendo}%)... ({IndexTareaCorriendo}/{TotalTareas})";
            }
            else
            {
                Mensaje = null;
            }
            GetNovelsWPFEvents.Invoke_TareasCambio();
        }

        #endregion


        #region shorthands para el mensaje de estado

        private static ITarea TareaEnPantalla;
        private static string IndexTareaCorriendo
        {
            get
            {
                if (Tareas == null) return string.Empty;
                return (Tareas.IndexOf(TareaEnPantalla) + 1).ToString();
                //nada de (0/1)
            }
        }

        private static string EstadoTareas => Tareas == null ? string.Empty : TareaEnPantalla.Estado;

        private static string Item => Tareas == null ? string.Empty : TareaEnPantalla.NombreItem;

        private static string PorcentajeTareaCorriendo => Tareas == null ? string.Empty : TareaEnPantalla.PorcentajeTarea.ToString();

        private static string TotalTareas => Tareas == null ? string.Empty : Tareas.Count.ToString();
        #endregion


    }
}
