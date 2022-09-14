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

using Microsoft.AspNetCore.Http;
using Mindbox.I18n.Abstractions;

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
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
	Task<ILocale?> TryGetLocale(HttpContext httpContext);
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
}