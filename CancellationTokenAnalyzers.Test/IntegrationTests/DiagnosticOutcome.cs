namespace CancellationTokenAnalyzers.Test.IntegrationTests
{
    public class DiagnosticOutcome
    {
        public DiagnosticOutcome(
            string diagnosticId,
            string filePath,
            int lineNumber,
            int columnNumber)
        {
            DiagnosticId = diagnosticId;
            FilePath = filePath;
            LineNumber = lineNumber;
            CharacterNumber = columnNumber;
        }

        public string DiagnosticId { get; }

        public string FilePath { get; }

        public int LineNumber { get; }

        public int CharacterNumber { get; }
    }
}
