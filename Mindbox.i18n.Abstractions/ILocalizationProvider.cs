namespace Mindbox.i18n.Abstractions;

public interface ILocalizationProvider
{
	string Translate(ILocale locale, string key);
}