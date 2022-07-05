using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mindbox.I18n.AspNetCore;

public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Добавляет сервисы необходимые для использования локализации Mindbox.
	/// </summary>
	/// <param name="services"> Сервисы. </param>
	/// <returns> <paramref name="services"/>. </returns>
	public static IServiceCollection AddI18nRequestLocalization(this IServiceCollection services)
	{
		services.TryAddSingleton<ILocalizationContextAccessor, LocalizationContextAccessor>();
		services.TryAddTransient<IRequestLocalizationProvider, AcceptLanguageHeaderLocalizationProvider>();

		return services;
	}
}