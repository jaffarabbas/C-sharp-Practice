using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvanceCsharp.Classes;
using AdvanceCsharp.Delegates;
using AdvanceCsharp.Polimorphism;

namespace AdvanceCsharp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //we cannot make obj of a abstract class
            //AbstractClasses obj = new Child();
            //obj.AbMethod();
            //partial classes
            //PartialClass pr = new PartialClass();
            //pr.add(1, "jaffar");
            //pr.add(2, "akli");
            //pr.view();
            //Console.ReadLine();
            //sealed
            //SealedClass selClass = new SealedClass();
            //selClass.Fname = 34;
            //Console.WriteLine(selClass.Fname);
            //Console.ReadLine();
            //PoliMorphicClass poli = new PoliMorphicClass();
            //ChildClass c = new ChildClass();
            //poli.tester();
            //c.tester();
            //poli.overLoad();
            //c.overLoad();
            //c.overLoad(2);
            //Console.ReadLine();
            //delegates
            DelegatesClass.TestDelegates();
        }
    }
}
