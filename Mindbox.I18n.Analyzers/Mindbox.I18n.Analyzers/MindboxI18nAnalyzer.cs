using System;
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

			context.RegisterOperationAction(
				HandleAssignmentOperation, 
				OperationKind.SimpleAssignment,
				OperationKind.CompoundAssignment);
			
			context.RegisterOperationAction(
				HandleVariableDeclarationOperation, 
				OperationKind.VariableDeclarator);

			context.RegisterOperationAction(
				HandleInvocationOperation, 
				OperationKind.Invocation);
		}

		private void HandleAssignmentOperation(OperationAnalysisContext context)
		{
			var effectiveOperation = (IAssignmentOperation)context.Operation;

			if (IsNonLocalizableStringAssignmentToLocalizableString(effectiveOperation))
			{
				ReportDiagnosticAboutLocalizableStringAssignment(context, effectiveOperation.Value.Syntax);
			}
		}
		
		private void HandleVariableDeclarationOperation(OperationAnalysisContext context)
		{
			var effectiveOperation = (IVariableDeclaratorOperation)context.Operation;

			if (!IsLocalizableString(effectiveOperation.Symbol.Type))
				return;

			//if (IsLocalizableString(effectiveOperation.Initializer))


			//if (IsAssignmentToLocalizableString(effectiveOperation))
			//{
			//	ReportDiagnosticAboutLocalizableStringAssignment(context, effectiveOperation.Value.Syntax);
			//}
		}

		private void ReportDiagnosticAboutLocalizableStringAssignment(
			OperationAnalysisContext context, 
			SyntaxNode localizationKeyValue)
		{
			var semanticModel = context.Compilation.GetSemanticModel(localizationKeyValue.SyntaxTree);
			var localizationKey = semanticModel.GetConstantValue(localizationKeyValue);

			var location = localizationKeyValue.GetLocation();

			if (localizationKey.HasValue)
			{
				ReportDiagnosticAboutLocalizationKey(context, localizationKey.Value, location);
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					Diagnostics.KeyMustHaveCorrectFormat,
					location,
					string.Empty));
		}

		private void ReportDiagnosticAboutLocalizationKey(
			OperationAnalysisContext context, 
			object localizationKeyValue,
			Location location)
		{
			var stringKey = localizationKeyValue as string;
			if (stringKey is null)
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						Diagnostics.KeyMustHaveCorrectFormat,
						location,
						string.Empty));
			}

			var localizationKey = LocalizationKey.TryParse(stringKey);

			if (localizationKey == null)
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						Diagnostics.KeyMustHaveCorrectFormat,
						location,
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
								location,
								stringKey));
					}
					else
					{
						context.ReportDiagnostic(
							Diagnostic.Create(
								Diagnostics.TranslationHint,
								location,
								translation));
					}
				}
			}
		}

		private bool IsNonLocalizableStringAssignmentToLocalizableString(IAssignmentOperation effectiveOperation)
		{
			if (!IsLocalizableString(effectiveOperation.Target))
				return false;

			if (IsLocalizableString(effectiveOperation.Value))
				return false;

			return true;
		}

		private bool IsLocalizableString(IOperation operation)
		{
			if (IsLocalizableString(operation.Type))
				return true;

			return LocalizationAttributeHelper.IsStringReferenceWithLocalizationKeyAttribute(operation);
		}

		private bool IsLocalizableString(ITypeSymbol type)
		{
			return type.Name.Contains(typeof(LocalizableString).Name);
		}

		private void HandleInvocationOperation(OperationAnalysisContext context)
		{
			var effectiveOperation = (IInvocationOperation)context.Operation;

			foreach (var effectiveOperationArgument in effectiveOperation.Arguments)
			{
				HandleArgumentOperation(context, effectiveOperationArgument);
			}
		}

		private void HandleArgumentOperation(OperationAnalysisContext context, IArgumentOperation argument)
		{
			//if (argument.Parameter.IsString && argument.Parameter.HasLocalizationAttribute || argument.Parameter.IsLocalizationString)
			
			//if (IsAssignmentToLocalizableString(effectiveOperation))
			//{
			//	ReportDiagnosticAboutLocalizableStringAssignment(context, effectiveOperation.Value.Syntax);
			//}
		}

		private void OnCompilationStart(CompilationStartAnalysisContext context)
		{
			translationSource = translationSource ?? 
			    TranslationSourceContainer.TryGetTranslationSourceFromAnalyzerOptions(context.Options);

			context.RegisterSyntaxNodeAction(
				AnalyzeStringLiteralExpression,
				SyntaxKind.StringLiteralExpression);

			//context.RegisterSyntaxNodeAction(
			//	AnalyzePossiblyIncorrectLocalizableStringExpression,
			//	SyntaxKind.InterpolatedStringExpression,
			//	SyntaxKind.InvocationExpression,
			//	SyntaxKind.AddExpression,
			//	SyntaxKind.ConditionalAccessExpression,
			//	SyntaxKind.IdentifierName);
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

			// check for an attribute constructor argument
			if (expression.Parent?.IsKind(SyntaxKind.AttributeArgument) == true)
			{
				var parameterSymbol = LocalizationAttributeHelper.GetAttributeParameterSymbol(context, (AttributeArgumentSyntax) expression.Parent);
				return LocalizationAttributeHelper.CheckForLocalizationAttribute(parameterSymbol);
			}

			var operation = context.SemanticModel.GetOperation(expression);

			// field or property with an attribute
			if (operation?.Parent is IAssignmentOperation assignmentOperation && operation == assignmentOperation.Value)
			{
				if (LocalizationAttributeHelper.IsStringReferenceWithLocalizationKeyAttribute(assignmentOperation.Target) 
				    && !LocalizationAttributeHelper.IsStringReferenceWithLocalizationKeyAttribute(assignmentOperation.Value))
				{
					return true;
				}
			}

			// method invocation with an attributed parameter
			if (operation?.Parent is IArgumentOperation argumentOperation)
			{
				if (LocalizationAttributeHelper.IsStringReferenceWithLocalizationKeyAttribute(argumentOperation) 
				    && !LocalizationAttributeHelper.IsStringReferenceWithLocalizationKeyAttribute(argumentOperation.Value))
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
