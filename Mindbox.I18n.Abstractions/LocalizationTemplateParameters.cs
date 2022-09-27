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