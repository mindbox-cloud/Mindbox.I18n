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

using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Mindbox.I18n.Abstractions;
using Mindbox.I18n.Analyzers.Configuration;
using Mindbox.I18n.Analyzers.Logging;

namespace Mindbox.I18n.Analyzers;
#nullable disable
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
		var configuration = JsonSerializer.Deserialize<AnalysisSettingsConfiguration>(
			File.ReadAllText(configurationFilePath));

		ILogger logger = configuration.Logging?.Enabled == true
			? new TextFileLogger(Path.Combine(Path.GetDirectoryName(configurationFilePath), configuration.Logging.LogPath))
			: NullLogger.Instance;

		logger.LogInformation("Analyzer translation source created.");

		var solutionFilePath = Path.Combine(
			Path.GetDirectoryName(configurationFilePath),
			configuration.TranslationSource.SolutionFilePath);

		_locale = Locales.GetByName(configuration.TranslationSource.Locale);
		_translationSource = new AnalyzerFileSystemTranslationSource(
			solutionFilePath, new[] { _locale }, logger);
		_translationSource.Initialize();
	}
}