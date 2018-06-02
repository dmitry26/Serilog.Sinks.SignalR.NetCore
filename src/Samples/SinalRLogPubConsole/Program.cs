using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace SinalRLogPubConsole
{
	class Program
	{
		static async Task Main(string[] args)
		{
			try
			{
				Console.Title = System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location);

				await RunAsync(args);
			}
			catch (Exception x)
			{
				Console.WriteLine(x);
				Console.ReadLine();
			}
		}

		public static async Task RunAsync(string[] args)
		{
			var builder = new HostBuilder()
				.ConfigureAppConfiguration((hostContext,config) =>
				{
					config.AddEnvironmentVariables();
					config.AddJsonFile("appsettings.json",optional: true);
				})
				.ConfigureServices((hostContext,services) =>
				{
					services.AddScoped<IHostedService,MyService>();
				})
				.UseSerilog((hostContext,config) => config.ReadFrom.Configuration(hostContext.Configuration));

			await builder.RunConsoleAsync();
		}
	}
}
