using System;
using System.Threading.Tasks;

namespace AsyncAnalyzers.Test.TestData
{
    internal partial class Test
    {
        public Task<int> XAsync()
        {
            return Task.FromResult(0);
        }
    }
}