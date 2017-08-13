using System;
using System.Threading.Tasks;

namespace AsyncAnalyzers.Test.TestData
{
    internal partial class Test
    {
        public Task<int> X()
        {
            return Task.FromResult(0);
        }
    }
}