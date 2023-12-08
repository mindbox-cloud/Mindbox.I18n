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

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Mindbox.I18n.Abstractions;

namespace Mindbox.I18n;

public abstract class FileSystemTranslationSourceBase : ITranslationSource
{
	protected const string TranslationFileSuffix = ".i18n.json";

	private static readonly Regex _translationFileRegex = new(
		$@"(?<namespace>[^\\\/.]+)(?:\.[^\\\/.]+)*\.(?<locale>[^\\\/]+){Regex.Escape(TranslationFileSuffix)}$",
		RegexOptions.IgnoreCase | RegexOptions.Compiled);

	private readonly Dictionary<string, TranslationData> _translationsPerLocale;
	protected ILogger Logger { get; }

	protected FileSystemTranslationSourceBase(
		IReadOnlyList<ILocale> supportedLocales,
		ILogger logger)
	{
		Logger = logger;
		_translationsPerLocale = supportedLocales.ToDictionary(
			locale => locale.Name,
			locale => new TranslationData(locale, logger));
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

		if (!translationFileRegexMatch.Success)
			return;

		var @namespace = translationFileRegexMatch.Groups["namespace"].Value;
		var localeName = translationFileRegexMatch.Groups["locale"].Value;

		if (_translationsPerLocale.TryGetValue(localeName, out var translationData))
			translationData.AddOrUpdateNamespace(@namespace, translationFile);
	}

	protected abstract IEnumerable<string> GetTranslationFiles();

	public string? TryGetTranslation(ILocale locale, LocalizationKey localizationKey)
	{
		if (_translationsPerLocale.TryGetValue(locale.Name, out var translationData))
			return translationData.TryGetTranslation(localizationKey);

		Logger.LogError($"Locale \"{locale.Name}\" was not found for key \"{localizationKey.FullKey}\".");
		return null;
	}
}