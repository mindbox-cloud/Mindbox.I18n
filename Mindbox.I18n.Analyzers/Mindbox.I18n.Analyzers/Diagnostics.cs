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

using Microsoft.CodeAnalysis;

namespace Mindbox.I18n.Analyzers;
#pragma warning disable CA1724
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
			"No translation found for key \"{0}\"",
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