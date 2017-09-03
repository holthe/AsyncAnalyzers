using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AsyncAnalyzers.SuggestCancellationToken
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DefaultCancellationTokenAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "_DefaultCancellationToken";
        public const string MessageForDefaultCancellationToken = "Consider using non-default cancellation token for '{0}'.";

        private const string Category = "CancellationToken";
        private const string Title = "Consider using non-default cancellation token.";
        private const string Description = "";

        private static readonly DiagnosticDescriptor RuleForDefaultCancellationToken = new DiagnosticDescriptor(DiagnosticId, Title, MessageForDefaultCancellationToken, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        private static readonly List<string> DefaultCancellationTokenNames = new List<string> { "CancellationToken.None", "default(CancellationToken)" };

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(RuleForDefaultCancellationToken);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(CancellationTokenArgumentValidator, SyntaxKind.AwaitExpression);
        }

        private static void CancellationTokenArgumentValidator(SyntaxNodeAnalysisContext context)
        {
            var awaitNode = (AwaitExpressionSyntax)context.Node;
            var invocationExpression = awaitNode.GetInvocationExpressionSyntax();

            var semanticModel = context.SemanticModel;
            var cancellationTokenArguments = invocationExpression.ArgumentsOfType<CancellationToken>(semanticModel);
            foreach (var argument in cancellationTokenArguments)
            {
                if (DefaultCancellationTokenNames.Select(checkValue => checkValue.ToString()).Contains(argument.Expression.ToString()))
                {
                    var diagnostic = Diagnostic.Create(RuleForDefaultCancellationToken, awaitNode.GetLocation(), (invocationExpression?.Expression as MemberAccessExpressionSyntax)?.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
