using System;
using System.Threading.Tasks;

namespace AsyncAnalyzers.Test.TestData
{
    internal partial class Test
    {
        public Task X()
        {
            return Task.FromResult(0);
        }
    }
}