using System.IO;
using System.Threading.Tasks;
using AsyncAnalyzers.SuggestCancellationToken;
using AsyncAnalyzers.Test.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace AsyncAnalyzers.Test
{
    public class AsyncMethodCancellationTokenTests : TestBase
    {
        private DiagnosticResult _expectedDiagnosticResultForDefaultCancellationToken;

        public AsyncMethodCancellationTokenTests()
        {
            _expectedDiagnosticResultForDefaultCancellationToken = new DiagnosticResult
            {
                Id = DefaultCancellationTokenAnalyzer.DiagnosticId,
                Message = string.Format(DefaultCancellationTokenAnalyzer.MessageForDefaultCancellationToken, "Run"),
                Severity = DiagnosticSeverity.Warning
            };
        }

        [Theory]
        [InlineData("AsyncMethod.DefaultCancellationToken.cs")]
        [InlineData("AsyncMethod.CancellationTokenNone.cs")]
        public async Task AsyncMethod_DefaultCancellationToken_VerifyDiagnostic(string testFile)
        {
            var test = File.ReadAllText(Path.Combine(TestDataInputDir, testFile));

            _expectedDiagnosticResultForDefaultCancellationToken.Locations =
                new[]
                {
                    new DiagnosticResultLocation(DiagnosticLocationPath, 11, 20)
                };

            await VerifyCSharpDiagnosticAsync(test, _expectedDiagnosticResultForDefaultCancellationToken);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return null;
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DefaultCancellationTokenAnalyzer();
        }
    }
}