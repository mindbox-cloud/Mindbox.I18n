using System;
using System.Collections.Generic;
using System.Linq;
using Mindbox.I18n.Abstractions;

namespace Mindbox.I18n;

public class LocalizationProviderBuilder
{
	private readonly HashSet<string> _supportedLanguages = new();
	private ILogger? _logger;
	private ITranslationSource? _translationSource;

	public LocalizationProviderBuilder WithSupportedLanguage(string language)
	{
		_supportedLanguages.Add(language);
		return this;
	}

	public LocalizationProviderBuilder WithLogger(ILogger logger)
	{
		if (_logger != null)
			throw new InvalidOperationException("You can't set logger twice");

		_logger = logger;
		return this;
	}

	public LocalizationProviderBuilder WithTranslationSource(ITranslationSource translationSource)
	{
		if (_translationSource != null)
			throw new InvalidOperationException("You can't set translation source twice");

		_translationSource = translationSource ?? throw new ArgumentNullException(nameof(translationSource));
		return this;
	}

	public LocalizationProvider Build()
	{
		return new LocalizationProvider(new InitializationOptions
		{
			SupportedLanguages = _supportedLanguages.ToList(),
			TranslationSource = _translationSource ?? throw new InvalidOperationException(nameof(_translationSource)),
			Logger = _logger ?? throw new InvalidOperationException(nameof(_logger))
		});
	}
}