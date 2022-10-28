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
public class LocalizerTests
{
	[TestMethod]
	public void TryGetParameterizedLocalizedString_AllLocalizationParametersEmpty_ReturnNull()
	{
		var mockTemplateFactory = new Mock<ITemplateFactory>();

		var localizer = new Localizer(
			new LocalizationProviderStub(),
			mockTemplateFactory.Object,
			NullLogger<Localizer>.Instance);

		var localizableString = LocalizableString.ForKey(LocalizationProviderStub.SimpleLocalizableStringKey);

		var result = localizer.TryGetParameterizedLocalizedString(Locales.enUS, localizableString);

		Assert.IsNull(result);
	}

	[TestMethod]
	public void TryGetParameterizedLocalizedString_TemplateNotAccepted_ReturnNull()
	{
		var compositeModelDefinitionStub = new CompositeModelDefinitionStub();
		var template = new TemplateStub(compositeModelDefinitionStub);

		var mockTemplateFactory = new Mock<ITemplateFactory>();
		mockTemplateFactory
			.Setup(x =>
				x.CreateTemplate(It.Is<string>(inputString =>
					inputString == LocalizationProviderStub.ParameterizedLocalizableStringValue)))
			.Returns(template);

		var localizer = new Localizer(
			new LocalizationProviderStub(),
			mockTemplateFactory.Object,
			NullLogger<Localizer>.Instance);

		var localizableString = LocalizableString
			.ForKey(LocalizationProviderStub.ParameterizedLocalizableStringKey)
			.WithParameters(parameters => parameters.WithField("firstParam", 1));

		var result = localizer.TryGetParameterizedLocalizedString(Locales.enUS, localizableString);

		mockTemplateFactory.Verify();
		Assert.IsNull(result);
	}


	[TestMethod]
	public void TryGetParameterizedLocalizedString_TemplateAccepted_ReturnString()
	{
		var compositeModelDefinitionStub = new CompositeModelDefinitionStub(
			new Dictionary<string, IModelDefinition>()
			{
				["firstParam"] = Mock.Of<IModelDefinition>(),
				["secondParam"] = Mock.Of<IModelDefinition>()
			});
		var mockTemplate = new Mock<TemplateStub>(compositeModelDefinitionStub);
		mockTemplate.Setup(x =>
				x.Render(It.IsAny<ICompositeModelValue>(), It.IsAny<ICallContextContainer?>()))
			.Returns(LocalizationProviderStub.ParameterizedLocalizableStringValue);

		var mockTemplateFactory = new Mock<ITemplateFactory>();
		mockTemplateFactory
			.Setup(x =>
				x.CreateTemplate(It.Is<string>(inputString =>
					inputString == LocalizationProviderStub.ParameterizedLocalizableStringValue)))
			.Returns(mockTemplate.Object);

		var localizer = new Localizer(
			new LocalizationProviderStub(),
			mockTemplateFactory.Object,
			NullLogger<Localizer>.Instance);

		var localizableString = LocalizableString
			.ForKey(LocalizationProviderStub.ParameterizedLocalizableStringKey)
			.WithParameters(parameters => parameters
				.WithField("firstParam", 1)
				.WithField("secondParam", 2));

		var result = localizer.TryGetParameterizedLocalizedString(Locales.enUS, localizableString);

		mockTemplate.Verify();
		Assert.IsNotNull(result);
		Assert.AreEqual(LocalizationProviderStub.ParameterizedLocalizableStringValue, result);
	}
}