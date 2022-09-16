// Copyright 2022 Mindbox Ltd
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Mindbox.I18n.Abstractions;

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