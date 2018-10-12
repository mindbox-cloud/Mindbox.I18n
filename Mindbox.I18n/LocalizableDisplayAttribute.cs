using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mindbox.I18n;

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
