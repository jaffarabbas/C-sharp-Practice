using System;

namespace rider_test_1
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            int a=2, b=3;
            a += b - a;
            Console.WriteLine(a);
            b = a - a + b;
            Console.WriteLine(b);
        }
    }
}