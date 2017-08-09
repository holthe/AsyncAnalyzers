using AsyncAnalyzers.Test.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace AsyncAnalyzers.Test
{
    public class AsyncMethodNameAnalyzerTests : Verifiers.CodeFixVerifier
    {
        private DiagnosticResult _expectedDiagnosticResultForMissingAsync;
        private DiagnosticResult _expectedDiagnosticResultForSuperfluousAsync;

        public AsyncMethodNameAnalyzerTests()
        {
            _expectedDiagnosticResultForMissingAsync = new DiagnosticResult
            {
                Id = AsyncMethodNameAnalyzer.DiagnosticIdForMissingAsyncSuffix,
                Message = "\'X\' does not end with Async",
                Severity = DiagnosticSeverity.Error
            };

            _expectedDiagnosticResultForSuperfluousAsync = new DiagnosticResult
            {
                Id = AsyncMethodNameAnalyzer.DiagnosticIdForSuperFluousAsyncSuffix,
                Message = "\'XAsync\' is not a TAP method but ends with Async",
                Severity = DiagnosticSeverity.Error
            };
        }

        [Fact]
        public void NoDiagnosticFound()
        {
            const string test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void MethodNotReturningIAsyncResult_AsyncSuffix_DiagnosticFound_CanFix()
        {
            const string test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public int XAsync() { }
        }
    }";
            _expectedDiagnosticResultForSuperfluousAsync.Locations =
                new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 13, 24)
                };

            VerifyCSharpDiagnostic(test, _expectedDiagnosticResultForSuperfluousAsync);

            const string fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public int X() { }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void MethodReturningVoidWithAsyncModifier_AsyncSuffix_NoDiagnosticFound()
        {
            const string test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public async void XAsync() { }
        }
    }";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void MethodReturningVoidWithAsyncModifier_NoAsyncSuffix_DiagnosticFound_CanFix()
        {
            const string test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public async void X() { }
        }
    }";

            _expectedDiagnosticResultForMissingAsync.Locations =
                new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 13, 31)
                };

            VerifyCSharpDiagnostic(test, _expectedDiagnosticResultForMissingAsync);

            const string fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public async void XAsync() { }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void MethodReturningGenericTaskWithAsyncModifier_NoAsyncSuffix_DiagnosticFound_CanFix()
        {
            const string test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public async Task<T> X() { }
        }
    }";
            _expectedDiagnosticResultForMissingAsync.Locations =
                new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 13, 34)
                };

            VerifyCSharpDiagnostic(test, _expectedDiagnosticResultForMissingAsync);

            const string fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public async Task<T> XAsync() { }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void MethodReturningTaskWithAsyncModifier_NoAsyncSuffix_DiagnosticFound_CanFix()
        {
            const string test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public async Task X() { }
        }
    }";
            _expectedDiagnosticResultForMissingAsync.Locations =
                new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 13, 31)
                };

            VerifyCSharpDiagnostic(test, _expectedDiagnosticResultForMissingAsync);

            const string fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public async Task XAsync() { }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void MethodReturningTaskWithAsyncModifier_AsyncSuffix_NoDiagnosticFound_CanFix()
        {
            const string test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public async Task XAsync() { }
        }
    }";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void MethodReturningTaskWithoutAsyncModifier_NoAsyncSuffix_DiagnosticFound_CanFix()
        {
            const string test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public Task X() { }
        }
    }";
            _expectedDiagnosticResultForMissingAsync.Locations =
                new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 13, 25)
                };

            VerifyCSharpDiagnostic(test, _expectedDiagnosticResultForMissingAsync);

            const string fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public Task XAsync() { }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void MethodReturningTaskWithoutAsyncModifier_AsyncSuffix_NoDiagnosticFound()
        {
            const string test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public Task XAsync() { }
        }
    }";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void DelegateMethodReturningVoid_NoAsyncSuffix_NoDiagnostic()
        {
            const string test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public delegate void MyDelegate(object sender, EventArgs args);
        }
    }";

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