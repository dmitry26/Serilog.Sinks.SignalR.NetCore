// Copyright (c) DMO Consulting LLC. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Serilog.Sinks.SignalR
{
	public static class TaskExts
	{
		public static Task TimeoutAfter(this Task task,int timeoutMlsec,CancellationToken cancelToken = default(CancellationToken))
		{
			return task.TimeoutAfter(TimeSpan.FromMilliseconds(timeoutMlsec),cancelToken);
		}

		public static async Task TimeoutAfter(this Task task,TimeSpan timeout,CancellationToken cancelToken = default(CancellationToken))
		{
			if (task == null)
				throw new ArgumentNullException("task");

			if (task.IsCompleted || timeout == Timeout.InfiniteTimeSpan)
			{
				await task.ConfigureAwait(false); //propagate exception/cancellation
				return;
			}

			if (timeout < Timeout.InfiniteTimeSpan)
				throw new ArgumentOutOfRangeException("timeout");

			if (timeout == TimeSpan.Zero)
				throw new TimeoutException();

			using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancelToken))
			{
				if (task == await Task.WhenAny(task,Task.Delay(timeout,cts.Token)).ConfigureAwait(false))
				{
					cts.Cancel(); //ensure that the Delay task is cleaned up
					await task.ConfigureAwait(false); //propagate exception/cancellation
					return;
				}
			}

			throw new TimeoutException();
		}
	}
}
