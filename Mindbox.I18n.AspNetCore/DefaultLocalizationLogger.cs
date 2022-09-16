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