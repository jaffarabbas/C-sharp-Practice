using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvanceCsharp.Classes
{
    public class StaticClass
    {
        public static Dictionary<string, string> dic = new Dictionary<string, string>();
        public static void c(string a,string b)
        {
            dic.Add(a, b);
        }

        public static Dictionary<string,string> getDictionary()
        {
            return dic;
        }
    }
}
