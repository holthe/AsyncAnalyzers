using System;
using System.Collections.Generic;
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

        public static IEnumerable<ArgumentSyntax> ArgumentsOfType<T>(this InvocationExpressionSyntax invocationExpression, SemanticModel semanticModel)
        {
            var checkType = semanticModel.Compilation.GetTypeByMetadataName(typeof(T).FullName);
            return invocationExpression.ArgumentList.Arguments.Where(argument => semanticModel.GetTypeInfo(argument?.Expression).Type?.OriginalDefinition.Equals(checkType) == true);
        }

        public static bool HasArgument<T>(this InvocationExpressionSyntax invocationExpression, SemanticModel semanticModel, params T[] possibilities)
        {
            return invocationExpression.ArgumentsOfType<T>(semanticModel).Any(argument => possibilities.Any(possibility => argument.Equals(possibility)));
        }

        /// <summary>
        /// Gets the <c>InvocationExpressionSyntax</c> from the given <see cref="SyntaxNode"/>.
        /// </summary>
        public static InvocationExpressionSyntax GetInvocationExpressionSyntax(this SyntaxNode node)
        {
            return node.ChildNodes()
                .Select(item => new { item, invocation = item as InvocationExpressionSyntax })
                .Select(t => t.invocation ?? GetInvocationExpressionSyntax(t.item))
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
