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

namespace Mindbox.I18n;

internal class TranslationData
{
	private readonly ILocale _locale;

	public TranslationData(ILocale locale)
	{
		_locale = locale;
	}

	private readonly ConcurrentDictionary<string, TranslationSet> _translationSetsByNamespace =
		new();

	public void AddOrUpdateNamespace(string @namespace, string filePath)
	{
		_translationSetsByNamespace.AddOrUpdate(
			@namespace,
			ns => new TranslationSet(filePath),
			(ns, oldSet) => new TranslationSet(filePath));
	}

	internal bool TryGetTranslation(LocalizationKey localizationKey, out string? translation, out Exception? exception)
	{
		translation = null;
		exception = default;

		if (!_translationSetsByNamespace.TryGetValue(localizationKey.Namespace, out var translationSet))
		{
			exception = TranslationException.MissingNamespace(_locale.Name, localizationKey.Namespace, localizationKey.FullKey);
			return false;
		}

		try
		{
			if (translationSet.Data.Value.TryGetValue(localizationKey.FullKey, out translation))
				return true;
		}
		catch (Exception e)
		{
			exception = e;
			return false;
		}

		exception = TranslationException.MissingKey(_locale.Name, localizationKey.Namespace, localizationKey.FullKey);

		return false;
	}

	private class TranslationSet
	{
		public Lazy<Dictionary<string, string>> Data { get; }

		public TranslationSet(string filePath)
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
						throw new TranslationException($"Error loading file {filePath}", e);
					}
				});
		}
	}
}