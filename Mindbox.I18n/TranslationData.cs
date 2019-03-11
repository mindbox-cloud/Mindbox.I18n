using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mindbox.I18n
{
	internal class TranslationData
	{
		private readonly Locale locale;
		private readonly ILogger logger;

		public TranslationData(Locale locale, ILogger logger)
		{
			this.locale = locale;
			this.logger = logger;
		}

		private readonly ConcurrentDictionary<string, TranslationSet> translationSetsByNamespace =
			new ConcurrentDictionary<string, TranslationSet>();

		public void AddOrUpdateNamespace(string @namespace, string filePath)
		{
			translationSetsByNamespace.AddOrUpdate(
				@namespace,
				ns => new TranslationSet(filePath, logger),
				(ns, oldSet) => new TranslationSet(filePath, logger));
		}

		internal string TryGetTranslation(LocalizationKey localizationKey)
		{
			if (translationSetsByNamespace.TryGetValue(localizationKey.Namespace, out var translationSet))
			{
				if (translationSet.Data.Value.TryGetValue(localizationKey.FullKey, out string value))
				{
					return value;
				}
				else
				{
					logger.LogMissingKey(locale.Name, localizationKey.Namespace, localizationKey.FullKey);
				}
			}
			else
			{
				logger.LogMissingNamespace(localizationKey.Namespace, localizationKey.FullKey);
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

							using (var streamReader = new StreamReader(filePath))
							using (var jsonReader = new JsonTextReader(streamReader))
							{
								return JObject.Load(jsonReader).ToObject<Dictionary<string, string>>();
							}

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
}