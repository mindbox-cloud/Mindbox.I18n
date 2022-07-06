namespace Mindbox.I18n.Analyzers;

internal interface IAnalyzerTranslationSource
{
	string TryGetTranslation(LocalizationKey key);
}