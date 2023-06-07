using System;
using System.Collections.Generic;
using Mindbox.I18n.Abstractions;

namespace Mindbox.I18n.TranslationSources;
public class DictionaryTranslationSource : ITranslationSource
{
	private readonly ILocale _supportedLocale;

	private readonly Dictionary<string, string> _translationData;

	public DictionaryTranslationSource(ILocale supportedLocale, Dictionary<string, string> translationData)
	{
		_supportedLocale = supportedLocale ?? throw new ArgumentNullException(nameof(supportedLocale));
		_translationData = translationData ?? throw new ArgumentNullException(nameof(translationData));
	}

	public void Initialize()
	{
	}

	public string TryGetTranslation(ILocale locale, LocalizationKey localizationKey)
	{
		if (!locale.Equals(_supportedLocale))
			throw new InvalidOperationException($"Locale {locale} is not supported by this source");

		if (_translationData.TryGetValue(localizationKey.FullKey, out var translation))
			return translation;
		else
		{
			throw new InvalidOperationException(
				$"Translation not found for key {localizationKey.FullKey} in locale {locale.Name}");
		}
	}
}