using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mindbox.I18n;

public abstract class FileSystemTranslationSourceBase : ITranslationSource
{
	protected const string TranslationFileSuffix = ".i18n.json";

	private static readonly Regex _translationFileRegex = new(
		$@"([^\\\/]+)\.([^\\\/]+){Regex.Escape(TranslationFileSuffix)}$",
		RegexOptions.IgnoreCase | RegexOptions.Compiled);

	private readonly Dictionary<string, TranslationData> _translationsPerLocale;
	protected ILogger Logger { get; }

	protected FileSystemTranslationSourceBase(IReadOnlyList<Locale> supportedLocales, ILogger logger)
	{
		Logger = logger;
		_translationsPerLocale = supportedLocales.ToDictionary(
			locale => locale.Name,
			locale => new TranslationData(locale, Logger));
	}

	public virtual void Initialize()
	{
		LoadTranslationFiles();
	}

	protected void LoadTranslationFiles()
	{
		var translationFiles = GetTranslationFiles()
			.Select(PathHelpers.ConvertToUnixPath)
			.ToList();

		foreach (var translationFile in translationFiles)
		{
			LoadTranslationFile(translationFile);
		}
	}

	protected void LoadTranslationFile(string translationFile)
	{
		var translationFileRegexMatch = _translationFileRegex.Match(translationFile);
		if (translationFileRegexMatch.Success)
		{
			var @namespace = translationFileRegexMatch.Groups[1].Value;
			var localeName = translationFileRegexMatch.Groups[2].Value;

			if (_translationsPerLocale.TryGetValue(localeName, out var translationData))
				translationData.AddOrUpdateNamespace(@namespace, translationFile);
		}
	}

	protected abstract IEnumerable<string> GetTranslationFiles();

	public string TryGetTranslation(Locale locale, LocalizationKey localizationKey)
	{
		if (!_translationsPerLocale.TryGetValue(locale.Name, out var translationData))
		{
			Logger.LogMissingLocale(locale, localizationKey.FullKey);
			return null;
		}

		return translationData.TryGetTranslation(localizationKey);
	}
}