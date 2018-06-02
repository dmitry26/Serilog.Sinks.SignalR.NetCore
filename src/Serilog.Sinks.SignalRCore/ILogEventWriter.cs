// Copyright (c) DMO Consulting LLC. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.SignalR
{
	public interface ILogEventWriter
	{
		/// <summary>
		/// Invokes the specified method on the connection(s).
		/// </summary>
		/// <param name="msgs">Log event messages.</param>
		/// <returns>A task that represents when the data has been sent to the client.</returns>
		Task WriteLogEvents(IEnumerable<LogMessage> msgs);
	}
}
