using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.SignalR;

namespace Serilog
{
	/// <summary>
	/// Adds the WriteTo.SignalR() extension method to <see cref="LoggerConfiguration"/>.
	/// </summary>
	public static class SignalRClientLoggerConfigExts
	{
		/// <summary>
		/// Adds a sink that writes log events as documents to a SignalR hub.
		/// </summary>
		/// <param name="loggerConfiguration">The logger configuration.</param>
		/// <param name="url">The url of the LogHub, or the name of the connection string.</param>
		/// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
		/// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
		/// <param name="period">The time to wait between checking for event batches.</param>
		/// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
		/// <param name="groups">The group configuration set.</param>
		/// <param name="serverTimeoutSec">The timeout interval for the connection.</param>
		/// <param name="handshakeTimeoutSec">The timeout for incoming handshake requests by clients.</param>
		/// <param name="waitBeforeRetrySec">Seconds to wait before retrying to connect to the server.</param>		
		/// <returns>Logger configuration, allowing configuration to continue.</returns>
		/// <exception cref="ArgumentNullException">A required parameter is null.</exception>
		public static LoggerConfiguration SignalRClient(
			this LoggerSinkConfiguration loggerConfiguration,
			string url,
			LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
			int batchPostingLimit = SignalRClientSink.DefaultBatchPostingLimit,
			TimeSpan? period = null,
			IFormatProvider formatProvider = null,
			IReadOnlyCollection<GroupTemplate> groups = null,
			int? serverTimeoutSec = null,
			int? handshakeTimeoutSec = null,
			int? waitBeforeRetrySec = null,
			IConfiguration cfg = null)
		{
			if (loggerConfiguration == null) throw new ArgumentNullException(nameof(loggerConfiguration));

			if (url == null) throw new ArgumentNullException(nameof(url));

			if (url.LastIndexOf('/') < 0)
			{
				if (cfg == null) throw new ArgumentNullException(nameof(cfg));

				var conName = url;
				url = cfg.GetConnectionString(conName);

				if (string.IsNullOrEmpty(url)) throw new InvalidOperationException($"Invalid connection name: {conName}");
			}

			if (groups == null || groups.Count == 0)
				groups = DefaultGroup;
			else
				groups = groups.Concat(DefaultGroup).GroupBy(x => x.GroupName).Select(x => x.First()).ToArray();

			var con = LogHubConnection.Create(url);

			if (serverTimeoutSec.HasValue)
				con.ServerTimeout = TimeSpan.FromSeconds(serverTimeoutSec.Value);

			if (handshakeTimeoutSec.HasValue)
				con.HandshakeTimeout = TimeSpan.FromSeconds(handshakeTimeoutSec.Value);

			if (waitBeforeRetrySec.HasValue)
				con.WaitBeforeRetry = TimeSpan.FromSeconds(waitBeforeRetrySec.Value);

			con.StartAsync();

			return loggerConfiguration.Sink(new SignalRClientSink(con,batchPostingLimit,
					period ?? SignalRClientSink.DefaultPeriod,formatProvider,groups),
				restrictedToMinimumLevel);
		}

		private static IReadOnlyCollection<GroupTemplate> DefaultGroup => new GroupTemplate[] {
			new GroupTemplate(LogHub.DefaultGroupName,SignalRClientSink.DefaultOutputTemplate)
		};
	}
}
