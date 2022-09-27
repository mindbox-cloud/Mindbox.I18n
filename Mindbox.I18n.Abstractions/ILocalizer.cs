namespace Mindbox.I18n.Abstractions;

public interface ILocalizer
{
	string GetLocalizedString(
		ILocale locale,
		LocalizableString localizableString,
		LocalizationTemplateParameters? localizationTemplateParameters = null);

	string GetLocalizedEnum(ILocale locale, Enum value);
}