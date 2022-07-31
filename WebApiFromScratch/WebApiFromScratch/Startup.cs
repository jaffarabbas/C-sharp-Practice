using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace WebApiFromScratch
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddTransient<CustomeMiddleWare1>();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.Use(async (context, next) =>
            //{
            //    await context.Response.WriteAsync("middle 1\n");
            //    await next();
            //    await context.Response.WriteAsync("middle 2\n");
            //});

            ////map function
            //app.Map("/map", CustomeMiddleWare);

            ////custome middleware

            //app.UseMiddleware<CustomeMiddleWare1>();
            
            //app.Use(async (context, next) =>
            //{
            //    await context.Response.WriteAsync("middle 3\n");
            //    await next();
            //    await context.Response.WriteAsync("middle 4\n");
            //});

            //app.Run(async context =>
            //{
            //    await context.Response.WriteAsync("run middle\n");
            //});

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseRouting();
            
            app.UseEndpoints(endpoints =>
            {
                //for gernel routes
                //endpoints.MapGet("/", async context => {
                //   await context.Response.WriteAsync("Hello World!");
                //});
                endpoints.MapControllers();
            });
        }

        private void CustomeMiddleWare(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("run custome middle\n");
            });
        }
    }
}
