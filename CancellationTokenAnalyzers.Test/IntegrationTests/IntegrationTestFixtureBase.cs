using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using Shouldly;

namespace CancellationTokenAnalyzers.Test.IntegrationTests
{
    public abstract class IntegrationTestFixtureBase
    {
        // Not marking this with [Test] directly, cause then double-clicking in the Test Explorer leads here,
        // instead of to the subclass with all the relevant info
        public async Task PerformTestsAsync()
        {
            var workspace = await BuildWorkspaceAsync();

            var diagnostics = await workspace.AnalyzeAsync(Analyzer);

            VerifyDiagnosticOutcomes(diagnostics);

            var fixedDiagnostics = await TryPerformCodeFixesAsync(workspace, diagnostics);
            fixedDiagnostics.ShouldBeEmpty();

            await VerifyDocumentOutcomesAsync(workspace);
        }

        protected abstract DiagnosticAnalyzer Analyzer { get; }

        protected abstract CodeFixProvider CodeFixProvider { get; }

        protected abstract ImmutableArray<DiagnosticOutcome> ExpectedDiagnostics { get; }

        protected abstract Task<ImmutableArray<DocumentOutcome>> BuildExpectedDocumentsAsync();
     
        protected abstract Task<Workspace> BuildWorkspaceAsync();

        private void VerifyDiagnosticOutcomes(
            ImmutableArray<Diagnostic> diagnostics)
        {
            diagnostics.Select(diagnostic => diagnostic.Id)
                .ShouldBe(ExpectedDiagnostics.Select(outcome => outcome.DiagnosticId), ignoreOrder: true);

            var outcomeSequence = Enumerable.Zip(
                diagnostics
                    .OrderBy(diagnostic => diagnostic.Location.SourceTree.FilePath)
                    .ThenBy(diagnostic => diagnostic.Location.SourceSpan.Start),
                ExpectedDiagnostics
                    .OrderBy(expectedOutcome => expectedOutcome.FilePath)
                    .ThenBy(expectedOutcome => expectedOutcome.LineNumber)
                    .ThenBy(expectedOutcome => expectedOutcome.CharacterNumber));

            foreach (var (diagnostic, expectedOutcome) in outcomeSequence)
            {
                var lineSpan = diagnostic.Location.GetLineSpan();

                lineSpan.Path.ShouldBe(expectedOutcome.FilePath);
                lineSpan.StartLinePosition.Line.ShouldBe(expectedOutcome.LineNumber - 1);
                lineSpan.StartLinePosition.Character.ShouldBe(expectedOutcome.CharacterNumber - 1);
            }
        }

        private async Task<ImmutableArray<Diagnostic>> TryPerformCodeFixesAsync(
            Workspace workspace,
            ImmutableArray<Diagnostic> diagnostics)
        {
            var currentDiagnostics = diagnostics;
            foreach (var _ in Enumerable.Range(0, diagnostics.Length))
            {
                var diagnostic = currentDiagnostics
                    .Where(d => CodeFixProvider.FixableDiagnosticIds.Contains(d.Id))
                    .FirstOrDefault();
                if (diagnostic is null)
                    break;

                var document = workspace.CurrentSolution.GetDocument(diagnostic.Location.SourceTree);
                if (document is null)
                    continue;

                var codeActions = new List<CodeAction>();

                var context = new CodeFixContext(
                    document,
                    diagnostic,
                    (codeAction, applicableDiagnostics) => codeActions.Add(codeAction),
                    CancellationToken.None);

                await CodeFixProvider.RegisterCodeFixesAsync(context);

                if (codeActions.Count == 0)
                    break;

                var operations = await codeActions[0].GetOperationsAsync(CancellationToken.None);
                foreach (var operation in operations)
                    operation.Apply(workspace, CancellationToken.None);

                currentDiagnostics = await workspace.AnalyzeAsync(Analyzer);
            }

            return currentDiagnostics;
        }

        private async Task VerifyDocumentOutcomesAsync(
            Workspace workspace)
        {
            var expectedDocuments = await BuildExpectedDocumentsAsync();

            foreach (var document in workspace.CurrentSolution.Projects.SelectMany(project => project.Documents))
            {
                var documentLines = document
                    .GetTextAsync().Result
                    .Lines
                    .Select(x => x.ToString())
                    .ToArray();

                var expectedLines = expectedDocuments
                    .First(x => x.Name == document.Name)
                    .Text
                    .Replace("\r", "")
                    .Split('\n');

                foreach (var (documentLine, expectedLine) in Enumerable.Zip(documentLines, expectedLines))
                    documentLine.ShouldBe(expectedLine);

                documentLines.Length.ShouldBe(expectedLines.Length);
            }
        }
    }
}
