using ImterviewPractice;
using System;

namespace MyApp // Note: actual namespace depends on the project name.
{
    public class Program
    {

        static void Main(string[] args)
        {
            Abstract obj = new Test(10);
            Test obj2 = new Test(10);
            //Console.WriteLine(obj.get());
            //Console.WriteLine(obj.s);
            //Console.WriteLine(obj.calculate(4));
            //Console.WriteLine(obj2.calculate(3));
            //Console.WriteLine(); ;
            //obj2.rec(10);
            int[] arr = {2,3,5};
            Console.WriteLine(obj2.feb(arr, 3));
        }
    }
}