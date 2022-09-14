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
using Mindbox.I18n.Abstractions;

namespace Mindbox.I18n;

public class NullI18NextLogger : ILogger
{
	public void LogMissingNamespace(ILocale locale, string @namespace, string key)
	{
		// empty
	}

	public void LogMissingLocale(ILocale locale, string key)
	{
		// empty
	}

	public void LogMissingKey(ILocale locale, string @namespace, string key)
	{
		// empty
	}

	public void LogInvalidKey(string key)
	{
		// empty
	}

	public void LogInvalidOperation(string message)
	{
		// empty
	}

	public void LogError(Exception exception, string message)
	{
		// empty
	}
}