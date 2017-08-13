using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Rename;

namespace AsyncAnalyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AsyncSuffixCodeFixProvider))]
    [Shared]
    public class AsyncSuffixCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Append missing or remove superfluous Async suffix.";

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(AsyncMethodNameAnalyzer.DiagnosticIdForMissingAsyncSuffix, AsyncMethodNameAnalyzer.DiagnosticIdForSuperfluousAsyncSuffix);

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
            var syntaxToken = root.FindToken(diagnosticSpan.Start);

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedSolution: c => diagnostic.Id == AsyncMethodNameAnalyzer.DiagnosticIdForMissingAsyncSuffix
                        ? AppendAsyncSuffixAsync(context.Document, syntaxToken, c)
                        : RemoveAsyncSuffixAsync(context.Document, syntaxToken, c),
                    equivalenceKey: Title),
                diagnostic);
        }

        private static async Task<Solution> AppendAsyncSuffixAsync(Document document, SyntaxToken syntaxToken, CancellationToken cancellationToken)
        {
            // Compute new uppercase name.
            var newName = $"{syntaxToken.Text}Async";

            // Get the symbol representing the type to be renamed.
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var typeSymbol = semanticModel.GetDeclaredSymbol(syntaxToken.Parent, cancellationToken);

            // Produce a new solution that has all references to that type renamed, including the declaration.
            var originalSolution = document.Project.Solution;
            var optionSet = originalSolution.Workspace.Options;
            var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

            // Return the new solution with the now-uppercase type name.
            return newSolution;
        }

        private static async Task<Solution> RemoveAsyncSuffixAsync(Document document, SyntaxToken syntaxToken, CancellationToken cancellationToken)
        {
            // Compute new uppercase name.
            var newName = syntaxToken.Text.Substring(0, syntaxToken.Text.Length - 5);

            // Get the symbol representing the type to be renamed.
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var typeSymbol = semanticModel.GetDeclaredSymbol(syntaxToken.Parent, cancellationToken);

            // Produce a new solution that has all references to that type renamed, including the declaration.
            var originalSolution = document.Project.Solution;
            var optionSet = originalSolution.Workspace.Options;
            var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

            // Return the new solution with the now-uppercase type name.
            return newSolution;
        }
    }
}