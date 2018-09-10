using System;
using System.Collections.Generic;
using System.Text;

namespace Mindbox.I18n.Analyzers.Test
{
    public class TestTranslationSource : IAnalyzerTranslationSource
    {
		private readonly Dictionary<LocalizationKey, string> translations
			= new Dictionary<LocalizationKey, string>();

	    public void AddTranslation(string localizationKey, string value)
	    {
		    var parsedLocalizationKey = LocalizationKey.TryParse(localizationKey);
			if (parsedLocalizationKey == null)
				throw new ArgumentException("Passed string is not a valid localization key", nameof(localizationKey));

		    translations.Add(parsedLocalizationKey, value);
	    }
		
	    public string TryGetTranslation(LocalizationKey localizationKey)
	    {
		    translations.TryGetValue(localizationKey, out var value);
		    return value;
	    }
    }
}
