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
	public const string FirstKey = "firstKey";
	public const string SecondKey = "secondKey";
	public const string DefaultStringValue = "value";
	public const int DefaultIntValue = 123;

	[TestMethod]
	public void Union_WithUniqueValues_Works()
	{
		var firstParameters = new LocalizationTemplateParameters().WithField(FirstKey, DefaultStringValue);
		var secondParameters = new LocalizationTemplateParameters().WithField(SecondKey, DefaultIntValue);

		var result = LocalizationTemplateParameters.Union(firstParameters, secondParameters);

		Assert.AreEqual(2, result.Fields.Count);
		Assert.AreEqual(DefaultStringValue, result.Fields[FirstKey]);
		Assert.AreEqual(DefaultIntValue, result.Fields[SecondKey]);
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
}