namespace Mindbox.I18n.Abstractions;

public interface ILocalizableString
{
	public abstract string Render(ILocalizationProvider localizationProvider, ILocale locale);
}