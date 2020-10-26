using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace WeatherClientLib.Services.TimeService
{
    public class TimeService : ITimeService
    {
        public async IAsyncEnumerable<DateTime> GetCurrentTimeAsync([EnumeratorCancellation] CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                yield return DateTime.Now;
                await Task.Delay(1000);
            }
        }
    }
}
