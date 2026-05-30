using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using Xunit;
using EditorController = SSCMS.Web.Controllers.Home.Write.EditorController;

namespace SSCMS.Web.Tests.Controllers.Home.Write
{
    public class EditorControllerTests
    {
        [Fact]
        public async Task InsertKeepsContentUnapprovedWithoutCheckPermission()
        {
            var authManager = CreateAuthManager(MenuUtils.ContentPermissions.Add);
            var createManager = CreateCreateManager();
            var storageManager = CreateStorageManager();

            var siteRepository = new Mock<ISiteRepository>();
            siteRepository
                .Setup(x => x.GetAsync(1))
                .ReturnsAsync(new Site { Id = 1, CheckContentLevel = 1 });

            var channelRepository = new Mock<IChannelRepository>();
            channelRepository
                .Setup(x => x.GetAsync(2))
                .ReturnsAsync(new Channel { Id = 2, SiteId = 1 });

            var requestContent = new Content
            {
                Id = 3,
                Title = "draft",
                Checked = true,
                CheckedLevel = 1
            };

            var pathManager = new Mock<IPathManager>();
            pathManager
                .Setup(x => x.EncodeContentAsync(
                    It.IsAny<Site>(),
                    It.IsAny<Channel>(),
                    requestContent,
                    It.IsAny<string>()))
                .ReturnsAsync(requestContent);

            Content insertedContent = null;
            var contentRepository = new Mock<IContentRepository>();
            contentRepository
                .Setup(x => x.InsertAsync(It.IsAny<Site>(), It.IsAny<Channel>(), It.IsAny<Content>()))
                .Callback<Site, Channel, Content>((_, _, content) => insertedContent = content)
                .ReturnsAsync(3);

            var controller = CreateController(
                authManager.Object,
                createManager,
                pathManager.Object,
                storageManager,
                siteRepository.Object,
                channelRepository.Object,
                contentRepository.Object);

            await controller.Insert(new EditorController.SaveRequest
            {
                SiteId = 1,
                ChannelId = 2,
                Content = requestContent
            });

            Assert.NotNull(insertedContent);
            Assert.False(insertedContent.Checked);
            Assert.Equal(0, insertedContent.CheckedLevel);
            authManager.Verify(
                x => x.HasContentPermissionsAsync(1, 2, MenuUtils.ContentPermissions.CheckLevel1),
                Times.Once);
        }

        [Fact]
        public async Task UpdateKeepsContentUnapprovedWithoutCheckPermission()
        {
            var authManager = CreateAuthManager(MenuUtils.ContentPermissions.Edit);
            var createManager = CreateCreateManager();
            var storageManager = CreateStorageManager();

            var siteRepository = new Mock<ISiteRepository>();
            siteRepository
                .Setup(x => x.GetAsync(1))
                .ReturnsAsync(new Site { Id = 1, CheckContentLevel = 1 });

            var channelRepository = new Mock<IChannelRepository>();
            channelRepository
                .Setup(x => x.GetAsync(2))
                .ReturnsAsync(new Channel { Id = 2, SiteId = 1 });

            var sourceContent = new Content
            {
                Id = 3,
                SiteId = 1,
                ChannelId = 2,
                Title = "source",
                Checked = false,
                CheckedLevel = 0
            };
            var requestContent = new Content
            {
                Id = 3,
                SiteId = 1,
                ChannelId = 2,
                Title = "updated",
                Checked = true,
                CheckedLevel = 1
            };

            var pathManager = new Mock<IPathManager>();
            pathManager
                .Setup(x => x.EncodeContentAsync(
                    It.IsAny<Site>(),
                    It.IsAny<Channel>(),
                    requestContent,
                    It.IsAny<string>()))
                .ReturnsAsync(requestContent);

            Content updatedContent = null;
            var contentRepository = new Mock<IContentRepository>();
            contentRepository
                .Setup(x => x.GetAsync(It.IsAny<Site>(), It.IsAny<Channel>(), 3))
                .ReturnsAsync(sourceContent);
            contentRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Site>(), It.IsAny<Channel>(), It.IsAny<Content>()))
                .Callback<Site, Channel, Content>((_, _, content) => updatedContent = content)
                .Returns(Task.CompletedTask);

            var contentCheckRepository = new Mock<IContentCheckRepository>();
            contentCheckRepository
                .Setup(x => x.InsertAsync(It.IsAny<ContentCheck>()))
                .Returns(Task.CompletedTask);

            var controller = CreateController(
                authManager.Object,
                createManager,
                pathManager.Object,
                storageManager,
                siteRepository.Object,
                channelRepository.Object,
                contentRepository.Object,
                contentCheckRepository.Object);

            await controller.Update(new EditorController.SaveRequest
            {
                SiteId = 1,
                ChannelId = 2,
                ContentId = 3,
                Content = requestContent
            });

            Assert.NotNull(updatedContent);
            Assert.False(updatedContent.Checked);
            Assert.Equal(0, updatedContent.CheckedLevel);
            contentCheckRepository.Verify(x => x.InsertAsync(It.IsAny<ContentCheck>()), Times.Never);
            authManager.Verify(
                x => x.HasContentPermissionsAsync(1, 2, MenuUtils.ContentPermissions.CheckLevel1),
                Times.Once);
        }

        private static Mock<IAuthManager> CreateAuthManager(string contentPermission)
        {
            var authManager = new Mock<IAuthManager>();
            authManager.Setup(x => x.AdminId).Returns(7);
            authManager.Setup(x => x.UserId).Returns(8);
            authManager.Setup(x => x.IsSiteAdminAsync()).ReturnsAsync(false);
            authManager
                .Setup(x => x.HasSitePermissionsAsync(1, MenuUtils.SitePermissions.Contents))
                .ReturnsAsync(true);
            authManager
                .Setup(x => x.HasContentPermissionsAsync(1, 2, contentPermission))
                .ReturnsAsync(true);
            return authManager;
        }

        private static ICreateManager CreateCreateManager()
        {
            var createManager = new Mock<ICreateManager>();
            createManager
                .Setup(x => x.CreateContentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);
            createManager
                .Setup(x => x.TriggerContentChangedEventAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);
            return createManager.Object;
        }

        private static IStorageManager CreateStorageManager()
        {
            var storageManager = new Mock<IStorageManager>();
            storageManager
                .Setup(x => x.IsStorageAsync(It.IsAny<int>(), SyncType.Images))
                .ReturnsAsync(false);
            return storageManager.Object;
        }

        private static EditorController CreateController(
            IAuthManager authManager,
            ICreateManager createManager,
            IPathManager pathManager,
            IStorageManager storageManager,
            ISiteRepository siteRepository,
            IChannelRepository channelRepository,
            IContentRepository contentRepository,
            IContentCheckRepository contentCheckRepository = null)
        {
            return new EditorController(
                authManager,
                Mock.Of<ICloudManager>(),
                createManager,
                pathManager,
                Mock.Of<IDatabaseManager>(),
                Mock.Of<IPluginManager>(),
                storageManager,
                siteRepository,
                channelRepository,
                contentRepository,
                Mock.Of<IContentGroupRepository>(),
                Mock.Of<IContentTagRepository>(),
                Mock.Of<ITableStyleRepository>(),
                Mock.Of<IRelatedFieldItemRepository>(),
                contentCheckRepository ?? Mock.Of<IContentCheckRepository>());
        }
    }
}
