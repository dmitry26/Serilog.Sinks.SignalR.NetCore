// Copyright (c) DMO Consulting LLC. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Serilog.Sinks.SignalR
{
	public class LogMessage
	{
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		/// <param name="level">Log event Level</param>
		/// <param name="msg">Log event text.</param>
		public LogMessage(int level,string msg)
		{
			LogLevel = level;
			Message = msg;
		}

		/// <summary>
		/// Gets or sets the log event level.
		/// </summary>
		public int LogLevel { get; set; }

		/// <summary>
		/// Gets or sets the log event text.
		/// </summary>
		public string Message { get; set; }
	}
}
