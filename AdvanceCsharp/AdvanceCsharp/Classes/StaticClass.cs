using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvanceCsharp.Classes
{
    public class StaticClass
    {
        public static Dictionary<string, string> c(string a,string b)
        {
            Dictionary<string, string> ab = new Dictionary<string, string>();
            ab.Add(a, b);
            return ab;
        }
    }
}
