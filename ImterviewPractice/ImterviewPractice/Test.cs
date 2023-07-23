using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImterviewPractice
{
    public class Test : Abstract
    {
        public int a = 3;

        public override void vi()
        {
            Console.WriteLine("helo");
        }

        public Test(int n) : base(n)
        {
            this.n = n;
        }

        public override int get()
        {
            return n;
        }

        public override decimal calculate(int a)
        {
            return 4 * a;
        }

        public void rec(int num)
        {
            if(num >= 0)
            {
                if(num % 2 == 0)
                {
                    Console.WriteLine(num);
                }
                rec(num-1);
            }
        }

        public int feb(int[] arr,int n)
        {
            int v = 0;
            foreach (var item in arr)
            {
                v += item;
            }
            if (n <= 0) return v+n;
            return v + feb(arr, n - 1) + feb(arr, n - 2);
        }
    }
}
