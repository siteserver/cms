using SiteServer.CMS.Core;
using Xunit;

namespace SiteServer.CMS.Tests
{
    public class IntegrationTests : IClassFixture<EnvironmentFixture>
    {
        private readonly EnvironmentFixture _fixture;

        public IntegrationTests(EnvironmentFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void TrueTest()
        {
            SystemManager.SyncDatabase();

            Assert.True(true);
        }
    }
}
