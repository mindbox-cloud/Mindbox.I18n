using System;
using System.Collections.Generic;

namespace Mindbox.I18n.Analyzers.Test;
#nullable disable
public class TestTranslationSource : IAnalyzerTranslationSource
{
	private readonly Dictionary<LocalizationKey, string> _translations
		= new();

	public void AddTranslation(string localizationKey, string value)
	{
		var parsedLocalizationKey = LocalizationKey.TryParse(localizationKey);
		if (parsedLocalizationKey == null)
			throw new ArgumentException("Passed string is not a valid localization key", nameof(localizationKey));

		_translations.Add(parsedLocalizationKey, value);
	}

	public string TryGetTranslation(LocalizationKey localizationKey)
	{
		_translations.TryGetValue(localizationKey, out var value);
		return value;
	}
}