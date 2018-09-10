using System;
using System.Diagnostics;

namespace Mindbox.I18n
{
	public abstract class LocalizableString
	{
		public abstract string Key { get; }

		public static LocalizableString LocaleIndependent(string localeIndependentString)
		{
			return new LocaleIndependentLocalizableString(localeIndependentString);
		}

		public override string ToString()
		{
			return ToStringCore();
		}

		protected abstract string ToStringCore();

		public static implicit operator LocalizableString(string key)
		{
			// Strictly speaking, this is illegal and will result in ArgumentNullException later.
			if (key == null)
				return null;

			return new LocaleDependentLocalizableString(key);
		}
	}


	[DebuggerDisplay("{" + nameof(localeIndependentString) + "}")]
	internal sealed class LocaleIndependentLocalizableString : LocalizableString
	{
		private readonly string localeIndependentString = null;

		internal LocaleIndependentLocalizableString(string localeIndependentString)
		{
			this.localeIndependentString = localeIndependentString;
		}

		public override string Key => throw new InvalidOperationException(
			$"You can't get a key of a {nameof(LocaleIndependentLocalizableString)}");
		
		protected override string ToStringCore()
		{
			return localeIndependentString;
		}
	}

	[DebuggerDisplay("{" + nameof(Key) + "}")]
	internal sealed class LocaleDependentLocalizableString : LocalizableString
	{
		public override string Key { get; }

		internal LocaleDependentLocalizableString(string key)
		{
			Key = key ?? throw new ArgumentNullException(nameof(key));
		}
		
		protected override string ToStringCore()
		{
			return Key;
		}
	}
}
