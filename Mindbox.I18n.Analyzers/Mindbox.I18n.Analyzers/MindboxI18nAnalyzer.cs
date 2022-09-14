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

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Mindbox.I18n.Analyzers;
#nullable disable
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MindboxI18nAnalyzer : DiagnosticAnalyzer
{
	// For testing purposes
	private readonly IAnalyzerTranslationSource _explicitTranslationSource;

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
		_explicitTranslationSource = translationSource;
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
			_explicitTranslationSource ??
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
		return assignmentOperationTarget switch
		{
			ILocalReferenceOperation localReferenceOperation => localReferenceOperation.Local,
			IMemberReferenceOperation memberReferenceOperation => memberReferenceOperation.Member,
			_ => null,
		};
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