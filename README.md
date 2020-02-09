# CancellationTokenAnalyzers
A Roslyn Analyzer to support proper usage of CancellationToken values, within `async` or `Task`-based workloads.

# Analysis Rules
ID | Title | Severity | CodeFix |
---- | --- | --- | --- |
[CTU0001](CTU0001.md) | CancellationToken available, but not supplied | Warning | Yes |
