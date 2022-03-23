using System.Collections.Immutable;
using System.Linq;
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

		private DiagnosticsContext _diagnosticsContext;

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
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

			context.EnableConcurrentExecution();
			context.RegisterCompilationStartAction(OnCompilationStart);
		}

		private void OnCompilationStart(CompilationStartAnalysisContext context)
		{
			_diagnosticsContext = new DiagnosticsContext(
				explicitTranslationSource ??
					TranslationSourceContainer.TryGetTranslationSourceFromAnalyzerOptions(context.Options));

			context.RegisterSyntaxNodeAction(
				HandleAttributeSyntax,
				SyntaxKind.Attribute);

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
			
			context.RegisterOperationAction(
				HandleMethodInvocation,
				OperationKind.Invocation);
			
			// кажется где-то тут
		}

		private void HandleAssignmentOperation(OperationAnalysisContext context)
		{
			var assignmentOperation = (IAssignmentOperation)context.Operation;

			var targetSymbol = TryGetAssignmentOperationTargetSymbol(assignmentOperation.Target);
			if (targetSymbol == null)
				return;

			var assignmentValueSyntax = assignmentOperation.Value.Syntax;
			if (!IsStringToStringWithLocalizationKeyAttributeAssignment(
				context.Operation.SemanticModel,
				assignmentOperation.Target.Type,
				targetSymbol,
				assignmentValueSyntax))
				return;

			_diagnosticsContext.ReportDiagnosticAboutLocalizableStringAssignment(
				context.ReportDiagnostic,
				assignmentValueSyntax);
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
			if (!IsStringToStringWithLocalizationKeyAttributeAssignment(
				context.Operation.SemanticModel,
				declaratorOperation.Symbol.Type,
				declaratorOperation.Symbol,
				declarationValueSyntax))
				return;

			_diagnosticsContext.ReportDiagnosticAboutLocalizableStringAssignment(
				context.ReportDiagnostic,
				declarationValueSyntax);
		}

		private void HandlePropertyInitializerOperation(OperationAnalysisContext context)
		{
			var propertyInitializerOperation = (IPropertyInitializerOperation)context.Operation;

			var initializedProperty = propertyInitializerOperation.InitializedProperties.Single();
			var initializationValueSyntax = propertyInitializerOperation.Value.Syntax;

			if (!IsStringToStringWithLocalizationKeyAttributeAssignment(
				context.Operation.SemanticModel,
				initializedProperty.Type,
				initializedProperty,
				initializationValueSyntax))
				return;

			_diagnosticsContext.ReportDiagnosticAboutLocalizableStringAssignment(
				context.ReportDiagnostic,
				initializationValueSyntax);
		}

		private void HandleFieldInitializerOperation(OperationAnalysisContext context)
		{
			var fieldInitializerOperation = (IFieldInitializerOperation)context.Operation;

			var initializedField = fieldInitializerOperation.InitializedFields.Single();

			var initializationValueSyntax = fieldInitializerOperation.Value.Syntax;
			if (!IsStringToStringWithLocalizationKeyAttributeAssignment(
				context.Operation.SemanticModel,
				initializedField.Type,
				initializedField,
				initializationValueSyntax))
				return;

			_diagnosticsContext.ReportDiagnosticAboutLocalizableStringAssignment(
				context.ReportDiagnostic,
				initializationValueSyntax);
		}

		private void HandleArgumentOperation(OperationAnalysisContext context)
		{
			var argumentOperation = (IArgumentOperation)context.Operation;

			var argumentValueSyntax = argumentOperation.Value.Syntax;
			if (!IsStringToStringWithLocalizationKeyAttributeAssignment(
				context.Operation.SemanticModel,
				argumentOperation.Parameter.Type,
				argumentOperation.Parameter,
				argumentValueSyntax))
				return;

			_diagnosticsContext.ReportDiagnosticAboutLocalizableStringAssignment(
				context.ReportDiagnostic,
				argumentValueSyntax);
		}

		private void HandleConversionOperation(OperationAnalysisContext context)
		{
			var conversionOperation = (IConversionOperation)context.Operation;

			if (conversionOperation.OperatorMethod == null)
				return;

			if (!conversionOperation.OperatorMethod.ContainingType.IsLocalizableString())
				return;

			_diagnosticsContext.ReportDiagnosticAboutLocalizableStringAssignment(
				context.ReportDiagnostic,
				conversionOperation.Operand.Syntax);
		}
		
		private void HandleMethodInvocation(OperationAnalysisContext context)
		{
			var invocationOperation = (IInvocationOperation)context.Operation;
			
			if (invocationOperation.TargetMethod == null)
				return;

			if (!invocationOperation.TargetMethod.ContainingType.IsLocalizableLocaleIndependent())
				return;

			var params1 = invocationOperation.TargetMethod.Parameters.SingleOrDefault();


		}

		private void HandleAttributeSyntax(SyntaxNodeAnalysisContext context)
		{
			var attributeSyntax = (AttributeSyntax)context.Node;

			if (attributeSyntax.ArgumentList == null)
				return;

			foreach (var attributeArgumentSyntax in attributeSyntax.ArgumentList.Arguments)
			{
				var parameterSymbol = attributeArgumentSyntax.DetermineParameter(context.SemanticModel);
				if (parameterSymbol.IsMarkedWithLocalizationKeyAttribute())
					_diagnosticsContext.ReportDiagnosticAboutLocalizableStringAssignment(
						context.ReportDiagnostic,
						attributeArgumentSyntax.Expression);
			}
		}

		private bool IsStringToStringWithLocalizationKeyAttributeAssignment(
			SemanticModel semanticModel,
			ITypeSymbol targetTypeSymbol,
			ISymbol targetSymbol,
			SyntaxNode possiblyStringValueSyntax)
		{
			if (!IsLeftSideStringWithLocalizationKeyAttribute(targetTypeSymbol, targetSymbol))
				return false;

			var typeInfo = semanticModel.GetTypeInfo(possiblyStringValueSyntax);

			if (typeInfo.Type == null)
				return false;

			if (!typeInfo.Type.IsString())
				return false;

			var symbol = semanticModel.GetSymbolInfo(possiblyStringValueSyntax).Symbol;
			if (symbol.IsMarkedWithLocalizationKeyAttribute())
				return false;

			return true;
		}

		private bool IsLeftSideStringWithLocalizationKeyAttribute(ITypeSymbol symbolType, ISymbol symbol)
		{
			if (!symbolType.IsString())
				return false;

			if (symbol.IsMarkedWithLocalizationKeyAttribute())
				return true;

			return false;
		}
	}
}
