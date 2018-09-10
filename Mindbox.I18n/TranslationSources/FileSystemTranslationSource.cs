using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mindbox.I18n
{
	public class FileSystemTranslationSource : ITranslationSource
	{
		protected const string TranslationFileSuffix = ".i18n.json";

		private static readonly Regex TranslationFileRegex = new Regex(
			$@"([^\\\/]+)\.([^\\\/]+){Regex.Escape(TranslationFileSuffix)}$",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);

		protected readonly string BaseDirectory;
		private readonly IReadOnlyList<Locale> supportedLocales;
		private readonly IReadOnlyList<string> ignoredPathRules;

		private Dictionary<string, TranslationData> translationsPerLocale;

		public FileSystemTranslationSource(
			string baseDirectory,
			IReadOnlyList<Locale> supportedLocales,
			IReadOnlyList<string> ignoredPathRules)
		{
			BaseDirectory = baseDirectory;
			this.supportedLocales = supportedLocales;
			this.ignoredPathRules = ignoredPathRules;
		}

		public virtual void Initialize()
		{
			LoadTranslationFiles();
		}

		protected void LoadTranslationFiles()
		{
			translationsPerLocale = supportedLocales.ToDictionary(
				locale => locale.Name,
				locale => new TranslationData());

			var translationFiles = Directory.GetFiles(
					BaseDirectory,
					$"*{TranslationFileSuffix}",
					SearchOption.AllDirectories)
				.Where(path => !ignoredPathRules.Any(
					ignoredPart => path.IndexOf(ignoredPart, StringComparison.InvariantCultureIgnoreCase) > 0))
				.ToList();

			foreach (var translationFile in translationFiles)
			{
				var translationFileRegexMatch = TranslationFileRegex.Match(translationFile);
				if (translationFileRegexMatch.Success)
				{
					var @namespace = translationFileRegexMatch.Groups[1].Value;
					var localeName = translationFileRegexMatch.Groups[2].Value;

					if (translationsPerLocale.TryGetValue(localeName, out TranslationData translationData))
						translationData.AddNamespace(@namespace, translationFile);
				}
			}
		}

		public string TryGetTranslation(Locale locale, LocalizationKey localizationKey)
		{
			if (!translationsPerLocale.TryGetValue(locale.Name, out var translationData))
				return null;

			return translationData.TryGetTranslation(localizationKey);
		}
	}
}