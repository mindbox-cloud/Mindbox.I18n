using System;
using System.Collections.Generic;
using System.IO;

namespace Mindbox.I18n
{
	public class LocalizationProvider
	{
		public InitializationOptions InitializationOptions { get; }

		private readonly ITranslationSource translationSource;

		public LocalizationProvider(InitializationOptions options)
		{
			InitializationOptions = options;
			translationSource = options.TranslationSource;
			translationSource.Initialize();

			CheckInitializationOptions();
		}

		public string TryGetTranslation(Locale locale, string key)
		{
			var localizationKey = LocalizationKey.TryParse(key);
			if (localizationKey == null)
			{
				InitializationOptions.Logger.LogInvalidKey(key);
				return null;
			}

			return translationSource.TryGetTranslation(locale, localizationKey);
		}
		public string TryGetTranslation(Locale locale, LocalizationKey key)
		{
			return translationSource.TryGetTranslation(locale, key);
		}

		public string Translate(Locale locale, string key)
		{
			return TryGetTranslation(locale, key) ?? key;
		}

		private void CheckInitializationOptions()
		{
			if (InitializationOptions.Logger == null)
				throw new InvalidOperationException($"{nameof(InitializationOptions)} is null");
		}
	}
}