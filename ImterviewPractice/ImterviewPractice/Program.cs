using ImterviewPractice;
using System;

namespace MyApp // Note: actual namespace depends on the project name.
{
    public class Program
    {

        static void Main(string[] args)
        {
            Test ob = new Test(10);
            Console.WriteLine(ob.get());
        }
    }
}