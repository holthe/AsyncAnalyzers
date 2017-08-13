using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AsyncAnalyzers.Test.TestData
{
    internal partial class Test
    {
        public Task<List<int>> XAsync()
        {
            return Task.FromResult(new List<int>());
        }
    }
}