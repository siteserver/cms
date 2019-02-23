using System.Net;
using System.Net.Http;
using System.Web.Http;
using SiteServer.API.Controllers.V1;
using Xunit;
using Xunit.Abstractions;

namespace SiteServer.API.Tests.V1
{
    public class TestV1PingController : IClassFixture<EnvironmentFixture>
    {
        public EnvironmentFixture Fixture { get; }
        private readonly ITestOutputHelper _output;

        public TestV1PingController(EnvironmentFixture fixture, ITestOutputHelper output)
        {
            Fixture = fixture;
            _output = output;
        }

        [Fact]
        public void Get_ShouldReturnPong()
        {
            var controller = new V1PingController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            var response = controller.Get();

            var responseContent = response.Content as StringContent;
            Assert.NotNull(responseContent);

            var result = responseContent.ReadAsStringAsync().Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("pong", result);
            _output.WriteLine(result);
        }
    }
}