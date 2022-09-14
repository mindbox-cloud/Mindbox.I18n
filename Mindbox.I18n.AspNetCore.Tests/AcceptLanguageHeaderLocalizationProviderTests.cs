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
/// Тесты <see cref="AcceptLanguageHeaderLocalizationProvider"/>.
/// </summary>
[TestClass]
public class AcceptLanguageHeaderLocalizationProviderTests
{
	private readonly AcceptLanguageHeaderLocalizationProvider _provider = new();

	[TestMethod]
	public async Task Provide_When_ManyLanguages_Success()
	{
		var httpContext = CreateContext("ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");

		var result = await _provider.TryGetLocale(httpContext);

		result.Should().NotBeNull();
		result.Name.Should().Be("ru-RU");
	}

	[TestMethod]
	public async Task Provide_When_ManyLanguages_WillNotTakeUnknown()
	{
		var httpContext = CreateContext("en-GB,en;q=0.9,en-US;q=0.8,en;q=0.7");

		var result = await _provider.TryGetLocale(httpContext);

		result.Should().NotBeNull();
		result.Name.Should().Be("en-US");
	}

	[TestMethod]
	public async Task Provide_When_NoLanguages_ReturnsNull()
	{
		var httpContext = CreateContext("en-GB,en;q=0.9,fr-FR;q=0.8,en;q=0.7");

		var result = await _provider.TryGetLocale(httpContext);

		result.Should().BeNull();
	}

	private HttpContext CreateContext(string languageHeaders)
	{
		var httpContextMock = new Mock<HttpContext>();
		var httpRequestMock = new Mock<HttpRequest>();
		var headersMock = new Mock<IHeaderDictionary>();
		httpContextMock.SetupGet(x => x.Request).Returns(httpRequestMock.Object).Verifiable();
		httpRequestMock.SetupGet(x => x.Headers).Returns(headersMock.Object).Verifiable();

		headersMock.Setup(x => x.AcceptLanguage).Returns(languageHeaders).Verifiable();
		return httpContextMock.Object;
	}
}