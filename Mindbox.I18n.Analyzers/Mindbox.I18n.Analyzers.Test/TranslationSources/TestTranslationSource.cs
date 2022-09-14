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

using System;
using System.Collections.Generic;

namespace Mindbox.I18n.Analyzers.Test;

public class TestTranslationSource : IAnalyzerTranslationSource
{
	private readonly Dictionary<LocalizationKey, string> _translations
		= new();

	public void AddTranslation(string localizationKey, string value)
	{
		var parsedLocalizationKey = LocalizationKey.TryParse(localizationKey);
		if (parsedLocalizationKey == null)
			throw new ArgumentException("Passed string is not a valid localization key", nameof(localizationKey));

		_translations.Add(parsedLocalizationKey, value);
	}

	public string TryGetTranslation(LocalizationKey localizationKey)
	{
		_translations.TryGetValue(localizationKey, out var value);
		return value;
	}
}