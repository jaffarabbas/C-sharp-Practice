using System;

namespace EffortEstimation
{
    class FunctionalPoint
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

        public double CalculateFP(double caf,double ufp)
        {
            return ufp * caf;
        }
        public double CalculateUFP()
        {
            if (scale == 0 || scale == 1)
            {
                return ((3*ExternalInput)+(4*ExternalOutput)+(3*ExternalInquiries)+(7*InternalFiles)+(5*ExternalInterface));
            }
            else if (scale == 2 || scale == 3)
            {
                return ((4*ExternalInput)+(5*ExternalOutput)+(4*ExternalInquiries)+(10*InternalFiles)+(7*ExternalInterface));
            }
            else
            {
                return ((6*ExternalInput)+(7*ExternalOutput)+(6*ExternalInquiries)+(15*InternalFiles)+(10*ExternalInterface));
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
            Console.WriteLine("F : "+F);
            Console.WriteLine("Step 2 : Calculate Complexity Adjustment Factor (CAF)\nCAF = 0.65 + ( 0.01 * F )");
            CAF = CalculateCAF(F);
            Console.WriteLine("CAF : "+CAF);
            Console.WriteLine("Step 3 : Calculate Unadjusted Function Point (UFP)");
            Console.WriteLine("EL\t3\t4\t6\nEO\t4\t5\t7\t\nEQ\t3\t4\t6\t\nILF\t7\t10\t15\t\nELF\t5\t7\t10\t");
            UFP = CalculateUFP();
            Console.WriteLine("UFP : "+UFP);
            Console.WriteLine("Step-4: Calculate Function Point");
            FP = CalculateFP(CAF, UFP);
            Console.WriteLine("Functional Point : "+FP);
        }
    }
    
    public class Assignment3
    {
        public static void Main(string[] args)
        {  
            int index;
            Console.WriteLine("Enter your Estimation Algorithm\n1)Functional Point (1)");
            while (true)
            {
                Console.WriteLine("Enter : 1 and 0 to stop");
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
                            FunctionalPoint funcObj = new FunctionalPoint();
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