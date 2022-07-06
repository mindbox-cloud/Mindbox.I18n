using System;
using System.Diagnostics;

namespace Mindbox.I18n;

[DebuggerDisplay("{" + nameof(Key) + "}")]
internal sealed class LocaleDependentString : LocalizableString
{
	public override string Key { get; }

	internal LocaleDependentString(string key)
	{
		Key = key ?? throw new ArgumentNullException(nameof(key));
	}

	public override string Render(LocalizationProvider localizationProvider, Locale locale)
	{
		return localizationProvider.Translate(locale, Key);
	}
}