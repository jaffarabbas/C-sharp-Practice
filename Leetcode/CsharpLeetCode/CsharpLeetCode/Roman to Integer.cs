using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsharpLeetCode
{
    public class Roman_to_Integer
    {
        public int RomanToInt(string s)
        {
            int number;
            Dictionary<string, int> map = new Dictionary<string, int>();
            map.Add("I", 1);
            map.Add("V", 5);
            map.Add("X", 10);
            map.Add("L", 50);
            map.Add("C", 100);
            map.Add("D", 500);
            map.Add("M", 1000);
            number = map.ToList().IndexOf("I");
            for (int i = s.Length - 2; i >= 0; i--)
            {
                if(map.Values.ToList().IndexOf(s.ElementAt(i)) < map.Values.ToList().IndexOf(s.ElementAt(i + 1)))
                {
                    number -= map.Values.ToList().IndexOf(s.ElementAt(i));
                }
                else
                {
                    number += map.Values.ToList().IndexOf(s.ElementAt(i));
                }
            }
            return number;
        }
        static void Main(string[] args)
        {
            Roman_to_Integer p = new Roman_to_Integer();
            int a = p.RomanToInt("IV");
            Console.WriteLine(a);
            Console.Read();
        }

    }
}
