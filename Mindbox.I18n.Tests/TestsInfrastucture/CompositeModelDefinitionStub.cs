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

public class CompositeModelDefinitionStub : ICompositeModelDefinition
{
	public IReadOnlyDictionary<string, IModelDefinition> Fields { get; }

	public IReadOnlyDictionary<IMethodCallDefinition, IModelDefinition> Methods =>
		new Dictionary<IMethodCallDefinition, IModelDefinition>();

	public CompositeModelDefinitionStub()
	{
		Fields = new Dictionary<string, IModelDefinition>()
		{
			["firstParam"] = Mock.Of<IModelDefinition>(),
			["secondParam"] = Mock.Of<IModelDefinition>()
		};
	}

	public CompositeModelDefinitionStub(IReadOnlyDictionary<string, IModelDefinition> fields)
	{
		Fields = fields ?? throw new ArgumentNullException(nameof(fields));
	}
}