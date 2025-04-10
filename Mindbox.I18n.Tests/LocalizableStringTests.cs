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

using System.Globalization;

namespace Mindbox.I18n.Tests;

[TestClass]
public class LocalizableStringTests
{
	private const string DateTimeFormat = "dd.MM.yyyy HH:mm:ss";
	private const string DefaultKey = "Prefix:key";
	private const int DefaultIntValue = 123;
	private readonly DateTime _defaultDateTimeValue = DateTime.UtcNow;

	[TestMethod]
	public void ForKey_NullValue_ThrowException()
	{
		Assert.ThrowsException<ArgumentNullException>(() => LocalizableString.ForKey(null!));
	}

	[TestMethod]
	public void ForKey_KeyAreEqual()
	{
		var localizableString = LocalizableString.ForKey(DefaultKey);

		Assert.IsNotNull(localizableString);
		Assert.AreEqual(DefaultKey, localizableString.Key);
	}

	[TestMethod]
	public void ForKey_WithParameters()
	{
		var localizableString = LocalizableString
			.ForKey(DefaultKey)
			.WithParameters(parameters => parameters
				.WithField("FirstValue", DefaultIntValue)
				.WithField("SecondValue", _defaultDateTimeValue));

		Assert.IsNotNull(localizableString.LocalizationParameters);
		Assert.AreEqual(DefaultIntValue,
			(localizableString.LocalizationParameters.Fields["FirstValue"] as PrimitiveParameter)!.ValueProvider(Locales.enUS));
		Assert.AreEqual(_defaultDateTimeValue.ToString(DateTimeFormat, CultureInfo.InvariantCulture),
			(localizableString.LocalizationParameters.Fields["SecondValue"] as PrimitiveParameter)!.ValueProvider(Locales.enUS));
	}

	[TestMethod]
	public void ForKey_WithLocalizableStringParameter_WithParameters()
	{
		var localizableString = LocalizableString
			.ForKey(DefaultKey)
			.WithParameters(rootParameters => rootParameters
				.WithField("FirstValue", DefaultIntValue)
				.WithField("SecondValue", _defaultDateTimeValue)
				.WithField("ThirdValue", LocalizableString
					.ForKey(DefaultKey)
					.WithParameters(childParameters => childParameters
						.WithField("FirstValue", DefaultIntValue)
						.WithField("SecondValue", _defaultDateTimeValue))));

		Assert.IsNotNull(localizableString.LocalizationParameters);

		Assert.AreEqual(DefaultIntValue,
			(localizableString.LocalizationParameters.Fields["FirstValue"] as PrimitiveParameter)!.ValueProvider(Locales.enUS));
		Assert.AreEqual(_defaultDateTimeValue.ToString(DateTimeFormat, CultureInfo.InvariantCulture),
			(localizableString.LocalizationParameters.Fields["SecondValue"] as PrimitiveParameter)!.ValueProvider(Locales.enUS));

		Assert.AreEqual(DefaultIntValue,
			((localizableString.LocalizationParameters.Fields["ThirdValue"] as LocalizableStringParameter)!
				.Value.LocalizationParameters!.Fields["FirstValue"] as PrimitiveParameter)!.ValueProvider(Locales.enUS));

		Assert.AreEqual(_defaultDateTimeValue.ToString(DateTimeFormat, CultureInfo.InvariantCulture),
			((localizableString.LocalizationParameters.Fields["ThirdValue"] as LocalizableStringParameter)!
				.Value.LocalizationParameters!.Fields["SecondValue"] as PrimitiveParameter)!.ValueProvider(Locales.enUS));
	}
}