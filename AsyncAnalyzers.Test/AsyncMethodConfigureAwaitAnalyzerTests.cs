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
                Severity = DiagnosticSeverity.Error
            };
        }

        [Theory]
        [InlineData("LibraryMethod.NoConfigureAwait.cs")]
        [InlineData("LibraryMethod.ConfigureAwaitTrue.cs")]
        public void LibraryMethod_NoConfigureAwaitFalse_DiagnosticFound_CanFix(string testFile)
        {
            var test = File.ReadAllText(Path.Combine(TestDataInputDir, testFile));
            _expectedDiagnosticResultForMissingConfigureAwait.Locations =
                new[]
                {
                    new DiagnosticResultLocation(DiagnosticLocationPath, 10, 20)
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