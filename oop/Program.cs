using oop.SOLID;
using System;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            SingleResponsiblity singleResponsiblity = new SingleResponsiblity();
            singleResponsiblity.Execute();
            OpenClose openClose = new OpenClose();
            openClose.CalculateArea();
        }
    }
}