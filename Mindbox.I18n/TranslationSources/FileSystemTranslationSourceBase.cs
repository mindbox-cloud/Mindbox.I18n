using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mindbox.I18n
{
	public abstract class FileSystemTranslationSourceBase : ITranslationSource
	{
		protected const string TranslationFileSuffix = ".i18n.json";

		private static readonly Regex TranslationFileRegex = new Regex(
			$@"([^\\\/]+)\.([^\\\/]+){Regex.Escape(TranslationFileSuffix)}$",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private readonly Dictionary<string, TranslationData> translationsPerLocale;

		protected FileSystemTranslationSourceBase(IReadOnlyList<Locale> supportedLocales)
		{
			translationsPerLocale = supportedLocales.ToDictionary(
				locale => locale.Name,
				locale => new TranslationData());
		}

		public virtual void Initialize()
		{
			LoadTranslationFiles();
		}

		protected void LoadTranslationFiles()
		{
			var translationFiles = GetTranslationFiles();

			foreach (var translationFile in translationFiles)
			{
				LoadTranslationFile(translationFile);
			}
		}

		protected void LoadTranslationFile(string translationFile)
		{
			var translationFileRegexMatch = TranslationFileRegex.Match(translationFile);
			if (translationFileRegexMatch.Success)
			{
				var @namespace = translationFileRegexMatch.Groups[1].Value;
				var localeName = translationFileRegexMatch.Groups[2].Value;

				if (translationsPerLocale.TryGetValue(localeName, out TranslationData translationData))
					translationData.AddOrUpdateNamespace(@namespace, translationFile);
			}
		}

		protected abstract IEnumerable<string> GetTranslationFiles();

		public string TryGetTranslation(Locale locale, LocalizationKey localizationKey)
		{
			if (!translationsPerLocale.TryGetValue(locale.Name, out var translationData))
				return null;

			return translationData.TryGetTranslation(localizationKey);
		}
	}
}