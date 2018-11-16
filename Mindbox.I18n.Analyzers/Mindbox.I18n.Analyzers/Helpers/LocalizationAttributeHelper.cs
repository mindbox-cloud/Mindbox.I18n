using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Mindbox.I18n.Analyzers
{
	public static class LocalizationAttributeHelper
	{
		public static bool IsStringReferenceWithLocalizationKeyAttribute(IOperation operation)
		{
			if (operation.Type == null)
				return false;

			if (!operation.Type.Name.Equals("string", StringComparison.InvariantCultureIgnoreCase))
				return false;

			switch (operation)
			{
				case IMemberReferenceOperation memberReferenceOperation:
					return CheckForLocalizationAttribute(memberReferenceOperation.Member);
				case IArgumentOperation argumentOperation:
					return CheckForLocalizationAttribute(argumentOperation.Parameter);
				case IParameterReferenceOperation parameterReferenceOperation:
					return CheckForLocalizationAttribute(parameterReferenceOperation.Parameter);
				default:
					return false;
			}
		}

		public static IMethodSymbol GetAttributeConstructorMethodSymbol(AttributeArgumentSyntax expression, SemanticModel semanticModel)
		{
			SyntaxNode attributeNode = expression;
			while (!(attributeNode is AttributeSyntax))
			{
				attributeNode = attributeNode.Parent;
				if (attributeNode == null)
					return null;
			}
			return (IMethodSymbol) semanticModel.GetSymbolInfo(attributeNode).Symbol;
		}

		public static bool CheckForLocalizationAttribute(ISymbol symbol)
		{
			if (symbol == null)
				return false;

			return symbol.GetAttributes().Any(attribute => attribute.AttributeClass.Name.Contains("LocalizationKey"));
		}

		public static ISymbol GetAttributeParameterSymbol(SyntaxNodeAnalysisContext context, AttributeArgumentSyntax argument)
		{
			var methodSymbol = GetAttributeConstructorMethodSymbol(argument, context.SemanticModel);

			var parameters = methodSymbol.Parameters;

			if (argument.NameEquals != null)
			{
				var attributeSymbol = (ITypeSymbol)methodSymbol.ContainingSymbol;
				var members = attributeSymbol.GetBaseTypesAndThis()
					.SelectMany(attributeType => attributeType.GetMembers());
				var targetMember = members.FirstOrDefault(member => member.Name == argument.NameEquals.Name.Identifier.ValueText);
				return targetMember;
			}
			else if (argument.NameColon != null)
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
				var argumentList = (AttributeArgumentListSyntax) argument.Parent;
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

		private static IEnumerable<ITypeSymbol> GetBaseTypesAndThis(this ITypeSymbol type)
		{
			var current = type;
			while (current != null)
			{
				yield return current;
				current = current.BaseType;
			}
		}
	}
}