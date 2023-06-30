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

namespace Mindbox.I18n.Tests;

[TestClass]
public sealed class LocalizationProviderTests
{
	[TestMethod]
	public void TryTranslate_ShouldReturnTranslation()
	{
		var translationSourceMock = new Mock<ITranslationSource>();

		var localizationProvider = new LocalizationProvider(new InitializationOptions
		{
			Logger = NullLogger<LocalizationProvider>.Instance,
			TranslationSource = translationSourceMock.Object
		});

		const string expectedResult = "translation";

		translationSourceMock
			.Setup(s => s.TryGetTranslation(
				Locales.ruRU,
				It.Is<LocalizationKey>(k => k.FullKey == DefaultValues.SimpleLocalizableStringKey)))
			.Returns(expectedResult);

		var result = localizationProvider.TryTranslate(Locales.ruRU, DefaultValues.SimpleLocalizableStringKey);

		Assert.IsNotNull(result);
		Assert.AreEqual(expectedResult, result);
	}

	[TestMethod]
	public void TryTranslate_ShouldFallback_WhenFallbackLocalePassed()
	{
		var translationSourceMock = new Mock<ITranslationSource>();
		var fallbackLocale = Locales.enUS;

		var localizationProvider = new LocalizationProvider(new InitializationOptions
		{
			Logger = NullLogger<LocalizationProvider>.Instance,
			TranslationSource = translationSourceMock.Object,
			FallbackLocale = fallbackLocale
		});

		const string expectedResult = "translation";

		translationSourceMock
			.Setup(s => s.TryGetTranslation(
				Locales.ruRU,
				It.Is<LocalizationKey>(k => k.FullKey == DefaultValues.SimpleLocalizableStringKey)))
			.Returns(null as string);

		translationSourceMock
			.Setup(s => s.TryGetTranslation(
				fallbackLocale,
				It.Is<LocalizationKey>(k => k.FullKey == DefaultValues.SimpleLocalizableStringKey)))
			.Returns(expectedResult);

		var result = localizationProvider.TryTranslate(Locales.ruRU, DefaultValues.SimpleLocalizableStringKey);

		Assert.IsNotNull(result);
		Assert.AreEqual(expectedResult, result);
	}

	[TestMethod]
	public void TryTranslate_ShouldReturnNull_WhenFallbackLocaleIsNotPassed()
	{
		var translationSourceMock = new Mock<ITranslationSource>();
		var fallbackLocale = Locales.enUS;

		var localizationProvider = new LocalizationProvider(new InitializationOptions
		{
			Logger = NullLogger<LocalizationProvider>.Instance,
			TranslationSource = translationSourceMock.Object,
			FallbackLocale = fallbackLocale
		});

		translationSourceMock
			.Setup(s => s.TryGetTranslation(
				Locales.ruRU,
				It.Is<LocalizationKey>(k => k.FullKey == DefaultValues.SimpleLocalizableStringKey)))
			.Returns(null as string);

		var result = localizationProvider.TryTranslate(Locales.ruRU, DefaultValues.SimpleLocalizableStringKey);

		Assert.IsNull(result);
	}
}