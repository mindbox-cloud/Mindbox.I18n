﻿// Copyright 2022 Mindbox Ltd
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
public class LocalizerTests
{
	[TestMethod]
	public void TryGetLocalizedString_AllLocalizationParametersEmpty_ReturnLocalizedValue()
	{
		var mockTemplateFactory = new Mock<ITemplateFactory>();

		var localizer = new Localizer(
			new LocalizationProviderStub(),
			mockTemplateFactory.Object,
			NullLogger<Localizer>.Instance);

		var localizableString = LocalizableString.ForKey(DefaultValues.SimpleLocalizableStringKey);

		var result = localizer.TryGetLocalizedString(Locales.enUS, localizableString);

		Assert.IsNotNull(result);
		Assert.AreEqual(DefaultValues.SimpleLocalizableStringValue, result);
	}

	[TestMethod]
	public void TryGetLocalizedString_TemplateNotAccepted_ReturnNull()
	{
		var compositeModelDefinitionStub = new CompositeModelDefinitionStub(DefaultValues.CompositeModelDefinitionFields);
		var template = new TemplateStub(compositeModelDefinitionStub);

		var mockTemplateFactory = new Mock<ITemplateFactory>();
		mockTemplateFactory
			.Setup(x =>
				x.CreateTemplate(It.Is<string>(inputString =>
					inputString == DefaultValues.ParameterizedLocalizableStringValue)))
			.Returns(template);

		var localizer = new Localizer(
			new LocalizationProviderStub(),
			mockTemplateFactory.Object,
			NullLogger<Localizer>.Instance);

		var localizableString = LocalizableString
			.ForKey(DefaultValues.ParameterizedLocalizableStringKey)
			.WithParameters(parameters => parameters.WithField("firstParam", 1));

		var result = localizer.TryGetLocalizedString(Locales.enUS, localizableString);

		mockTemplateFactory.Verify();
		Assert.IsNull(result);
	}

	[TestMethod]
	public void TryGetLocalizedString_TemplateAccepted_ReturnString()
	{
		var compositeModelDefinitionStub = new CompositeModelDefinitionStub(DefaultValues.CompositeModelDefinitionFields);
		var mockTemplate = new Mock<TemplateStub>(compositeModelDefinitionStub);
		mockTemplate.Setup(x =>
				x.Render(It.IsAny<ICompositeModelValue>(), It.IsAny<ICallContextContainer?>()))
			.Returns(DefaultValues.ParameterizedLocalizableStringValue);

		var mockTemplateFactory = new Mock<ITemplateFactory>();
		mockTemplateFactory
			.Setup(x =>
				x.CreateTemplate(It.Is<string>(inputString =>
					inputString == DefaultValues.ParameterizedLocalizableStringValue)))
			.Returns(mockTemplate.Object);

		var localizer = new Localizer(
			new LocalizationProviderStub(),
			mockTemplateFactory.Object,
			NullLogger<Localizer>.Instance);

		var localizableString = LocalizableString
			.ForKey(DefaultValues.ParameterizedLocalizableStringKey)
			.WithParameters(parameters => parameters
				.WithField("firstParam", 1)
				.WithField("secondParam", 2));

		var result = localizer.TryGetLocalizedString(Locales.enUS, localizableString);

		mockTemplate.Verify();
		Assert.IsNotNull(result);
		Assert.AreEqual(DefaultValues.ParameterizedLocalizableStringValue, result);
	}
}