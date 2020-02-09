﻿using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;

using NUnit.Framework;

using CancellationTokenAnalyzers.Diagnostics;

namespace CancellationTokenAnalyzers.Test.IntegrationTests.CancellationTokenNotSuppliedWithUnorderedArgumentsList
{
    [TestFixture]
    public sealed class TestFixture
        : CancellationTokenUsageIntegrationTestFixtureBase
    {
        [Test]
        public Task DiagnosticsAndCodeFixes_AreExpected()
            => PerformTestsAsync();

        protected sealed override ImmutableArray<DiagnosticOutcome> ExpectedDiagnostics { get; }
            = ImmutableArray.Create(
                new DiagnosticOutcome(  CancellationTokenAvailableButNotSuppliedDiagnostic.Descriptor.Id,   "FakeFolder/FakeDocument1.cs",  31, 19));

        protected override async Task<ImmutableArray<DocumentOutcome>> BuildExpectedDocumentsAsync()
            => ImmutableArray.Create(
                new DocumentOutcome(
                    name:   "FakeDocument1",
                    text:   await GetType().GetNamespaceResourceTextAsync("Document1.fixed.fake.cs")));

        protected override async Task<Workspace> BuildWorkspaceAsync()
            => new AdhocWorkspace()
                .WithFakeDocument(
                    name:       "FakeDocument1",
                    filePath:   "FakeFolder/FakeDocument1.cs",
                    text:       await GetType().GetNamespaceResourceTextAsync("Document1.fake.cs"));
    }
}
