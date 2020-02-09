# CancellationTokenAnalyzers

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Continuous Deployment](https://github.com/JakenVeina/CancellationTokenAnalyzers/workflows/Continuous%20Deployment/badge.svg)](https://github.com/JakenVeina/CancellationTokenAnalyzers/actions?query=workflow%3A%22Continuous+Deployment%22)
[![NuGet](https://img.shields.io/nuget/v/CancellationTokenAnalyzers.svg)](https://www.nuget.org/packages/CancellationTokenAnalyzers/)

A Roslyn Analyzer to support proper usage of CancellationToken values, within `async` or `Task`-based workloads.

# Analysis Rules
ID | Title | Severity | CodeFix |
---- | --- | --- | --- |
[CTU0001](CTU0001.md) | CancellationToken available, but not supplied | Warning | Yes |
