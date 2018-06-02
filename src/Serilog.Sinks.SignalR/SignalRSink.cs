using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace Serilog.Sinks.SignalR
{
	/// <summary>
	/// Writes log events as messages to a SignalR hub.
	/// </summary>
	public class SignalRSink : SignalRCoreSink
	{
		readonly IHubClients<ILogEventWriter> _hub;

		/// <summary>
		/// A reasonable default for the number of events posted in
		/// each batch.
		/// </summary>
		public const int DefaultBatchPostingLimit = 5;

		/// <summary>
		/// A reasonable default time to wait between checking for event batches.
		/// </summary>
		public static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(2);

		public const string DefaultOutputTemplate = "[{Timestamp:MM/dd/yy HH:mm:ss} {Level:u3}] {Message}{NewLine}{Exception}";

		/// <summary>
		/// Construct a sink posting to the specified database.
		/// </summary>
		/// <param name="context">The hub context.</param>
		/// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
		/// <param name="period">The time to wait between checking for event batches.</param>
		/// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
		/// <param name="groups">The group configuration set.</param>
		public SignalRSink(IHubClients<ILogEventWriter> hub,int batchPostingLimit,TimeSpan period,
			IFormatProvider formatProvider = null,IReadOnlyCollection<GroupTemplate> groups = null)
			: base(batchPostingLimit,period,formatProvider,groups)
		{
			_hub = hub;
		}

		protected override void SendMessages(string groupName,IEnumerable<LogMessage> msgs) =>
			_hub.Group(groupName).WriteLogEvents(msgs);
	}
}
