using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Mindbox.I18n.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mindbox.I18n;

internal class TranslationData
{
	private readonly ILocale _locale;
	private readonly ILogger _logger;

	public TranslationData(ILocale locale, ILogger logger)
	{
		_locale = locale;
		_logger = logger;
	}

	private readonly ConcurrentDictionary<string, TranslationSet> _translationSetsByNamespace =
		new();

	public void AddOrUpdateNamespace(string @namespace, string filePath)
	{
		_translationSetsByNamespace.AddOrUpdate(
			@namespace,
			ns => new TranslationSet(filePath, _logger),
			(ns, oldSet) => new TranslationSet(filePath, _logger));
	}

	internal string TryGetTranslation(LocalizationKey localizationKey)
	{
		if (_translationSetsByNamespace.TryGetValue(localizationKey.Namespace, out var translationSet))
		{
			if (translationSet.Data.Value.TryGetValue(localizationKey.FullKey, out string value))
			{
				return value;
			}
			else
			{
				_logger.LogMissingKey(_locale, localizationKey.Namespace, localizationKey.FullKey);
			}
		}
		else
		{
			_logger.LogMissingNamespace(_locale, localizationKey.Namespace, localizationKey.FullKey);
		}

		return null;
	}

	private class TranslationSet
	{
		public string FilePath { get; }

		public Lazy<Dictionary<string, string>> Data { get; }

		public TranslationSet(string filePath, ILogger logger)
		{
			FilePath = filePath;
			Data = new Lazy<Dictionary<string, string>>(
				() =>
				{
					try
					{
						using var streamReader = new StreamReader(filePath);
						using var jsonReader = new JsonTextReader(streamReader);
						return JObject.Load(jsonReader).ToObject<Dictionary<string, string>>();
					}
					catch (Exception e)
					{
						logger.LogError(e, $"Error loading file {filePath}");
						return new Dictionary<string, string>();
					}
				});
		}
	}
}