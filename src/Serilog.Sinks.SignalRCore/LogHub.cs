// Copyright (c) DMO Consulting LLC. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Serilog.Sinks.SignalR
{
	public class LogHub : Hub<ILogEventWriter>, ILogHub
	{
		public const string DefaultGroupName = "loggers";

		public const string OnLogMethodName = nameof(ILogEventWriter.WriteLogEvents);

		/// <summary>
		/// Invokes a method on the named group of connection(s) represented by the <see cref="ILogEventWriter"/> instance.
		/// Does not wait for a response from the receiver.
		/// </summary>
		/// <param name="groupName">The group name of loggers.</param>
		/// <param name="level">Log event level.</param>
		/// <param name="msg">Log event text.</param>
		/// <returns>A task that represents when the data has been sent to the client.</returns>
		public Task SendLogEvent(string groupName,int level,string msg) =>
			SendLogEvents(groupName,new LogMessage[] { new LogMessage(level,msg) });

		/// <summary>
		/// Invokes a method on the named group of connection(s) represented by the <see cref="ILogEventWriter"/> instance.
		/// Does not wait for a response from the receiver.
		/// </summary>
		/// <param name="groupName">The group name of loggers.</param>
		/// <param name="msgs">Log event messages.</param>
		/// <returns>A task that represents when the data has been sent to the client.</returns>
		public Task SendLogEvents(string groupName,IEnumerable<LogMessage> msgs) =>
			(msgs != null && msgs.Any())
				? Clients.Group(ValidateGroupName(groupName)).WriteLogEvents(msgs)
				: Task.CompletedTask;

		/// <summary>
		/// Invokes a method on the named group of connection(s) represented by the <see cref="ILogEventWriter"/> instance.
		/// The caller is excluded from the group.
		/// Does not wait for a response from the receiver.
		/// </summary>
		/// <param name="groupName">The group name of loggers.</param>
		/// <param name="msg">Log event message.</param>
		/// <returns>A task that represents when the data has been sent to the client.</returns>
		public Task SendLogEventToOthers(string groupName,int level,string msg) =>
			SendLogEventsToOthers(groupName,new LogMessage[] { new LogMessage(level,msg) });

		/// <summary>
		/// Invokes a method on the named group of connection(s) represented by the <see cref="ILogEventWriter"/> instance.
		/// The caller is excluded from the group.
		/// Does not wait for a response from the receiver.
		/// </summary>
		/// <param name="groupName">The group name of loggers.</param>
		/// <param name="msgs">Log event messages.</param>
		/// <returns>A task that represents when the data has been sent to the client.</returns>
		public Task SendLogEventsToOthers(string groupName,IEnumerable<LogMessage> msgs) =>
			(msgs != null && msgs.Any())
				? Clients.OthersInGroup(ValidateGroupName(groupName)).WriteLogEvents(msgs)
				: Task.CompletedTask;

		/// <summary>
		/// Add connection ID to the group.
		/// </summary>
		/// <param name="groupName">The group name of loggers.</param>
		/// <returns>A task that represents when the operation has been completed.</returns>
		public Task JoinGroup(string groupName) =>
			Groups.AddToGroupAsync(Context.ConnectionId,ValidateGroupName(groupName));

		/// <summary>
		/// Remove connection ID from the group.
		/// </summary>
		/// <param name="groupName">The group name of loggers.</param>
		/// <returns>A task that represents when the operation has been completed.</returns>
		public Task LeaveGroup(string groupName) =>
			Groups.RemoveFromGroupAsync(Context.ConnectionId,ValidateGroupName(groupName));

		private string ValidateGroupName(string name) => string.IsNullOrWhiteSpace(name) ? DefaultGroupName : name;
	}
}