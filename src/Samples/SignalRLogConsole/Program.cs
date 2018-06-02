using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SignalR;

namespace SignalRLogConsole
{
	class Program
	{
		static async Task Main(string[] args)
		{
			try
			{
				Console.Title = System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location);

				var appSettings = new ConfigurationBuilder()
					.SetBasePath(AppContext.BaseDirectory)
					.AddJsonFile("appsettings.json",true,false)
					.Build();

				var hub = LogHubConnection.Create(appSettings.GetConnectionString("SignalRLog"));

				Log.Logger = new LoggerConfiguration()
					.ReadFrom.Configuration(appSettings)
					.CreateLogger();

				var logger = Log.Logger.ForContext<Program>();

				logger.Debug("Starting...");

				hub.OnWriteLog(msgs =>
				{
					foreach (var msg in msgs)
					{
						logger.Write((LogEventLevel)msg.LogLevel,msg.Message);
					}
				});

				// rejoin group on auto reconnect
				hub.Connected += () =>
				{
					// join default group
					return hub.JoinGroup(null);
				};

				hub.Closed += x =>
				{
					return Task.Run(() => logger.Debug($"Disconnected: {x?.Message}"));
				};

				_ = hub.StartAsync();

				await Console.In.ReadLineAsync();
			}
			catch (Exception x)
			{
				Console.WriteLine(x);
				Console.ReadLine();
			}
		}
	}
}
