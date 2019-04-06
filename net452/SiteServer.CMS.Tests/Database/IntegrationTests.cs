using System;
using SiteServer.CMS.Core;
using SiteServer.Utils;
using Xunit;
using Xunit.Abstractions;

namespace SiteServer.CMS.Tests.Database
{
    [TestCaseOrderer("SiteServer.CMS.Tests.PriorityOrderer", "SiteServer.CMS.Tests")]
    public class IntegrationTests : IClassFixture<EnvironmentFixture>
    {
        private readonly EnvironmentFixture _fixture;
        private readonly ITestOutputHelper _output;

        public IntegrationTests(EnvironmentFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

        [SkippableFact, TestPriority(0)]
        public void TrueTest()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);
            
            SystemManager.SyncDatabase();

            Assert.True(true);
        }
    }
}
