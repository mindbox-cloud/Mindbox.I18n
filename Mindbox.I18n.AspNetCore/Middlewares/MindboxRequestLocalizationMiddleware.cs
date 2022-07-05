using Microsoft.AspNetCore.Http;

namespace Mindbox.I18n.AspNetCore;

/// <summary>
/// Добавляет настройки локализации пользователя и проекта в контекст запроса основываясь на информации из http запроса.
/// </summary>
internal class MindboxRequestLocalizationMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILocalizationContextAccessor _accessor;
	private readonly IRequestLocalizationProvider[] _localizationProviders;

	public MindboxRequestLocalizationMiddleware(
		RequestDelegate next,
		IEnumerable<IRequestLocalizationProvider> localizationProviders,
		ILocalizationContextAccessor accessor)
	{
		_next = next;
		_localizationProviders = localizationProviders.ToArray();
		_accessor = accessor;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		// язык пользователя
		Locale? userLocale = null;
		foreach (var provider in _localizationProviders)
		{
			userLocale = await provider.TryGetLocaleAsync(context);
			if (userLocale != null)
			{
				break;
			}
		}

		_accessor.Context ??= new LocalizationContext();
		_accessor.Context.UserLocale = userLocale;

		await _next(context);
	}
}