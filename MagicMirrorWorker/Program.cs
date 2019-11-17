using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using System.Net;

namespace MagicMirrorWorker
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		// Additional configuration is required to successfully run gRPC on macOS.
		// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
					webBuilder.ConfigureKestrel(options =>
					{
						options.Listen(IPAddress.Any, 5050, listenOptions =>
						{
							listenOptions.Protocols = HttpProtocols.Http2;
						});
						options.Listen(IPAddress.Any, 5051, listenOptions =>
						{
							listenOptions.Protocols = HttpProtocols.Http1;
						});
						options.Listen(IPAddress.Any, 5001, listenOptions =>
						{
							listenOptions.UseHttps();
							listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
						});
					});
				});
	}
}
