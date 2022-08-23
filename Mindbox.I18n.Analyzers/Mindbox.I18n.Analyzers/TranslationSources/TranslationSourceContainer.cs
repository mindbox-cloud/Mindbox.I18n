using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Mindbox.I18n.Analyzers;
#nullable disable
internal static class TranslationSourceContainer
{
	private const string ConfigurationFileName = "Mindbox.I18n.analysis-settings.json";

	private static readonly Dictionary<string, IAnalyzerTranslationSource> _translationSources
		= new(StringComparer.InvariantCultureIgnoreCase);

	private static readonly object _sourcesLockToken = new();

	public static IAnalyzerTranslationSource TryGetTranslationSource()
	{
		var configurationFilePath = TryDiscoverAnalyzerConfigFilePath();
		if (configurationFilePath == null)
			return null;

		lock (_sourcesLockToken)
		{
			_translationSources.TryGetValue(configurationFilePath, out var translationSource);
			if (translationSource == null)
			{
				translationSource = new AnalyzerTranslationSource(configurationFilePath);
				_translationSources.Add(configurationFilePath, translationSource);
			}

			return translationSource;
		}
	}

	private static string TryDiscoverAnalyzerConfigFilePath()
	{
		var entrypoints = new[]
		{
			Directory.GetCurrentDirectory(), Assembly.GetExecutingAssembly().Location,
			AppContext.BaseDirectory
		};

		HashSet<string> visitedPaths = new();
		foreach (var entrypoint in entrypoints)
		{
			var currentPath = entrypoint;
			while (currentPath != null)
			{
				if (visitedPaths.Contains(currentPath))
					break;

				var possibleConfigFileName = Path.Combine(currentPath, ConfigurationFileName);
				if (File.Exists(possibleConfigFileName))
				{
					return possibleConfigFileName;
				}

				visitedPaths.Add(currentPath);
				currentPath = Directory.GetParent(currentPath)?.FullName;
			}
		}

		return null;
	}
}