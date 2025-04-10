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

public static class DefaultValues
{
	public const string SimpleLocalizableStringKey = "Test:SimpleLocalizableStringKey";
	public const string SimpleLocalizableStringValue = "Test message";
	public const string ParameterizedLocalizableStringKey = "Test:ParameterizedLocalizableStringKey";
	public const string ParameterizedLocalizableStringValue = "Test with message: ${firstParam} and ${secondParam}";
	public const string NonExistentLocalizableStringKey = "Test:NonExistentLocalizableStringKey";

	public const string ParameterizedLocalizableStringWithLocalizableStringParameterKey =
		"Test:ParameterizedLocalizableStringWithLocalizableStringParameterKey";

	public const string ParameterizedLocalizableStringWithLocalizableStringParameterValue =
		"Test with message: ${firstParam} and ${secondParam} and ${thirdParam}";

	public static IReadOnlyDictionary<string, IModelDefinition> CompositeModelDefinitionFields =>
		new Dictionary<string, IModelDefinition>()
		{
			["firstParam"] = Mock.Of<IModelDefinition>(),
			["secondParam"] = Mock.Of<IModelDefinition>()
		};

	public static IReadOnlyDictionary<string, IModelDefinition> CompositeModelDefinitionFieldsWithLocalizableStringParameter =>
		new Dictionary<string, IModelDefinition>()
		{
			["firstParam"] = Mock.Of<IModelDefinition>(),
			["secondParam"] = Mock.Of<IModelDefinition>(),
			["thirdParam"] = Mock.Of<IModelDefinition>()
		};
}