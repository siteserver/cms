using System.Threading.Tasks;
using Xunit;

namespace SSCMS.Web.Tests.IntegrationTests.Admin
{
    public partial class DashboardControllerTests
        : IClassFixture<TestWebApplicationFactory>
    {
        private readonly TestWebApplicationFactory _factory;

        public DashboardControllerTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/admin/dashboard")]
        public async Task GetTests(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.False(response.IsSuccessStatusCode);
        }
    }
}
