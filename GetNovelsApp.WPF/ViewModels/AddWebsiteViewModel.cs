using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        }

        #region Props

        private List<string> _xPathsTitulo;
        private List<string> _xPathsTextos;
        private List<string> _xPathsLinks;
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


        /// <summary>
        /// Llamado cuando el usuario presiona "Listo"
        /// </summary>
        /// <returns></returns>
        public bool DominioAceptado()
        {
            //Limpiar texto de textbox
            Archivador ar = new Archivador();
            return ar.PerfilExiste(Dominio);
            //If true, desbloquea los siguienes inputs.

        }

        public void IngresaPathLinks(string xPathNuevo)
        {
            xPathsLinks.Add(xPathNuevo);
        }

        public void IngresaPathTexto(string xPathNuevo)
        {
            xPathsTextos.Add(xPathNuevo);
        }

        public void IngresaPathTitulo(string xPathNuevo)
        {
            xPathsTitulo.Add(xPathNuevo);
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }


        #endregion


        #region Viendo vista previa

        public async Task PruebaWebsite()
        {
            InformacionNovelaOnline info = await Task.Run(() => ManipuladorDeLinks.EncuentraInformacionNovela(new Uri(LinkPrueba), OrdenLinks, xPathsTitulo, xPathsLinks));

            ActualizaVistasPrevias(info);
        }

        private void ActualizaVistasPrevias(InformacionNovelaOnline info)
        {
            UltimaInformacionObtenida = info;

            VistaPrevia_Titulo = info.Titulo;

            VistaPrevia_PrimerLink = info.LinksDeCapitulos.First().ToString();
            VistaPrevia_UltimoLink = info.LinksDeCapitulos.Last().ToString();
        }

        public void InvierteOrdenLinks()
        {
            string link1 = VistaPrevia_PrimerLink;
            string link2 = VistaPrevia_UltimoLink;

            VistaPrevia_PrimerLink = link2;
            VistaPrevia_UltimoLink = link1;
            InvierteOrden();
        }

        private void InvierteOrden()
        {
            if ((int)OrdenLinks == 1) OrdenLinks = (OrdenLinks)2;
            else if ((int)OrdenLinks == 2) OrdenLinks = (OrdenLinks)1;
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

        public void AgregaPerfil()
        {
            IPath Website = GetNovelsFactory.FabricaWebsite(Dominio, xPathsLinks, xPathsTextos, xPathsTitulo, OrdenLinks);
            GuardaWebsiteNuevoEnDB(Website);            
        }

        private void GuardaWebsiteNuevoEnDB(IPath Website)
        {
            ar.GuardaPerfil(Website);
        }

        #endregion
    }
}
