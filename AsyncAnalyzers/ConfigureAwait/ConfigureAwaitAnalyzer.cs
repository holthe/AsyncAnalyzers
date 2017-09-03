using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AsyncAnalyzers.ConfigureAwait
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConfigureAwaitAnalyzer : DiagnosticAnalyzer
    {
        public const string ConfigureAwaitIdentifier = "ConfigureAwait";

        public const string DiagnosticId = "_MissingConfigureAwait";
        public const string MessageForMissingConfigureAwait = "Consider using .ConfigureAwait(false) on async method '{0}'.";

        private const string Category = "ConfigureAwait";
        private const string TitleForMissingConfigureAwait = "Consider using .ConfigureAwait(false).";
        private const string DescriptionForMissingConfigureAwait = "Async library methods must use .ConfigureAwait(false).";

        private static readonly DiagnosticDescriptor RuleForMissingConfigureAwait = new DiagnosticDescriptor(DiagnosticId, TitleForMissingConfigureAwait, MessageForMissingConfigureAwait, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: DescriptionForMissingConfigureAwait);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(RuleForMissingConfigureAwait);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(ConfigureAwaitMethodValidator, SyntaxKind.AwaitExpression);
        }

        private static void ConfigureAwaitMethodValidator(SyntaxNodeAnalysisContext context)
        {
            var awaitNode = (AwaitExpressionSyntax)context.Node;
            var possibleConfigureAwait = awaitNode.GetInvocationExpressionSyntax();

            var memberAccess = possibleConfigureAwait?.Expression as MemberAccessExpressionSyntax;
            if (memberAccess == null)
            {
                return;
            }

            var hasConfigureAwaitIdentifier = memberAccess.HasIdentifier(ConfigureAwaitIdentifier);
            if (hasConfigureAwaitIdentifier && possibleConfigureAwait.IsFirstArgumentFalse())
            {
                return;
            }

            var name = hasConfigureAwaitIdentifier
                ? ((memberAccess.Expression as InvocationExpressionSyntax)?.Expression as MemberAccessExpressionSyntax)?.Name
                : memberAccess.Name;

            var diagnostic = Diagnostic.Create(RuleForMissingConfigureAwait, awaitNode.GetLocation(), name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
