using System;
using SS.CMS.Core.Common;
using SS.CMS.Utils;
using Xunit;

namespace SS.CMS.Api.Tests
{
    [TestCaseOrderer("SS.CMS.Api.Tests.PriorityOrderer", "SS.CMS.Api.Tests")]
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

            SystemManager.SyncDatabase(_fixture.ConfigRepository);

            Assert.True(true);
        }
    }
}
