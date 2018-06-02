// Copyright (c) DMO Consulting LLC. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Serilog
{
	public class AppBuilderContext : IConfiguration
	{
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		/// <param name="cfg">Represents a set of key/value application configuration properties.</param>
		/// <param name="appServices">Gets or sets the IServiceProvider that provides access to the application's service container.</param>
		public AppBuilderContext(IConfiguration cfg,IServiceProvider appServices)
		{
			_configuration = cfg;
			ServiceProvider = appServices;
		}

		private readonly IConfiguration _configuration;

		/// <summary>
		/// Gets or sets a configuration value.
		/// </summary>
		public IServiceProvider ServiceProvider { get; private set; }

		/// <summary>
		/// Gets or sets a configuration value.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string this[string key]
		{
			get => _configuration[key];
			set => _configuration[key] = value;
		}

		/// <summary>
		/// Gets the immediate descendant configuration sub-sections
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IConfigurationSection> GetChildren() => _configuration.GetChildren();

		/// <summary>
		/// Returns a IChangeToken that can be used to observe when this configuration is reloaded.
		/// </summary>
		/// <returns></returns>
		public IChangeToken GetReloadToken() => _configuration.GetReloadToken();

		/// <summary>
		/// Gets a configuration sub-section with the specified key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public IConfigurationSection GetSection(string key) => _configuration.GetSection(key);
	}
}
