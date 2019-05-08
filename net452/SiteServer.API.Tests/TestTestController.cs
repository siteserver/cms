using System.Web.Http.Results;
using SiteServer.API.Controllers;
using SiteServer.API.Tests.Utils;
using SiteServer.Utils;
using Xunit;
using Xunit.Abstractions;

namespace SiteServer.API.Tests
{
    public class TestTestController : IClassFixture<EnvironmentFixture>
    {
        public EnvironmentFixture Fixture { get; }
        private readonly ITestOutputHelper _output;

        public TestTestController(EnvironmentFixture fixture, ITestOutputHelper output)
        {
            Fixture = fixture;
            _output = output;
        }

        [SkippableFact]
        public void GetAdminOnly_ShouldReturnUnauthorizedResult()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var controller = ControllerUtils.NewAnonymousController<TestController>();
            var actionResult = controller.GetAdminOnly();

            Assert.IsType<UnauthorizedResult>(actionResult);

            var adminInfo = AdminUtils.CreateAdminIfNotExists();
            var accessToken = AdminUtils.GetAccessToken(adminInfo);
            controller = ControllerUtils.NewAdminController<TestController>(accessToken);

            actionResult = controller.GetAdminOnly();
            Assert.IsType<UnauthorizedResult>(actionResult);
        }

        [SkippableFact]
        public void GetAdminOnly_ShouldReturnOkResult()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var adminInfo = AdminUtils.CreateSuperAdminIfNotExists();
            var accessToken = AdminUtils.GetAccessToken(adminInfo);
            var controller = ControllerUtils.NewAdminController<TestController>(accessToken);

            dynamic results = controller.GetAdminOnly();
            var content = results.Content;

            Assert.NotNull(content);

            _output.WriteLine(TranslateUtils.JsonSerialize(content));
        }

        //[Fact]
        //public void Get_ShouldReturnList()
        //{
        //    var controller = new TestController
        //    {
        //        Request = new HttpRequestMessage(),
        //        Configuration = new HttpConfiguration()
        //    };

        //    dynamic results = controller.Get();
        //    var content = results.Content;

        //    Assert.NotNull(content);

        //    _output.WriteLine(TranslateUtils.JsonSerialize(content));
        //}

        //[Fact]
        //public void GetString_ShouldReturnHello()
        //{
        //    var controller = new TestController
        //    {
        //        Request = new HttpRequestMessage(),
        //        Configuration = new HttpConfiguration()
        //    };

        //    var response = controller.GetString() as OkNegotiatedContentResult<string>;
        //    Assert.NotNull(response);
        //    Assert.Equal("Hello", response.Content);
        //}
    }
}