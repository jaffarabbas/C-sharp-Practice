using System;

namespace EffortEstimation
{
    class SLIM
    {
        //K = (LOC / (C* t4/3 )) * 3
        public double loc;
        public double C;
        public double t;

        public void CalculateK()
        {
            double K = ((loc / (C * (t * (1.33)))) * 3);
            Console.WriteLine("SLIM ESTIMATION : "+Math.Round(K,2));
        }

        public void CalculateEstimetation()
        {
            Console.WriteLine("Enter LOC : ");
            loc = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("Enter C (610 - 57314): ");
            C = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("Enter t : ");
            t = Convert.ToDouble(Console.ReadLine());
            CalculateK();
        }
    }

    class COCOMO
    {
        double personMonth;
        double developmentTime;

        public void Organic(double range)
        {
            personMonth = Math.Round(2.4 * Math.Pow(range,1.05),2);
            developmentTime = Math.Round(2.5 * Math.Pow(personMonth, 0.38),2);
            Console.WriteLine("Organic\nPerson Month : "+personMonth+"\nDevelopment Time : "+developmentTime+" DT");
        }
        
        public void Semidetached(double range)
        {
            personMonth = Math.Round(3.0 * Math.Pow(range,1.12),2);
            developmentTime = Math.Round(2.5 * Math.Pow(personMonth, 0.35),2);
            Console.WriteLine("Semidetached\nPerson Month : "+personMonth+"\nDevelopment Time : "+developmentTime+" DT");
        }
        
        public void Embedded(double range)
        {
            personMonth = Math.Round(3.6 * Math.Pow(range,1.20),2);
            developmentTime = Math.Round(2.5 * Math.Pow(personMonth, 0.32),2);
            Console.WriteLine("Embedded\nPerson Month : "+personMonth+"\nDevelopment Time : "+developmentTime+" DT");
        }

        public void Runner()
        {
            Console.WriteLine("Write KLOC Range\nOrganic\n2-50 KLOC\nSemidetached\n51-300 KLOC\nEmbedded\n>300 KLOC");
            Console.WriteLine("Write KLOC : ");
            int range = Convert.ToInt32(Console.ReadLine());
            if (range >= 2 && range <= 50) {
                Organic(range);
            } else if (range >= 51 && range <= 300) {
                Semidetached(range);
            } else if (range > 300) {
                Embedded(range);
            }
        } 
    }
    class Program
    {
        public static void Main(string[] args)
        {  
           int index;
           Console.WriteLine("Enter your Estimation Algorithm\n1)SLIM (1)\nCOCOMO (2)");
           while (true)
           {
               Console.WriteLine("Enter : 1-2 and 0 to stop");
               index = Convert.ToInt32(Console.ReadLine());
               if (index == 0)
               {
                   break;
               }
               else
               {
                   switch (index)
                   {
                       case 1:
                           SLIM slimObj = new SLIM();
                           slimObj.CalculateEstimetation();
                           break;
                       case 2:
                           COCOMO cocomoObj = new COCOMO();
                           cocomoObj.Runner();
                           break;
                       default:
                           break;
                   }
               }
           }
        }
    }
}