// Copyright (c) DMO Consulting LLC. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Serilog.Sinks.SignalR
{
	public class GroupTemplate
	{
		public GroupTemplate() { }

		/// <summary>
		/// Initializes a new instance
		/// </summary>
		/// <param name="name">The group name of loggers.</param>
		/// <param name="template">A message template describing the format used to write to the sink.</param>
		public GroupTemplate(string name,string template)
		{
			GroupName = name;
			OutputTemplate = template;
		}

		/// <summary>
		/// Gets or sets the group name of loggers.
		/// </summary>
		public string GroupName { get; set; }

		/// <summary>
		/// Gets or sets a message template describing the format used to write to the group of loggers.
		/// </summary>
		public string OutputTemplate { get; set; }
	}
}
