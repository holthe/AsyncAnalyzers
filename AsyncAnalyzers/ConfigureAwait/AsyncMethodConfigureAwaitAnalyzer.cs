using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AsyncAnalyzers.ConfigureAwait
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AsyncMethodConfigureAwaitAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "_MissingConfigureAwait";
        public const string MessagForMissingConfigureAwait = "Consider using .ConfigureAwait(false) on async method '{0}'";

        private const string Category = "ConfigureAwait";
        private const string TitleForMissingAsync = "Consider using .ConfigureAwait(false)";
        private const string DescriptionForMissingAsync = "Async library methods must use .ConfigureAwait(false)";

        private static readonly DiagnosticDescriptor RuleForMissingConfigureAwait = new DiagnosticDescriptor(DiagnosticId, TitleForMissingAsync, MessagForMissingConfigureAwait, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: DescriptionForMissingAsync);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(RuleForMissingConfigureAwait);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(ConfigureAwaitMethodValidator, SyntaxKind.AwaitExpression);
        }

        private static void ConfigureAwaitMethodValidator(SyntaxNodeAnalysisContext context)
        {
            var awaitNode = (AwaitExpressionSyntax)context.Node;
            var possibleConfigureAwait = FindExpressionForConfigureAwait(awaitNode);

            var memberAccess = possibleConfigureAwait?.Expression as MemberAccessExpressionSyntax;
            if (memberAccess == null)
            {
                return;
            }

            var hasConfigureAwaitIdentifier = memberAccess.Name.Identifier.Text.Equals("ConfigureAwait", StringComparison.Ordinal);
            var hasFalseArgument = possibleConfigureAwait.ArgumentList.Arguments.FirstOrDefault()?.Expression?.IsKind(SyntaxKind.FalseLiteralExpression) == true;
            if (!hasConfigureAwaitIdentifier || !hasFalseArgument)
            {
                NameSyntax name;
                if (hasConfigureAwaitIdentifier)
                {
                    var ex = (memberAccess.Expression as InvocationExpressionSyntax)?.Expression as MemberAccessExpressionSyntax;
                    name = ex?.Name;
                }
                else
                {
                    name = memberAccess.Name;
                }

                var diagnostic = Diagnostic.Create(RuleForMissingConfigureAwait, awaitNode.GetLocation(), name);
                context.ReportDiagnostic(diagnostic);
            }
        }

        public static InvocationExpressionSyntax FindExpressionForConfigureAwait(SyntaxNode node)
        {
            return node.ChildNodes()
                .Select(item => new { item, invocation = item as InvocationExpressionSyntax })
                .Select(t => t.invocation ?? FindExpressionForConfigureAwait(t.item))
                .FirstOrDefault();
        }
    }
}
