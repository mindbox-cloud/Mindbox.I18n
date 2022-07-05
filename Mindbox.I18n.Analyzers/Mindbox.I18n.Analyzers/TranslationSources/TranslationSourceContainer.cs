using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Mindbox.I18n.Analyzers;

internal static class TranslationSourceContainer
{
	private const string ConfigurationFileName = "Mindbox.I18n.analysis-settings.json";

	private static readonly Dictionary<string, IAnalyzerTranslationSource> _translationSources
		= new(StringComparer.InvariantCultureIgnoreCase);

	private static readonly object _sourcesLockToken = new();

	public static IAnalyzerTranslationSource TryGetTranslationSourceFromAnalyzerOptions(AnalyzerOptions analyzerOptions)
	{
		var configurationFile = TryGetConfigurationFile(analyzerOptions);
		if (configurationFile == null)
			return null;

		string key = configurationFile.Path;

		lock (_sourcesLockToken)
		{
			_translationSources.TryGetValue(key, out var translationSource);
			if (translationSource == null)
			{
				translationSource = new AnalyzerTranslationSource(configurationFile.Path);
				_translationSources.Add(key, translationSource);
			}

			return translationSource;
		}
	}

	private static AdditionalText TryGetConfigurationFile(AnalyzerOptions analyzerOptions)
	{
		return analyzerOptions.AdditionalFiles
			.SingleOrDefault(file => Path.GetFileName(file.Path).Equals(
				ConfigurationFileName,
				StringComparison.InvariantCultureIgnoreCase));
	}
}