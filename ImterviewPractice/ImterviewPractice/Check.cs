using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImterviewPractice
{
    public class Check : Check2
    {
        public override void get()
        {
            Console.WriteLine("sadasd");
        }

        public bool isPrime(int n)
        {
            if(n <= 1) return false;
            if(n <= 3) return true;

            for(int i = 2; i < Math.Sqrt(n); i++) {
                if (n % i == 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
