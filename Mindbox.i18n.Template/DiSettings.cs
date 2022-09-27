using System;
using Microsoft.Extensions.DependencyInjection;
using Mindbox.I18n.Abstractions;

namespace Mindbox.I18n.Template;

public static class DiSettings
{
	public static IServiceCollection AddDefaultLocalizer(this IServiceCollection services) =>
		services.AddSingleton<ILocalizer, Localizer>();

	public static IServiceCollection AddDefaultLocalizer(
		this IServiceCollection services,
		Func<IServiceProvider, Localizer> settings) =>
		services.AddSingleton<ILocalizer, Localizer>(settings);
}