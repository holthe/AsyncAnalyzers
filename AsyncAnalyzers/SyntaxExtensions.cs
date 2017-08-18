using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AsyncAnalyzers
{
    public static class SyntaxExtensions
    {
        /// <summary>
        /// Gets a value indicating whether or not the first argument of <paramref name="invocationExpression"/> is <c>false</c>.
        /// </summary>
        public static bool IsFirstArgumentFalse(this InvocationExpressionSyntax invocationExpression)
        {
            return invocationExpression.ArgumentList.Arguments.FirstOrDefault()?.Expression?.IsKind(SyntaxKind.FalseLiteralExpression) == true;
        }

        /// <summary>
        /// Gets the <c>ConfigureAwait</c> expression from the given <see cref="SyntaxNode"/>.
        /// </summary>
        public static InvocationExpressionSyntax GetConfigureAwaitExpression(this SyntaxNode node)
        {
            return node.ChildNodes()
                .Select(item => new { item, invocation = item as InvocationExpressionSyntax })
                .Select(t => t.invocation ?? GetConfigureAwaitExpression(t.item))
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets a value indicating whether or not <paramref name="expression"/> has <see cref="SimpleNameSyntax.Identifier"/> called <paramref name="identifier"/>.
        /// </summary>
        public static bool HasIdentifier(this MemberAccessExpressionSyntax expression, string identifier)
        {
            return expression.Name.Identifier.Text.Equals(identifier, StringComparison.Ordinal);
        }
    }
}
