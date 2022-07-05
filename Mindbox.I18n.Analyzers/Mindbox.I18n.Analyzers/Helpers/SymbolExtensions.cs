using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Mindbox.I18n.Analyzers;

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
			.Any(attribute => attribute.AttributeClass.Name.Contains("LocalizationKey"));
	}

	public static bool IsLocalizableString(this ITypeSymbol type)
	{
		return type.Name.Contains(nameof(LocalizableString));
	}

	public static bool IsString(this ITypeSymbol symbolType)
	{
		return symbolType.Name.Equals("string", StringComparison.InvariantCultureIgnoreCase);
	}
}