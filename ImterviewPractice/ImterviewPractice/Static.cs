using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImterviewPractice
{
    public class Static 
    {
        private readonly static Static s = new Static();
        private Static()
        {
            
        }

        public static Static get()
        {
            return s;
        }

        public int a = 4;
    }
}
