using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvanceCsharp.Classes
{
    abstract class AbstractClasses : InterfaceClass
    {
        public int value = 3;
        public string name { get; set; } = String.Empty;
        public abstract int AbMethod();

        public void getname()
        {
            throw new NotImplementedException();
        }

        public void setname()
        {
            throw new NotImplementedException();
        }
    }
    class Child : AbstractClasses
    {
        public void print()
        {
            Console.WriteLine(value);
        }

        public override int AbMethod()
        {
            return value;
        }
    }
}
