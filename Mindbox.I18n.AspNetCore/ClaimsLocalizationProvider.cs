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

using Microsoft.AspNetCore.Http;
using Mindbox.I18n.Abstractions;

namespace Mindbox.I18n.AspNetCore;

/// <summary>
/// Провайдер получающий язык пользователя из Claims пользователя.
/// </summary>
public class ClaimsLocalizationProvider : IRequestLocalizationProvider
{
	private readonly string _keyOfLocaleClaim;
	public const string DefaultLocaleKey = "Locale";

	public ClaimsLocalizationProvider(string? keyOfLocaleClaim = null)
	{
		_keyOfLocaleClaim = keyOfLocaleClaim ?? DefaultLocaleKey;
	}

	public async Task<ILocale?> TryGetLocale(HttpContext httpContext)
	{
		var languageFromToken = httpContext.User.FindFirst(_keyOfLocaleClaim)?.Value;

		if (string.IsNullOrWhiteSpace(languageFromToken))
		{
			return null;
		}

		var locale = Locales.TryGetByName(languageFromToken);

		return locale;
	}
}