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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mindbox.I18n.Abstractions;

namespace Mindbox.I18n.AspNetCore;

public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Добавляет сервисы необходимые для использования локализации Mindbox.
	/// Это включает провайдеры локализации по заголовку языка и claim пользователя.
	/// </summary>
	/// <param name="services">
	/// Сервисы, к которым будут добавлены зависимости.
	/// </param>
	/// <param name="keyOfLocaleClaim">
	/// Ключ для локали в Claims пользователя. Если не задан, используется значение по умолчанию.
	/// </param>
	/// <returns>
	/// Возвращает объект <see cref="IServiceCollection"/> с добавленными сервисами.
	/// </returns>
	public static IServiceCollection AddI18nRequestLocalization(this IServiceCollection services, string? keyOfLocaleClaim = null)
	{
		services
			.AddLocalizationContextAccessor()
			.AddAcceptLanguageHeaderLocalizationProvider()
			.AddClaimsLocalizationProvider(keyOfLocaleClaim);

		return services;
	}

	/// <summary>
	/// Добавляет сервис для доступа к контексту локализации.
	/// </summary>
	/// <param name="services">
	/// Сервисы, к которым будет добавлен <see cref="ILocalizationContextAccessor"/>.
	/// </param>
	/// <returns>
	/// Возвращает объект <see cref="IServiceCollection"/> с добавленным контекстом локализации.
	/// </returns>
	public static IServiceCollection AddLocalizationContextAccessor(this IServiceCollection services)
	{
		services.TryAddSingleton<ILocalizationContextAccessor, LocalizationContextAccessor>();

		return services;
	}

	/// <summary>
	/// Добавляет провайдер локализации через заголовок <c>Accept-Language</c> HTTP-запроса.
	/// </summary>
	/// <param name="services">
	/// Сервисы, к которым будет добавлен <see cref="IRequestLocalizationProvider"/>.
	/// </param>
	/// <returns>
	/// Возвращает объект <see cref="IServiceCollection"/> с добавленным провайдером локализации.
	/// </returns>
	public static IServiceCollection AddAcceptLanguageHeaderLocalizationProvider(this IServiceCollection services)
	{
		services.TryAddTransient<IRequestLocalizationProvider, AcceptLanguageHeaderLocalizationProvider>();

		return services;
	}

	/// <summary>
	/// Добавляет провайдер локализации для извлечения локали из claim пользователя.
	/// </summary>
	/// <param name="services">
	/// Сервисы, к которым будет добавлен <see cref="IRequestLocalizationProvider"/>.
	/// </param>
	/// <param name="keyOfLocaleClaim">
	/// Ключ для локали в claim пользователя. Если не задан, используется значение по умолчанию.
	/// </param>
	/// <returns>
	/// Возвращает объект <see cref="IServiceCollection"/> с добавленным провайдером локализации.
	/// </returns>
	public static IServiceCollection AddClaimsLocalizationProvider(
		this IServiceCollection services,
		string? keyOfLocaleClaim = null)
	{
		services.TryAddTransient<IRequestLocalizationProvider>(_ => new ClaimsLocalizationProvider(keyOfLocaleClaim));

		return services;
	}
}