using System;
using Mindbox.I18n.Abstractions;

namespace Mindbox.I18n;

public class LocalizationProvider : ILocalizationProvider
{
	public InitializationOptions InitializationOptions { get; }

	private readonly ITranslationSource _translationSource;

	public LocalizationProvider(InitializationOptions options)
	{
		InitializationOptions = options;
		_translationSource = options.TranslationSource;
		_translationSource.Initialize();

		if (InitializationOptions.Logger == null)
			throw new InvalidOperationException($"{nameof(InitializationOptions)} is null");
	}

	public string TryGetTranslation(ILocale locale, string key)
	{
		try
		{
			var localizationKey = LocalizationKey.TryParse(key);
			if (localizationKey == null)
			{
				InitializationOptions.Logger.LogInvalidKey(key);
				return null;
			}
			return _translationSource.TryGetTranslation(locale, localizationKey);
		}
		catch (Exception e)
		{
			InitializationOptions.Logger.LogError(e, $"Error occured while translating key {key}");
			return null;
		}
	}

	public string TryGetTranslation(ILocale locale, LocalizationKey key)
	{
		try
		{
			return _translationSource.TryGetTranslation(locale, key);
		}
		catch (Exception e)
		{
			InitializationOptions.Logger.LogError(e, $"Error occured while translating key {key.FullKey}");
			return null;
		}
	}

	public string Translate(ILocale locale, string key)
	{
		return TryGetTranslation(locale, key) ?? key;
	}
}