using System;
using System.Collections.Generic;
using Serilog.Events;

namespace Serilog.Sinks.SignalR
{
	/// <summary>
	/// Writes log events as messages to a SignalR hub.
	/// </summary>
	public class SignalRClientSink : SignalRCoreSink
	{
		readonly LogHubConnection _hubConnection;

		/// <summary>
		/// A reasonable default for the number of events posted in
		/// each batch.
		/// </summary>
		public const int DefaultBatchPostingLimit = 5;

		/// <summary>
		/// A reasonable default time to wait between checking for event batches.
		/// </summary>
		public static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(1);

		public const string DefaultOutputTemplate = "[{Timestamp:MM/dd/yy HH:mm:ss} {Level:u3}] {Message}{NewLine}{Exception}";

		/// <summary>
		/// Initializes a new instance of the SignalR sink.
		/// </summary>
		/// <param name="hubConnect">The LogHub connection.</param>
		/// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
		/// <param name="period">The time to wait between checking for event batches.</param>
		/// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
		/// <param name="groups">The group configuration set.</param>
		public SignalRClientSink(LogHubConnection hubConnect,int batchPostingLimit,TimeSpan period,
			IFormatProvider formatProvider,IReadOnlyCollection<GroupTemplate> groups)
			: base(batchPostingLimit,period,formatProvider,groups)
		{
			_hubConnection = hubConnect;
		}

		protected override void SendMessages(string groupName,IEnumerable<LogMessage> msgs) =>
			_hubConnection.SendLogEvents(groupName,msgs);
	}
}
