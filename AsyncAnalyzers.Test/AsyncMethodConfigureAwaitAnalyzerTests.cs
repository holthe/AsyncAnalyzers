using System.IO;
using System.Threading.Tasks;
using AsyncAnalyzers.ConfigureAwait;
using AsyncAnalyzers.Test.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace AsyncAnalyzers.Test
{
    public class AsyncMethodConfigureAwaitAnalyzerTests : Verifiers.CodeFixVerifier
    {
        private const int SingleFileCount = 0;
        private static readonly string DiagnosticLocationPath = $"{DefaultFilePathPrefix}{SingleFileCount}.{CSharpDefaultFileExt}";

        private static readonly string TestDataInputDir = Path.Combine("TestData", "Input");
        private static readonly string TestDataOutputDir = Path.Combine("TestData", "Output");

        private DiagnosticResult _expectedDiagnosticResultForMissingConfigureAwait;

        public AsyncMethodConfigureAwaitAnalyzerTests()
        {
            _expectedDiagnosticResultForMissingConfigureAwait = new DiagnosticResult
            {
                Id = AsyncMethodConfigureAwaitAnalyzer.DiagnosticId,
                Message = string.Format(AsyncMethodConfigureAwaitAnalyzer.MessagForMissingConfigureAwait, "LibraryMethodAsync"),
                Severity = DiagnosticSeverity.Error
            };
        }

        [Fact]
        public void LibraryMethod_NoConfigureAwaitFalse_DiagnosticFound_CanFix()
        {
            const string testFile = "LibraryMethod.NoConfigureAwait.cs";
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
        public void LibraryMethod_ConfigureAwaitTrue_DiagnosticFound_CanFix()
        {
            const string testFile = "LibraryMethod.ConfigureAwaitTrue.cs";
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
            return new AsyncMethodConfigureAwaitAnalyzer();
        }
    }
}