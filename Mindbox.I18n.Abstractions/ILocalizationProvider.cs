namespace Mindbox.I18n.Abstractions;

public interface ILocalizationProvider
{
	string Translate(ILocale locale, string key);
}