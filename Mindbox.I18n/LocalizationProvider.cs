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
using Microsoft.Extensions.Logging;
using Mindbox.I18n.Abstractions;

namespace Mindbox.I18n;

internal class LocalizationProvider : ILocalizationProvider
{
	public InitializationOptions InitializationOptions { get; }

	private readonly ITranslationSource _translationSource;

	public LocalizationProvider(InitializationOptions options)
	{
		InitializationOptions = options;
		_translationSource = options.TranslationSource;
		_translationSource.Initialize();

		if (InitializationOptions.Logger == null)
			throw new InvalidOperationException($"{nameof(InitializationOptions)} is null");
	}

	public string? TryGetTranslation(ILocale locale, string key)
	{
		try
		{
			var localizationKey = LocalizationKey.TryParse(key);
			if (localizationKey == null)
			{
				InitializationOptions.Logger.LogError($"Key \"{key}\" is not a valid key.");
				return null;
			}
			return _translationSource.TryGetTranslation(locale, localizationKey);
		}
		catch (Exception e)
		{
			InitializationOptions.Logger.LogError(e, $"Error occured while translating key {key}");
			return null;
		}
	}

	public string? TryGetTranslation(ILocale locale, LocalizationKey key)
	{
		try
		{
			return _translationSource.TryGetTranslation(locale, key);
		}
		catch (Exception e)
		{
			InitializationOptions.Logger.LogError(e, $"Error occured while translating key {key.FullKey}");
			return null;
		}
	}

	public string Translate(ILocale locale, string key)
	{
		return TryGetTranslation(locale, key) ?? key;
	}
}