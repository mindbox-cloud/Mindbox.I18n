using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mindbox.I18n.Abstractions;

namespace Mindbox.I18n;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDefaultLocalizationProvider(
		this IServiceCollection services,
		Mindbox.I18n.Abstractions.ILogger? loggerOverride = null)
	{
		services.AddSingleton(sp => CreateLocalizationProvider(sp, loggerOverride));

		return services;
	}

	private static LocalizationProvider CreateLocalizationProvider(
		IServiceProvider serviceProvider,
		Mindbox.I18n.Abstractions.ILogger? loggerOverride = null)
	{
		var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		var localizationDirectory = Path.Combine(assemblyDirectory!, "Resources", "Localization");

		var supportedLocales = new[]
		{
			Locales.ruRU,
			Locales.enUS
		};

		var localizationLogger = loggerOverride
		                         ?? new DefaultLocalizationLogger(serviceProvider
			                         .GetRequiredService<ILogger<DefaultLocalizationLogger>>());

		LocalizableString.InitializeLogger(localizationLogger);

		var translationSource = new DiscoveringFileSystemTranslationSource(
			localizationDirectory,
			supportedLocales,
			Array.Empty<string>(),
			localizationLogger);

		return new LocalizationProviderBuilder()
			.WithTranslationSource(translationSource)
			.WithLogger(localizationLogger)
			.Build();
	}
}
