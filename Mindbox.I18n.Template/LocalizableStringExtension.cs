using Mindbox.I18n.Abstractions;
using Mindbox.Quokka;

namespace Mindbox.I18n.Template;

public static class LocalizableStringExtensions
{
	public static string RenderWithTemplate(
		this ILocalizableString localizableString,
		ILocalizationProvider localizationProvider,
		ILocale locale,
		ITemplateFactory templateFactory,
		Dictionary<string, Func<string>>? parameters = null)
	{
		var @string = localizableString.Render(localizationProvider, locale);
		if (parameters is null)
			return @string;

		var template = templateFactory.CreateTemplate(@string);
		return template.Render(parameters.ToCompositeModelValue());
	}
}