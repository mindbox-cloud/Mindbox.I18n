namespace Mindbox.I18n.Abstractions;

public interface ILogger
{
	void LogMissingNamespace(ILocale locale, string @namespace, string key);
	void LogMissingLocale(ILocale locale, string key);
	void LogMissingKey(ILocale locale, string @namespace, string key);
	void LogInvalidKey(string key);
	void LogInvalidOperation(string message);
	void LogError(Exception exception, string message);
}