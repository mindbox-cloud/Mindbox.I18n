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
			if (TryReportDiagnosticForLiteralExpression(reportDiagnostic, localizationKeyValueNode))
				return;

			if (TryReportDiagnosticForConditionalExpression(reportDiagnostic, localizationKeyValueNode))
				return;

			reportDiagnostic(
				Diagnostic.Create(
					Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys,
					localizationKeyValueNode.GetLocation(),
					string.Empty));
		}

		private bool TryReportDiagnosticForLiteralExpression(
			Action<Diagnostic> reportDiagnostic, 
			SyntaxNode localizationKeyValueNode)
		{
			if (!(localizationKeyValueNode is LiteralExpressionSyntax literal))
				return false;

			if (!(literal.Token.Value is string stringKey))
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

		private bool TryReportDiagnosticForConditionalExpression(
			Action<Diagnostic> reportDiagnostic,
			SyntaxNode localizationKeyValueNode)
		{
			if (!(localizationKeyValueNode is ConditionalExpressionSyntax conditionalExpression))
				return false;

			ReportDiagnosticAboutLocalizableStringAssignment(reportDiagnostic, conditionalExpression.WhenTrue);
			ReportDiagnosticAboutLocalizableStringAssignment(reportDiagnostic, conditionalExpression.WhenFalse);

			return true;
		}
	}
}