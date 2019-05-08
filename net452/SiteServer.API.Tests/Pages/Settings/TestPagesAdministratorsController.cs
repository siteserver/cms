using SiteServer.API.Controllers.Pages.Settings;
using SiteServer.API.Tests.Utils;
using SiteServer.Utils;
using Xunit;
using Xunit.Abstractions;

namespace SiteServer.API.Tests.Pages.Settings
{
    public class TestPagesAdministratorsController : IClassFixture<EnvironmentFixture>
    {
        public EnvironmentFixture Fixture { get; }
        private readonly ITestOutputHelper _output;

        public TestPagesAdministratorsController(EnvironmentFixture fixture, ITestOutputHelper output)
        {
            Fixture = fixture;
            _output = output;
        }

        [SkippableFact]
        public void GetList_ShouldReturnOkResult()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var adminInfo = AdminUtils.CreateSuperAdminIfNotExists();
            var accessToken = AdminUtils.GetAccessToken(adminInfo);
            var controller = ControllerUtils.NewAdminController<PagesAdministratorsController>(accessToken);

            dynamic results = controller.GetList();
            var res = results.Content;

            Assert.NotNull(res);
            Assert.NotNull(res.Value);
            Assert.NotNull(res.Count);
            Assert.NotNull(res.Pages);
            Assert.NotNull(res.Roles);
            Assert.NotNull(res.Departments);
            Assert.NotNull(res.Areas);
            _output.WriteLine(TranslateUtils.JsonSerialize(res));
        }
    }
}