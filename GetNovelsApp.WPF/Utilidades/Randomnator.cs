using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetNovelsApp.WPF.Utilidades
{
    public static class Randomnator
    {
        // Instantiate random number generator.  
        private static Random _random = new Random();


        // Generates a random number within a range.      
        public static int RandomIndex(int top)
        {
            return _random.Next(0, top);
        }
    }
}
