namespace Mindbox.I18n;

public interface ITranslationSource
{
	void Initialize();

	string TryGetTranslation(Locale locale, LocalizationKey localizationKey);
}