using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

using CancellationTokenAnalyzers.Diagnostics;

namespace CancellationTokenAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class CancellationTokenUsageAnalyzer : DiagnosticAnalyzer
    {
        public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
            = ImmutableArray.Create(
                CancellationTokenAvailableButNotSuppliedDiagnostic.Descriptor);

        public sealed override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterCompilationStartAction(AnalyzeCompilationStart);
        }

        private static void AnalyzeCompilationStart(CompilationStartAnalysisContext context)
        {
            var cancellationTokenType = context.Compilation.GetTypeByMetadataName(typeof(CancellationToken).FullName);
            if (cancellationTokenType is null)
                return;

            context.RegisterOperationAction(
                c => AnalyzeMethodInvocation2(c, cancellationTokenType),
                OperationKind.Invocation);
        }

        //private static void AnalyzeMethodInvocation(
        //    OperationAnalysisContext context,
        //    ITypeSymbol cancellationTokenType)
        //{
        //    var invocation = (IInvocationOperation)context.Operation;

        //    // Check to see if the invocation is already properly cancelled.
        //    // This check is really fast, do this first.
        //    var (cancellationTokenArgument, cancellationTokenIndex) = invocation.Arguments
        //        .Where(argument => !argument.Parameter.IsThis || invocation.Instance != null)
        //        .Select((argument, index) => (argument, index))
        //        .FirstOrDefault(tuple => SymbolEqualityComparer.IncludeNullability.Equals(tuple.argument.Parameter.Type, cancellationTokenType));
            
        //    // If there's a CancellationToken given explicitly, we're done
        //    if (cancellationTokenArgument?.IsImplicit == false)
        //        return;

        //    // Check to see if there's an available CancellationToken in scope.
        //    // This check requires a lookup, but with a relatively small scope, do this second.
        //    var availableCancellationToken = invocation.SemanticModel
        //        .LookupSymbols(
        //            position: invocation.Syntax.SpanStart)
        //        .Select<ISymbol, (ISymbol symbol, ITypeSymbol? type)>(symbol => symbol switch
        //        {
        //            IFieldSymbol fieldSymbol            => (symbol, fieldSymbol.Type),
        //            ILocalSymbol localSymbol            => (symbol, localSymbol.Type),
        //            IParameterSymbol parameterSymbol    => (symbol, parameterSymbol.Type),
        //            _                                   => (symbol, null),
        //        })
        //        .FirstOrDefault((tuple) => SymbolEqualityComparer.IncludeNullability.Equals(tuple.type, cancellationTokenType))
        //        .symbol;
            
        //    if (availableCancellationToken is null)
        //        return;

        //    // Check to see if there's an available overload or extension method that takes a CancellationToken
        //    // This check requires a lookup, including all static members in all referenced assemblies in the project, do this last.
        //    if (cancellationTokenArgument is null)
        //    {
        //        var cancellableMethod = invocation.SemanticModel
        //            .LookupSymbols(
        //                position:                       invocation.Syntax.SpanStart,
        //                container:                      invocation.Instance?.Type ?? invocation.TargetMethod.ContainingType,
        //                name:                           invocation.TargetMethod.Name,
        //                includeReducedExtensionMethods: true)
        //            .OfType<IMethodSymbol>()
        //            .FirstOrDefault(m => !SymbolEqualityComparer.IncludeNullability.Equals(m, invocation.TargetMethod)
        //                && m.IsStatic == invocation.TargetMethod.IsStatic
        //                && m.Parameters.Any(p => SymbolEqualityComparer.IncludeNullability.Equals(p.Type, cancellationTokenType))
        //                && Enumerable.SequenceEqual(
        //                    m.Parameters.Where(p => !SymbolEqualityComparer.IncludeNullability.Equals(p.Type, cancellationTokenType)),
        //                    invocation.TargetMethod.Parameters,
        //                    ParameterSymbolCompatibilityComparer.Default));

        //        if (cancellableMethod is null)
        //            return;

        //        cancellationTokenIndex = cancellableMethod.Parameters
        //            .Select((parameter, index) => (parameter, index))
        //            .FirstOrDefault(tuple => SymbolEqualityComparer.IncludeNullability.Equals(tuple.parameter.Type, cancellationTokenType))
        //            .index;
        //    }

        //    // If we made it here, we have everything we need to build a diagnostic, with codefix metadata
        //    context.ReportDiagnostic(CancellationTokenAvailableButNotSuppliedDiagnostic.Create(
        //        invocation.Syntax.GetLocation(),
        //        availableCancellationToken.Name,
        //        cancellationTokenIndex));
        //}

        private static void AnalyzeMethodInvocation2(
            OperationAnalysisContext context,
            ITypeSymbol cancellationTokenType)
        {
            var invocation = (IInvocationOperation)context.Operation;

            // Check to see if the invocation is already properly cancelled.
            // This check is really fast, do this first.
            var cancellationTokenArgument = invocation.Arguments
                .FirstOrDefault(argument => SymbolEqualityComparer.IncludeNullability.Equals(argument.Parameter.Type, cancellationTokenType));

            if (cancellationTokenArgument?.IsImplicit == false)
                return;


            // Check to see if there's an available CancellationToken in scope.
            // This check requires a lookup, but with a relatively small scope, do this second.
            var availableCancellationToken = invocation.SemanticModel
                .LookupSymbols(
                    position: invocation.Syntax.SpanStart)
                .Select<ISymbol, (ISymbol symbol, ITypeSymbol? type)>(symbol => symbol switch
                {
                    IFieldSymbol fieldSymbol => (symbol, fieldSymbol.Type),
                    ILocalSymbol localSymbol => (symbol, localSymbol.Type),
                    IParameterSymbol parameterSymbol => (symbol, parameterSymbol.Type),
                    _ => (symbol, null),
                })
                .FirstOrDefault((tuple) => SymbolEqualityComparer.IncludeNullability.Equals(tuple.type, cancellationTokenType))
                .symbol;

            if (availableCancellationToken is null)
                return;


            // Check to see if there's an available overload or extension method that takes a CancellationToken
            // This check requires a lookup, including all static members in all referenced assemblies in the project, do this last.
            var cancellableMethod = (cancellationTokenArgument is null)
                ? invocation.SemanticModel
                    .LookupSymbols(
                        position: invocation.Syntax.SpanStart,
                        container: invocation.Instance?.Type ?? invocation.TargetMethod.ContainingType,
                        name: invocation.TargetMethod.Name,
                        includeReducedExtensionMethods: true)
                    .OfType<IMethodSymbol>()
                    .FirstOrDefault(m => !SymbolEqualityComparer.IncludeNullability.Equals(m, invocation.TargetMethod)
                        && m.IsStatic == invocation.TargetMethod.IsStatic
                        && m.Parameters.Any(p => SymbolEqualityComparer.IncludeNullability.Equals(p.Type, cancellationTokenType))
                        && Enumerable.SequenceEqual(
                            m.Parameters.Where(p => !SymbolEqualityComparer.IncludeNullability.Equals(p.Type, cancellationTokenType)),
                            invocation.TargetMethod.Parameters,
                            ParameterSymbolCompatibilityComparer.Default))
                : invocation.TargetMethod;

            if (cancellableMethod is null)
                return;


            // If we have found an available CancellationToken and cancellable method, we can go ahead and build a diagnostic.
            // To do this, we need to figure out how and where to insert the token into the the argument list.
            var (firstMismatchedArgument, firstMismatchedParameter, firstMismatchIndex) = Enumerable.Zip(
                    invocation.Arguments
                        .Skip(invocation.TargetMethod.IsExtensionMethod ? 1 : 0),
                    cancellableMethod.Parameters
                        .Skip(cancellableMethod.IsExtensionMethod ? 1 : 0),
                    (argument, parameter) => (argument, parameter))
                .Select((tuple, index) => (tuple.argument, tuple.parameter, index))
                .SkipWhile(tuple => !tuple.argument.IsImplicit
                    && (tuple.argument.Parameter.Name == tuple.parameter.Name)
                    && SymbolEqualityComparer.IncludeNullability.Equals(tuple.argument.Parameter.Type, tuple.parameter.Type))
                .FirstOrDefault();

            var invocationLocation = invocation.Syntax.GetLocation();

            // If we never found a mismatch, insert at the end of the list, and we don't need a named argument.
            if (firstMismatchedParameter is null)
                context.ReportDiagnostic(CancellationTokenAvailableButNotSuppliedDiagnostic.Create(
                    invocationLocation,
                    availableCancellationToken.Name,
                    null,
                    null));
            // If the first mismatch we found is the CancellationToken parameter, insert at that index, and we don't need a named argument.
            else if(SymbolEqualityComparer.IncludeNullability.Equals(firstMismatchedParameter.Type, cancellationTokenType))
                context.ReportDiagnostic(CancellationTokenAvailableButNotSuppliedDiagnostic.Create(
                    invocationLocation,
                    availableCancellationToken.Name,
                    firstMismatchIndex,
                    null));
            // For everything else, insert at the end of the list, with a named argument.
            else
                context.ReportDiagnostic(CancellationTokenAvailableButNotSuppliedDiagnostic.Create(
                    invocationLocation,
                    availableCancellationToken.Name,
                    null,
                    cancellableMethod.Parameters
                        .First(parameter => SymbolEqualityComparer.IncludeNullability.Equals(parameter.Type, cancellationTokenType))
                        .Name));
        }
    }
}
