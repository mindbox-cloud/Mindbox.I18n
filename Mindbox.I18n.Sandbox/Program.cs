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
namespace Mindbox.I18n.Sandbox;

internal class Program
{
	private static void Main(string[] _1)
	{
		// Incorrect key
		_ = "Abacaba";

		// Correct key, should be found
		_ = "Cars:Bntl";

		// Correct key, should be found
		_ = "Bands:DeathCab";

		// Correct key but doesn't have a translation
		_ = "Bands:SomeUnknownGuys";

		// Correct key but doesn't have a translation
		_ = "Bands:UnknownAtTheBeginning";

		// Should be an error because the translation file is ignored by the configuration
		_ = "Drinks:Coke";
		_ = $"Try {"that"}";
	}
}