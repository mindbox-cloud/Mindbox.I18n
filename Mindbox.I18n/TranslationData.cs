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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Mindbox.I18n.Abstractions;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Mindbox.I18n;

#pragma warning disable IDE0290
#pragma warning disable IDE0028
internal class TranslationData
{
	private readonly ILocale _locale;
	private readonly ILogger _logger;

	public TranslationData(ILocale locale, ILogger logger)
	{
		_locale = locale;
		_logger = logger;
	}

	private readonly ConcurrentDictionary<string, TranslationSet> _translationSetsByNamespace =
		new();

	public void AddOrUpdateNamespace(string @namespace, string filePath)
	{
		_translationSetsByNamespace.AddOrUpdate(
			@namespace,
			ns => new TranslationSet(filePath, _logger),
			(ns, oldSet) => new TranslationSet(filePath, _logger));
	}

	internal string? TryGetTranslation(LocalizationKey localizationKey)
	{
		if (!_translationSetsByNamespace.TryGetValue(localizationKey.Namespace, out var translationSet))
		{
			MissingNamespaceLog(localizationKey.Namespace, localizationKey.FullKey);
			return null;
		}

		if (translationSet.Data.Value.TryGetValue(localizationKey.FullKey, out var translation))
			return translation;

		MissingKeyLog(localizationKey.Namespace, localizationKey.FullKey);

		return null;
	}

	private void MissingKeyLog(string @namespace, string key) =>
		_logger.LogError($"Key \"{key}\" was not found in namespace \"{@namespace}\" for locale \"{_locale.Name}\".");

	private void MissingNamespaceLog(string @namespace, string key) =>
		_logger.LogError($"Namespace \"{@namespace}\" was not found for key \"{key}\" for locale \"{_locale.Name}\".");

	private class TranslationSet
	{
		public Lazy<Dictionary<string, string>> Data { get; }

		public TranslationSet(string filePath, ILogger logger)
		{
			Data = new Lazy<Dictionary<string, string>>(
				() =>
				{
					try
					{
						using var stream = File.OpenRead(filePath);
						return JsonSerializer.Deserialize<Dictionary<string, string>>(stream)
							?? throw new InvalidOperationException(nameof(stream));
					}
					catch (Exception e)
					{
						logger.LogError(e, $"Error loading file {filePath}");
						return new Dictionary<string, string>();
					}
				});
		}
	}
}