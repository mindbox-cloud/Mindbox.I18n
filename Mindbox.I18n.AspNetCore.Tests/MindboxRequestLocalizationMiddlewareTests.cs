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
	private Mock<IRequestLocalizationProvider> localizationProviderMock;
	private RequestLocalizationContextAccessor accessor;

	[TestInitialize]
	public void TestInitialize()
	{
		localizationProviderMock = new Mock<IRequestLocalizationProvider>();
		accessor = new RequestLocalizationContextAccessor();
	}

	[TestCleanup]
	public void TestCleanup()
	{
		localizationProviderMock.Verify();
		localizationProviderMock.VerifyNoOtherCalls();
	}

	[TestMethod]
	public async Task Middleware_GetLocales_Success()
	{
		RequestDelegate next = _ =>
		{
			// проверяем тут, чтобы избежать смены контекста для AsyncLocal
			accessor.Context.Should().NotBeNull();
			accessor.Context.UserLocale.Should().NotBeNull();
			accessor.Context.UserLocale.Should().Be(Locales.enUS);
			return Task.CompletedTask;
		};

		var middleware = CreateMiddleware(next);
		var httpContext = Mock.Of<HttpContext>();

		localizationProviderMock.Setup(x => x.TryGetLocale(httpContext)).ReturnsAsync(Locales.enUS).Verifiable();

		await middleware.Invoke(httpContext).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task Middleware_GetLocales_When_NoTenant_Should_ReturnsNull()
	{
		RequestDelegate next = _ =>
		{
			// проверяем тут, чтобы избежать смены контекста для AsyncLocal
			accessor.Context.Should().NotBeNull();
			accessor.Context.UserLocale.Should().BeNull();
			return Task.CompletedTask;
		};

		var middleware = CreateMiddleware(next);
		var httpContext = Mock.Of<HttpContext>();

		localizationProviderMock.Setup(x => x.TryGetLocale(httpContext)).ReturnsAsync(() => null).Verifiable();

		await middleware.Invoke(httpContext).ConfigureAwait(false);
	}

	private MindboxRequestLocalizationMiddleware CreateMiddleware(RequestDelegate next)
	{
		return new MindboxRequestLocalizationMiddleware(
			next,
			new[] { localizationProviderMock.Object },
			accessor);
	}
}