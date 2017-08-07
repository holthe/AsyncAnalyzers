using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AsyncAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AsyncMethodNameAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticIdForMissingAsync = "Missing Async";
        public const string DiagnosticIdForSuperFluousAsync = "Superfluous Async";

        private const string Category = "Naming";
        private static readonly string _titleForMissingAsync = "TAP methods must end with Async";
        private static readonly string _messageFormatForMissingAsync = "'{0}' does not end with Async";
        private static readonly string _descriptionForMissingAsync = "TAP methods should have the Async suffix.";

        private static readonly string _titleForSuperfluousAsync = "Only TAP methods must end with Async";
        private static readonly string _messageFormatForSuperfluousAsync = "'{0}' is not a TAP method but ends with Async";
        private static readonly string _descriptionForSuperfluousAsync = "Only TAP methods should have the Async suffix.";

        private static readonly DiagnosticDescriptor _ruleForMissingAsync = new DiagnosticDescriptor(DiagnosticIdForMissingAsync, _titleForMissingAsync, _messageFormatForMissingAsync, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: _descriptionForMissingAsync);
        private static readonly DiagnosticDescriptor _ruleForSuperfluousAsync = new DiagnosticDescriptor(DiagnosticIdForSuperFluousAsync, _titleForSuperfluousAsync, _messageFormatForSuperfluousAsync, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: _descriptionForSuperfluousAsync);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_ruleForMissingAsync, _ruleForSuperfluousAsync);

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
                    context.ReportDiagnostic(Diagnostic.Create(_ruleForMissingAsync, location, methodSymbol.Name));
                }
            }

            // Methods that are neither marked async nor returning IAsyncResult must not have Async suffix
            if (!methodSymbol.IsAsync && !returnsAsyncResultImplementation && methodSymbol.Name.EndsWith("Async"))
            {
                foreach (var location in methodSymbol.Locations)
                {
                    context.ReportDiagnostic(Diagnostic.Create(_ruleForSuperfluousAsync, location, methodSymbol.Name));
                }
            }
        }
    }
}
