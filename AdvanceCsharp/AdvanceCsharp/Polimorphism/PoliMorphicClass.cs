using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvanceCsharp.Polimorphism
{
    public class PoliMorphicClass
    {
        public virtual void tester()
        {
            Console.WriteLine("Test");
        }
        public void overLoad()
        {
            Console.WriteLine("no over");
        }
    }
    public class ChildClass : PoliMorphicClass
    {
        public override void tester()
        {
            Console.WriteLine("Child test");
        }

        public new void overLoad()
        {
            Console.WriteLine("yes over");
        }

        public void overLoad(int a)
        {
            Console.WriteLine("yes over {0}",a);
        }
    }
}
