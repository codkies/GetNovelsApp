using System.Collections.Generic;
using System.Web;
using HtmlAgilityPack;
using GetNovelsApp.Core.Modelos;
using System.Linq;
using System;
using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.Core.Conexiones.Internet;

namespace GetNovelsApp.Core
{
    public class Scraper : IReportero
    {
        #region Constructor

        public Scraper()
        {
            Comunicador.Reporta($"Creando conector.", this);
            conector = new Conector(tiempoTopeEnSegundos: 300); //5 minutos de tiempo de espera
        }

        #endregion


        #region Props

        //Informacion
        public int EntradasIgnoradas { get; private set; } = 0;

        public long CaracteresVistos { get; private set; } = 0;

        public int CapitulosEncontrados { get; private set; } = 0;

        public string Nombre => "Scraper";


        #endregion


        #region Fields

        /// <summary>
        /// Instancia inicializada en el constructor del Scraper.
        /// </summary>
        private Conector conector;

        #endregion


        #region Scraping Capitulo

        public Capitulo CompletaCapitulo(Capitulo capitulo)
        {
            Uri direccion = capitulo.Link;

            List<string> textosRaw = ObtenTextoRaw(direccion);
            if(textosRaw == null)
            {
                Console.WriteLine(capitulo.Link + "Nulo");
                Console.ReadLine();
            }
            string Texto = OrdenaTextoRaw(textosRaw);

            capitulo.UpdateTexto(Texto);

            CapitulosEncontrados++;
            CaracteresVistos += capitulo.Caracteres;

            return capitulo;
        }


        #region Privados
        private List<string> ObtenTextoRaw(Uri direccion)
        {
            HtmlNodeCollection nodos = conector.IntentaNodos(direccion, GetNovelsConfig.xPathsTextos);

            List<string> CapituloDesordenado = ObtenInnerText(nodos);

            if (CapituloDesordenado.Any()) return CapituloDesordenado;
            else return null;
        }


        private List<string> ObtenInnerText(HtmlNodeCollection nodes)
        {
            List<string> CapitulosDesordenado = new List<string>();

            foreach (var item in nodes)
            {
                string entrada = item.InnerText;
                bool paso = RevisaEntrada(entrada);
                if (paso) CapitulosDesordenado.Add(entrada);
            }

            return CapitulosDesordenado;
        }


        private string OrdenaTextoRaw(List<string> capituloDesordenado)
        {
            string capituloOrdenado = string.Empty;
            foreach (string entrada in capituloDesordenado)
            {
                var x = HttpUtility.HtmlDecode(entrada);
                if (x.Equals(string.Empty)) continue;
                capituloOrdenado += $"{x}\n\n";
            }
            return capituloOrdenado;
        }


        /// <summary>
        /// Revisa si una entrada pasa los Checks
        /// </summary>
        /// <param name="entrada"></param>
        /// <returns></returns>
        private bool RevisaEntrada(string entrada)
        {
            List<string> Checks = new List<string>()
            {
                "Edited by", "Translated by", "Editor:", "Translator:"
            };

            foreach (string checks in Checks)
            {
                if (entrada.Contains(checks))
                {
                    EntradasIgnoradas++;
                    return false;
                }
            }
            return true;
        }

        #endregion


        #endregion
    }
}