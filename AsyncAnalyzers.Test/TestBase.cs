using System.IO;

namespace AsyncAnalyzers.Test
{
    public class TestBase : Verifiers.CodeFixVerifier
    {
        protected const int SingleFileCount = 0;
        protected static readonly string DiagnosticLocationPath = $"{DefaultFilePathPrefix}{SingleFileCount}.{CSharpDefaultFileExt}";

        protected static readonly string TestDataInputDir = Path.Combine("TestData", "Input");
        protected static readonly string TestDataOutputDir = Path.Combine("TestData", "Output");
    }
}
