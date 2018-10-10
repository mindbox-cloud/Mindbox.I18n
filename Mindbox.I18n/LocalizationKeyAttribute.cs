using System;

namespace Mindbox.I18n
{
	[AttributeUsage(validOn:AttributeTargets.Field|AttributeTargets.Property|AttributeTargets.Parameter)]
	public class LocalizationKeyAttribute : Attribute
	{
	}
}