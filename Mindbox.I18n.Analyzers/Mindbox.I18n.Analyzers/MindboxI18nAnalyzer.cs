using System;
using System.Collections.Immutable;
using System.Diagnostics.SymbolStore;
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
				HandlePropertyInitializerOperation, 
				OperationKind.PropertyInitializer);

			context.RegisterOperationAction(
				HandleFieldInitializerOperation, 
				OperationKind.FieldInitializer);

			context.RegisterOperationAction(
				HandleArgumentOperation, 
				OperationKind.Argument);

			context.RegisterOperationAction(
				HandleConversionOperation,
				OperationKind.Conversion);
		}

		private void HandleConversionOperation(OperationAnalysisContext context)
		{
			var conversionOperation = (IConversionOperation)context.Operation;

			Console.WriteLine(conversionOperation.Conversion);
		}

		private void HandleAssignmentOperation(OperationAnalysisContext context)
		{
			var assignmentOperation = (IAssignmentOperation)context.Operation;

			var targetSymbol = TryGetAssignmentOperationTargetSymbol(assignmentOperation.Target);
			if (targetSymbol == null)
				return;

			var assignmentValueSyntax = assignmentOperation.Value.Syntax;
			if (!IsStringToLocalizableStringAssignment(
				context.Operation.SemanticModel,
				assignmentOperation.Target.Type,
				targetSymbol,
				assignmentValueSyntax))
				return;

			ReportDiagnosticAboutLocalizableStringAssignment(context, assignmentValueSyntax);
		}

		private ISymbol TryGetAssignmentOperationTargetSymbol(IOperation assignmentOperationTarget)
		{
			switch (assignmentOperationTarget)
			{
				case ILocalReferenceOperation localReferenceOperation:
					return localReferenceOperation.Local;
				case IMemberReferenceOperation memberReferenceOperation:
					return memberReferenceOperation.Member;
				default:
					return null;
			}
		}

		private void HandleVariableDeclarationOperation(OperationAnalysisContext context)
		{
			var declaratorOperation = (IVariableDeclaratorOperation)context.Operation;

			if (declaratorOperation.Initializer == null)
				return;

			var declarationValueSyntax = declaratorOperation.Initializer.Value.Syntax;
			if (!IsStringToLocalizableStringAssignment(
				context.Operation.SemanticModel,
				declaratorOperation.Symbol.Type,
				declaratorOperation.Symbol,
				declarationValueSyntax))
				return;

			ReportDiagnosticAboutLocalizableStringAssignment(context, declarationValueSyntax);
		}

		private void HandlePropertyInitializerOperation(OperationAnalysisContext context)
		{
			var propertyInitializerOperation = (IPropertyInitializerOperation)context.Operation;

			var initializedProperty = propertyInitializerOperation.InitializedProperties.Single();
			var initializationValueSyntax = propertyInitializerOperation.Value.Syntax;

			if (!IsStringToLocalizableStringAssignment(
				context.Operation.SemanticModel,
				initializedProperty.Type,
				initializedProperty,
				initializationValueSyntax))
				return;

			ReportDiagnosticAboutLocalizableStringAssignment(context, initializationValueSyntax);
		}

		private void HandleFieldInitializerOperation(OperationAnalysisContext context)
		{
			var fieldInitializerOperation = (IFieldInitializerOperation)context.Operation;

			var initializedField = fieldInitializerOperation.InitializedFields.Single();

			var initializationValueSyntax = fieldInitializerOperation.Value.Syntax;
			if (!IsStringToLocalizableStringAssignment(
				context.Operation.SemanticModel,
				initializedField.Type,
				initializedField,
				initializationValueSyntax))
				return;

			ReportDiagnosticAboutLocalizableStringAssignment(context, initializationValueSyntax);
		}

		private void HandleArgumentOperation(OperationAnalysisContext context)
		{
			var argumentOperation = (IArgumentOperation)context.Operation;

			var argumentValueSyntax = argumentOperation.Value.Syntax;
			if (!IsStringToLocalizableStringAssignment(
				context.Operation.SemanticModel,
				argumentOperation.Parameter.Type,
				argumentOperation.Parameter,
				argumentValueSyntax))
				return;

			ReportDiagnosticAboutLocalizableStringAssignment(context, argumentValueSyntax);
		}

		private bool IsStringToLocalizableStringAssignment(
			SemanticModel semanticModel,
			ITypeSymbol targetTypeSymbol,
			ISymbol targetSymbol,
			SyntaxNode assignmentValueSyntax)
		{
			if (!IsLeftSideLocalizableString(targetTypeSymbol, targetSymbol))
				return false;

			var typeInfo = semanticModel.GetTypeInfo(assignmentValueSyntax);

			if (!IsString(typeInfo.Type))
				return false;

			var symbol = semanticModel.GetSymbolInfo(assignmentValueSyntax).Symbol;
			if (LocalizationAttributeHelper.CheckForLocalizationAttribute(symbol))
				return false;

			return true;
		}

		private bool IsLeftSideLocalizableString(ITypeSymbol symbolType, ISymbol symbol)
		{
			if (IsLocalizableString(symbolType))
				return true;

			if (!IsString(symbolType))
				return false;

			if (LocalizationAttributeHelper.CheckForLocalizationAttribute(symbol))
				return true;

			return false;
		}

		private bool IsString(ITypeSymbol symbolType)
		{
			return symbolType.Name.Equals("string", StringComparison.InvariantCultureIgnoreCase);
		}

		private void ReportDiagnosticAboutLocalizableStringAssignment(
			OperationAnalysisContext context, 
			SyntaxNode localizationKeyValueNode)
		{
			var location = localizationKeyValueNode.GetLocation();

			var literal = localizationKeyValueNode as LiteralExpressionSyntax;
			if (literal == null)
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys,
						location,
						string.Empty));
				return;
			}

			var value = literal.Token.Value;
			ReportDiagnosticAboutLocalizationKey(context, value, location);
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

		private bool IsLocalizableString(ITypeSymbol type)
		{
			return type.Name.Contains(typeof(LocalizableString).Name);
		}

		private void OnCompilationStart(CompilationStartAnalysisContext context)
		{
			translationSource = translationSource ?? 
			    TranslationSourceContainer.TryGetTranslationSourceFromAnalyzerOptions(context.Options);

			//context.RegisterSyntaxNodeAction(
			//	AnalyzeStringLiteralExpression,
			//	SyntaxKind.StringLiteralExpression);

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
