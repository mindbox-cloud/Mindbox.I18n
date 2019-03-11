using System;

namespace Mindbox.I18n
{
	public interface ILogger
	{
		void LogMissingNamespace(string @namespace, string key);
		void LogMissingLocale(Locale locale, string key);
		void LogMissingKey(string localeName, string @namespace, string key);
		void LogInvalidKey(string key);
		void LogInvalidOperation(string message);
		void LogError(Exception exception, string message);
	}
}