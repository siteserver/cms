using System;
using SiteServer.API.Tests;
using SiteServer.CMS.Core;
using SiteServer.Utils;
using Xunit;
using Xunit.Abstractions;

namespace SiteServer.API.Tests.Database
{
    [TestCaseOrderer("SiteServer.CMS.Tests.PriorityOrderer", "SiteServer.CMS.Tests")]
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
