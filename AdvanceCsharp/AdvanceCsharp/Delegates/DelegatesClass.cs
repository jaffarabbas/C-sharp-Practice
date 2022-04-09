using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvanceCsharp.Delegates
{
    public delegate void MyDelegate(string msg);
    public class DelegatesClass
    {
        public static void TestDelegates()
        {
            MyDelegate myDelegate = MethodA;
            InvokeDelegate(myDelegate);
        }

        public static void InvokeDelegate(MyDelegate del)
        {
            del("Hello World");
        }

        public static void MethodA(string message)
        {
            Console.WriteLine("Called ClassA.MethodA() with parameter: " + message);
        }
    }
}
