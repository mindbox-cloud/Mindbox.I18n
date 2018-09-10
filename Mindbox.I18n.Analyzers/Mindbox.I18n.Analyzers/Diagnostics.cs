using Microsoft.CodeAnalysis;

namespace Mindbox.I18n.Analyzers
{
	public static class Diagnostics
	{
		private const string Category = "Localization";

		public static DiagnosticDescriptor TranslationHint { get; } =
			new DiagnosticDescriptor(
				"Mindbox1800",
				"Translation will be shown for the localization key",
				"\"{0}\"",
				Category,
				DiagnosticSeverity.Info,
				true);

		internal static DiagnosticDescriptor KeyMustHaveCorrectFormat { get; } =
			new DiagnosticDescriptor(
				"Mindbox1801",
				"Localization key must have correct format",
				"Incorrect localization key \"{0}\". \n"
				+ "The correct format is \"Namespace:Key\". \n"
				+ "Both namespace and key can only contain latin characters, digits and underscores",
				Category,
				DiagnosticSeverity.Error,
				true);

		public static DiagnosticDescriptor TranslationMustExistForLocalizationKey { get; } =
			new DiagnosticDescriptor(
				"Mindbox1802",
				"Translation for key must exist",
				"Translation not found for key \"{0}\"",
				Category,
				DiagnosticSeverity.Error,
				true);

		public static DiagnosticDescriptor OnlyStringLiteralsCanBeUsedAsKeys { get; } =
			new DiagnosticDescriptor(
				"Mindbox1803",
				"Only string literals can be used as localization keys",
				"Expression of this type can't be used as a localization key. " +
				$"Only string literals or other {nameof(LocalizableString)} instances " +
				$"can be used as a {nameof(LocalizableString)} value",
				Category,
				DiagnosticSeverity.Error,
				true);
	}
}
