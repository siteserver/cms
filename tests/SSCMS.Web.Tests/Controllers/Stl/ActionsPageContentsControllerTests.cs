using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Web.Controllers.Stl;
using Xunit;

namespace SSCMS.Web.Tests.Controllers.Stl
{
    public class ActionsPageContentsControllerTests
    {
        [Fact]
        public async Task SubmitRejectsUnsignedPageContentsRequests()
        {
            var siteRepository = new Mock<ISiteRepository>();

            var controller = new ActionsPageContentsController(
                Mock.Of<ISettingsManager>(),
                Mock.Of<IAuthManager>(),
                Mock.Of<IParseManager>(),
                siteRepository.Object,
                Mock.Of<IChannelRepository>(),
                Mock.Of<ITemplateRepository>());

            var result = await controller.Submit(new ActionsPageContentsController.SubmitRequest
            {
                SiteId = 1,
                PageChannelId = 1,
                TemplateId = 1,
                TotalNum = 10,
                PageCount = 2,
                CurrentPageIndex = 1,
                StlPageContentsElement = "encrypted"
            });

            Assert.IsType<UnauthorizedResult>(result.Result);
            siteRepository.Verify(x => x.GetAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task SubmitRejectsTamperedPageContentsRequests()
        {
            var siteRepository = new Mock<ISiteRepository>();
            var settingsManager = new Mock<ISettingsManager>();
            settingsManager
                .Setup(x => x.Decrypt("signed", null))
                .Returns("1:1:1:10:2:0:encrypted");

            var controller = new ActionsPageContentsController(
                settingsManager.Object,
                Mock.Of<IAuthManager>(),
                Mock.Of<IParseManager>(),
                siteRepository.Object,
                Mock.Of<IChannelRepository>(),
                Mock.Of<ITemplateRepository>());

            var result = await controller.Submit(new ActionsPageContentsController.SubmitRequest
            {
                SiteId = 1,
                PageChannelId = 1,
                TemplateId = 1,
                TotalNum = 10,
                PageCount = 2,
                CurrentPageIndex = 1,
                StlPageContentsElement = "encrypted",
                Token = "signed"
            });

            Assert.IsType<UnauthorizedResult>(result.Result);
            siteRepository.Verify(x => x.GetAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task SubmitRejectsInvalidEncryptedToken()
        {
            var siteRepository = new Mock<ISiteRepository>();
            var settingsManager = new Mock<ISettingsManager>();
            settingsManager
                .Setup(x => x.Decrypt("invalid", null))
                .Throws(new System.Exception("invalid token"));

            var controller = new ActionsPageContentsController(
                settingsManager.Object,
                Mock.Of<IAuthManager>(),
                Mock.Of<IParseManager>(),
                siteRepository.Object,
                Mock.Of<IChannelRepository>(),
                Mock.Of<ITemplateRepository>());

            var result = await controller.Submit(new ActionsPageContentsController.SubmitRequest
            {
                SiteId = 1,
                PageChannelId = 1,
                TemplateId = 1,
                TotalNum = 10,
                PageCount = 2,
                CurrentPageIndex = 1,
                StlPageContentsElement = "encrypted",
                Token = "invalid"
            });

            Assert.IsType<UnauthorizedResult>(result.Result);
            siteRepository.Verify(x => x.GetAsync(It.IsAny<int>()), Times.Never);
        }
    }
}
