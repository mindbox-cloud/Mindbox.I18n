using Microsoft.AspNetCore.Builder;

namespace Mindbox.I18n.AspNetCore;

/// <summary>
/// Методы расширения для добавления <see cref="MindboxRequestLocalizationMiddleware"/> в приложение.
/// </summary>
public static class ApplicationBuilderExtensions
{
	/// <summary>
	/// Добавляет <see cref="MindboxRequestLocalizationMiddleware"/> для автоматического получения языка
	/// пользователя и проекта на основе данных из http запроса.
	/// </summary>
	/// <param name="app"> <see cref="IApplicationBuilder"/>. </param>
	/// <returns> <paramref name="app"/>. </returns>
	public static IApplicationBuilder UseI18nRequestLocalization(this IApplicationBuilder app)
	{
		return app.UseMiddleware<MindboxRequestLocalizationMiddleware>();
	}
}