using System.Diagnostics;
using Mindbox.i18n.Abstractions;

namespace Mindbox.I18n;

[DebuggerDisplay("{" + nameof(Key) + "}")]
internal sealed class LocaleIndependentString : LocalizableString
{
	internal LocaleIndependentString(string localeIndependentString)
	{
		Key = localeIndependentString;
	}

	public override string Key { get; }

	public override string Render(ILocalizationProvider localizationProvider, ILocale locale)
	{
		return Key;
	}
}