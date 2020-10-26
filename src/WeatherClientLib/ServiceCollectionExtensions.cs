using Grpc.Net.ClientFactory;
using Microsoft.Extensions.DependencyInjection;
using System;
using WeatherClientLib.Models;
using WeatherClientLib.Services.TimeService;
using WeatherClientLib.Services.WeatherService;
using static MagicMirrorWorker.Protos.Weather;

namespace WeatherClientLib
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGrpcWeatherForecastService(
            this IServiceCollection services,
            Action<GrpcClientFactoryOptions> configure)
        {
            services.AddGrpcClient<WeatherClient>(configure);
            services.AddScoped<IWeatherForecastService, GrpcWeatherForecastService>();
            return services;
        }

        public static IServiceCollection AddHttpWeatherForecastService(
            this IServiceCollection services,
            Action<HttpWeatherForecastServiceOptions> configure)
        {
            services.AddScoped<IWeatherForecastService, HttpWeatherForecastService>();
            services.Configure(configure);
            return services;
        }

        public static void AddTimeService(this IServiceCollection services) => services.AddSingleton<ITimeService, TimeService>();
    }
}
