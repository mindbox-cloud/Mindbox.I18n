using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Mindbox.I18n.Analyzers
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class MindboxI18nAnalyzer : DiagnosticAnalyzer
	{
		// For testing purposes
		private IAnalyzerTranslationSource translationSource;

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
			this.translationSource = translationSource;
		}

		public override void Initialize(AnalysisContext context)
		{
			context.EnableConcurrentExecution();

			context.RegisterCompilationStartAction(OnCompilationStart);
		}

		private void OnCompilationStart(CompilationStartAnalysisContext context)
		{
			translationSource = translationSource ?? 
			    TranslationSourceContainer.TryGetTranslationSourceFromAnalyzerOptions(context.Options);

			context.RegisterSyntaxNodeAction(
				AnalyzeStringLiteralExpression,
				SyntaxKind.StringLiteralExpression);

			context.RegisterSyntaxNodeAction(
				AnalyzePossiblyIncorrectLocalizableStringExpression,
				SyntaxKind.InterpolatedStringExpression,
				SyntaxKind.InvocationExpression,
				SyntaxKind.AddExpression,
				SyntaxKind.ConditionalAccessExpression,
				SyntaxKind.IdentifierName);
		}

		private void AnalyzeStringLiteralExpression(SyntaxNodeAnalysisContext context)
		{
			if (!IsStringToLocalizationKeyAssignment(context))
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
			if (!IsStringToLocalizationKeyAssignment(context))
				return;

			context.ReportDiagnostic(
				Diagnostic.Create(
					Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys,
					context.Node.GetLocation()));
		}

		/// <summary>
		/// Checks whether the expression is string conversion to a
		/// LocalizableString or assignment to a member marked with LocalizationKeyAttribute
		/// (both cases assume the string must be a valid localization key)
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		private bool IsStringToLocalizationKeyAssignment(SyntaxNodeAnalysisContext context)
		{
			var expression = context.Node;
			var operation = context.SemanticModel.GetOperation(expression);

			// check for an attribute constructor argument
			if (expression.Parent?.IsKind(SyntaxKind.AttributeArgument) == true)
			{
				var parameterSymbol = LocalizationAttributeHelper.GetAttributeParameterSymbol(context, (AttributeArgumentSyntax) expression.Parent);
				return LocalizationAttributeHelper.CheckForLocalizationAttribute(parameterSymbol);
			}

			// field or property with an attribute
			if (operation?.Parent is IAssignmentOperation assignmentOperation && operation == assignmentOperation.Value)
			{
				if (LocalizationAttributeHelper.IsReferenceWithLocalizationKeyAttribute(assignmentOperation.Target) 
				    && !LocalizationAttributeHelper.IsReferenceWithLocalizationKeyAttribute(assignmentOperation.Value))
				{
					return true;
				}
			}

			// method invocation with an attributed parameter
			if (operation?.Parent is IArgumentOperation argumentOperation)
			{
				if (LocalizationAttributeHelper.IsReferenceWithLocalizationKeyAttribute(argumentOperation) 
				    && !LocalizationAttributeHelper.IsReferenceWithLocalizationKeyAttribute(argumentOperation.Value))
				{
					return true;
				}
			}

			// implicit conversion to LocalizableString
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
