using System;
using SS.CMS.Core.Common;
using SS.CMS.Utils;
using Xunit;

namespace SS.CMS.Core.Tests
{
    [TestCaseOrderer("SS.CMS.Core.Tests.PriorityOrderer", "SS.CMS.Core.Tests")]
    public class IntegrationTests : IClassFixture<EnvironmentFixture>
    {
        private readonly EnvironmentFixture _fixture;

        public IntegrationTests(EnvironmentFixture fixture)
        {
            _fixture = fixture;
        }

        [SkippableFact, TestPriority(0)]
        public void TrueTest()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            Console.WriteLine("test!!");

            SystemManager.SyncDatabase();

            Assert.True(true);
        }
    }
}
