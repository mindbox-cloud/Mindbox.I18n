using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Mindbox.I18n.Analyzers
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class MindboxI18nAnalyzer : DiagnosticAnalyzer
	{
		// For testing purposes
		private readonly IAnalyzerTranslationSource explicitTranslationSource;

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
			ImmutableArray.Create(
				Diagnostics.TranslationHint,
				Diagnostics.KeyMustHaveCorrectFormat,
				Diagnostics.TranslationMustExistForLocalizationKey,
				Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys);

		public MindboxI18nAnalyzer()
		{
		}

		internal MindboxI18nAnalyzer(IAnalyzerTranslationSource translationSource)
		{
			explicitTranslationSource = translationSource;
		}

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterCompilationStartAction(OnCompilationStart);
		}

		private void OnCompilationStart(CompilationStartAnalysisContext context)
		{
			var translationSource = explicitTranslationSource ?? 
			    TranslationSourceContainer.TryGetTranslationSourceFromAnalyzerOptions(context.Options);

			context.RegisterSyntaxNodeAction(
				syntaxNodeAnalysisContext => AnalyzeStringLiteralExpression(syntaxNodeAnalysisContext, translationSource),
				SyntaxKind.StringLiteralExpression);

			context.RegisterSyntaxNodeAction(
				AnalyzePossiblyIncorrectLocalizableStringExpression,
				SyntaxKind.InterpolatedStringExpression,
				SyntaxKind.InvocationExpression,
				SyntaxKind.AddExpression,
				SyntaxKind.ConditionalAccessExpression,
				SyntaxKind.IdentifierName);
		}

		private void AnalyzeStringLiteralExpression(SyntaxNodeAnalysisContext context, IAnalyzerTranslationSource translationSource)
		{
			if (!IsStringToLocalizableStringConversion(context))
				return;

			var expression = (LiteralExpressionSyntax) context.Node;
			var stringKey = expression.Token.ValueText;

			var localizationKey = LocalizationKey.TryParse(stringKey);

			if (localizationKey == null)
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						Diagnostics.KeyMustHaveCorrectFormat,
						expression.GetLocation(),
						stringKey));
			}
			else
			{
				if (translationSource != null)
				{
					var translation = translationSource.TryGetTranslation(localizationKey);

					if (translation == null)
					{
						context.ReportDiagnostic(
							Diagnostic.Create(
								Diagnostics.TranslationMustExistForLocalizationKey,
								expression.GetLocation(),
								stringKey));
					}
					else
					{
						context.ReportDiagnostic(
							Diagnostic.Create(
								Diagnostics.TranslationHint,
								expression.GetLocation(),
								translation));
					}
				}
			}
		}

		private void AnalyzePossiblyIncorrectLocalizableStringExpression(SyntaxNodeAnalysisContext context)
		{
			if (!IsStringToLocalizableStringConversion(context))
				return;

			context.ReportDiagnostic(
				Diagnostic.Create(
					Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys,
					context.Node.GetLocation()));
		}

		private bool IsStringToLocalizableStringConversion(SyntaxNodeAnalysisContext context)
		{
			var expression = context.Node;

			var typeInfo = context.SemanticModel.GetTypeInfo(expression);
			if (typeInfo.ConvertedType == null)
				return false;
			if (typeInfo.ConvertedType.Equals(typeInfo.Type))
				return false;

			// Todo: match full type name (need to migrate projects to Mindbox.I18n first).
			return typeInfo.ConvertedType.Name.EndsWith("LocalizableString");
		}
	}
}
