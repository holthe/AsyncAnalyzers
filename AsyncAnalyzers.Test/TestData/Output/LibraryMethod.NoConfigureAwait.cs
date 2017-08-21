using System;
using System.Threading.Tasks;

namespace AsyncAnalyzers.Test.TestData
{
    internal partial class Test
    {
        public async Task XAsync()
        {
            var library = new Library();
            return await library.Api.LibraryMethodAsync().ConfigureAwait(false);
        }
    }
}