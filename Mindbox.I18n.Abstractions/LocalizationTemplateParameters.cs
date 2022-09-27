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

using Mindbox.Quokka;

namespace Mindbox.I18n.Abstractions;

public sealed class LocalizationTemplateParameters
{
	public Dictionary<string, string> Fields { get; } = new();

	public LocalizationTemplateParameters WithField(string fieldName, string value)
	{
		Fields.Add(fieldName, value);
		return this;
	}

	public ICompositeModelValue ToCompositeModelValue()
		=> new CompositeModelValue(Fields.Select(f => new ModelField(f.Key, new PrimitiveModelValue(f.Value))));
}