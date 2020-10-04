using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GetNovelsApp.Core;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.Conexiones.Internet;
using GetNovelsApp.Core.ConfiguracionApp.xPaths;
using GetNovelsApp.WPF.Utilidades;

namespace GetNovelsApp.WPF.ViewModels
{
    public class AddWebsiteViewModel : ObservableObject
    {
        private InformacionNovelaOnline UltimaInformacionObtenida;
        private OrdenLinks OrdenLinks = OrdenLinks.Descendiente;
        private Archivador ar = new Archivador();

        public AddWebsiteViewModel()
        {
            Command_IngresarDominio = new RelayCommand<string>(IngresarDominio, Puede_IngresarDominio);
            Command_IngresaPathTexto = new RelayCommand<string>(IngresarPathTexto, Puede_IngresarPathTexto);
            Command_IngresaPathLinks = new RelayCommand<string>(IngresaPathLinks, Puede_IngresaPathLinks);
            Command_IngresaPathTitulo = new RelayCommand<string>(IngresaPathTitulo, Puede_IngresaPathTitulo);
            Command_Reset = new RelayCommand(Reset, Puedo_Reset);
            Command_PruebaWebsite = new RelayCommand(PruebaWebsite, Puede_PruebaWebsite);
            Command_InvierteOrdenLinks = new RelayCommand(InvierteOrdenLinks);
            Command_AgregaPerfil = new RelayCommand<Window>(AgregaPerfil, Puedo_AgregaPerfil);
        }

        #region Props

        private List<string> _xPathsTitulo = new List<string>();
        private List<string> _xPathsTextos = new List<string>();
        private List<string> _xPathsLinks = new List<string>();
        private string _dominio;
        private string _linkPrueba;        

        public List<string> xPathsTitulo { get => _xPathsTitulo; set => OnPropertyChanged(ref _xPathsTitulo, value); }
        public List<string> xPathsTextos { get => _xPathsTextos; set => OnPropertyChanged(ref _xPathsTextos, value); }
        public List<string> xPathsLinks { get => _xPathsLinks; set => OnPropertyChanged(ref _xPathsLinks, value); }
        public string Dominio { get => _dominio; set => OnPropertyChanged(ref _dominio, value); }
        public string LinkPrueba { get => _linkPrueba; set => OnPropertyChanged(ref _linkPrueba, value); }

        #endregion


        #region Props de vista previa

        private string vistaPrevia_Titulo;
        private string vistaPrevia_CantidadLinks;
        private string vistaPrevia_PrimerLink;
        private string vistaPrevia_UltimoLink;
        private string vistaPrevia_TextoRandom;


        public string VistaPrevia_Titulo { get => vistaPrevia_Titulo; set => OnPropertyChanged(ref vistaPrevia_Titulo, value); }

        public string VistaPrevia_CantidadLinks { get => vistaPrevia_CantidadLinks; set => OnPropertyChanged(ref vistaPrevia_CantidadLinks, value); }

        public string VistaPrevia_PrimerLink { get => vistaPrevia_PrimerLink; set => OnPropertyChanged(ref vistaPrevia_PrimerLink, value); }

        public string VistaPrevia_UltimoLink { get => vistaPrevia_UltimoLink; set => OnPropertyChanged(ref vistaPrevia_UltimoLink, value); }

        public string VistaPrevia_TextoRandom { get => vistaPrevia_TextoRandom; set => OnPropertyChanged(ref vistaPrevia_TextoRandom, value); }


        #endregion


        #region Ingresando info basica


        public RelayCommand<string> Command_IngresarDominio { get; set; }

        public void IngresarDominio(string dominio)
        {           
            Debug.WriteLine("Dominio: " + dominio);
            Dominio = dominio;

        }

        /// <summary>
        /// Llamado cuando el usuario presiona "Listo"
        /// </summary>
        /// <returns></returns>
        public bool Puede_IngresarDominio(string Dominio)
        {
            //TO DO: Limpiar texto de textbox
            return ar.PerfilExiste(Dominio) == false;
            //If true, desbloquea los siguienes inputs.
        }



        public RelayCommand<string> Command_IngresaPathTexto{ get; set; }


        public void IngresarPathTexto(string xPathNuevo)
        {
            xPathsTextos.Add(xPathNuevo);
        }

        public bool Puede_IngresarPathTexto(string xPathNuevo)
        {
            return xPathsTextos.Contains(xPathNuevo) == false;
        }



        public RelayCommand<string> Command_IngresaPathLinks{ get; set; }

        public void IngresaPathLinks(string xPathNuevo)
        {
            xPathsLinks.Add(xPathNuevo);
        }

        public bool Puede_IngresaPathLinks(string xPathNuevo)
        {
            return xPathsLinks.Contains(xPathNuevo) == false;
        }


        public RelayCommand<string> Command_IngresaPathTitulo { get; set; }

        public void IngresaPathTitulo(string xPathNuevo)
        {
            xPathsTitulo.Add(xPathNuevo);
        }

        public bool Puede_IngresaPathTitulo(string xPathNuevo)
        {
            return xPathsTitulo.Contains(xPathNuevo) == false;
        }



        public RelayCommand Command_Reset { get; set; }

        public void Reset()
        {
            xPathsLinks = new List<string>();
            xPathsTextos = new List<string>();
            xPathsTitulo = new List<string>();
        }

        public bool Puedo_Reset()
        {
            return xPathsLinks.Any() | xPathsTextos.Any() | xPathsTitulo.Any();
        }


        #endregion


        #region Viendo vista previa



        public RelayCommand Command_PruebaWebsite { get; set; }

        public async void PruebaWebsite()
        {
            Debug.WriteLine("Probando website");
            InformacionNovelaOnline info = await Task.Run(() => ManipuladorDeLinks.EncuentraInformacionNovela(new Uri(LinkPrueba), OrdenLinks, xPathsTitulo, xPathsLinks));
            //InformacionNovelaOnline info = await Task.Run(() => ManipuladorDeLinks.EncuentraInformacionNovela(new Uri(LinkPrueba)));
            Debug.WriteLine("Website encontrado");

            ActualizaVistasPrevias(info);
        }

        public bool Puede_PruebaWebsite()
        {
            return LinkPrueba != null & xPathsTitulo.Any() & xPathsLinks.Any();
        }


        public RelayCommand Command_InvierteOrdenLinks { get; private set; }

        public void InvierteOrdenLinks()
        {
            string link1 = VistaPrevia_PrimerLink;
            string link2 = VistaPrevia_UltimoLink;

            
            VistaPrevia_PrimerLink = link2;
            VistaPrevia_UltimoLink = link1;

            if ((int)OrdenLinks == 1) OrdenLinks = (OrdenLinks)2;
            else if ((int)OrdenLinks == 2) OrdenLinks = (OrdenLinks)1;
        }



        private void ActualizaVistasPrevias(InformacionNovelaOnline info)
        {
            UltimaInformacionObtenida = info;

            VistaPrevia_Titulo = info.Titulo;
            VistaPrevia_CantidadLinks = info.LinksDeCapitulos.Count.ToString();
            VistaPrevia_PrimerLink = info.LinksDeCapitulos.First().ToString();
            VistaPrevia_UltimoLink = info.LinksDeCapitulos.Last().ToString();
        }

        

        public void VerTexto()
        {
            Scraper s = new Scraper();
            int randomIndex = Randomnator.RandomIndex(UltimaInformacionObtenida.LinksDeCapitulos.Count);
            VistaPrevia_TextoRandom = s.ObtenCapituloTexto(UltimaInformacionObtenida.LinksDeCapitulos[randomIndex]);
            throw new NotImplementedException("Implementa boton para que aparezca el texto");
            //Show it
        }


        #endregion


        #region Final del proceso

        public RelayCommand<Window> Command_AgregaPerfil { get; set; }

        public void AgregaPerfil(Window ventana)
        {
            IPath Website = GetNovelsFactory.FabricaWebsite(Dominio, xPathsLinks, xPathsTextos, xPathsTitulo, OrdenLinks);
            GuardaWebsiteNuevoEnDB(Website);
            ventana.Close();
        }

        public bool Puedo_AgregaPerfil(Window ventana)
        {
            //return Dominio != string.Empty & xPathsLinks.Any() & xPathsTextos.Any & xPathsTitulo.Any();
            return xPathsTextos.Any() & xPathsTitulo.Any() & xPathsLinks.Any() & Dominio != null;
        }

        private void GuardaWebsiteNuevoEnDB(IPath Website)
        {
            ar.GuardaPerfil(Website);
        }

        #endregion
    }
}
