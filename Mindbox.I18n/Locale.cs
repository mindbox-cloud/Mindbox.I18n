// Copyright 2022 Mindbox Ltd
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
	private CultureInfo? _culture;

	public string Name { get; }
	public CultureInfo Culture => _culture ??= new CultureInfo(Name);

	static Locale()
	{
	}

	internal Locale(string name)
	{
		Name = name;
	}

	public override bool Equals(object? obj)
	{
		if (obj is null)
			return false;
		if (ReferenceEquals(this, obj))
			return true;
		if (obj.GetType() != GetType())
			return false;
		return Equals((ILocale)obj);
	}

	[SuppressMessage("Globalization", "CA1307:Specify StringComparison for clarity")]
	public override int GetHashCode() => Name.GetHashCode();

	public bool Equals(ILocale? other) => Name == other?.Name;

	public override string ToString() => Name;
}