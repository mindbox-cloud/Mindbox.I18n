using Microsoft.AspNetCore.Http;

namespace Mindbox.I18n.AspNetCore;

/// <summary>
/// Провайдер, позволяющий получить данные по языку в виде <see cref="Locale"/> на основе <see cref="HttpRequest"/>.
/// </summary>
public interface IRequestLocalizationProvider
{
	/// <summary>
	/// Получает <see cref="Locale"/>.
	/// </summary>
	/// <param name="httpContext"> <see cref="HttpContext"/> запроса. </param>
	/// <returns>
	///     Полученная <see cref="Locale"/>.
	///     Возвращает <c>null</c> если определить язык не удалось.
	/// </returns>
	Task<Locale?> TryGetLocaleAsync(HttpContext httpContext);
}