using Mindbox.I18n.Abstractions;
using Mindbox.Quokka;

namespace Mindbox.I18n.Template;

public static class TemplateExtensions
{
	public static bool Accepts(this ITemplate template, LocalizationTemplateParameters localizationTemplateParameters)
	{
		var requiredFields = template.GetModelDefinition().Fields;

		if (requiredFields.Count != localizationTemplateParameters.Fields.Count)
			return false;

		foreach (var field in requiredFields)
		{
			if (!localizationTemplateParameters.Fields.ContainsKey(field.Key))
				return false;
		}

		return true;
	}
}