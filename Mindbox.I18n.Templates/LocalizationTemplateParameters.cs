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
using System.Linq;
using Mindbox.Quokka;

namespace Mindbox.I18n.Template;

public sealed class LocalizationTemplateParameters
{
	private Dictionary<string, IModelValue> Fields { get; } = new();

	public LocalizationTemplateParameters WithField(
		string fieldName,
		bool value)
	{
		return WithFieldCore(fieldName, value);
	}

	public LocalizationTemplateParameters WithField(
		string fieldName,
		bool? value)
	{
		return WithFieldCore(fieldName, value);
	}

	public LocalizationTemplateParameters WithField(
		string fieldName,
		DateTime value)
	{
		return WithFieldCore(fieldName, value);
	}

	public LocalizationTemplateParameters WithField(
		string fieldName,
		DateTime? value)
	{
		return WithFieldCore(fieldName, value);
	}

	public LocalizationTemplateParameters WithField(
		string fieldName,
		int value)
	{
		return WithFieldCore(fieldName, value);
	}

	public LocalizationTemplateParameters WithField(
		string fieldName,
		int? value)
	{
		return WithFieldCore(fieldName, value);
	}

	public LocalizationTemplateParameters WithField(
		string fieldName,
		string value)
	{
		return WithFieldCore(fieldName, value);
	}

	public LocalizationTemplateParameters WithField(
		string fieldName,
		TimeSpan value)
	{
		return WithFieldCore(fieldName, value);
	}

	public LocalizationTemplateParameters WithField(
		string fieldName,
		TimeSpan? value)
	{
		return WithFieldCore(fieldName, value);
	}

	public LocalizationTemplateParameters WithField(
		string fieldName,
		decimal value)
	{
		return WithFieldCore(fieldName, value);
	}

	public LocalizationTemplateParameters WithField(
		string fieldName,
		decimal? value)
	{
		return WithFieldCore(fieldName, value);
	}

	public ICompositeModelValue ToCompositeModelValue()
	{
		return new CompositeModelValue(Fields.Select(f => new ModelField(f.Key, f.Value)));
	}

	private LocalizationTemplateParameters WithFieldCore<TEntityType>(
		string fieldName,
		TEntityType value)
	{
		if (string.IsNullOrWhiteSpace(fieldName))
			throw new ArgumentNullException(nameof(fieldName));

		Fields.Add(fieldName, new PrimitiveModelValue(value));

		return this;
	}
}