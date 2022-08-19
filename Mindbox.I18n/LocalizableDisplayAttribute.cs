using System;

namespace Mindbox.I18n;

[AttributeUsage(AttributeTargets.Field)]
public sealed class LocalizableDisplayAttribute : Attribute
{
#pragma warning disable CA1019
	public LocalizableDisplayAttribute([LocalizationKey] string name)
	{
		LocalizableName = LocalizableString.ForKey(name);
	}

	public LocalizableDisplayAttribute([LocalizationKey] string name, [LocalizationKey] string description)
	{
		LocalizableName = LocalizableString.ForKey(name);
		LocalizableDescription = LocalizableString.ForKey(description);
	}
#pragma warning restore CA1019

	public LocalizableString LocalizableName { get; }

	public LocalizableString? LocalizableDescription { get; }
}