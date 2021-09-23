using System;

namespace InterviewPractice
{
    class check
    {
        private int a = 4;

        public int A { get => a; set => a = value; }
    }
    class check1 : check
    {
        public void prints()
        {
            A = 3;
            Console.WriteLine(A);
        }
    }


    class Program 
    {
        static void Main(string[] args)
        {
            check1 obj = new check1();
            obj.prints();
        }
    }
}
