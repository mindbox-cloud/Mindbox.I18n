using System.IO;
using Mindbox.i18n.Abstractions;
using Newtonsoft.Json;

namespace Mindbox.I18n.Analyzers;

internal class AnalyzerTranslationSource : IAnalyzerTranslationSource
{
	private readonly ITranslationSource _translationSource;
	private readonly ILocale _locale;

	public string TryGetTranslation(LocalizationKey key)
	{
		return _translationSource.TryGetTranslation(_locale, key);
	}

	public AnalyzerTranslationSource(string configurationFilePath)
	{
		var configuration = JsonConvert.DeserializeObject<AnalysisSettingsConfiguration>(
			File.ReadAllText(configurationFilePath));

		var solutionFilePath = Path.Combine(
			Path.GetDirectoryName(configurationFilePath),
			configuration.TranslationSource.SolutionFilePath);

		_locale = Locales.GetByName(configuration.TranslationSource.Locale);
		_translationSource = new AnalyzerFileSystemTranslationSource(
			solutionFilePath, new[] { _locale }, new NullI18NextLogger());
		_translationSource.Initialize();
	}
}