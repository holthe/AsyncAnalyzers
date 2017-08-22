using System.IO;
using AsyncAnalyzers.ConfigureAwait;
using AsyncAnalyzers.Test.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace AsyncAnalyzers.Test
{
    public class AsyncMethodConfigureAwaitAnalyzerTests : TestBase
    {
        private DiagnosticResult _expectedDiagnosticResultForMissingConfigureAwait;

        public AsyncMethodConfigureAwaitAnalyzerTests()
        {
            _expectedDiagnosticResultForMissingConfigureAwait = new DiagnosticResult
            {
                Id = ConfigureAwaitAnalyzer.DiagnosticId,
                Message = string.Format(ConfigureAwaitAnalyzer.MessagForMissingConfigureAwait, "LibraryMethodAsync"),
                Severity = DiagnosticSeverity.Warning
            };
        }

        [Theory]
        [InlineData("LibraryMethod.NoConfigureAwait.cs", 11, 20)]
        [InlineData("LibraryMethod.ConfigureAwaitTrue.cs", 10, 20)]
        public void LibraryMethod_NoConfigureAwaitFalse_DiagnosticFound_CanFix(string testFile, int diagnosticLine, int diagnosticColumn)
        {
            var test = File.ReadAllText(Path.Combine(TestDataInputDir, testFile));
            _expectedDiagnosticResultForMissingConfigureAwait.Locations =
                new[]
                {
                    new DiagnosticResultLocation(DiagnosticLocationPath, diagnosticLine, diagnosticColumn)
                };

            VerifyCSharpDiagnostic(test, _expectedDiagnosticResultForMissingConfigureAwait);

            var fixtest = File.ReadAllText(Path.Combine(TestDataOutputDir, testFile));
            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void LibraryMethod_ConfigureAwaitFalse_NoDiagnosticFound()
        {
            const string testFile = "LibraryMethod.ConfigureAwaitFalse.cs";
            var test = File.ReadAllText(Path.Combine(TestDataInputDir, testFile));

            VerifyCSharpDiagnostic(test);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new ConfigureAwaitCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ConfigureAwaitAnalyzer();
        }
    }
}