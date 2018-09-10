namespace Mindbox.I18n
{
	public interface ILogger
	{
		void LogMissingKey(string localeName, string @namespace, string key);
		void LogInvalidKey(string key);
	}
}