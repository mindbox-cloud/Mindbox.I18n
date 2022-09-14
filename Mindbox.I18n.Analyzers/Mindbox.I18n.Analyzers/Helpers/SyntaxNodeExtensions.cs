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

using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mindbox.I18n.Analyzers;
#nullable disable
public static class SyntaxNodeExtensions
{
	public static IParameterSymbol DetermineParameter(
		this AttributeArgumentSyntax argument,
		SemanticModel semanticModel,
		bool allowParams = false,
		CancellationToken cancellationToken = default)
	{
		// if argument is a named argument it can't map to a parameter.
		if (argument.NameEquals != null)
			return null;

		if (argument.Parent is not AttributeArgumentListSyntax argumentList)
			return null;

		if (argumentList.Parent is not AttributeSyntax invocableExpression)
			return null;

		var symbol = semanticModel.GetSymbolInfo(invocableExpression, cancellationToken).Symbol;
		if (symbol == null)
			return null;

		var parameters = symbol.GetParameters();

		// Handle named argument
		if (argument.NameColon != null && !argument.NameColon.IsMissing)
		{
			var name = argument.NameColon.Name.Identifier.ValueText;
			return parameters.FirstOrDefault(p => p.Name == name);
		}

		// Handle positional argument
		var index = argumentList.Arguments.IndexOf(argument);
		if (index < 0)
			return null;

		if (index < parameters.Length)
			return parameters[index];

		if (allowParams)
		{
			var lastParameter = parameters.LastOrDefault();
			if (lastParameter == null)
				return null;

			if (lastParameter.IsParams)
				return lastParameter;
		}

		return null;
	}
}