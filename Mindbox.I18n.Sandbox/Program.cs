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

using Mindbox.I18n.Abstractions;

#pragma warning disable CS0219
#pragma warning disable IDE0059

namespace Mindbox.I18n.Sandbox;

internal class Program
{
	private static void Main(string[] _1)
	{
		LocalizableString str;

		// Incorrect key
		// str = "Abacaba";

		// Correct key, should be found
		str = "Cars:Bntl";

		// Correct key, but the file is not included in the project, so won't be found
		// str = "Outside:Found";

		// Correct key, should be found
		str = "Bands:DeathCab";

		// Correct key but doesn't have a translation
		// str = "Bands:SomeUnknownGuys";

		// Correct key but doesn't have a translation
		str = "Bands:UnknownAtTheBeginning";

		// Should be an error because the translation file is ignored by the configuration
		// (actually, ignoring is NOT SUPPORTED, so should actually be found
		str = "Drinks:Coke";

		// Should be an error because an interpolated string can't represent a localization key
		// str = $"Try {"that"}";
	}
}