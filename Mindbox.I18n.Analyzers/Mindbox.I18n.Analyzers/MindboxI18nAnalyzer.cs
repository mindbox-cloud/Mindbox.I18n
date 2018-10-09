using System;
using System.Collections.Immutable;
using System.Linq;
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

		private bool IsReferenceWithLocalizationKeyAttribute(IOperation operation)
		{
			switch (operation)
			{
				case IMemberReferenceOperation memberReferenceOperation:
					return CheckAttribute(memberReferenceOperation.Member);
				case IArgumentOperation argumentOperation:
					return CheckAttribute(argumentOperation.Parameter);
				case IParameterReferenceOperation parameterReferenceOperation:
					return CheckAttribute(parameterReferenceOperation.Parameter);
				default:
					return false;
			}
		}

		private bool CheckAttribute(ISymbol symbol)
		{
			return symbol.GetAttributes().Any(attribute => attribute.AttributeClass.Name.Contains("LocalizationKey"));
		}

		private bool IsStringToLocalizableStringConversion(SyntaxNodeAnalysisContext context)
		{
			var expression = context.Node;
			var operation = context.SemanticModel.GetOperation(expression);

			if (expression.Parent?.IsKind(SyntaxKind.AttributeArgument) == true)
			{
				var methodSymbol = (IMethodSymbol)context.SemanticModel.GetSymbolInfo((AttributeSyntax)expression.Parent.Parent.Parent).Symbol;
				var parameterSymbol = Extensions.GetParameterSymbol(methodSymbol, (AttributeArgumentSyntax) expression.Parent);
				return CheckAttribute(parameterSymbol);
			}

			if (operation?.Parent is IAssignmentOperation assignmentOperation && operation == assignmentOperation.Value)
			{
				if (IsReferenceWithLocalizationKeyAttribute(assignmentOperation.Target) && !IsReferenceWithLocalizationKeyAttribute(assignmentOperation.Value))
				{
					return true;
				}
			}

			if (operation?.Parent is IArgumentOperation argumentOperation)
			{
				if (IsReferenceWithLocalizationKeyAttribute(argumentOperation) && !IsReferenceWithLocalizationKeyAttribute(argumentOperation.Value))
				{
					return true;
				}
			}

			var typeInfo = context.SemanticModel.GetTypeInfo(expression);
			if (typeInfo.ConvertedType == null)
				return false;
			if (typeInfo.ConvertedType.Equals(typeInfo.Type))
				return false;

			// Todo: match full type name (need to migrate projects to Mindbox.I18n first).
			return typeInfo.ConvertedType.Name.EndsWith("LocalizableString");
		}
	}

	public static class Extensions
	{
		/// <summary>
		/// To be able to convert positional arguments to named we need to find
		/// corresponding <see cref="IParameterSymbol" /> for each argument.
		/// </summary>
		public static IParameterSymbol GetParameterSymbol(IMethodSymbol methodSymbol, AttributeArgumentSyntax argument)
		{
			var parameters = methodSymbol.Parameters;

			if (argument.NameColon != null)
			{
				var nameText = argument.NameColon.Name?.Identifier.ValueText;
				if (nameText == null)
					return default;

				foreach (var parameter in parameters)
				{
					if (string.Equals(parameter.Name, nameText, StringComparison.Ordinal))
						return parameter;
				}
			}
			else
			{
				var argumentList = argument.Parent as AttributeArgumentListSyntax;
				var index = argumentList.Arguments.IndexOf(argument);
				if (index < 0)
					return default;

				if (index < parameters.Length)
					return parameters[index];

				if (index >= parameters.Length &&
					parameters[parameters.Length - 1].IsParams)
				{
					return parameters[parameters.Length - 1];
				}
			}

			return default;
	
		}
	}
}
