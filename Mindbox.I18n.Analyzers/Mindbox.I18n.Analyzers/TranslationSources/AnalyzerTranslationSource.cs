using System.IO;
using Newtonsoft.Json;

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
			var configuration = JsonConvert.DeserializeObject<AnalysisSettingsConfiguration>(
				File.ReadAllText(configurationFilePath));

			var solutionFilePath = Path.Combine(
				Path.GetDirectoryName(configurationFilePath),
				configuration.TranslationSource.SolutionFilePath);

			locale = Locales.GetByName(configuration.TranslationSource.Locale);
			translationSource = new AnalyzerFileSystemTranslationSource(
				solutionFilePath, new [] { locale }, new NullI18NextLogger());
			translationSource.Initialize();
		}
	}
}
