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
            //DelegatesClass.TestDelegates();
            //Dictionary<string, string> k = StaticClass.c("as", "a1");
            //Dictionary<string, string> k2 = StaticClass.c("as", "a2");
            //Dictionary<string, string> k3 = StaticClass.c("as", "a3");
            //Console.WriteLine(k["as"]);
            //Console.WriteLine(k2["as"]);
            //Console.WriteLine(k3["as"]);
            //Console.ReadLine();
            StaticClass.c("asd","asdt");
            Console.WriteLine(StaticClass.getDictionary()["asd"]);
        }
    }
}
