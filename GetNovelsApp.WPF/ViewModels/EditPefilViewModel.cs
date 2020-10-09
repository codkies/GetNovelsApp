using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GetNovelsApp.Core;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.ConfiguracionApp.xPaths;
using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.WPF.Utilidades;

namespace GetNovelsApp.WPF.ViewModels
{
    public class EditPefilViewModel : ObservableObject, IReportero
    {
        public string Nombre => "EditPerfilViewModel";
        private readonly IPath Perfil;
        private readonly Archivador ar;

        private string dominio;
        private ObservableCollection<string> links;
        private ObservableCollection<string> titulos;
        private ObservableCollection<string> textos;
        private int indexOrden;

        public EditPefilViewModel(IPath perfil)
        {
            //Readonly fields
            ar = new Archivador();
            Perfil = perfil;

            //Props for the XAML
            AplicaPropieades();

            //Commands de guardado
            Command_GuardaCambios = new RelayCommand<string>(GuardaCambios, Puedo_GuardaCambios);
            Command_GuardarNuevo = new RelayCommand<string>(GuardarNuevo, Puedo_GuardarNuevo);

            //Command de agg xPaths nuevos
            Command_AggLinkPath = new RelayCommand<TextBox>(AggLinkPath, Puedo_AggLinkPath);
            Command_AggTextoPath = new RelayCommand<TextBox>(AggTextoPath, Puedo_AggTextoPath);
            Command_AggTituloPath = new RelayCommand<TextBox>(AggTituloPath, Puedo_AggTituloPath);
        }


        public string Dominio { get => dominio; set => OnPropertyChanged(ref dominio, value); }
        public ObservableCollection<string> Links { get => links; set => OnPropertyChanged(ref links, value); }
        public ObservableCollection<string> Titulos { get => titulos; set => OnPropertyChanged(ref titulos, value); }
        public ObservableCollection<string> Textos { get => textos; set => OnPropertyChanged(ref textos, value); }
        public ObservableCollection<string> OrdenLinks { get; set; } = new ObservableCollection<string>()
        {
            "NULL", "Ascendente", "Descendiente"
        };

        public int IndexOrden { get => indexOrden; set => OnPropertyChanged(ref indexOrden, value); }


        private void AplicaPropieades()
        {
            Dominio = Perfil.Dominio;
            Links = new ObservableCollection<string>(Perfil.xPathsLinks);
            Titulos = new ObservableCollection<string>(Perfil.xPathsTitulo);
            Textos = new ObservableCollection<string>(Perfil.xPathsTextos);            
            IndexOrden = (int)Perfil.OrdenLinks;
        }


        #region Comando Guarda cambios

        public RelayCommand<string> Command_GuardaCambios { get; set; }

        private void GuardaCambios(string ordenLinks)
        {
            Enum.TryParse(ordenLinks, out OrdenLinks orden);
            var PerfilNuevo = GetNovelsFactory.FabricaWebsite(Dominio, Links.ToList(), Textos.ToList(), Titulos.ToList(), orden);

            Progreso progresoGuardaCambios = new Progreso();
            progresoGuardaCambios.ProgressChanged += Progreso_ProgressChanged;
            IReporte r = GetNovelsFactory.FabricaReporteNovela(100, 0, "Editando perfil", this, Perfil.Dominio);
            ManagerTareas.MuestraReporte(r);

            Task.Run(() => ar.ActualizaPerfil(PerfilNuevo, Perfil, progresoGuardaCambios));
        }
        private bool Puedo_GuardaCambios(string empty)
        {
            var indexAceptable = IndexOrden > 0;
            var domCambio = Dominio != Perfil.Dominio;
            var linksCambiaron = !Links.Equals(Perfil.xPathsLinks);
            var textosCambiaron = !Links.Equals(Perfil.xPathsTextos);
            var tituloCambiaron = !Links.Equals(Perfil.xPathsTitulo);

            return indexAceptable & (linksCambiaron | textosCambiaron | tituloCambiaron | domCambio == false);
        }


        #endregion

        #region Comando Guardar como Perfil nuevo


        public RelayCommand<string> Command_GuardarNuevo { get; set; }

        private void GuardarNuevo(string ordenLinks)
        {
            Enum.TryParse(ordenLinks, out OrdenLinks orden);
            var PerfilNuevo = GetNovelsFactory.FabricaWebsite(Dominio, Links.ToList(), Textos.ToList(), Titulos.ToList(), orden);

            Progreso progresoGuardaNuevo = new Progreso();
            progresoGuardaNuevo.ProgressChanged += Progreso_ProgressChanged;
            IReporte r = GetNovelsFactory.FabricaReporteNovela(100, 0, "Guardando perfil nuevo", this, PerfilNuevo.Dominio);
            ManagerTareas.MuestraReporte(r);

            Task.Run(() => ar.GuardaPerfil(PerfilNuevo, progresoGuardaNuevo));
        }

        private bool Puedo_GuardarNuevo(string empty)
        {
            var indexAceptable = IndexOrden > 0;
            var domCambio = Dominio != Perfil.Dominio;
            var linksCambiaron = !Links.Equals(Perfil.xPathsLinks);
            var textosCambiaron = !Links.Equals(Perfil.xPathsTextos);
            var tituloCambiaron = !Links.Equals(Perfil.xPathsTitulo);

            return indexAceptable & (linksCambiaron | textosCambiaron | tituloCambiaron | domCambio == true);
        }

        #endregion


        #region Comando Agrega xPath Link

        public RelayCommand<TextBox> Command_AggLinkPath { get; set; }

        public void AggLinkPath(TextBox tx)
        {
            var path = tx.Text;
            tx.Text = "";
            Links.Add(path);
        }

        private bool Puedo_AggLinkPath(TextBox tx)
        {
            var path = tx.Text;
            return Links.Contains(path) == false;
        }

        #endregion

        #region Comando Agrega xPath Texto

        public RelayCommand<TextBox> Command_AggTextoPath { get; set; }

        public void AggTextoPath(TextBox tx)
        {
            var path = tx.Text;
            tx.Text = "";
            Textos.Add(path);
        }

        

        private bool Puedo_AggTextoPath(TextBox tx)
        {
            var path = tx.Text;
            return Textos.Contains(path) == false;
        }

        #endregion

        #region Comando Agrega xPath Titulo

        public RelayCommand<TextBox> Command_AggTituloPath { get; set; }

        public void AggTituloPath(TextBox tx)
        {
            var path = tx.Text;
            tx.Text = "";
            Titulos.Add(path);
        }

        private bool Puedo_AggTituloPath(TextBox tx)
        {
            var path = tx.Text;
            return Titulos.Contains(path) == false;
        }

        #endregion



        #region Event handlers


        private void Progreso_ProgressChanged(object sender, IReporte e)
        {
            ManagerTareas.ActualizaReporte(e);
        }

        #endregion
    }
}
