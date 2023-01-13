using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCheffPractice.Problems.Basic_Programming
{
    public class XJUMP
    {
        public static int code()
        {
            int i, j;
            i = Convert.ToInt32(Console.ReadLine());
            j = Convert.ToInt32(Console.ReadLine());
            int steps = i / j;
            int stairs = i % j;
            return stairs + steps;
        }

        public void submission()
        {
            int n = Console.Read();
            int i = 1;
            while (i <= n)
            {
                code();
                i++;
            }
        }
    }
}
