using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImterviewPractice
{
    public abstract class Abstract
    {
        protected int n;
        public int s = 3;
        public virtual void vi()
        {
            Console.WriteLine("sadasd");
        }
        public Abstract(int n)
        {
            this.n = n;
        }

        public abstract int get();

        public virtual decimal calculate(int a)
        {
            return (decimal)Math.Sqrt(a);
        }
    }
}
