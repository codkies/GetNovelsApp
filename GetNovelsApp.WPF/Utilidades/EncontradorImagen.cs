using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetNovelsApp.WPF.Utilidades
{
    public class EncontradorImagen
    {
        /// <summary>
        /// Descarga una imagen del link deseado y regresa el Path donde se encuentra.
        /// </summary>
        public static string DescargaImagen(string url)
        {
            string filePath = System.IO.Path.GetFileName(url);

            System.Net.WebClient cln = new System.Net.WebClient();
            filePath = $@"C:\NovelApp\{filePath}";

            cln.DownloadFile(url, filePath);
            return filePath;
        }

        /// <summary>
        /// Descarga una imagen del link deseado y regresa el Path donde se encuentra.
        /// </summary>
        public static string DescargaImagen(Uri imagenLink)
        {
            return DescargaImagen(imagenLink.ToString());
        }
    }
}
