using System.IO;
using System.Threading.Tasks;
using Moq;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Web.Controllers.Admin.Cms.Forms;
using Xunit;

namespace SSCMS.Web.Tests.Controllers.Admin.Cms.Forms
{
    public class FormDataAddControllerTests
    {
        [Fact]
        public async Task DeleteFileRejectsPathTraversalOutsideContentRoot()
        {
            var tempRoot = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var contentRoot = Path.Combine(tempRoot, "content");
            Directory.CreateDirectory(contentRoot);
            var outsideFilePath = Path.Combine(tempRoot, "outside.txt");
            await File.WriteAllTextAsync(outsideFilePath, "do not delete");

            try
            {
                var authManager = new Mock<IAuthManager>();
                authManager
                    .Setup(x => x.HasSitePermissionsAsync(1, It.IsAny<string>()))
                    .ReturnsAsync(true);

                var pathManager = new Mock<IPathManager>();
                pathManager.Setup(x => x.ContentRootPath).Returns(contentRoot);

                var controller = new FormDataAddController(
                    authManager.Object,
                    pathManager.Object,
                    Mock.Of<IFormManager>(),
                    Mock.Of<ISiteRepository>(),
                    Mock.Of<IFormRepository>(),
                    Mock.Of<IFormDataRepository>());

                await controller.DeleteFile(new FormDataAddController.DeleteRequest
                {
                    SiteId = 1,
                    FormId = 1,
                    FileUrl = "../outside.txt"
                });

                Assert.True(File.Exists(outsideFilePath));
            }
            finally
            {
                if (Directory.Exists(tempRoot))
                {
                    Directory.Delete(tempRoot, true);
                }
            }
        }
    }
}
