using System.Threading.Tasks;
using Xunit;

namespace SS.CMS.Web.Tests.IntegrationTests.Cms
{
    public partial class CmsControllerTests
    {
        [Theory]
        [InlineData("/api/ping")]
        public async Task CmsInfoTests(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var pong = await response.Content.ReadAsStringAsync();
            Assert.NotNull(pong);
            Assert.Equal("pong", pong);
        }
    }
}