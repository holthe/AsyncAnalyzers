using System.IO;
using AsyncAnalyzers.Test.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace AsyncAnalyzers.Test
{
    public class AsyncMethodNameAnalyzerTests : Verifiers.CodeFixVerifier
    {
        private const int SingleFileCount = 0;
        private static readonly string DiagnosticLocationPath = $"{DefaultFilePathPrefix}{SingleFileCount}.{CSharpDefaultFileExt}";

        private static readonly string TestDataInputDir = Path.Combine("TestData", "Input");
        private static readonly string TestDataOutputDir = Path.Combine("TestData", "Output");

        private DiagnosticResult _expectedDiagnosticResultForMissingAsync;
        private DiagnosticResult _expectedDiagnosticResultForSuperfluousAsync;

        public AsyncMethodNameAnalyzerTests()
        {
            _expectedDiagnosticResultForMissingAsync = new DiagnosticResult
            {
                Id = AsyncMethodNameAnalyzer.DiagnosticIdForMissingAsyncSuffix,
                Message = string.Format(AsyncMethodNameAnalyzer.MessageFormatForMissingAsync, "X"),
                Severity = DiagnosticSeverity.Error
            };

            _expectedDiagnosticResultForSuperfluousAsync = new DiagnosticResult
            {
                Id = AsyncMethodNameAnalyzer.DiagnosticIdForSuperfluousAsyncSuffix,
                Message = string.Format(AsyncMethodNameAnalyzer.MessageFormatForSuperfluousAsync, "XAsync"),
                Severity = DiagnosticSeverity.Error
            };
        }

        [Fact]
        public void MethodNotReturningIAsyncResult_AsyncSuffix_DiagnosticFound_CanFix()
        {
            const string testFile = "NoIAsyncResult.AsyncSuffix.cs";
            var test = File.ReadAllText(Path.Combine(TestDataInputDir, testFile));
            _expectedDiagnosticResultForSuperfluousAsync.Locations =
                new[]
                {
                    new DiagnosticResultLocation(DiagnosticLocationPath, 5, 20)
                };

            VerifyCSharpDiagnostic(test, _expectedDiagnosticResultForSuperfluousAsync);

            var fixtest = File.ReadAllText(Path.Combine(TestDataOutputDir, testFile));
            VerifyCSharpFix(test, fixtest);
        }

        [Theory]
        [InlineData("AsyncTask.NoAsyncSuffix.cs", 8, 27)]
        [InlineData("AsyncVoid.NoAsyncSuffix.cs", 5, 27)]
        [InlineData("GenericTask.NoAsyncSuffix.cs", 8, 26)]
        [InlineData("Task.NoAsyncSuffix.cs", 8, 21)]
        public void VerifyMissingAsyncDiagnosticAndFix(string testFile, int diagnosticLine, int diagnosticColumn)
        {
            var test = File.ReadAllText(Path.Combine(TestDataInputDir, testFile));

            _expectedDiagnosticResultForMissingAsync.Locations =
                new[]
                {
                    new DiagnosticResultLocation(DiagnosticLocationPath, diagnosticLine, diagnosticColumn)
                };

            VerifyCSharpDiagnostic(test, _expectedDiagnosticResultForMissingAsync);

            var fixtest = File.ReadAllText(Path.Combine(TestDataOutputDir, testFile));
            VerifyCSharpFix(test, fixtest);
        }

        [Theory]
        [InlineData("NoIAsyncResult.AsyncSuffix.cs", 5, 20)]
        public void VerifySuperfluousAsyncDiagnosticAndFix(string testFile, int diagnosticLine, int diagnosticColumn)
        {
            var test = File.ReadAllText(Path.Combine(TestDataInputDir, testFile));

            _expectedDiagnosticResultForSuperfluousAsync.Locations =
                new[]
                {
                    new DiagnosticResultLocation(DiagnosticLocationPath, diagnosticLine, diagnosticColumn)
                };

            VerifyCSharpDiagnostic(test, _expectedDiagnosticResultForSuperfluousAsync);

            var fixtest = File.ReadAllText(Path.Combine(TestDataOutputDir, testFile));
            VerifyCSharpFix(test, fixtest);
        }

        [Theory]
        [InlineData("AsyncTask.AsyncSuffix.cs")]
        [InlineData("AsyncVoid.AsyncSuffix.cs")]
        [InlineData("DelegateVoid.NoAsyncSuffix.cs")]
        [InlineData("GenericramaTask.AsyncSuffix.cs")]
        [InlineData("Task.AsyncSuffix.cs")]
        public void VerifyNoDiagnostic(string testFile)
        {
            var test = File.ReadAllText(Path.Combine(TestDataInputDir, testFile));

            VerifyCSharpDiagnostic(test);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new AsyncSuffixCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new AsyncMethodNameAnalyzer();
        }
    }
}