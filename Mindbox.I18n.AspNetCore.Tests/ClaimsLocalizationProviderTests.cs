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

using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mindbox.I18n.Abstractions;
using Moq;

namespace Mindbox.I18n.AspNetCore.Tests
{
	/// <summary>
	/// Тесты <see cref="ClaimsLocalizationProvider"/>.
	/// </summary>
	[TestClass]
	public class ClaimsLocalizationProviderTests
	{
		private readonly ClaimsLocalizationProvider _provider = new();

		[TestMethod]
		public async Task Provide_When_LocaleIsInToken_ReturnsCorrectLocale()
		{
			// Arrange
			var httpContext = CreateContextWithClaims("ru-RU");

			// Mock the Locales class (assuming it's a static class or method)
			var localeMock = new Mock<ILocale>();
			localeMock.Setup(x => x.Name).Returns("ru-RU");

			// Act
			var result = await _provider.TryGetLocale(httpContext);

			// Assert
			result.Should().NotBeNull("because the locale 'ru-RU' should be recognized");
			result!.Name.Should().Be("ru-RU");
		}

		[TestMethod]
		public async Task Provide_When_LocaleIsWhitespaceInToken_ReturnsNull()
		{
			// Arrange
			var httpContext = CreateContextWithClaims("   ");

			// Act
			var result = await _provider.TryGetLocale(httpContext);

			// Assert
			result.Should().BeNull();
		}

		[TestMethod]
		public async Task Provide_When_InvalidLocaleInToken_ReturnsNull()
		{
			// Arrange
			var httpContext = CreateContextWithClaims("invalid-locale");

			// Act
			var result = await _provider.TryGetLocale(httpContext);

			// Assert
			result.Should().BeNull();
		}

		private HttpContext CreateContextWithClaims(string locale)
		{
			var httpContextMock = new Mock<HttpContext>();
			var userMock = new Mock<ClaimsPrincipal>();

			httpContextMock.Setup(x => x.User).Returns(userMock.Object);

			userMock.Setup(x => x.FindFirst(ClaimsLocalizationProvider.DefaultLocaleKey))
				.Returns(new Claim(ClaimsLocalizationProvider.DefaultLocaleKey, locale!));

			return httpContextMock.Object;
		}
	}
}