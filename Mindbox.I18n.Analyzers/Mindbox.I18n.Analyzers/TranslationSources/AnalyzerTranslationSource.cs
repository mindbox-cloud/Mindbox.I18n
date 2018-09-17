using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mindbox.I18n.Analyzers
{
	internal class AnalyzerTranslationSource : IAnalyzerTranslationSource
	{
		private readonly ITranslationSource translationSource;
		private readonly Locale locale;

		public string TryGetTranslation(LocalizationKey key)
		{
			return translationSource.TryGetTranslation(locale, key);
		}

		public AnalyzerTranslationSource(string configurationFilePath)
		{
			using (var streamReader = new StreamReader(configurationFilePath))
			using (var jsonReader = new JsonTextReader(streamReader))
			{
				var configuration = JObject.Load(jsonReader);

				var translationSourceSettings = configuration
					.GetValue("translationSource").Value<JObject>();
				
				var relativePath = translationSourceSettings
					.GetValue("baseDirectory").Value<string>();

				var localeName = translationSourceSettings
					.GetValue("locale").Value<string>();

				IReadOnlyList<string> ignoredPaths = Array.Empty<string>();
				if (translationSourceSettings.TryGetValue("ignorePaths", out var ignorePathsSettings))
				{
					ignoredPaths = ((JArray)ignorePathsSettings)
						.Values<string>()
						.ToArray();
				}

				var baseDirectory = Path.Combine(
					Path.GetDirectoryName(configurationFilePath),
					relativePath);

				locale = Locales.GetByName(localeName);
				translationSource = new WatchingFileSystemTranslationSource(baseDirectory, new [] { locale }, ignoredPaths);
				translationSource.Initialize();
			}
		}
	}
}
