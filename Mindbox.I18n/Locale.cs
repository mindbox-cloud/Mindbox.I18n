using System;
using System.Collections.Generic;
using System.Linq;

using Mindbox.I18n.Abstractions;

namespace Mindbox.I18n;

public static class Locales
{
	private static readonly IDictionary<string, ILocale> _localesByName;

	public static ILocale ruRU { get; } = new Locale("ru-RU");
	public static ILocale enUS { get; } = new Locale("en-US");

	public static ILocale GetByName(string name) => _localesByName[name];

	public static ILocale? TryGetByName(string name) => _localesByName.TryGetValue(name, out var locale)
		? locale
		: null;

#pragma warning disable CA1810
	static Locales()
#pragma warning restore CA1810
	{
		_localesByName = new[]
		{
			ruRU,
			enUS
		}.ToDictionary(locale => locale.Name);
	}
}

public class Locale : ILocale, IEquatable<ILocale>
{
	public string Name { get; }

	static Locale()
	{
	}

	internal Locale(string name)
	{
		Name = name;
	}

	public override bool Equals(object obj)
	{
		if (obj is null)
			return false;
		if (ReferenceEquals(this, obj))
			return true;
		if (obj.GetType() != GetType())
			return false;
		return Equals((ILocale)obj);
	}

	public override int GetHashCode() => Name.GetHashCode();

	public bool Equals(ILocale other) => Name == other?.Name;

	public override string ToString() => Name;
}