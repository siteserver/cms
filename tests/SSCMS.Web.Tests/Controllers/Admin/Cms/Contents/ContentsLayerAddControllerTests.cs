using System.Threading.Tasks;
using Moq;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Web.Controllers.Admin.Cms.Contents;
using Xunit;

namespace SSCMS.Web.Tests.Controllers.Admin.Cms.Contents
{
    public class ContentsLayerAddControllerTests
    {
        [Fact]
        public async Task SubmitKeepsContentUnapprovedWithoutCheckPermission()
        {
            var authManager = new Mock<IAuthManager>();
            authManager.Setup(x => x.AdminId).Returns(7);
            authManager.Setup(x => x.IsSiteAdminAsync()).ReturnsAsync(false);
            authManager
                .Setup(x => x.HasSitePermissionsAsync(1, MenuUtils.SitePermissions.Contents))
                .ReturnsAsync(true);
            authManager
                .Setup(x => x.HasContentPermissionsAsync(1, 2, MenuUtils.ContentPermissions.Add))
                .ReturnsAsync(true);

            var createManager = new Mock<ICreateManager>();
            createManager
                .Setup(x => x.CreateContentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);
            createManager
                .Setup(x => x.TriggerContentChangedEventAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            var siteRepository = new Mock<ISiteRepository>();
            siteRepository
                .Setup(x => x.GetAsync(1))
                .ReturnsAsync(new Site { Id = 1, CheckContentLevel = 1 });

            var channelRepository = new Mock<IChannelRepository>();
            channelRepository
                .Setup(x => x.GetAsync(2))
                .ReturnsAsync(new Channel { Id = 2, SiteId = 1 });

            Content insertedContent = null;
            var contentRepository = new Mock<IContentRepository>();
            contentRepository
                .Setup(x => x.InsertAsync(It.IsAny<Site>(), It.IsAny<Channel>(), It.IsAny<Content>()))
                .Callback<Site, Channel, Content>((_, _, content) => insertedContent = content)
                .ReturnsAsync(3);

            var controller = new ContentsLayerAddController(
                authManager.Object,
                createManager.Object,
                siteRepository.Object,
                channelRepository.Object,
                contentRepository.Object);

            await controller.Submit(new ContentsLayerAddController.SubmitRequest
            {
                SiteId = 1,
                ChannelId = 2,
                CheckedLevel = 1,
                Titles = "draft"
            });

            Assert.NotNull(insertedContent);
            Assert.False(insertedContent.Checked);
            Assert.Equal(0, insertedContent.CheckedLevel);
            createManager.Verify(
                x => x.CreateContentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()),
                Times.Never);
            authManager.Verify(
                x => x.HasContentPermissionsAsync(1, 2, MenuUtils.ContentPermissions.CheckLevel1),
                Times.Once);
        }
    }
}
