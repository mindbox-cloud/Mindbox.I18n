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

using Microsoft.Extensions.Logging;
using Mindbox.I18n.Abstractions;


namespace Mindbox.I18n.AspNetCore;

internal class DefaultLocalizationLogger: Abstractions.ILogger
{
	private readonly ILogger<DefaultLocalizationLogger> _logger;

		public DefaultLocalizationLogger(ILogger<DefaultLocalizationLogger> logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public void LogMissingNamespace(ILocale locale, string @namespace, string key) =>
			_logger.LogCritical(
				$"Namespace \"{@namespace}\" was not found for key \"{key}\" for locale \"{locale.Name}\".");

		public void LogMissingLocale(ILocale locale, string key) =>
			_logger.LogCritical($"Locale \"{locale.Name}\" was not found for key \"{key}\".");

		public void LogMissingKey(ILocale locale, string @namespace, string key) =>
			_logger.LogCritical(
				$"Key \"{key}\" was not found in namespace \"{@namespace}\" for locale \"{locale.Name}\".");

		public void LogInvalidKey(string key) => _logger.LogCritical($"Key \"{key}\" is not a valid key.");

		public void LogInvalidOperation(string message) => _logger.LogCritical(message);

		public void LogError(Exception exception, string message) => _logger.LogCritical(exception, message);

}