using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;

namespace CancellationTokenAnalyzers
{
    public class ParameterSymbolCompatibilityComparer
        : IEqualityComparer<IParameterSymbol>
    {
        public static readonly ParameterSymbolCompatibilityComparer Default
            = new ParameterSymbolCompatibilityComparer();

        public bool Equals(
                IParameterSymbol x,
                IParameterSymbol y)
            => (x.Name == y.Name)
                && (x.IsParams == y.IsParams)
                && SymbolEqualityComparer.IncludeNullability.Equals(x.Type, y.Type);

        public int GetHashCode(
                IParameterSymbol obj)
            => HashCode.Combine(
                SymbolEqualityComparer.IncludeNullability.GetHashCode(obj.Type),
                obj.Name.GetHashCode());
        
    }
}
