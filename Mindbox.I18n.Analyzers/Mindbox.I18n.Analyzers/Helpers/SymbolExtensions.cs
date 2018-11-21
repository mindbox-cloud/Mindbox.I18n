using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Mindbox.I18n.Analyzers
{
	public static class SymbolExtensions
	{
		public static ImmutableArray<IParameterSymbol> GetParameters(this ISymbol symbol)
		{
			switch (symbol)
			{
				case IMethodSymbol m: return m.Parameters;
				case IPropertySymbol nt: return nt.Parameters;
				default: return ImmutableArray<IParameterSymbol>.Empty;
			}
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
			return type.Name.Contains(typeof(LocalizableString).Name);
		}

		public static bool IsString(this ITypeSymbol symbolType)
		{
			return symbolType.Name.Equals("string", StringComparison.InvariantCultureIgnoreCase);
		}
	}
}