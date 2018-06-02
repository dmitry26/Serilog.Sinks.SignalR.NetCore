// Copyright (c) DMO Consulting LLC. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.SignalR
{
	public interface ILogHub
	{
		/// <summary>
		/// Invokes a method on the named group of connection(s) represented by the <see cref="ILogEventWriter"/> instance.
		/// Does not wait for a response from the receiver.
		/// </summary>
		/// <param name="groupName">The group name of loggers.</param>
		/// <param name="msgs">Log event messages.</param>
		/// <returns>A task that represents when the data has been sent to the client.</returns>
		Task SendLogEvents(string groupName,IEnumerable<LogMessage> msgs);

		/// <summary>
		/// Invokes a method on the named group of connection(s) represented by the <see cref="ILogEventWriter"/> instance.
		/// The caller is excluded from the group.
		/// Does not wait for a response from the receiver.
		/// </summary>
		/// <param name="groupName">The group name of loggers.</param>
		/// <param name="msgs">Log event messages.</param>
		/// <returns>A task that represents when the data has been sent to the client.</returns>
		Task SendLogEventsToOthers(string groupName,IEnumerable<LogMessage> msgs);

		/// <summary>
		/// Add connection ID to the group.
		/// </summary>
		/// <param name="groupName"></param>
		/// <returns>A task that represents when the operation has been completed.</returns>
		Task JoinGroup(string groupName);

		/// <summary>
		/// Remove connection ID from the group.
		/// </summary>
		/// <param name="groupName">Group name</param>
		/// <returns>A task that represents when the operation has been completed.</returns>
		Task LeaveGroup(string groupName);
	}
}
