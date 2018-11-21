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
			if (TryReportDiagnosticForLiteralExression(reportDiagnostic, localizationKeyValueNode))
				return;

			if (TryReportDiagnosticForConditionalExression(reportDiagnostic, localizationKeyValueNode))
				return;

			reportDiagnostic(
				Diagnostic.Create(
					Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys,
					localizationKeyValueNode.GetLocation(),
					string.Empty));

			return;
		}

		private bool TryReportDiagnosticForLiteralExression(
			Action<Diagnostic> reportDiagnostic, 
			SyntaxNode localizationKeyValueNode)
		{
			var literal = localizationKeyValueNode as LiteralExpressionSyntax;

			if (literal == null)
				return false;

			var stringKey = literal.Token.Value as string;
			if (stringKey == null)
			{
				reportDiagnostic(
					Diagnostic.Create(
						Diagnostics.KeyMustHaveCorrectFormat,
						localizationKeyValueNode.GetLocation(),
						string.Empty));
				return true;
			}

			var localizationKey = LocalizationKey.TryParse(stringKey);

			if (localizationKey == null)
			{
				reportDiagnostic(
					Diagnostic.Create(
						Diagnostics.KeyMustHaveCorrectFormat,
						localizationKeyValueNode.GetLocation(),
						stringKey));
				return true;
			}

			if (_translationSource == null) 
				return true;

			var translation = _translationSource.TryGetTranslation(localizationKey);

			if (translation == null)
			{
				reportDiagnostic(
					Diagnostic.Create(
						Diagnostics.TranslationMustExistForLocalizationKey,
						localizationKeyValueNode.GetLocation(),
						stringKey));

				return true;
			}

			reportDiagnostic(
				Diagnostic.Create(
					Diagnostics.TranslationHint,
					localizationKeyValueNode.GetLocation(),
					translation));

			return true;
		}

		private bool TryReportDiagnosticForConditionalExression(
			Action<Diagnostic> reportDiagnostic,
			SyntaxNode localizationKeyValueNode)
		{
			var conditionalExression = localizationKeyValueNode as ConditionalExpressionSyntax;
			if (conditionalExression == null)
				return false;

			ReportDiagnosticAboutLocalizableStringAssignment(reportDiagnostic, conditionalExression.WhenTrue);
			ReportDiagnosticAboutLocalizableStringAssignment(reportDiagnostic, conditionalExression.WhenFalse);

			return true;
		}
	}
}