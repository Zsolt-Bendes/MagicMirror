using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicMirrorWorker.Workers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MagicMirrorWorker
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddCors(c => c.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin();
            }));

            services.AddMemoryCache();
            services.AddControllers();
            services.AddHttpClient(Constants.OPEN_WEATHER_CLIENT_NAME);
            services.AddHostedService<WeatherWorker>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<Services.WeatherService>();
                
                endpoints.MapGet("/proto", async req =>
                {
                    await req.Response.SendFileAsync("Protos/weather.proto", req.RequestAborted);
                });

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });

                endpoints.MapControllers();
            });
        }
    }
}
