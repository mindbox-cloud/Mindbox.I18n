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

using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace Mindbox.I18n.AspNetCore.Tests;

/// <summary>
/// Тесты <see cref="MindboxRequestLocalizationMiddleware"/>.
/// </summary>
[TestClass]
public class MindboxRequestLocalizationMiddlewareTests
{
	private Mock<IRequestLocalizationProvider> _localizationProviderMock;
	private LocalizationContextAccessor _accessor;

	[TestInitialize]
	public void TestInitialize()
	{
		_localizationProviderMock = new Mock<IRequestLocalizationProvider>();
		_accessor = new LocalizationContextAccessor();
	}

	[TestCleanup]
	public void TestCleanup()
	{
		_localizationProviderMock.Verify();
		_localizationProviderMock.VerifyNoOtherCalls();
	}

	[TestMethod]
	public async Task Middleware_GetLocales_Success()
	{
		Task next(HttpContext _)
		{
			// проверяем тут, чтобы избежать смены контекста для AsyncLocal
			_accessor.Context.Should().NotBeNull();
			_accessor.Context.UserLocale.Should().NotBeNull();
			_accessor.Context.UserLocale.Should().Be(Locales.enUS);
			return Task.CompletedTask;
		}

		var middleware = CreateMiddleware(next);
		var httpContext = Mock.Of<HttpContext>();

		_localizationProviderMock.Setup(x => x.TryGetLocale(httpContext)).ReturnsAsync(Locales.enUS).Verifiable();

		await middleware.Invoke(httpContext).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task Middleware_GetLocales_When_NoHeader_Should_ReturnsNull()
	{
		Task next(HttpContext _)
		{
			// проверяем тут, чтобы избежать смены контекста для AsyncLocal
			_accessor.Context.Should().NotBeNull();
			_accessor.Context.UserLocale.Should().BeNull();
			return Task.CompletedTask;
		}

		var middleware = CreateMiddleware(next);
		var httpContext = Mock.Of<HttpContext>();

		_localizationProviderMock.Setup(x => x.TryGetLocale(httpContext)).ReturnsAsync(() => null).Verifiable();

		await middleware.Invoke(httpContext).ConfigureAwait(false);
	}

	private MindboxRequestLocalizationMiddleware CreateMiddleware(RequestDelegate next)
	{
		return new MindboxRequestLocalizationMiddleware(
			next,
			new[] { _localizationProviderMock.Object },
			_accessor);
	}
}