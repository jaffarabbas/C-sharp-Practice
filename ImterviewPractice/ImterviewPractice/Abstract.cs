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
        public Abstract(int n)
        {
            this.n = n;
        }

        public abstract int get();
    }
}
