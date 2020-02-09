using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CancellationTokenAnalyzers.Test.IntegrationTests
{
    public abstract class CancellationTokenUsageIntegrationTestFixtureBase
        : IntegrationTestFixtureBase
    {
        protected sealed override DiagnosticAnalyzer Analyzer { get; }
            = new CancellationTokenUsageAnalyzer();

        protected sealed override CodeFixProvider CodeFixProvider { get; }
            = new CancellationTokenUsageCodeFixProvider();
    }
}
