using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.SignalR;

namespace Serilog
{
	/// <summary>
	/// Adds the WriteTo.SignalR() extension method to <see cref="LoggerConfiguration"/>.
	/// </summary>
	public static class SignalRLoggerConfigExts
	{
		public static IServiceProvider ServiceProvider { get; set; }

		/// <summary>
		/// Adds a sink that writes log events as documents to a SignalR hub.
		/// </summary>
		/// <param name="loggerConfiguration">The logger configuration.</param>
		/// <param name="context">The hub context.</param>
		/// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
		/// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
		/// <param name="period">The time to wait between checking for event batches.</param>
		/// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
		/// <param name="groups">The group configuration set.</param>
		/// <returns>Logger configuration, allowing configuration to continue.</returns>
		/// <exception cref="ArgumentNullException">A required parameter is null.</exception>
		public static LoggerConfiguration SignalR(
			this LoggerSinkConfiguration loggerConfiguration,
			LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
			int batchPostingLimit = SignalRSink.DefaultBatchPostingLimit,
			TimeSpan? period = null,
			IFormatProvider formatProvider = null,
			IReadOnlyCollection<GroupTemplate> groups = null,
			IConfiguration cfg = null)
		{
			if (loggerConfiguration == null) throw new ArgumentNullException(nameof(loggerConfiguration));

			var hub = GetHubProxy(cfg as AppBuilderContext);

			if (hub == null) throw new ArgumentNullException(nameof(hub));

			if (groups == null || groups.Count == 0)
				groups = DefaultGroup;
			else
				groups = groups.Concat(DefaultGroup).GroupBy(x => x.GroupName).Select(x => x.First()).ToArray();

			return loggerConfiguration.Sink(new SignalRSink(hub,batchPostingLimit,period ?? SignalRSink.DefaultPeriod,formatProvider,groups),
				restrictedToMinimumLevel);
		}

		private static IReadOnlyCollection<GroupTemplate> DefaultGroup => new GroupTemplate[] {
			new GroupTemplate(LogHub.DefaultGroupName,SignalRSink.DefaultOutputTemplate)
		};

		private static IHubClients<ILogEventWriter> GetHubProxy(AppBuilderContext cfg) => (cfg ?? throw new ArgumentNullException(nameof(cfg)))
				.ServiceProvider.GetService<IHubContext<LogHub,ILogEventWriter>>()
				.Clients;
	}
}
