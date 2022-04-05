using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvanceCsharp.Classes
{
    sealed class SealedClass
    {
        private int fname;
        public int Fname
        {
            get { return fname; }
            set { fname = value; }
        }
    }

    //class test : SealedClass
    //{

    //}
}
