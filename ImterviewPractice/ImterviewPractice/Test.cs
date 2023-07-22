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
    }
}
