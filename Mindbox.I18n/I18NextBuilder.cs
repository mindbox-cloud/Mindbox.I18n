using System;
using System.Collections.Generic;
using System.Linq;

namespace Mindbox.I18n
{
	public class I18NextBuilder
	{
		private readonly HashSet<string> supportedLanguages = new HashSet<string>();
		private ILogger logger;
		private ITranslationSource translationSource;

		public I18NextBuilder WithSupportedLanguage(string language)
		{
			supportedLanguages.Add(language);
			return this;
		}

		public I18NextBuilder WithLogger(ILogger logger)
		{
			if (this.logger != null)
				throw new InvalidOperationException("You can't set logger twice");

			this.logger = logger;
			return this;
		}

		public I18NextBuilder WithTranslationSource(ITranslationSource translationSource)
		{
			if (translationSource == null)
				throw new ArgumentNullException(nameof(translationSource));
			if (this.translationSource != null)
				throw new InvalidOperationException("You can't set translation source twice");

			this.translationSource = translationSource;
			return this;
		}

		public I18Next Build()
		{
			return new I18Next(new InitializationOptions
			{
				SupportedLanguages = supportedLanguages.ToList(),
				TranslationSource = translationSource,
				Logger = logger
			});
		}
	}
}