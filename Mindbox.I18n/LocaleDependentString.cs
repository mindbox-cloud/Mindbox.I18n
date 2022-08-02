using System;
using System.Diagnostics;
using Mindbox.i18n.Abstractions;

namespace Mindbox.I18n;

[DebuggerDisplay("{" + nameof(Key) + "}")]
internal sealed class LocaleDependentString : LocalizableString
{
	public override string Key { get; }

	internal LocaleDependentString(string key)
	{
		Key = key ?? throw new ArgumentNullException(nameof(key));
	}

	public override string Render(ILocalizationProvider localizationProvider, ILocale locale)
	{
		return localizationProvider.Translate(locale, Key);
	}
}