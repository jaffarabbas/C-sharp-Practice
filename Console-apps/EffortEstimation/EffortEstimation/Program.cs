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
            Console.WriteLine("SLIM ESTIMATION : " + Math.Round(K, 2));
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
            personMonth = Math.Round(2.4 * Math.Pow(range, 1.05), 2);
            developmentTime = Math.Round(2.5 * Math.Pow(personMonth, 0.38), 2);
            Console.WriteLine("Organic\nPerson Month : " + personMonth + "\nDevelopment Time : " + developmentTime + " DT");
        }

        public void Semidetached(double range)
        {
            personMonth = Math.Round(3.0 * Math.Pow(range, 1.12), 2);
            developmentTime = Math.Round(2.5 * Math.Pow(personMonth, 0.35), 2);
            Console.WriteLine("Semidetached\nPerson Month : " + personMonth + "\nDevelopment Time : " + developmentTime + " DT");
        }

        public void Embedded(double range)
        {
            personMonth = Math.Round(3.6 * Math.Pow(range, 1.20), 2);
            developmentTime = Math.Round(2.5 * Math.Pow(personMonth, 0.32), 2);
            Console.WriteLine("Embedded\nPerson Month : " + personMonth + "\nDevelopment Time : " + developmentTime + " DT");
        }

        public void Runner()
        {
            Console.WriteLine("Write KLOC Range\nOrganic\n2-50 KLOC\nSemidetached\n51-300 KLOC\nEmbedded\n>300 KLOC");
            Console.WriteLine("Write KLOC : ");
            int range = Convert.ToInt32(Console.ReadLine());
            if (range >= 2 && range <= 50)
            {
                Organic(range);
            }
            else if (range >= 51 && range <= 300)
            {
                Semidetached(range);
            }
            else if (range > 300)
            {
                Embedded(range);
            }
        }
    }

    class Functional_Point
    {
        private double ExternalInput, ExternalOutput, ExternalInquiries, InternalFiles, ExternalInterface, F, CAF, UFP, FP;
        int scale;

        public double CalculateF(int scale)
        {
            return (14 * scale);
        }

        public double CalculateCAF(double F)
        {
            return (0.65 + (0.01 * F));
        }

        public double CalculateFP(double caf, double ufp)
        {
            return ufp * caf;
        }
        public double CalculateUFP()
        {
            if (scale == 0 || scale == 1)
            {
                return ((3 * ExternalInput) + (4 * ExternalOutput) + (3 * ExternalInquiries) + (7 * InternalFiles) + (5 * ExternalInterface));
            }
            else if (scale == 2 || scale == 3)
            {
                return ((4 * ExternalInput) + (5 * ExternalOutput) + (4 * ExternalInquiries) + (10 * InternalFiles) + (7 * ExternalInterface));
            }
            else
            {
                return ((6 * ExternalInput) + (7 * ExternalOutput) + (6 * ExternalInquiries) + (15 * InternalFiles) + (10 * ExternalInterface));
            }
        }
        public void GetValue()
        {
            Console.WriteLine("1. Number of external inputs (EI)");
            ExternalInput = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("2. Number of external outputs (EO)");
            ExternalOutput = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("3. Number of external inquiries (EQ)");
            ExternalInquiries = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("4. Number of internal files (ILF)");
            InternalFiles = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("5. Number of external interfaces (EIF)");
            ExternalInterface = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("0-No-Influence\n1-Incidental\n2-Moderate\n3-Average\n4-Significant\n5-Essential");
            scale = Convert.ToInt32(Console.ReadLine());
        }

        public void Calculation()
        {
            GetValue();
            Console.WriteLine("Step 1 : F = 14 * scale");
            F = CalculateF(scale);
            Console.WriteLine("F : " + F);
            Console.WriteLine("Step 2 : Calculate Complexity Adjustment Factor (CAF)\nCAF = 0.65 + ( 0.01 * F )");
            CAF = CalculateCAF(F);
            Console.WriteLine("CAF : " + CAF);
            Console.WriteLine("Step 3 : Calculate Unadjusted Function Point (UFP)");
            Console.WriteLine("EL\t3\t4\t6\nEO\t4\t5\t7\t\nEQ\t3\t4\t6\t\nILF\t7\t10\t15\t\nELF\t5\t7\t10\t");
            UFP = CalculateUFP();
            Console.WriteLine("UFP : " + UFP);
            Console.WriteLine("Step-4: Calculate Function Point");
            FP = CalculateFP(CAF, UFP);
            Console.WriteLine("Functional Point : " + FP);
        }
    }
    class Program
    {
        public static void Main(string[] args)
        {
            int index;
            Console.WriteLine("Enter your Estimation Algorithm\n1)SLIM (1)\n2)COCOMO (2)\n3)Functional Point (3)");
            while (true)
            {
                Console.WriteLine("Enter : 1-2-3 and 0 to stop");
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
                        case 3:
                            Functional_Point funcObj = new Functional_Point();
                            funcObj.Calculation();
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}