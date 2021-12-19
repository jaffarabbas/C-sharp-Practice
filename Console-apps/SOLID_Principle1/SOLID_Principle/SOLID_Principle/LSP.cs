﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOLID_Principle
{
    public class Car : IDrivable
    {
       ...
    }

    public class Truck : IDrivable
    {
        public void GoForward()
        {
            Console.WriteLine("Truck going forward.");
        }

        public void TurnLeft()
        {
            Console.WriteLine("Truck turns left.");
        }

        public void TurnRight()
        {
            Console.WriteLine("Truck turns right.");
        }

        public void GoBackward()
        {
            Console.WriteLine("Truck backing up.");
        }
    }
}
