using Xunit;
using System.Threading.Tasks;
using SS.CMS.Utils;
using static SS.CMS.Api.Controllers.Cms.CmsController;

namespace SS.CMS.Api.Tests.IntegrationTests.Cms
{
    public partial class CmsControllerTests
    {
        [Theory]
        [InlineData("/api/cms")]
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

            var json = await response.Content.ReadAsStringAsync();
            var info = TranslateUtils.JsonDeserialize<InfoResponse>(json);
            Assert.NotNull(info);
            Assert.Equal("localhost/api", info.ApiUrl);
        }
    }
}