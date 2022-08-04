using Microsoft.AspNetCore.Http;
using Mindbox.I18n.Abstractions;

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

#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
	public async Task Invoke(HttpContext context)
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
	{
		// язык пользователя
		ILocale? userLocale = null;
		foreach (var provider in _localizationProviders)
		{
			userLocale = await provider.TryGetLocale(context);
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