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