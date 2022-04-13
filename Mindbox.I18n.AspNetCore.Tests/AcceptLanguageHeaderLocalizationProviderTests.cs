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
	private readonly AcceptLanguageHeaderLocalizationProvider provider = new();

	[TestMethod]
	public async Task Provide_When_ManyLanguages_Success()
	{
		var httpContext = CreateContext("ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");

		var result = await provider.TryGetLocale(httpContext);

		result.Should().NotBeNull();
		result.Name.Should().Be("ru-RU");
	}

	[TestMethod]
	public async Task Provide_When_ManyLanguages_WillNotTakeUnknown()
	{
		var httpContext = CreateContext("en-GB,en;q=0.9,en-US;q=0.8,en;q=0.7");

		var result = await provider.TryGetLocale(httpContext);

		result.Should().NotBeNull();
		result.Name.Should().Be("en-US");
	}

	[TestMethod]
	public async Task Provide_When_NoLanguages_ReturnsNull()
	{
		var httpContext = CreateContext("en-GB,en;q=0.9,fr-FR;q=0.8,en;q=0.7");

		var result = await provider.TryGetLocale(httpContext);

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