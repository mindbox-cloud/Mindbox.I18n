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

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Mindbox.I18n.Analyzers;

#nullable disable

public static class SymbolExtensions
{
	public static ImmutableArray<IParameterSymbol> GetParameters(this ISymbol symbol)
	{
		return symbol switch
		{
			IMethodSymbol m => m.Parameters,
			IPropertySymbol nt => nt.Parameters,
			_ => ImmutableArray<IParameterSymbol>.Empty,
		};
	}

	public static bool IsMarkedWithLocalizationKeyAttribute(this ISymbol symbol)
	{
		if (symbol == null)
			return false;

		return symbol.GetAttributes()
			.Any(attribute => attribute.AttributeClass.Name.Contains("LocalizationKey", StringComparison.InvariantCulture));
	}

	public static bool IsLocalizableString(this ITypeSymbol type)
	{
		return type.Name.Contains(nameof(LocalizableString), StringComparison.InvariantCulture);
	}

	public static bool IsString(this ITypeSymbol symbolType)
	{
#pragma warning disable CA1309
		return symbolType.Name.Equals("string", StringComparison.InvariantCultureIgnoreCase);
#pragma warning restore CA1309
	}
}