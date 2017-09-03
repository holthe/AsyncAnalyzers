using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace AsyncAnalyzers.ConfigureAwait
{
    [Shared]
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConfigureAwaitCodeFixProvider))]
    public sealed class ConfigureAwaitCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Apply .ConfigureAwait(false).";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ConfigureAwaitAnalyzer.DiagnosticId);

        public override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var syntaxToken = root.FindNode(diagnosticSpan) as AwaitExpressionSyntax;

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c => AppendConfigureAwaitFalseAsync(context.Document, syntaxToken, c),
                    equivalenceKey: Title),
                diagnostic);
        }

        private static async Task<Document> AppendConfigureAwaitFalseAsync(Document document, AwaitExpressionSyntax node, CancellationToken cancellationToken)
        {
            LiteralExpressionSyntax falseExpression;
            InvocationExpressionSyntax fixedInvocationExpression;

            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var expression = node.GetInvocationExpressionSyntax();
            if (expression != null)
            {
                var memberAccess = expression.Expression as MemberAccessExpressionSyntax;
                if (memberAccess == null || !memberAccess.HasIdentifier(ConfigureAwaitAnalyzer.ConfigureAwaitIdentifier))
                {
                    falseExpression = SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression);
                    fixedInvocationExpression = SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, SyntaxFactory.IdentifierName(ConfigureAwaitAnalyzer.ConfigureAwaitIdentifier)),
                        SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[] { SyntaxFactory.Argument(falseExpression) })));
                    return document.WithSyntaxRoot(root.ReplaceNode(expression, fixedInvocationExpression.WithAdditionalAnnotations(Formatter.Annotation)));
                }

                if (expression.IsFirstArgumentFalse())
                {
                    throw new InvalidOperationException();
                }

                falseExpression = SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression);
                fixedInvocationExpression = SyntaxFactory.InvocationExpression(
                    expression.Expression,
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[] { SyntaxFactory.Argument(falseExpression) })));
                return document.WithSyntaxRoot(root.ReplaceNode(expression, fixedInvocationExpression.WithAdditionalAnnotations(Formatter.Annotation)));
            }

            falseExpression = SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression);
            fixedInvocationExpression = SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, node.Expression, SyntaxFactory.IdentifierName(ConfigureAwaitAnalyzer.ConfigureAwaitIdentifier)),
                SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[] { SyntaxFactory.Argument(falseExpression) })));
            return document.WithSyntaxRoot(root.ReplaceNode(node.Expression, fixedInvocationExpression.WithAdditionalAnnotations(Formatter.Annotation)));
        }
    }
}