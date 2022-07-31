using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace WebApiFromScratch
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CreateDefaultBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateDefaultBuilder(string[] args) =>
        
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
