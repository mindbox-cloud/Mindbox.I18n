using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mindbox.I18n
{
	public static class Locales
	{
		private static readonly IDictionary<string, Locale> localesByName;

		public static Locale ruRU { get; } = new Locale("ru-RU");
		public static Locale enUS { get; } = new Locale("en-US");

		public static Locale GetByName(string name) => localesByName[name];

		public static Locale TryGetByName(string name) => localesByName.TryGetValue(name, out var locale) 
			? locale 
			: null;

		static Locales()
		{
			localesByName = new []
			{
				ruRU,
				enUS
			}.ToDictionary(locale => locale.Name);
		}
	}

	public class Locale : IEquatable<Locale>
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
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != this.GetType())
				return false;
			return Equals((Locale)obj);
		}

		public override int GetHashCode() => Name.GetHashCode();

		public bool Equals(Locale other) => Name == other?.Name;

		public override string ToString() => Name;
	}
}