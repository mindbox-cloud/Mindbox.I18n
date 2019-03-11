using System;

namespace Mindbox.I18n
{
	public class NullI18NextLogger : ILogger
	{
		public void LogMissingNamespace(string @namespace, string key)
		{
			// empty
		}

		public void LogMissingLocale(Locale locale, string key)
		{
			// empty
		}

		public void LogMissingKey(string localeName, string @namespace, string key)
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
}
