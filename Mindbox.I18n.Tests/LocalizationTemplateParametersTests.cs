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
public class LocalizationTemplateParametersTests
{
	private const string FirstKey = "firstKey";
	private const string SecondKey = "secondKey";
	private const string DefaultStringValue = "value";
	private const int DefaultIntValue = 123;

	[TestMethod]
	public void Contact_AllIsNull_ReturnNull()
	{
		var result = LocalizationTemplateParameters.Contact(null, null);

		Assert.IsNull(result);
	}

	[TestMethod]
	public void Contact_SecondIsNull_ReturnFirst()
	{
		var firstParameters = new LocalizationTemplateParameters().WithField(FirstKey, DefaultStringValue);

		var result = LocalizationTemplateParameters.Contact(firstParameters, null);

		Assert.IsNotNull(result);
		Assert.AreEqual(result, firstParameters);
	}

	[TestMethod]
	public void Contact_FirstIsNull_ReturnSecond()
	{
		var secondParameters = new LocalizationTemplateParameters().WithField(SecondKey, DefaultIntValue);

		var result = LocalizationTemplateParameters.Contact(null, secondParameters);

		Assert.IsNotNull(result);
		Assert.AreEqual(result, secondParameters);
	}

	[TestMethod]
	public void Contact_AllReturned()
	{
		var firstParameters = new LocalizationTemplateParameters().WithField(FirstKey, DefaultStringValue);
		var secondParameters = new LocalizationTemplateParameters().WithField(SecondKey, DefaultIntValue);

		var result = LocalizationTemplateParameters.Contact(firstParameters, secondParameters);

		Assert.IsNotNull(result);
		Assert.AreEqual(2, result.Fields.Count);
		Assert.AreEqual(DefaultStringValue, (result.Fields[FirstKey] as PrimitiveParameter)!.Value);
		Assert.AreEqual(DefaultIntValue, (result.Fields[SecondKey] as PrimitiveParameter)!.Value);
	}

	[TestMethod]
	public void Union_WithUniqueValues_Works()
	{
		var firstParameters = new LocalizationTemplateParameters().WithField(FirstKey, DefaultStringValue);
		var secondParameters = new LocalizationTemplateParameters().WithField(SecondKey, DefaultIntValue);

		var result = LocalizationTemplateParameters.Union(firstParameters, secondParameters);

		Assert.AreEqual(2, result.Fields.Count);
		Assert.AreEqual(DefaultStringValue, (result.Fields[FirstKey] as PrimitiveParameter)!.Value);
		Assert.AreEqual(DefaultIntValue, (result.Fields[SecondKey] as PrimitiveParameter)!.Value);
	}

	[TestMethod]
	public void Union_KeysAreDuplicated_ThrowException()
	{
		var firstParameters = new LocalizationTemplateParameters()
			.WithField(FirstKey, DefaultStringValue);

		var secondParameters = new LocalizationTemplateParameters()
			.WithField(FirstKey, DefaultIntValue)
			.WithField(SecondKey, DefaultIntValue);

		Assert.ThrowsException<InvalidOperationException>(() =>
			LocalizationTemplateParameters.Union(firstParameters, secondParameters));
	}

	[TestMethod]
	public void ToCompositeModelValue_ContainsAllFields()
	{
		const string subFiledKey = "subKey";

		var parameters = new LocalizationTemplateParameters()
			.WithField(FirstKey, DefaultStringValue)
			.WithField(SecondKey, new LocalizationTemplateParameters()
				.WithField(subFiledKey, DefaultIntValue));

		var compositeModel = parameters.ToCompositeModelValue();

		Assert.AreEqual(2, compositeModel.Fields.Count());

		var firstField = compositeModel.Fields.FirstOrDefault(x => x.Name == FirstKey)?.Value as IPrimitiveModelValue;

		Assert.IsNotNull(firstField);
		Assert.AreEqual(DefaultStringValue, firstField.Value);

		var secondFiled = compositeModel.Fields.FirstOrDefault(x => x.Name == SecondKey)?.Value as ICompositeModelValue;

		Assert.IsNotNull(secondFiled);

		var subFiled = secondFiled.Fields.FirstOrDefault(x => x.Name == subFiledKey)?.Value as IPrimitiveModelValue;

		Assert.IsNotNull(subFiled);
		Assert.AreEqual(DefaultIntValue, subFiled.Value);
	}
}