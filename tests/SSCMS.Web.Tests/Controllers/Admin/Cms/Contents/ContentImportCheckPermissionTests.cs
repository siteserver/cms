using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using Xunit;
using AdminWordController = SSCMS.Web.Controllers.Admin.Cms.Contents.ContentsLayerWordController;
using HomeWordController = SSCMS.Web.Controllers.Home.Write.ContentsLayerWordController;
using ImportController = SSCMS.Web.Controllers.Admin.Cms.Contents.ContentsLayerImportController;

namespace SSCMS.Web.Tests.Controllers.Admin.Cms.Contents
{
    public class ContentImportCheckPermissionTests
    {
        [Fact]
        public async Task AdminWordSubmitChecksApprovalPermissionBeforeImport()
        {
            var authManager = CreateAuthManager();
            var controller = new AdminWordController(
                authManager.Object,
                Mock.Of<IPathManager>(),
                CreateCreateManager(),
                CreateSiteRepository(),
                CreateChannelRepository(),
                Mock.Of<IContentRepository>(),
                Mock.Of<IErrorLogRepository>());

            await controller.Submit(new AdminWordController.SubmitRequest
            {
                SiteId = 1,
                ChannelId = 2,
                CheckedLevel = 1,
                FileNames = new List<string>(),
                FileUrls = new List<string>()
            });

            authManager.Verify(
                x => x.HasContentPermissionsAsync(1, 2, MenuUtils.ContentPermissions.CheckLevel1),
                Times.Once);
        }

        [Fact]
        public async Task HomeWordSubmitChecksApprovalPermissionBeforeImport()
        {
            var authManager = CreateAuthManager();
            var tableStyleRepository = new Mock<ITableStyleRepository>();
            tableStyleRepository
                .Setup(x => x.GetContentStylesAsync(It.IsAny<Site>(), It.IsAny<Channel>()))
                .ReturnsAsync(new List<TableStyle>());

            var controller = new HomeWordController(
                authManager.Object,
                Mock.Of<IPathManager>(),
                CreateCreateManager(),
                CreateSiteRepository(),
                CreateChannelRepository(),
                Mock.Of<IContentRepository>(),
                tableStyleRepository.Object);

            await controller.Submit(new HomeWordController.SubmitRequest
            {
                SiteId = 1,
                ChannelId = 2,
                CheckedLevel = 1,
                Files = new List<HomeWordController.NameTitle>()
            });

            authManager.Verify(
                x => x.HasContentPermissionsAsync(1, 2, MenuUtils.ContentPermissions.CheckLevel1),
                Times.Once);
        }

        [Fact]
        public async Task ImportSubmitChecksApprovalPermissionBeforeImport()
        {
            var authManager = CreateAuthManager();
            authManager
                .Setup(x => x.AddSiteLogAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var siteRepository = new Mock<ISiteRepository>();
            siteRepository
                .Setup(x => x.GetAsync(1))
                .ReturnsAsync(new Site { Id = 1, CheckContentLevel = 1 });
            siteRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Site>()))
                .Returns(Task.CompletedTask);

            var controller = new ImportController(
                Mock.Of<ICacheManager>(),
                authManager.Object,
                Mock.Of<IPathManager>(),
                Mock.Of<IStorageManager>(),
                CreateCreateManager(),
                Mock.Of<IDatabaseManager>(),
                siteRepository.Object,
                CreateChannelRepository(),
                Mock.Of<ITableStyleRepository>());

            await controller.Submit(new ImportController.SubmitRequest
            {
                SiteId = 1,
                ChannelId = 2,
                ImportType = "zip",
                CheckedLevel = 1,
                FileNames = new List<string>(),
                FileUrls = new List<string>(),
                Attributes = new List<string>()
            });

            authManager.Verify(
                x => x.HasContentPermissionsAsync(1, 2, MenuUtils.ContentPermissions.CheckLevel1),
                Times.Once);
        }

        private static Mock<IAuthManager> CreateAuthManager()
        {
            var authManager = new Mock<IAuthManager>();
            authManager.Setup(x => x.AdminId).Returns(7);
            authManager.Setup(x => x.UserId).Returns(8);
            authManager.Setup(x => x.IsSiteAdminAsync()).ReturnsAsync(false);
            authManager
                .Setup(x => x.HasSitePermissionsAsync(1, MenuUtils.SitePermissions.Contents))
                .ReturnsAsync(true);
            authManager
                .Setup(x => x.HasContentPermissionsAsync(1, 2, MenuUtils.ContentPermissions.Add))
                .ReturnsAsync(true);
            return authManager;
        }

        private static ICreateManager CreateCreateManager()
        {
            var createManager = new Mock<ICreateManager>();
            createManager
                .Setup(x => x.CreateChannelAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);
            createManager
                .Setup(x => x.CreateContentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);
            createManager
                .Setup(x => x.TriggerContentChangedEventAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);
            return createManager.Object;
        }

        private static ISiteRepository CreateSiteRepository()
        {
            var siteRepository = new Mock<ISiteRepository>();
            siteRepository
                .Setup(x => x.GetAsync(1))
                .ReturnsAsync(new Site { Id = 1, CheckContentLevel = 1 });
            return siteRepository.Object;
        }

        private static IChannelRepository CreateChannelRepository()
        {
            var channelRepository = new Mock<IChannelRepository>();
            channelRepository
                .Setup(x => x.GetAsync(2))
                .ReturnsAsync(new Channel { Id = 2, SiteId = 1 });
            return channelRepository.Object;
        }
    }
}
