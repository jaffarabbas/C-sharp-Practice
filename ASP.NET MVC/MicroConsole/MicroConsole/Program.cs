using MicroService4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                MicroService microservice = new MicroService();
                microservice.Run(args);
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
