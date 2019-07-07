using System.Threading.Tasks;
using Xunit;
using SS.CMS.Api.Controllers.Admin;
using SS.CMS.Utils.Tests;
using Xunit.Abstractions;
using SS.CMS.Services;
using Moq;
using SS.CMS.Repositories;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Models;
using SS.CMS.Api.Controllers.Cms;

namespace SS.CMS.Api.Tests.Controllers.Admin
{
    public class InstallControllerTests : IClassFixture<EnvironmentFixture>
    {
        private readonly EnvironmentFixture _fixture;
        private readonly ITestOutputHelper _output;

        public InstallControllerTests(EnvironmentFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

        [Fact]
        public void Index_ReturnsAViewResult_WithAListOfBrainstormSessions()
        {
            // Arrange
            var mockPluginManager = new Mock<IPluginManager>();
            var mockUserManager = new Mock<IUserManager>();
            var mockUserRepository = new Mock<IUserRepository>();
            var mockSiteRepository = new Mock<ISiteRepository>();
            var mockConfigRepository = new Mock<IConfigRepository>();

            mockUserManager.Setup(x => x.GetUserAsync()).ReturnsAsync(() => new UserInfo());
            mockConfigRepository.Setup(x => x.GetConfigInfoAsync()).ReturnsAsync(() => new ConfigInfo());

            // var controller = new InstallController(_fixture.SettingsManager, mockPluginManager.Object, mockUserManager.Object, mockUserRepository.Object, mockSiteRepository.Object, mockConfigRepository.Object);
            CmsController controller = null;

            // Act
            var actionResult = controller.GetInfo();

            // Assert
            var result = Assert.IsType<CmsController.InfoResponse>(actionResult.Result);

            _output.WriteLine($"version:{result.ProductVersion}");
            Assert.True(result.IsInstalled);
        }
    }
}