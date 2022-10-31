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
using System.Linq;
using Mindbox.I18n.Abstractions;
using Mindbox.Quokka;

namespace Mindbox.I18n.Template;

public static class LocalizationTemplateParametersExtensions
{
	public static ICompositeModelValue ToCompositeModelValue(this LocalizationTemplateParameters parameters)
	{
		return new CompositeModelValue(parameters.Fields.Select(value =>
			new ModelField(
				value.Key,
				value.Value.ToModelValue()
			)));
	}

	public static IModelField ToModelField(this ParameterField value)
	{
		return new ModelField(
			value.Name,
			value.Value.ToModelValue()
		);
	}

	public static IModelValue ToModelValue(this ParameterValue value) => value switch
	{
		CompositeParameter composite => composite.ToModelValue(),
		PrimitiveParameter primitive => primitive.ToModelValue(),
		_ => throw new InvalidCastException()
	};

	public static IModelValue ToModelValue(this PrimitiveParameter value) => new PrimitiveModelValue(value.Value);

	public static IModelValue ToModelValue(this CompositeParameter value) =>
		new CompositeModelValue(value.Fields.Select(field => field.ToModelField()));
}