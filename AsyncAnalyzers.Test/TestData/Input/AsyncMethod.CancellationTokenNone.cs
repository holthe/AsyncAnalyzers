using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAnalyzers.Test.TestData
{
    internal partial class Test
    {
        public async Task XAsync()
        {
            return await Task.Run(() => {}, CancellationToken.None);
        }
    }
}