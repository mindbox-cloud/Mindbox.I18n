using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mindbox.I18n.Analyzers
{
	internal class DiagnosticsContext
	{
		private readonly IAnalyzerTranslationSource _translationSource;

		public DiagnosticsContext(IAnalyzerTranslationSource translationSource)
		{
			_translationSource = translationSource;
		}

		public void ReportDiagnosticAboutLocalizableStringAssignment(
			Action<Diagnostic> reportDiagnostic, 
			SyntaxNode localizationKeyValueNode)
		{
			var literal = localizationKeyValueNode as LiteralExpressionSyntax;

			if (literal == null)
			{
				reportDiagnostic(
					Diagnostic.Create(
						Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys,
						localizationKeyValueNode.GetLocation(),
						string.Empty));
				return;
			}

			var stringKey = literal.Token.Value as string;
			if (stringKey == null)
			{
				reportDiagnostic(
					Diagnostic.Create(
						Diagnostics.KeyMustHaveCorrectFormat,
						localizationKeyValueNode.GetLocation(),
						string.Empty));
				return;
			}

			var localizationKey = LocalizationKey.TryParse(stringKey);

			if (localizationKey == null)
			{
				reportDiagnostic(
					Diagnostic.Create(
						Diagnostics.KeyMustHaveCorrectFormat,
						localizationKeyValueNode.GetLocation(),
						stringKey));
				return;
			}

			if (_translationSource == null) 
				return;

			var translation = _translationSource.TryGetTranslation(localizationKey);

			if (translation == null)
			{
				reportDiagnostic(
					Diagnostic.Create(
						Diagnostics.TranslationMustExistForLocalizationKey,
						localizationKeyValueNode.GetLocation(),
						stringKey));
				return;
			}

			reportDiagnostic(
				Diagnostic.Create(
					Diagnostics.TranslationHint,
					localizationKeyValueNode.GetLocation(),
					translation));
		}
	}
}