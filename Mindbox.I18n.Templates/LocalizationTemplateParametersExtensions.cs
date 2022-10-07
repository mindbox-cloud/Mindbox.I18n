using System.Linq;
using Mindbox.I18n.Abstractions;
using Mindbox.Quokka;

namespace Mindbox.I18n.Template;

public static class LocalizationTemplateParametersExtensions
{
	public static ICompositeModelValue ToCompositeModelValue(this LocalizationTemplateParameters parameters)
	{
		return new CompositeModelValue(parameters.Fields.Select(f => new ModelField(f.Key, f.Value)));
	}
}