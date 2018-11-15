using System.Collections.Immutable;
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
	}
}