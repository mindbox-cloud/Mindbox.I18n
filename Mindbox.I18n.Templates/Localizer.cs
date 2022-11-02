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
using System.Reflection;
using Microsoft.Extensions.Logging;
using Mindbox.I18n.Abstractions;
using Mindbox.Quokka;

namespace Mindbox.I18n.Template;

public sealed class Localizer : ILocalizer
{
	private readonly ILocalizationProvider _localizationProvider;
	private readonly ITemplateFactory _templateFactory;
	private readonly ILogger<Localizer> _logger;

	public Localizer(
		InitializationOptions options,
		ITemplateFactory templateFactory,
		ILogger<Localizer> logger)
	{
		_localizationProvider = new LocalizationProvider(options);
		_templateFactory = templateFactory;
		_logger = logger;
	}

	internal Localizer(
		ILocalizationProvider localizationProvider,
		ITemplateFactory templateFactory,
		ILogger<Localizer> logger)
	{
		_localizationProvider = localizationProvider;
		_templateFactory = templateFactory;
		_logger = logger;
	}

	public string? TryGetLocalizedString(
		ILocale locale,
		LocalizableString localizableString,
		LocalizationTemplateParameters? localizationTemplateParameters = null)
	{
		var @string = GetTranslate(locale, localizableString);

		var localizationParameters = LocalizationTemplateParameters.Contact(
			localizableString.LocalizationParameters,
			localizationTemplateParameters);

		if (localizationParameters is null)
			return @string;

		var template = _templateFactory.CreateTemplate(@string);

		try
		{
			return template.Render(localizationParameters.ToCompositeModelValue(locale));
		}
		catch (TemplateException ex)
		{
			_logger.LogWarning(ex,
				"Rendering template for key {Key} in locale {Locale} threw an exception.",
				localizableString.Key,
				locale.Name);

			return null;
		}
	}

	public string GetLocalizedString(
		ILocale locale,
		LocalizableString localizableString,
		LocalizationTemplateParameters? localizationTemplateParameters = null)
	{
		return TryGetLocalizedString(locale, localizableString, localizationTemplateParameters)
		?? localizableString.Key;
	}

	public string GetLocalizedEnum(ILocale locale, Enum value)
	{
		var enumType = value.GetType();

		var localizableString = enumType
			.GetField(value.ToString())?
			.GetCustomAttribute<LocalizableEnumMemberAttribute>()?
			.LocalizableString;

		if (localizableString is not null)
			return GetLocalizedString(locale, localizableString);

		_logger.LogWarning(
			"No localization key defined for value {Value} of enum {EnumType}",
			value,
			value.GetType());

		return value.ToString();
	}

	private string GetTranslate(ILocale locale, LocalizableString localizableString) => localizableString switch
	{
		LocaleIndependentString => localizableString.Key,
		_ => _localizationProvider.Translate(locale, localizableString.Key)
	};
}