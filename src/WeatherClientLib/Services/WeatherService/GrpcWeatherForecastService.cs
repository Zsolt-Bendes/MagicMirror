using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MagicMirrorWorker.Protos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static MagicMirrorWorker.Protos.Weather;

namespace WeatherClientLib.Services.WeatherService
{
    public class GrpcWeatherForecastService : IWeatherForecastService
    {
        private readonly WeatherClient _weatherClient;

        public GrpcWeatherForecastService(WeatherClient weatherClient) => _weatherClient = weatherClient;

        public async Task<WeatherResponse> GetWeather() => await _weatherClient.GetWeatherAsync(new Empty());

        public IAsyncEnumerable<WeatherResponse> GetStreamingWeather(CancellationToken token) => _weatherClient.GetWeatherStream(new Empty(), cancellationToken: token)
                .ResponseStream.ReadAllAsync();
    }
}
