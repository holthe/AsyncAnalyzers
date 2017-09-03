using System.IO;
using System.Threading.Tasks;
using AsyncAnalyzers.Naming;
using AsyncAnalyzers.Test.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace AsyncAnalyzers.Test
{
    public class AsyncMethodNameAnalyzerTests : TestBase
    {
        private DiagnosticResult _expectedDiagnosticResultForMissingAsync;
        private DiagnosticResult _expectedDiagnosticResultForSuperfluousAsync;

        public AsyncMethodNameAnalyzerTests()
        {
            _expectedDiagnosticResultForMissingAsync = new DiagnosticResult
            {
                Id = AsyncMethodNameAnalyzer.DiagnosticIdForMissingAsyncSuffix,
                Message = string.Format(AsyncMethodNameAnalyzer.MessageFormatForMissingAsync, "X"),
                Severity = DiagnosticSeverity.Warning
            };

            _expectedDiagnosticResultForSuperfluousAsync = new DiagnosticResult
            {
                Id = AsyncMethodNameAnalyzer.DiagnosticIdForSuperfluousAsyncSuffix,
                Message = string.Format(AsyncMethodNameAnalyzer.MessageFormatForSuperfluousAsync, "XAsync"),
                Severity = DiagnosticSeverity.Warning
            };
        }

        [Theory]
        [InlineData("AsyncTask.NoAsyncSuffix.Fact.cs")]
        [InlineData("AsyncTask.NoAsyncSuffix.Test.cs")]
        [InlineData("AsyncTask.NoAsyncSuffix.TestMethod.cs")]
        [InlineData("AsyncTask.NoAsyncSuffix.Theory.cs")]
        public async Task AsyncMethod_NoAsyncSuffix_TestAttribute_NoDiagnosticFound(string testFile)
        {
            var test = File.ReadAllText(Path.Combine(TestDataInputDir, testFile));

            await VerifyCSharpDiagnosticAsync(test);
        }

        [Fact]
        public async Task MethodNotReturningIAsyncResult_AsyncSuffix_DiagnosticFound_CanFix()
        {
            const string testFile = "NoIAsyncResult.AsyncSuffix.cs";
            var test = File.ReadAllText(Path.Combine(TestDataInputDir, testFile));
            _expectedDiagnosticResultForSuperfluousAsync.Locations =
                new[]
                {
                    new DiagnosticResultLocation(DiagnosticLocationPath, 5, 20)
                };

            await VerifyCSharpDiagnosticAsync(test, _expectedDiagnosticResultForSuperfluousAsync);

            var fixtest = File.ReadAllText(Path.Combine(TestDataOutputDir, testFile));
            await VerifyCSharpFixAsync(test, fixtest);
        }

        [Theory]
        [InlineData("AsyncTask.NoAsyncSuffix.cs", 8, 27)]
        [InlineData("AsyncVoid.NoAsyncSuffix.cs", 5, 27)]
        [InlineData("GenericTask.NoAsyncSuffix.cs", 8, 26)]
        [InlineData("Task.NoAsyncSuffix.cs", 8, 21)]
        public async Task VerifyMissingAsyncDiagnosticAndFix(string testFile, int diagnosticLine, int diagnosticColumn)
        {
            var test = File.ReadAllText(Path.Combine(TestDataInputDir, testFile));

            _expectedDiagnosticResultForMissingAsync.Locations =
                new[]
                {
                    new DiagnosticResultLocation(DiagnosticLocationPath, diagnosticLine, diagnosticColumn)
                };

            await VerifyCSharpDiagnosticAsync(test, _expectedDiagnosticResultForMissingAsync);

            var fixtest = File.ReadAllText(Path.Combine(TestDataOutputDir, testFile));
            await VerifyCSharpFixAsync(test, fixtest);
        }

        [Theory]
        [InlineData("NoIAsyncResult.AsyncSuffix.cs", 5, 20)]
        public async Task VerifySuperfluousAsyncDiagnosticAndFix(string testFile, int diagnosticLine, int diagnosticColumn)
        {
            var test = File.ReadAllText(Path.Combine(TestDataInputDir, testFile));

            _expectedDiagnosticResultForSuperfluousAsync.Locations =
                new[]
                {
                    new DiagnosticResultLocation(DiagnosticLocationPath, diagnosticLine, diagnosticColumn)
                };

            await VerifyCSharpDiagnosticAsync(test, _expectedDiagnosticResultForSuperfluousAsync);

            var fixtest = File.ReadAllText(Path.Combine(TestDataOutputDir, testFile));
            await VerifyCSharpFixAsync(test, fixtest);
        }

        [Theory]
        [InlineData("AsyncTask.AsyncSuffix.cs")]
        [InlineData("AsyncVoid.AsyncSuffix.cs")]
        [InlineData("DelegateVoid.NoAsyncSuffix.cs")]
        [InlineData("GenericramaTask.AsyncSuffix.cs")]
        [InlineData("Task.AsyncSuffix.cs")]
        public async Task VerifyNoDiagnostic(string testFile)
        {
            var test = File.ReadAllText(Path.Combine(TestDataInputDir, testFile));

            await VerifyCSharpDiagnosticAsync(test);
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