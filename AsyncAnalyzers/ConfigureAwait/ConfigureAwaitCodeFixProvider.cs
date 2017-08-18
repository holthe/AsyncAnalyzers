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
using Microsoft.CodeAnalysis.Rename;

namespace AsyncAnalyzers.ConfigureAwait
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConfigureAwaitCodeFixProvider))]
    [Shared]
    public class ConfigureAwaitCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Apply .ConfigureAwait(false).";

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(AsyncMethodConfigureAwaitAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
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
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var expression = AsyncMethodConfigureAwaitAnalyzer.FindExpressionForConfigureAwait(node);
            if (expression != null)
            {
                var isConfigureAwait = (expression?.Expression as MemberAccessExpressionSyntax)?.Name.Identifier.Text.Equals("ConfigureAwait", StringComparison.Ordinal);

                if (isConfigureAwait != true)
                {
                    var falseExpression = SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression);
                    var newExpression = SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, SyntaxFactory.IdentifierName("ConfigureAwait")),
                        SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[] { SyntaxFactory.Argument(falseExpression) })));
                    return document.WithSyntaxRoot(root.ReplaceNode(expression, newExpression.WithAdditionalAnnotations(Formatter.Annotation)));
                }

                if (expression.ArgumentList.Arguments.FirstOrDefault()?.Expression?.IsKind(SyntaxKind.FalseLiteralExpression) != true)
                {
                    var falseExpression = SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression);
                    var newExpression = SyntaxFactory.InvocationExpression(expression.Expression,
                        SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[] { SyntaxFactory.Argument(falseExpression) })));
                    return document.WithSyntaxRoot(root.ReplaceNode(expression, newExpression.WithAdditionalAnnotations(Formatter.Annotation)));
                }
            }
            else
            {
                var falseExpression = SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression);
                var newExpression = SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, node.Expression, SyntaxFactory.IdentifierName("ConfigureAwait")),
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[] { SyntaxFactory.Argument(falseExpression) })));
                return document.WithSyntaxRoot(root.ReplaceNode(node.Expression, newExpression.WithAdditionalAnnotations(Formatter.Annotation)));
            }

            throw new InvalidOperationException();
        }
    }
}