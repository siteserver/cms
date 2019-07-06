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

namespace SS.CMS.Api.Tests.Controllers.Admin
{
    public class IndexControllerTests : IClassFixture<EnvironmentFixture>
    {
        private readonly EnvironmentFixture _fixture;
        private readonly ITestOutputHelper _output;

        public IndexControllerTests(EnvironmentFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfBrainstormSessions()
        {
            // Arrange
            var mockUserManager = new Mock<IUserManager>();
            var mockSiteRepository = new Mock<ISiteRepository>();
            var mockConfigRepository = new Mock<IConfigRepository>();

            mockUserManager.Setup(x => x.GetUserAsync()).ReturnsAsync(() => new UserInfo());
            mockConfigRepository.Setup(x => x.GetConfigInfoAsync()).ReturnsAsync(() => new ConfigInfo());

            var controller = new IndexController(_fixture.SettingsManager, mockUserManager.Object, mockSiteRepository.Object, mockConfigRepository.Object);

            // Act
            var result = await controller.Get();

            // Assert
            var viewModel = Assert.IsType<IndexController.GetModel>(result.Value);

            _output.WriteLine($"version:{viewModel.Version}");
            Assert.Equal("1.0.0.0", viewModel.Version);
        }
    }
}