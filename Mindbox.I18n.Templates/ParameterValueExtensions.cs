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

public static class ParameterValueExtensions
{
	public static IModelField ToModelField(this ParameterField value, ILocale locale)
	{
		return new ModelField(
			value.Name,
			value.Value.ToModelValue(locale)
		);
	}

	public static IModelValue ToModelValue(this ParameterValue value, ILocale locale) => value switch
	{
		CompositeParameter composite => composite.ToModelValue(locale),
		PrimitiveParameter primitive => primitive.ToModelValue(locale),
		_ => throw new InvalidCastException()
	};

	public static IModelValue ToModelValue(this PrimitiveParameter value, ILocale locale) =>
		new PrimitiveModelValue(value.ValueProvider(locale));

	public static IModelValue ToModelValue(this CompositeParameter value, ILocale locale) =>
		new CompositeModelValue(value.Fields.Select(field => field.ToModelField(locale)));
}