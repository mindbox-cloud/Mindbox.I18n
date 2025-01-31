using Microsoft.AspNetCore.Http;
using Mindbox.I18n.Abstractions;

namespace Mindbox.I18n.AspNetCore;

/// <summary>
/// Провайдер получающий язык пользователя из токена.
/// </summary>
public class TokenLocalizationProvider : IRequestLocalizationProvider
{
	private static readonly Task<ILocale?> _nullProviderCultureResult = Task.FromResult((ILocale?)null);

	public Task<ILocale?> TryGetLocale(HttpContext httpContext)
	{
		var languageFromToken = httpContext.User.FindFirst(Constants.LocaleKey)?.Value;

		if (string.IsNullOrWhiteSpace(languageFromToken))
		{
			return _nullProviderCultureResult;
		}

		var locale = Locales.TryGetByName(languageFromToken);

		return Task.FromResult(locale);
	}
}