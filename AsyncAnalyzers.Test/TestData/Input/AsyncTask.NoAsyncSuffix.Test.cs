using System;
using System.Threading.Tasks;

namespace AsyncAnalyzers.Test.TestData
{
    internal partial class Test
    {
        [Test]
        public async Task X()
        {
            return Task.FromResult(0);
        }
    }
}