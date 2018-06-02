// Copyright (c) DMO Consulting LLC. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.SignalR
{
	/// <summary>
	/// Writes log events as messages to a SignalR hub.
	/// </summary>
	public abstract class SignalRCoreSink : PeriodicBatchingSink
	{
		private readonly IFormatProvider _formatProvider;

		protected IReadOnlyCollection<GroupTemplate> Groups { get; private set; }

		/// <summary>
		/// Construct a sink posting to the specified database.
		/// </summary>
		/// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
		/// <param name="period">The time to wait between checking for event batches.</param>
		/// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
		/// <param name="groups">The group configuration set.</param>
		public SignalRCoreSink(int batchPostingLimit,TimeSpan period,
			IFormatProvider formatProvider,IReadOnlyCollection<GroupTemplate> groups)
			: base(batchPostingLimit,period)
		{
			_formatProvider = formatProvider;
			Groups = groups;
		}

		/// <summary>
		/// Emit a batch of log events, running asynchronously.
		/// </summary>
		/// <param name="events">The events to emit.</param>
		/// <remarks>Override either <see cref="PeriodicBatchingSink.EmitBatch"/> or <see cref="PeriodicBatchingSink.EmitBatchAsync"/>,
		/// not both.</remarks>
		protected override void EmitBatch(IEnumerable<LogEvent> events)
		{
			foreach (var grp in Groups)
			{
				var msgs = events.Select(e => new LogMessage((int)e.Level,FormatMessage(e,grp.OutputTemplate))).ToArray();
				SendMessages(grp.GroupName,msgs);
			}
		}

		protected abstract void SendMessages(string groupName,IEnumerable<LogMessage> msgs);

		protected string FormatMessage(LogEvent evt,string outputTemplate)
		{
			var formatter = new MessageTemplateTextFormatter(outputTemplate,null);
			var writer = new StringWriter(new StringBuilder(256));
			formatter.Format(evt,writer);
			return writer.GetStringBuilder().TrimEnd().ToString();
		}
	}
}
