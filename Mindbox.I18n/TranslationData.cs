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
		private readonly ConcurrentDictionary<string, TranslationSet> translationSetsByNamespace =
			new ConcurrentDictionary<string, TranslationSet>();

		public void AddNamespace(string @namespace, string filePath)
		{
			translationSetsByNamespace.TryAdd(@namespace, new TranslationSet(filePath));
		}

		internal string TryGetTranslation(LocalizationKey localizationKey)
		{
			if (translationSetsByNamespace.TryGetValue(localizationKey.Namespace, out var translationSet))
			{
				if (translationSet.Data.Value.TryGetValue(localizationKey.FullKey, out string value))
					return value;
			}

			return null;
		}

		private class TranslationSet
		{
			public string FilePath { get; }

			public Lazy<Dictionary<string, string>> Data { get; }

			public TranslationSet(string filePath)
			{
				FilePath = filePath;
				Data = new Lazy<Dictionary<string, string>>(
					() =>
					{
						using (var streamReader = new StreamReader(filePath))
						using (var jsonReader = new JsonTextReader(streamReader))
						{
							return JObject.Load(jsonReader).ToObject<Dictionary<string, string>>();
						}
					});
			}
		}
	}
}