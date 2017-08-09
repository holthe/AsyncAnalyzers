using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AsyncAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AsyncMethodNameAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticIdForMissingAsyncSuffix = "Missing Async";
        public const string DiagnosticIdForSuperFluousAsyncSuffix = "Superfluous Async";

        private const string Category = "Naming";
        private const string TitleForMissingAsync = "TAP methods must end with Async";
        private const string MessageFormatForMissingAsync = "'{0}' does not end with Async";
        private const string DescriptionForMissingAsync = "TAP methods should have the Async suffix.";

        private const string TitleForSuperfluousAsync = "Only TAP methods must end with Async";
        private const string MessageFormatForSuperfluousAsync = "'{0}' is not a TAP method but ends with Async";
        private const string DescriptionForSuperfluousAsync = "Only TAP methods should have the Async suffix.";

        private static readonly DiagnosticDescriptor RuleForMissingAsyncSuffix = new DiagnosticDescriptor(DiagnosticIdForMissingAsyncSuffix, TitleForMissingAsync, MessageFormatForMissingAsync, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: DescriptionForMissingAsync);
        private static readonly DiagnosticDescriptor RuleForSuperfluousAsyncSuffix = new DiagnosticDescriptor(DiagnosticIdForSuperFluousAsyncSuffix, TitleForSuperfluousAsync, MessageFormatForSuperfluousAsync, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: DescriptionForSuperfluousAsync);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(RuleForMissingAsyncSuffix, RuleForSuperfluousAsyncSuffix);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(AsyncMethodValidator, SymbolKind.Method);
        }

        private static void AsyncMethodValidator(SymbolAnalysisContext context)
        {
            var methodSymbol = context.Symbol as IMethodSymbol;
            if (methodSymbol == null)
            {
                return;
            }

            var asyncResultInterface = context.Compilation.GetTypeByMetadataName(typeof(IAsyncResult).FullName);
            var returnsAsyncResultImplementation = methodSymbol.ReturnType.Interfaces.Contains(asyncResultInterface);

            // Methods marked async or returning IAsyncResult must have Async suffix
            if ((methodSymbol.IsAsync || returnsAsyncResultImplementation) && !methodSymbol.Name.EndsWith("Async"))
            {
                foreach (var location in methodSymbol.Locations)
                {
                    context.ReportDiagnostic(Diagnostic.Create(RuleForMissingAsyncSuffix, location, methodSymbol.Name));
                }
            }

            // Methods that are neither marked async nor returning IAsyncResult must not have Async suffix
            if (!methodSymbol.IsAsync && !returnsAsyncResultImplementation && methodSymbol.Name.EndsWith("Async"))
            {
                foreach (var location in methodSymbol.Locations)
                {
                    context.ReportDiagnostic(Diagnostic.Create(RuleForSuperfluousAsyncSuffix, location, methodSymbol.Name));
                }
            }
        }
    }
}
