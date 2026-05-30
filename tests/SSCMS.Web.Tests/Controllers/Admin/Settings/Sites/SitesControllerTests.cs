using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Web.Controllers.Admin.Settings.Sites;
using Xunit;

namespace SSCMS.Web.Tests.Controllers.Admin.Settings.Sites
{
    public class SitesControllerTests
    {
        [Fact]
        public async Task EditRequiresAccessToRequestedSite()
        {
            var authManager = new Mock<IAuthManager>();
            authManager
                .Setup(x => x.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSites))
                .ReturnsAsync(true);
            authManager
                .Setup(x => x.IsSuperAdminAsync())
                .ReturnsAsync(false);
            authManager
                .Setup(x => x.HasSitePermissionsAsync(2))
                .ReturnsAsync(false);

            var settingsManager = new Mock<ISettingsManager>();
            settingsManager.Setup(x => x.IsSafeMode).Returns(false);
            settingsManager
                .Setup(x => x.GetSiteType(Types.SiteTypes.Web))
                .Returns(new SiteType { Id = Types.SiteTypes.Web });

            var siteRepository = new Mock<ISiteRepository>();
            siteRepository
                .Setup(x => x.GetAsync(2))
                .ReturnsAsync(new Site
                {
                    Id = 2,
                    Root = true,
                    SiteName = "Target",
                    SiteType = Types.SiteTypes.Web,
                    TableName = "cms_Content"
                });
            siteRepository
                .Setup(x => x.GetSiteIdsAsync(0))
                .ReturnsAsync(new List<int>());

            var controller = CreateController(settingsManager.Object, authManager.Object, siteRepository.Object);

            var result = await controller.Edit(new SitesController.EditRequest
            {
                SiteId = 2,
                SiteDir = "target",
                SiteName = "Target",
                SiteType = Types.SiteTypes.Web,
                ParentId = 0,
                TableRule = TableRule.Choose,
                TableChoose = "cms_Content"
            });

            Assert.IsType<UnauthorizedResult>(result.Result);
            siteRepository.Verify(x => x.UpdateAsync(It.IsAny<Site>()), Times.Never);
        }

        [Fact]
        public async Task DeleteRequiresAccessToRequestedSite()
        {
            var authManager = new Mock<IAuthManager>();
            authManager
                .Setup(x => x.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSites))
                .ReturnsAsync(true);
            authManager
                .Setup(x => x.IsSuperAdminAsync())
                .ReturnsAsync(false);
            authManager
                .Setup(x => x.HasSitePermissionsAsync(2))
                .ReturnsAsync(false);

            var settingsManager = new Mock<ISettingsManager>();
            settingsManager.Setup(x => x.IsSafeMode).Returns(false);

            var siteRepository = new Mock<ISiteRepository>();
            siteRepository
                .Setup(x => x.GetAsync(2))
                .ReturnsAsync(new Site
                {
                    Id = 2,
                    SiteDir = "target",
                    SiteName = "Target",
                    TableName = "cms_Content",
                    Children = new List<Site>()
                });
            siteRepository
                .Setup(x => x.GetSiteIdsAsync(0))
                .ReturnsAsync(new List<int>());

            var channelRepository = new Mock<IChannelRepository>();
            channelRepository
                .Setup(x => x.GetChannelIdsAsync(2))
                .ReturnsAsync(new List<int>());

            var controller = CreateController(
                settingsManager.Object,
                authManager.Object,
                siteRepository.Object,
                channelRepository.Object);

            var result = await controller.Delete(new SitesController.DeleteRequest
            {
                SiteId = 2,
                SiteDir = "target",
                DeleteFiles = false
            });

            Assert.IsType<UnauthorizedResult>(result.Result);
            siteRepository.Verify(x => x.DeleteAsync(2), Times.Never);
        }

        private static SitesController CreateController(
            ISettingsManager settingsManager,
            IAuthManager authManager,
            ISiteRepository siteRepository,
            IChannelRepository channelRepository = null)
        {
            return new SitesController(
                settingsManager,
                authManager,
                Mock.Of<IPathManager>(),
                siteRepository,
                channelRepository ?? Mock.Of<IChannelRepository>(),
                Mock.Of<IContentRepository>(),
                Mock.Of<ITableStyleRepository>(),
                Mock.Of<IChannelGroupRepository>(),
                Mock.Of<IContentGroupRepository>(),
                Mock.Of<IContentTagRepository>(),
                Mock.Of<IContentCheckRepository>(),
                Mock.Of<IFormRepository>(),
                Mock.Of<IFormDataRepository>(),
                Mock.Of<IRelatedFieldRepository>(),
                Mock.Of<IRelatedFieldItemRepository>(),
                Mock.Of<ISitePermissionsRepository>(),
                Mock.Of<ISpecialRepository>(),
                Mock.Of<IStatRepository>(),
                Mock.Of<ITemplateLogRepository>(),
                Mock.Of<ITemplateRepository>(),
                Mock.Of<ITranslateRepository>(),
                Mock.Of<IWxAccountRepository>(),
                Mock.Of<IWxChatRepository>(),
                Mock.Of<IWxMenuRepository>(),
                Mock.Of<IWxReplyKeywordRepository>(),
                Mock.Of<IWxReplyMessageRepository>(),
                Mock.Of<IWxReplyRuleRepository>());
        }
    }
}
