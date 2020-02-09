using System.Collections.Immutable;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.Diagnostics;

namespace Microsoft.CodeAnalysis
{
    public static class WorkspaceExtensions
    {
        public static async Task<ImmutableArray<Diagnostic>> AnalyzeAsync(
            this Workspace workspace,
            DiagnosticAnalyzer analyzer)
        {
            var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();

            foreach(var project in workspace.CurrentSolution.Projects)
            {
                var compilation = await project.GetCompilationAsync()!;

                diagnostics.AddRange(await compilation
                    .WithAnalyzers(ImmutableArray.Create(analyzer))
                    .GetAllDiagnosticsAsync());
            }

            return diagnostics.ToImmutable();
        }
    }
}
