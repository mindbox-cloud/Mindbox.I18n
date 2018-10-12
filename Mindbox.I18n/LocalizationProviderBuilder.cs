using System;
using System.Collections.Generic;
using System.Linq;

namespace Mindbox.I18n
{
	public class LocalizationProviderBuilder
	{
		private readonly HashSet<string> supportedLanguages = new HashSet<string>();
		private ILogger logger;
		private ITranslationSource translationSource;

		public LocalizationProviderBuilder WithSupportedLanguage(string language)
		{
			supportedLanguages.Add(language);
			return this;
		}

		public LocalizationProviderBuilder WithLogger(ILogger logger)
		{
			if (this.logger != null)
				throw new InvalidOperationException("You can't set logger twice");

			this.logger = logger;
			return this;
		}

		public LocalizationProviderBuilder WithTranslationSource(ITranslationSource translationSource)
		{
			if (translationSource == null)
				throw new ArgumentNullException(nameof(translationSource));
			if (this.translationSource != null)
				throw new InvalidOperationException("You can't set translation source twice");

			this.translationSource = translationSource;
			return this;
		}

		public LocalizationProvider Build()
		{
			return new LocalizationProvider(new InitializationOptions
			{
				SupportedLanguages = supportedLanguages.ToList(),
				TranslationSource = translationSource,
				Logger = logger
			});
		}
	}
}