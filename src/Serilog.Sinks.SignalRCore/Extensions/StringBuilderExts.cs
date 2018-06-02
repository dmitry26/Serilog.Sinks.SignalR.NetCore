// Copyright (c) DMO Consulting LLC. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Serilog.Sinks.SignalR
{
	public static class StringBuilderExts
	{
		/// <summary>
		/// Removes all trailing whitespaces.
		/// </summary>
		/// <param name="sb"></param>
		/// <returns></returns>
		public static StringBuilder TrimEnd(this StringBuilder sb)
		{
			if (sb == null) throw new ArgumentNullException(nameof(sb));

			for (int i = sb.Length - 1; i >= 0; --i)
			{
				if (!char.IsWhiteSpace(sb[i]))
				{
					sb.Length = i + 1;
					return sb;
				}
			}

			return sb;
		}
	}
}
