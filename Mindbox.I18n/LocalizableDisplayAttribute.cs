using System;

namespace Mindbox.I18n
{
	[AttributeUsage(AttributeTargets.Field)]
	public class LocalizableDisplayAttribute : Attribute
	{
		public LocalizableDisplayAttribute([LocalizationKey]string name)
		{
			LocalizableName = LocalizableString.ForKey(name);
		}

		public LocalizableDisplayAttribute([LocalizationKey]string name, [LocalizationKey]string description)
		{
			LocalizableName = LocalizableString.ForKey(name);
			LocalizableDescription = LocalizableString.ForKey(description);
		}


		public LocalizableString LocalizableName { get; }

		public LocalizableString LocalizableDescription { get; }
	}
}
