using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SinalRLogPubConsole
{
	public class MyService : BackgroundService
	{
		public MyService(ILogger<MyService> logger) => _logger = logger;

		private readonly ILogger _logger;

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("MyService is starting.");

			stoppingToken.Register(() => _logger.LogInformation("MyService is stopping."));
			var count = 1;

			while (!stoppingToken.IsCancellationRequested)
			{
				_logger.LogDebug($"MyService is doing background work #{count++}");

				await Task.Delay(TimeSpan.FromSeconds(5),stoppingToken).ConfigureAwait(false);
			}

			_logger.LogInformation("MyService background task is stopping.");
		}
	}
}
