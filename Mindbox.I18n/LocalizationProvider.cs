using System;

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

			if (InitializationOptions.Logger == null)
				throw new InvalidOperationException($"{nameof(InitializationOptions)} is null");
		}

		public string TryGetTranslation(Locale locale, string key)
		{
			try
			{
				var localizationKey = LocalizationKey.TryParse(key);
				if (localizationKey == null)
				{
					InitializationOptions.Logger.LogInvalidKey(key);
					return null;
				}
				return translationSource.TryGetTranslation(locale, localizationKey);
			}
			catch (Exception e)
			{
				InitializationOptions.Logger.LogError(e, $"Error occured while translating key {key}");
				return null;
			}
		}

		public string TryGetTranslation(Locale locale, LocalizationKey key)
		{
			try
			{
				return translationSource.TryGetTranslation(locale, key);
			}
			catch (Exception e)
			{
				InitializationOptions.Logger.LogError(e, $"Error occured while translating key {key.FullKey}");
				return null;
			}
		}

		public string Translate(Locale locale, string key)
		{
			return TryGetTranslation(locale, key) ?? key;
		}
	}
}