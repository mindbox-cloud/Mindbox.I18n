using Microsoft.AspNetCore.Http;

namespace Mindbox.I18n.AspNetCore;

/// <summary>
/// Добавляет настройки локализации пользователя и проекта в контекст запроса основываясь на информации из http запроса.
/// </summary>
internal class MindboxRequestLocalizationMiddleware
{
	private readonly RequestDelegate next;
	private readonly IRequestLocalizationContextAccessor accessor;
	private readonly IRequestLocalizationProvider[] localizationProviders;

	public MindboxRequestLocalizationMiddleware(
		RequestDelegate next,
		IEnumerable<IRequestLocalizationProvider> localizationProviders,
		IRequestLocalizationContextAccessor accessor)
	{
		this.next = next;
		this.localizationProviders = localizationProviders.ToArray();
		this.accessor = accessor;
	}

	public async Task Invoke(HttpContext context)
	{
		// язык пользователя
		Locale? userLocale = null;
		foreach (var provider in localizationProviders)
		{
			userLocale = await provider.TryGetLocale(context);
			if (userLocale != null)
			{
				break;
			}
		}

		accessor.Context ??= new RequestLocalizationContext();
		accessor.Context.UserLocale = userLocale;

		await next(context);
	}
}