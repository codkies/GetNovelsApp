using System.Collections.Generic;
using System.Linq;

namespace GetNovelsApp.Core.Utilidades
{
    public class ManipuladorStrings
    {

        /// <summary>
        /// Une las tags en una oracion con comas.
        /// </summary>
        /// <param name="ListaDeTags"></param>
        /// <returns></returns>
        public static string TagsEnString(List<string> ListaDeTags)
        {
            return string.Join(", ", ListaDeTags);
        }


        /// <summary>
        /// Separa una oracion de tags en una lista.
        /// </summary>
        /// <param name="Tags"></param>
        /// <returns></returns>
        public static List<string> TagsEnLista(string Tags)
        {
            return Tags.Split(',').ToList();
        }


    }
}
