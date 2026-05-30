using System;
using System.IO;
using System.Threading.Tasks;
using SSCMS.Utils;
using Xunit;

namespace SSCMS.Web.Tests.Security
{
    public class RestUtilsTests
    {
        [Theory]
        [InlineData("file:///etc/passwd")]
        [InlineData("http://127.0.0.1/admin")]
        [InlineData("http://169.254.169.254/latest/meta-data/")]
        public async Task DownloadRejectsUnsafeUrls(string url)
        {
            var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.tmp");

            var ex = await Assert.ThrowsAsync<Exception>(() => RestUtils.DownloadAsync(url, filePath));

            Assert.True(
                ex.Message.Contains("HTTP", StringComparison.OrdinalIgnoreCase) ||
                ex.Message.Contains("Private", StringComparison.OrdinalIgnoreCase),
                ex.Message);
            Assert.False(File.Exists(filePath));
        }
    }
}
