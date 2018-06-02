// Copyright (c) DMO Consulting LLC. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Serilog
{
	public static class LoggerConfigExts
	{
		/// <summary>
		/// Create logger from the external settings
		/// </summary>
		/// <param name="logCfg"></param>
		/// <param name="appCfg"></param>
		/// <param name="svcProv"></param>
		/// <returns></returns>
		public static ILogger CreateLoggerFromConfig(this LoggerConfiguration logCfg,
			IConfiguration appCfg,IServiceProvider svcProv = null) =>
			(logCfg ?? throw new ArgumentNullException(nameof(logCfg)))
				.ReadFrom
				.Configuration(svcProv == null ? appCfg : new AppBuilderContext(appCfg,svcProv))
				.CreateLogger();
	}
}
