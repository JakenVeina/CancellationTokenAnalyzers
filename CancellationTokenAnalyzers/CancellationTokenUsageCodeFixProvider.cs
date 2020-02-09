using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

using CancellationTokenAnalyzers.Diagnostics;

namespace CancellationTokenAnalyzers
{
    [Shared]
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CancellationTokenUsageCodeFixProvider))]
    public class CancellationTokenUsageCodeFixProvider
        : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds { get; }
            = ImmutableArray.Create(
                CancellationTokenAvailableButNotSuppliedDiagnostic.Descriptor.Id);

        public override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach(var diagnostic in context.Diagnostics)
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: diagnostic.GetMessage(),
                        equivalenceKey: diagnostic.Id,
                        createChangedDocument: cancellationToken => InsertCancellationTokenAsync(
                           context.Document,
                           diagnostic,
                           cancellationToken)),
                    diagnostic);

            return Task.CompletedTask;
        }

        private static async Task<Document> InsertCancellationTokenAsync(
            Document document,
            Diagnostic diagnostic,
            CancellationToken cancellationToken)
        {
            var (invocationLocation, cancellationTokenName, insertIndex, parameterName) = CancellationTokenAvailableButNotSuppliedDiagnostic.GetMetadata(diagnostic);
            
            var syntaxRoot = (await document.GetSyntaxRootAsync(cancellationToken))!;

            var invocation = (InvocationExpressionSyntax)syntaxRoot.FindNode(invocationLocation.SourceSpan, getInnermostNodeForTie: true);

            var arguments = invocation.ArgumentList.Arguments;

            var argument = SyntaxFactory.Argument(SyntaxFactory.IdentifierName(cancellationTokenName));
            if (parameterName is { })
                argument = argument.WithNameColon(
                    SyntaxFactory.NameColon(SyntaxFactory.IdentifierName(parameterName)));

            arguments = (insertIndex is null)
                ? arguments.Add(argument)
                : arguments.Insert(insertIndex.Value, argument);

            return document
                .WithSyntaxRoot(syntaxRoot
                    .ReplaceNode(invocation, invocation
                        .WithArgumentList(invocation.ArgumentList
                            .WithArguments(arguments))));
        }
    }
}
