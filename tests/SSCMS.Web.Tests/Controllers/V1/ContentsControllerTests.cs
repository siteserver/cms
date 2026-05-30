using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Web.Controllers.V1;
using Xunit;

namespace SSCMS.Web.Tests.Controllers.V1
{
    public class ContentsControllerTests
    {
        [Fact]
        public async Task ListRejectsUnsafeWhereColumnsBeforeQueryingSite()
        {
            var authManager = new Mock<IAuthManager>();
            authManager.Setup(x => x.ApiToken).Returns("token");

            var accessTokenRepository = new Mock<IAccessTokenRepository>();
            accessTokenRepository
                .Setup(x => x.IsScopeAsync("token", Constants.ScopeContents))
                .ReturnsAsync(true);

            var siteRepository = new Mock<ISiteRepository>();

            var controller = new ContentsController(
                authManager.Object,
                Mock.Of<ICreateManager>(),
                Mock.Of<IParseManager>(),
                Mock.Of<IDatabaseManager>(),
                Mock.Of<IPathManager>(),
                accessTokenRepository.Object,
                siteRepository.Object,
                Mock.Of<IChannelRepository>(),
                Mock.Of<IContentRepository>(),
                Mock.Of<IContentCheckRepository>());

            var result = await controller.List(new ContentsController.QueryRequest
            {
                SiteId = 1,
                Wheres = new List<ContentsController.ClauseWhere>
                {
                    new ContentsController.ClauseWhere
                    {
                        Column = "Title; drop table siteserver_Administrator",
                        Value = "test"
                    }
                }
            });

            Assert.IsType<BadRequestObjectResult>(result.Result);
            siteRepository.Verify(x => x.GetAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task ListRejectsUnsafeOrderColumnsBeforeQueryingSite()
        {
            var authManager = new Mock<IAuthManager>();
            authManager.Setup(x => x.ApiToken).Returns("token");

            var accessTokenRepository = new Mock<IAccessTokenRepository>();
            accessTokenRepository
                .Setup(x => x.IsScopeAsync("token", Constants.ScopeContents))
                .ReturnsAsync(true);

            var siteRepository = new Mock<ISiteRepository>();

            var controller = new ContentsController(
                authManager.Object,
                Mock.Of<ICreateManager>(),
                Mock.Of<IParseManager>(),
                Mock.Of<IDatabaseManager>(),
                Mock.Of<IPathManager>(),
                accessTokenRepository.Object,
                siteRepository.Object,
                Mock.Of<IChannelRepository>(),
                Mock.Of<IContentRepository>(),
                Mock.Of<IContentCheckRepository>());

            var result = await controller.List(new ContentsController.QueryRequest
            {
                SiteId = 1,
                Orders = new List<ContentsController.ClauseOrder>
                {
                    new ContentsController.ClauseOrder
                    {
                        Column = "Title desc; drop table siteserver_Administrator"
                    }
                }
            });

            Assert.IsType<BadRequestObjectResult>(result.Result);
            siteRepository.Verify(x => x.GetAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task UpdateKeepsContentUnapprovedWithoutCheckPermission()
        {
            var authManager = new Mock<IAuthManager>();
            authManager.Setup(x => x.ApiToken).Returns("token");
            authManager
                .Setup(x => x.HasContentPermissionsAsync(1, 2, MenuUtils.ContentPermissions.Edit))
                .ReturnsAsync(true);
            authManager
                .Setup(x => x.AddSiteLogAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var createManager = new Mock<ICreateManager>();
            createManager
                .Setup(x => x.CreateContentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);
            createManager
                .Setup(x => x.TriggerContentChangedEventAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            var accessTokenRepository = new Mock<IAccessTokenRepository>();
            accessTokenRepository
                .Setup(x => x.IsScopeAsync("token", Constants.ScopeContents))
                .ReturnsAsync(true);

            var siteRepository = new Mock<ISiteRepository>();
            siteRepository
                .Setup(x => x.GetAsync(1))
                .ReturnsAsync(new Site { Id = 1, CheckContentLevel = 1 });

            var channelRepository = new Mock<IChannelRepository>();
            channelRepository
                .Setup(x => x.GetAsync(2))
                .ReturnsAsync(new Channel { Id = 2, SiteId = 1 });
            channelRepository
                .Setup(x => x.GetChannelNameNavigationAsync(1, 2))
                .ReturnsAsync("channel");

            var content = new Content
            {
                Id = 3,
                SiteId = 1,
                ChannelId = 2,
                Title = "draft",
                Checked = false,
                CheckedLevel = 0
            };

            Content updatedContent = null;
            var contentRepository = new Mock<IContentRepository>();
            contentRepository
                .Setup(x => x.GetAsync(It.IsAny<Site>(), It.IsAny<Channel>(), 3))
                .ReturnsAsync(content);
            contentRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Site>(), It.IsAny<Channel>(), It.IsAny<Content>()))
                .Callback<Site, Channel, Content>((_, _, updated) => updatedContent = updated)
                .Returns(Task.CompletedTask);

            var controller = new ContentsController(
                authManager.Object,
                createManager.Object,
                Mock.Of<IParseManager>(),
                Mock.Of<IDatabaseManager>(),
                Mock.Of<IPathManager>(),
                accessTokenRepository.Object,
                siteRepository.Object,
                channelRepository.Object,
                contentRepository.Object,
                Mock.Of<IContentCheckRepository>());

            await controller.Update(1, 2, 3, new Dictionary<string, object>
            {
                [nameof(Content.Checked)] = true,
                [nameof(Content.CheckedLevel)] = 1
            });

            Assert.NotNull(updatedContent);
            Assert.False(updatedContent.Checked);
            Assert.Equal(0, updatedContent.CheckedLevel);
            authManager.Verify(
                x => x.HasContentPermissionsAsync(1, 2, MenuUtils.ContentPermissions.CheckLevel1),
                Times.Once);
            createManager.Verify(
                x => x.CreateContentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAllowsApprovalWhenSiteHasNoCheckWorkflow()
        {
            var authManager = new Mock<IAuthManager>();
            authManager.Setup(x => x.ApiToken).Returns("token");
            authManager
                .Setup(x => x.HasContentPermissionsAsync(1, 2, MenuUtils.ContentPermissions.Edit))
                .ReturnsAsync(true);
            authManager
                .Setup(x => x.AddSiteLogAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var createManager = new Mock<ICreateManager>();
            createManager
                .Setup(x => x.CreateContentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);
            createManager
                .Setup(x => x.TriggerContentChangedEventAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            var accessTokenRepository = new Mock<IAccessTokenRepository>();
            accessTokenRepository
                .Setup(x => x.IsScopeAsync("token", Constants.ScopeContents))
                .ReturnsAsync(true);

            var siteRepository = new Mock<ISiteRepository>();
            siteRepository
                .Setup(x => x.GetAsync(1))
                .ReturnsAsync(new Site { Id = 1, CheckContentLevel = 0 });

            var channelRepository = new Mock<IChannelRepository>();
            channelRepository
                .Setup(x => x.GetAsync(2))
                .ReturnsAsync(new Channel { Id = 2, SiteId = 1 });
            channelRepository
                .Setup(x => x.GetChannelNameNavigationAsync(1, 2))
                .ReturnsAsync("channel");

            Content updatedContent = null;
            var contentRepository = new Mock<IContentRepository>();
            contentRepository
                .Setup(x => x.GetAsync(It.IsAny<Site>(), It.IsAny<Channel>(), 3))
                .ReturnsAsync(new Content
                {
                    Id = 3,
                    SiteId = 1,
                    ChannelId = 2,
                    Title = "draft",
                    Checked = false,
                    CheckedLevel = 0
                });
            contentRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Site>(), It.IsAny<Channel>(), It.IsAny<Content>()))
                .Callback<Site, Channel, Content>((_, _, updated) => updatedContent = updated)
                .Returns(Task.CompletedTask);

            var controller = new ContentsController(
                authManager.Object,
                createManager.Object,
                Mock.Of<IParseManager>(),
                Mock.Of<IDatabaseManager>(),
                Mock.Of<IPathManager>(),
                accessTokenRepository.Object,
                siteRepository.Object,
                channelRepository.Object,
                contentRepository.Object,
                Mock.Of<IContentCheckRepository>());

            await controller.Update(1, 2, 3, new Dictionary<string, object>
            {
                [nameof(Content.CheckedLevel)] = 0
            });

            Assert.NotNull(updatedContent);
            Assert.True(updatedContent.Checked);
            Assert.Equal(0, updatedContent.CheckedLevel);
            authManager.Verify(
                x => x.HasContentPermissionsAsync(1, 2, MenuUtils.ContentPermissions.CheckLevel1),
                Times.Never);
        }
    }
}
