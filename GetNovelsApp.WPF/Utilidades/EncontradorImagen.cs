using System;
using System.Collections.Generic;
using System.IO;
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
            filePath = $@"C:\NovelApp\{filePath}";
            if (!File.Exists(filePath))
            {
                System.Net.WebClient cln = new System.Net.WebClient();
                cln.DownloadFile(url, filePath);               
            }
            return filePath;
        }

        /// <summary>
        /// Descarga una imagen del link deseado y regresa el Path donde se encuentra.
        /// </summary>
        public static string DescargaImagen(Uri imagenLink)
        {
            if (imagenLink == null) return string.Empty;
            return DescargaImagen(imagenLink.ToString());
        }
    }
}
