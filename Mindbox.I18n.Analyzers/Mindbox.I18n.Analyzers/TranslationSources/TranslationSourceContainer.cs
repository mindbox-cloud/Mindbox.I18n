// Copyright 2022 Mindbox Ltd
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Mindbox.I18n.Analyzers;
#nullable disable
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
#pragma warning disable CA1309
				StringComparison.InvariantCultureIgnoreCase));
#pragma warning restore CA1309
	}
}