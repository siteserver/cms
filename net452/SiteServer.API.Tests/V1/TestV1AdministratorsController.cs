using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Results;
using SiteServer.API.Controllers.V1;
using SiteServer.API.Tests.Utils;
using SiteServer.Utils;
using Xunit;
using Xunit.Abstractions;

namespace SiteServer.API.Tests.V1
{
    public class TestV1AdministratorsController : IClassFixture<EnvironmentFixture>
    {
        public EnvironmentFixture Fixture { get; }
        private readonly ITestOutputHelper _output;

        public TestV1AdministratorsController(EnvironmentFixture fixture, ITestOutputHelper output)
        {
            Fixture = fixture;
            _output = output;
        }

        [SkippableFact]
        public void List_ShouldReturnUnauthorizedResult()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var controller = new V1AdministratorsController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            var actionResult = controller.List();
            Assert.IsType<UnauthorizedResult>(actionResult);
        }

        [SkippableFact]
        public void Me_ShouldReturnUnauthorizedResult()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var controller = new V1AdministratorsController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            var actionResult = controller.GetSelf();
            Assert.IsType<UnauthorizedResult>(actionResult);
        }

        [SkippableFact]
        public void Me_ShouldReturnOkResult()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var adminInfo = AdminUtils.CreateAdminIfNotExists();
            var accessToken = AdminUtils.GetAccessToken(adminInfo);

            // Arrange
            var controller = new V1AdministratorsController();
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add(Rest.AuthKeyAdminHeader, accessToken);

            // Don't forget these lines, if you do then the request will be null.
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;

            dynamic results = controller.GetSelf();
            var content = results.Content;

            Assert.NotNull(content);

            _output.WriteLine(TranslateUtils.JsonSerialize(content));
        }
    }
}