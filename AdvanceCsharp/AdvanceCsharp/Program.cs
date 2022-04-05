using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvanceCsharp.Classes;
namespace AdvanceCsharp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //we cannot make obj of a abstract class
            //AbstractClasses obj = new AbstractClasses();
            Child obj = new Child();
            obj.print();
            obj.AbMethod();
        }
    }
}
