using System;
using System.Runtime.Serialization;

namespace Mindbox.I18n;

[Serializable]
public class TranslationException : Exception
{
	public static TranslationException MissingKey(string localeName, string @namespace, string key) =>
		new($"Key \"{key}\" was not found in namespace \"{@namespace}\" for locale \"{localeName}\".");

	public static TranslationException MissingNamespace(string localeName, string @namespace, string key) =>
		new($"Namespace \"{@namespace}\" was not found for key \"{key}\" for locale \"{localeName}\".");

	public TranslationException() { }
	public TranslationException(string message) : base(message) { }
	public TranslationException(string message, Exception inner) : base(message, inner) { }

	protected TranslationException(
		SerializationInfo info,
		StreamingContext context) : base(info, context)
	{
	}
}