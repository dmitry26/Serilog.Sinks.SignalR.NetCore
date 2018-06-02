// Copyright (c) DMO Consulting LLC. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace Serilog.Sinks.SignalR
{
	public class LogHubConnection : ILogHub
	{
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		/// <param name="con">A connection used to invoke hub methods on a SignalR Server.</param>
		private LogHubConnection(HubConnection con)
		{
			_hubConnection = con;
			_hubConnection.Closed += OnClose;
		}

		private readonly HubConnection _hubConnection;

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
			_hubConnection.SendAsync(nameof(SendLogEvents),groupName,msgs);

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
			_hubConnection.SendAsync(nameof(SendLogEventsToOthers),groupName,msgs);

		/// <summary>
		/// Add connection ID to the group.
		/// </summary>
		/// <param name="groupName">The group name of loggers.</param>
		/// <returns>A task that represents when the operation has been completed.</returns>
		public Task JoinGroup(string groupName) =>
			_hubConnection.SendAsync(nameof(JoinGroup),groupName);

		/// <summary>
		/// Remove connection ID from the group.
		/// </summary>
		/// <param name="groupName">The group name of loggers.</param>
		/// <returns>A task that represents when the operation has been completed.</returns>
		public Task LeaveGroup(string groupName) =>
			_hubConnection.SendAsync(nameof(LeaveGroup),groupName);

		/// <summary>
		/// Register a handler to be called when the hub method with specified method name is invoked
		/// </summary>
		/// <param name="handler">The handler that will be raised when the hub method is invoked.</param>
		/// <returns>A subscription that can be disposed to unsubscribe from the hub method.</returns>
		public IDisposable OnWriteLog(Action<IEnumerable<LogMessage>> handler) =>
			_hubConnection.On(LogHub.OnLogMethodName,handler);

		/// <summary>
		/// Hub connection closed event
		/// </summary>
		public event Func<Exception,Task> Closed
		{
			add => _hubConnection.Closed += value;
			remove => _hubConnection.Closed -= value;
		}

		protected Task OnClose(Exception x)
		{
			_connected = false;
			return TryStartAsyncIntern(default);
		}

		/// <summary>
		/// Starts a connection to the server
		/// </summary>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
		/// <returns>A task that represents when the operation has been completed.</returns>
		public Task StartAsync(CancellationToken cancellationToken = default) => StartAsync(Timeout.InfiniteTimeSpan);

		/// <summary>
		/// Starts a connection to the server
		/// </summary>
		/// <param name="timeout">The timespan to wait until the request times out, or the value Infinite to indicate that the request does not time out.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
		/// <returns>A task that represents when the operation has been completed.</returns>
		public async Task<bool> StartAsync(TimeSpan timeout,CancellationToken cancellationToken = default)
		{
			_running = true;

			try
			{
				await TryStartAsyncIntern(cancellationToken).TimeoutAfter(timeout,cancellationToken).ConfigureAwait(false);
				return true;
			}
			catch (Exception)
			{
				_running = false;
				return false;
			}
		}

		private bool _running;

		private bool _connected;

		private int _starting;

		private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1,1);

		protected async Task TryStartAsyncIntern(CancellationToken cancellationToken)
		{
			if (Interlocked.CompareExchange(ref _starting,1,0) != 0)
				return;

			try
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					await _connectionLock.WaitAsync(cancellationToken).ConfigureAwait(false);

					try
					{
						if (_connected || !_running)
							return;

						await _hubConnection.StartAsync(cancellationToken).ConfigureAwait(false);
						_connected = true;

						var connected = Connected;

						if (connected != null)
							_ = FireConnectedEvent(connected);

						return;
					}
					catch (Exception)
					{
					}
					finally
					{
						_connectionLock.Release();
					}

					if (cancellationToken.IsCancellationRequested)
						return;

					await Task.Delay(WaitBeforeRetry,cancellationToken).ConfigureAwait(false);
				}
			}
			finally
			{
				_starting = 0;
			}
		}

		/// <summary>
		/// Connected to the Hub event
		/// </summary>
		public event Func<Task> Connected;

		private async Task FireConnectedEvent(Func<Task> connected)
		{
			try
			{
				await connected.Invoke();
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// Stops a connection to the server
		/// </summary>
		/// <returns></returns>
		public async Task StopAsync(CancellationToken cancellationToken = default)
		{
			await _connectionLock.WaitAsync(cancellationToken).ConfigureAwait(false);

			try
			{
				_running = false;
				await _hubConnection.StopAsync(cancellationToken).ConfigureAwait(false);
			}
			finally
			{
				_connectionLock.Release();
			}
		}

		/// <summary>
		/// Gets or sets the server timeout interval for the connection. Changes to this value
		/// will not be applied until the Keep Alive timer is next reset.
		/// </summary>
		public TimeSpan ServerTimeout
		{
			get => _hubConnection.ServerTimeout;
			set => _hubConnection.ServerTimeout = value;
		}

		/// <summary>
		/// Gets or sets the interval used by the server to timeout incoming handshake requests by clients.
		/// </summary>
		public TimeSpan HandshakeTimeout
		{
			get => _hubConnection.HandshakeTimeout;
			set => _hubConnection.HandshakeTimeout = value;
		}

		public static readonly TimeSpan DefaultWaitBeforeRetry = TimeSpan.FromSeconds(5);

		/// <summary>
		/// Gets or sets the timespan used by the client to wait before retrying to connect to the server.
		/// </summary>
		public TimeSpan WaitBeforeRetry { get; set; } = DefaultWaitBeforeRetry;

		/// <summary>
		/// Create a LogHubConnection instance
		/// </summary>
		/// <param name="url">The url of the LogHub</param>
		/// <returns></returns>
		public static LogHubConnection Create(string url)
		{
			var con = BuildConnection(url);
			return new LogHubConnection(con);
		}

		private static HubConnection BuildConnection(string url) =>
			new HubConnectionBuilder()
				 .WithUrl(url)
				 .ConfigureLogging(logging =>
				 {
					 logging.AddSerilog();
				 })
				 .Build();
	}
}
