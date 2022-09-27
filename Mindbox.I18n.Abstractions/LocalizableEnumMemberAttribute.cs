using Mindbox.I18n.Abstractions;

namespace Mindbox.I18n.Template;

[AttributeUsage(AttributeTargets.Field)]
public sealed class LocalizableEnumMemberAttribute : Attribute
{
	public LocalizableString LocalizableString { get; }

#pragma warning disable CA1019
	public LocalizableEnumMemberAttribute([LocalizationKey] string localizationKey)
#pragma warning restore CA1019
		=> LocalizableString = LocalizableString.ForKey(localizationKey);
}