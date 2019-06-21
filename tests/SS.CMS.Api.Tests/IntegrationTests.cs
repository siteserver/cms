using System;
using SS.CMS.Core.Common;
using SS.CMS.Utils;
using SS.CMS.Utils.Tests;
using Xunit;
using Xunit.Abstractions;

namespace SS.CMS.Api.Tests
{
    public class IntegrationTests 
    {
        private readonly ITestOutputHelper _output;

        public IntegrationTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [SkippableFact]
        public void TrueTest()
        {
            Skip.IfNot(TestEnv.IsTestMachine);

            _output.WriteLine("test!!");

            //_fixture.TableManager.SyncDatabase();

            Assert.True(true);
        }
    }
}
