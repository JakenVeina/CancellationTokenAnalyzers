using System;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;

namespace CancellationTokenAnalyzers.Diagnostics
{
    public static class CancellationTokenAvailableButNotSuppliedDiagnostic
    {
        public static readonly DiagnosticDescriptor Descriptor
            = new DiagnosticDescriptor(
                id:                 "CTU0001",
                title:              "CancellationToken available, but not supplied",
                messageFormat:      "Supply CancellationToken to method call, or call equivalent overload",
                category:           "Usage",
                defaultSeverity:    DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description:        "This method, or an equivalent overload, accepts a CancellationToken, but was not given one, even though an in-scope CancellationToken value was found.");

        public static Diagnostic Create(
                Location invocationLocation,
                string cancellationTokenName,
                int? insertIndex,
                string? parameterName)
            => Diagnostic.Create(
                Descriptor,
                invocationLocation,
                ImmutableDictionary.CreateBuilder<string, string?>()
                    .With(_cancellationTokenNameKey, cancellationTokenName)
                    .With(_insertIndexKey, insertIndex?.ToString())
                    .With(_parameterNameKey, parameterName)
                    .ToImmutable());

        public static (Location invocationLocation, string cancellationTokenName, int? insertIndex, string? parameterName) GetMetadata(
                Diagnostic diagnostic)
            => (invocationLocation:     diagnostic.Location,
                cancellationTokenName:  diagnostic.Properties[_cancellationTokenNameKey],
                insertIndex:            diagnostic.Properties[_insertIndexKey]?.ParseInt32(),
                parameterName:          diagnostic.Properties[_parameterNameKey]);

        private const string _cancellationTokenNameKey
            = "cancellationTokenName";

        private const string _insertIndexKey
            = "insertIndex";

        private const string _parameterNameKey
            = "parameterKey";
    }
}
