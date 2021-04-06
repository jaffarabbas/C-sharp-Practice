using System;

namespace _2dMatrixs
{
    class Program
    {
        static void Main(string[] args)
        {
            int[,] arr = new int[3,3];

            for (int i = 0; i < 3; i++)
            {
                for (int x = 0; x < 3; x++)
                {
                    Console.WriteLine("Please enter number i : "+i+" x "+x);
                    arr[x, i] = int.Parse(Console.ReadLine());
                }
            }

            for (int i = 0; i < 3; i++)
            {
                for (int x = 0; x < 3; x++)
                {
                    Console.Write(arr[x, i] + " ");
                }
                Console.WriteLine(" ");
            }
        }
    }
}
