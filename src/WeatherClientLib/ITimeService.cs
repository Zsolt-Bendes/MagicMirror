using System;
using System.Collections.Generic;
using System.Threading;

namespace WeatherClientLib
{
	public interface ITimeService
	{
		IAsyncEnumerable<DateTime> GetCurrentTimeAsync(CancellationToken token);
	}
}
