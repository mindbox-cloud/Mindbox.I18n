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
public class TemplateExtensionsTests
{
	[TestMethod]
	public void Accepts_DifferentCountOfFields_ReturnFalse()
	{
		var compositeModelDefinition = new CompositeModelDefinitionStub();
		var template = new TemplateStub(compositeModelDefinition);
		var localizationTemplateParameters = new LocalizationTemplateParameters().WithField("param", "value");

		var result = template.Accepts(localizationTemplateParameters);

		Assert.IsFalse(result);
	}

	[TestMethod]
	public void Accepts_DifferentFields_ReturnFalse()
	{
		var compositeModelDefinition = new CompositeModelDefinitionStub();
		var template = new TemplateStub(compositeModelDefinition);
		var localizationTemplateParameters = new LocalizationTemplateParameters()
			.WithField("param1", "value1")
			.WithField("param2", "value2");

		var result = template.Accepts(localizationTemplateParameters);

		Assert.IsFalse(result);
	}


	[TestMethod]
	public void Accepts_FieldsAreSame_ReturnTrue()
	{
		var compositeModelDefinition = new CompositeModelDefinitionStub(
				new Dictionary<string, IModelDefinition>()
				{
					["param1"] = Mock.Of<IModelDefinition>(),
					["param2"] = Mock.Of<IModelDefinition>()
				}
			);
		var template = new TemplateStub(compositeModelDefinition);
		var localizationTemplateParameters = new LocalizationTemplateParameters()
			.WithField("param1", "value1")
			.WithField("param2", "value2");

		var result = template.Accepts(localizationTemplateParameters);

		Assert.IsTrue(result);
	}
}