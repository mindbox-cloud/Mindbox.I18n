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