using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using SSCMS.Core.Services;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using Xunit;

namespace SSCMS.Web.Tests.Services
{
    public class PathManagerUploadImageTests
    {
        [Fact]
        public async Task UploadImageRejectsNonImageContent()
        {
            var webRootPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(webRootPath);

            try
            {
                var settingsManager = new Mock<ISettingsManager>();
                settingsManager.Setup(x => x.WebRootPath).Returns(webRootPath);
                settingsManager.Setup(x => x.ContentRootPath).Returns(webRootPath);

                var pathManager = new PathManager(
                    Mock.Of<ICacheManager>(),
                    settingsManager.Object,
                    Mock.Of<IPluginManager>(),
                    Mock.Of<IDatabaseManager>(),
                    Mock.Of<ISpecialRepository>(),
                    Mock.Of<ITemplateLogRepository>(),
                    Mock.Of<ITemplateRepository>(),
                    Mock.Of<ISiteRepository>(),
                    Mock.Of<IChannelRepository>(),
                    Mock.Of<IContentRepository>(),
                    Mock.Of<ITableStyleRepository>());

                var bytes = Encoding.UTF8.GetBytes("not an image");
                await using var stream = new MemoryStream(bytes);
                var file = new FormFile(stream, 0, bytes.Length, "file", "payload.png");

                var (success, filePath, errorMessage) = await pathManager.UploadImageAsync(new Site
                {
                    Root = true,
                    ImageUploadExtensions = ".png",
                    ImageUploadTypeMaxSize = 1024
                }, file);

                Assert.False(success);
                Assert.Equal(string.Empty, filePath);
                Assert.NotEmpty(errorMessage);
                Assert.Empty(Directory.GetFiles(webRootPath, "*", SearchOption.AllDirectories));
            }
            finally
            {
                Directory.Delete(webRootPath, true);
            }
        }
    }
}
