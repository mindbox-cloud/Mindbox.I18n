// Copyright 2022 Mindbox Ltd
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Mindbox.I18n.Analyzers.Logging;

internal class TextFileLogger : ILogger
{
	private readonly string _filePath;
	private readonly object _lockToken = new();

	public TextFileLogger(string filePath)
	{
		_filePath = filePath;
	}

	public void Log<TState>(
		LogLevel logLevel,
		EventId eventId,
		TState state,
		Exception? exception,
		Func<TState, Exception?, string> formatter)
	{
		lock (_lockToken)
		{
			File.AppendAllLines(_filePath, new[] { formatter(state, exception) });
		}
	}

	public bool IsEnabled(LogLevel logLevel) => true;

	public IDisposable BeginScope<TState>(TState state)
		where TState : notnull
		=> NullLogger.Instance.BeginScope(state);
}