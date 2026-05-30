using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Web.Controllers.Stl;
using Xunit;

namespace SSCMS.Web.Tests.Controllers.Stl
{
    public class ActionsDownloadControllerTests
    {
        [Fact]
        public async Task GetRejectsUnsignedContentDownloadBeforeIncrementing()
        {
            var settingsManager = new Mock<ISettingsManager>();
            settingsManager.Setup(x => x.Decrypt("encrypted", null)).Returns("/files/missing.pdf");

            var pathManager = new Mock<IPathManager>();
            pathManager.Setup(x => x.GetSitePathAsync(It.IsAny<Site>())).ReturnsAsync("/tmp/site");
            pathManager.Setup(x => x.ParseSitePathAsync(It.IsAny<Site>(), "/files/missing.pdf")).ReturnsAsync("/tmp/site/files/missing.pdf");
            pathManager.Setup(x => x.IsFileDownload(It.IsAny<Site>(), ".pdf")).Returns(true);

            var siteRepository = new Mock<ISiteRepository>();
            siteRepository.Setup(x => x.GetAsync(1)).ReturnsAsync(new Site
            {
                Id = 1
            });

            var channelRepository = new Mock<IChannelRepository>();
            channelRepository.Setup(x => x.GetAsync(2)).ReturnsAsync(new Channel
            {
                Id = 2
            });
            channelRepository.Setup(x => x.GetTableName(It.IsAny<Site>(), It.IsAny<Channel>())).Returns("sscms_Content");

            var contentRepository = new Mock<IContentRepository>();

            var controller = new ActionsDownloadController(
                settingsManager.Object,
                pathManager.Object,
                siteRepository.Object,
                channelRepository.Object,
                contentRepository.Object);

            var result = await controller.Get(new ActionsDownloadController.GetRequest
            {
                SiteId = 1,
                ChannelId = 2,
                ContentId = 3,
                FileUrl = "encrypted"
            });

            Assert.IsType<UnauthorizedResult>(result);
            contentRepository.Verify(x => x.AddDownloadsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }
    }
}
