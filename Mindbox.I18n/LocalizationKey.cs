using System;
using System.Text.RegularExpressions;

namespace Mindbox.I18n
{
	public class LocalizationKey : IEquatable<LocalizationKey>
	{
		private static readonly Regex LocalizationKeyRegex =
			new Regex(@"^([a-zA-Z0-9_]+):([a-zA-Z0-9_]+)$", RegexOptions.Compiled);

		public string ShortKey { get; }
		public string Namespace { get; }
		public string FullKey { get; }

		public static LocalizationKey TryParse(string keyWithNamespace)
		{
			var match = LocalizationKeyRegex.Match(keyWithNamespace);
			if (!match.Success)
				return null;

			var @namespace = match.Groups[1].Value;
			var key = match.Groups[2].Value;

			return new LocalizationKey(@namespace, key, keyWithNamespace);
		}

		public override string ToString() => FullKey;

		private LocalizationKey(string @namespace, string shortKey, string fullKey)
		{
			Namespace = @namespace;
			ShortKey = shortKey;
			FullKey = fullKey;
		}

		public bool Equals(LocalizationKey other)
		{
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;

			return StringComparer.InvariantCultureIgnoreCase.Equals(FullKey, other.FullKey);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != this.GetType())
				return false;
			return Equals((LocalizationKey) obj);
		}

		public override int GetHashCode() => StringComparer.InvariantCultureIgnoreCase.GetHashCode(FullKey);
	}
}