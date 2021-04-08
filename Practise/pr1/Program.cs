using System;

namespace pr1
{
    class code{
        //printing hello word 100 times without using loop
        public static void print100(int num){
            if(num<=100){
                Console.WriteLine("Helo Jaffar");
                num++;
                print100(num);
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            code.print100(1);
            Console.WriteLine("kjgjhgjhjh");
        }
    }
}
