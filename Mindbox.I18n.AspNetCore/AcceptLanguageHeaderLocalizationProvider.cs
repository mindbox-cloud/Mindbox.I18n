using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Net.Http.Headers;

namespace Mindbox.I18n.AspNetCore;

/// <summary>
/// Провайдер получающий язык пользователя на основе http заголовка "Accept-Language".
/// Создан на основе <see cref="AcceptLanguageHeaderRequestCultureProvider"/>.
/// </summary>
public class AcceptLanguageHeaderLocalizationProvider : IRequestLocalizationProvider
{
	private static readonly Task<Locale?> nullProviderCultureResult = Task.FromResult((Locale?)null);

	/// <summary>
	/// Максимальное число значений из Accept-Language заголовка для попыток получения локализации.
	/// По умолчанию <c>3</c>.
	/// </summary>
	public int MaximumAcceptLanguageHeaderValuesToTry { get; init; } = 3;

	public Task<Locale?> TryGetLocale(HttpContext httpContext)
	{
		var acceptLanguageHeader = httpContext.Request.GetTypedHeaders().AcceptLanguage;

		if (acceptLanguageHeader.Count == 0)
		{
			return nullProviderCultureResult;
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
			var locale = Locales.TryGetByName(language.Value);
			if (locale != null)
			{
				return Task.FromResult<Locale?>(locale);
			}
		}

		return nullProviderCultureResult;
	}
}