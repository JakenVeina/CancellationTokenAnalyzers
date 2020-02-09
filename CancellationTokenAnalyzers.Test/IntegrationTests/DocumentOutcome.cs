namespace CancellationTokenAnalyzers.Test.IntegrationTests
{
    public class DocumentOutcome
    {
        public DocumentOutcome(
            string name,
            string text)
        {
            Name = name;
            Text = text;
        }

        public string Name { get; }

        public string Text { get; }
    }
}
