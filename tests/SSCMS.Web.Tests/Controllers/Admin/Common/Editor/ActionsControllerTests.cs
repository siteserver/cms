using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Web.Controllers.Admin.Common.Editor;
using Xunit;

namespace SSCMS.Web.Tests.Controllers.Admin.Common.Editor
{
    public class ActionsControllerTests
    {
        [Fact]
        public async Task UploadScrawlRequiresAccessToRequestedSite()
        {
            var authManager = new Mock<IAuthManager>();
            authManager
                .Setup(x => x.HasSitePermissionsAsync(2))
                .ReturnsAsync(false);

            var controller = new ActionsController(
                Mock.Of<IPathManager>(),
                Mock.Of<IStorageManager>(),
                Mock.Of<IVodManager>(),
                Mock.Of<ISiteRepository>(),
                authManager.Object);

            var result = await controller.UploadScrawl(2, new ActionsController.UploadScrawlRequest
            {
                File = "not-base64"
            });

            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task UploadScrawlRejectsNonImageBytes()
        {
            var site = new Site
            {
                Id = 1
            };

            var siteRepository = new Mock<ISiteRepository>();
            siteRepository.Setup(x => x.GetAsync(1)).ReturnsAsync(site);

            var pathManager = new Mock<IPathManager>();
            pathManager.Setup(x => x.GetUploadFileName(site, "scrawl.png")).Returns("scrawl.png");
            pathManager.Setup(x => x.IsImageExtensionAllowed(site, ".png")).Returns(true);
            pathManager.Setup(x => x.IsImageSizeAllowed(site, It.IsAny<long>())).Returns(true);
            pathManager.Setup(x => x.GetUploadDirectoryPathAsync(site, UploadType.Image)).ReturnsAsync("/tmp");

            var authManager = new Mock<IAuthManager>();
            authManager
                .Setup(x => x.HasSitePermissionsAsync(1))
                .ReturnsAsync(true);

            var controller = new ActionsController(
                pathManager.Object,
                Mock.Of<IStorageManager>(),
                Mock.Of<IVodManager>(),
                siteRepository.Object,
                authManager.Object);

            var result = await controller.UploadScrawl(1, new ActionsController.UploadScrawlRequest
            {
                File = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("<script>alert(1)</script>"))
            });

            Assert.Equal(Constants.ErrorUpload, result.Value.Error);
            pathManager.Verify(x => x.UploadAsync(It.IsAny<byte[]>(), It.IsAny<string>()), Times.Never);
        }
    }
}
