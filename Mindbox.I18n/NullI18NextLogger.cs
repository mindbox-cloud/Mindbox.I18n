using System;
using Mindbox.i18n.Abstractions;

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