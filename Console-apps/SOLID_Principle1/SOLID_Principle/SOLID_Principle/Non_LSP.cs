using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOLID_Principle
{
    public class Car //system that controls car driving operations 
    {
        public void GoForward()
        {
            Console.WriteLine("Car going forward.");
        }

        public void TurnLeft()
        {
            Console.WriteLine("Car turns left.");
        }

        public void TurnRight()
        {
            Console.WriteLine("Car turns right.");
        }

        public void GoBackward()
        {
            Console.WriteLine("Car backing up.");
        }
    }

    public class AutoPilot //the autopilot software class
    {
        private Car vehicle;

        public AutoPilot(Car vehicle)
        {
            this.vehicle = vehicle;
        }

        public void Navigate(string destination)
        {
            //imagine here it is a complex algorithm that navigates the car from point A to point B.
            vehicle.GoForward(); //simulate all driving operations. 
            vehicle.TurnLeft();
            vehicle.GoBackward();
            vehicle.TurnRight();
        }
    }

    public interface IDrivable // extracted interface from Car class
    {
        void GoForward();
        void TurnLeft();
        void TurnRight();
        void GoBackward();
    }

   
}
