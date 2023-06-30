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
using Microsoft.AspNetCore.Localization;
using Microsoft.Net.Http.Headers;
using Mindbox.I18n.Abstractions;

namespace Mindbox.I18n.AspNetCore;

/// <summary>
/// Провайдер получающий язык пользователя на основе http заголовка "Accept-Language".
/// Создан на основе <see cref="AcceptLanguageHeaderRequestCultureProvider"/>.
/// </summary>
public class AcceptLanguageHeaderLocalizationProvider : IRequestLocalizationProvider
{
	private static readonly Task<ILocale?> _nullProviderCultureResult = Task.FromResult((ILocale?)null);

	/// <summary>
	/// Максимальное число значений из Accept-Language заголовка для попыток получения локализации.
	/// По умолчанию <c>3</c>.
	/// </summary>
	public int MaximumAcceptLanguageHeaderValuesToTry { get; init; } = 3;

	public Task<ILocale?> TryGetLocale(HttpContext httpContext)
	{
		var acceptLanguageHeader = httpContext.Request.GetTypedHeaders().AcceptLanguage;

		if (acceptLanguageHeader.Count == 0)
		{
			return _nullProviderCultureResult;
		}

		var languages = acceptLanguageHeader.AsEnumerable();

		if (MaximumAcceptLanguageHeaderValuesToTry > 0)
		{
			// Для минимизации загрузки CPU на множестве попыток разбора языка,
			// обработаем только сконфигурированное число первых языков из заголовка и попробуем получить Locale
			languages = languages.Take(MaximumAcceptLanguageHeaderValuesToTry);
		}

		foreach (var language in languages
			.OrderByDescending(h => h, StringWithQualityHeaderValueComparer.QualityComparer)
			.Select(x => x.Value))
		{
			if (language.Value != null)
			{
				var locale = Locales.TryGetByName(language.Value);
				if (locale != null)
				{
					return Task.FromResult<ILocale?>(locale);
				}
			}
		}

		return _nullProviderCultureResult;
	}
}