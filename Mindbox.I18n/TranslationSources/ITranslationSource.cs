using Mindbox.I18n.Abstractions;

namespace Mindbox.I18n;

public interface ITranslationSource
{
	void Initialize();

	string? TryGetTranslation(ILocale locale, LocalizationKey localizationKey);
}