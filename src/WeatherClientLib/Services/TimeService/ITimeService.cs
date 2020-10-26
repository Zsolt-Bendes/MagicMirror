using System;
using System.Collections.Generic;
using System.Threading;

namespace WeatherClientLib.Services.TimeService
{
    public interface ITimeService
    {
        IAsyncEnumerable<DateTime> GetCurrentTimeAsync(CancellationToken token);
    }
}
