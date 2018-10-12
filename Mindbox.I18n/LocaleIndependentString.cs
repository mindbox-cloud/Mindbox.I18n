using System;
using System.Diagnostics;

namespace Mindbox.I18n
{
	[DebuggerDisplay("{" + nameof(Key) + "}")]
	internal sealed class LocaleIndependentString : LocalizableString
	{
		internal LocaleIndependentString(string localeIndependentString)
		{
			Key = localeIndependentString;
		}

		public override string Key { get; }

		public override string Render(LocalizationProvider localizationProvider, Locale locale)
		{
			return Key;
		}

		protected override string ToStringCore()
		{
			return Key;
		}
	}
}