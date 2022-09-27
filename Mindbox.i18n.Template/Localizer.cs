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
		ILocalizationProvider localizationProvider,
		ITemplateFactory templateFactory,
		ILogger<Localizer> logger)
	{
		_localizationProvider = localizationProvider;
		_templateFactory = templateFactory;
		_logger = logger;
	}

	public string GetLocalizedString(
		ILocale locale,
		LocalizableString localizableString,
		LocalizationTemplateParameters? localizationTemplateParameters = null)
	{
		var @string = localizableString.Render(_localizationProvider, locale);

		if (localizationTemplateParameters is null)
			return @string;

		var template = _templateFactory.CreateTemplate(@string);
		return template.Render(localizationTemplateParameters.ToCompositeModelValue());
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
}