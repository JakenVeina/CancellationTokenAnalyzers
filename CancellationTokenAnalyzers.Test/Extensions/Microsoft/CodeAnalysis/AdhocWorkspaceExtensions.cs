using System.Linq;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis
{
    public static class AdhocWorkspaceExtensions
    {
        public static AdhocWorkspace WithFakeProject(
            this AdhocWorkspace workspace,
            string name,
            string assemblyName)
        {
            workspace.AddProject(ProjectInfo.Create(
                    id:             ProjectId.CreateNewId(),
                    version:        VersionStamp.Default,
                    name:           name,
                    assemblyName:   assemblyName,
                    language:       LanguageNames.CSharp)
                .WithMetadataReferences(Enumerable.Empty<MetadataReference>()
                    .Append(CorlibReference)
                    .Append(SystemCoreReference)
                    .Append(CSharpSymbolsReference)
                    .Append(CodeAnalysisReference)));
            
            return workspace;
        }

        public static AdhocWorkspace WithFakeDocument(
            this AdhocWorkspace workspace,
            string name,
            string filePath,
            string text)
        {
            if (workspace.CurrentSolution.ProjectIds.Count == 0)
                workspace.WithFakeProject(
                    "FakeProject",
                    "FakeProjectAssembly");

            workspace.AddDocument(DocumentInfo.Create(
                id:         DocumentId.CreateNewId(workspace.CurrentSolution.ProjectIds[0]),
                name:       name,
                filePath:   filePath,
                loader:     TextLoader.From(TextAndVersion.Create(
                    version:    VersionStamp.Default,
                    text:       SourceText.From(text)))));

            return workspace;
        }

        private static readonly MetadataReference CorlibReference
            = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);

        private static readonly MetadataReference SystemCoreReference
            = MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location);

        private static readonly MetadataReference CSharpSymbolsReference
            = MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location);

        private static readonly MetadataReference CodeAnalysisReference
            = MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);
    }
}
