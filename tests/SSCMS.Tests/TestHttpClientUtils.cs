using System;
using System.Threading.Tasks;
using SSCMS.Utils;
using Xunit;

namespace SSCMS.Tests
{
    public class TestHttpClientUtils
    {
        [Theory]
        [InlineData("http://127.0.0.1/admin")]
        [InlineData("http://10.0.0.1/admin")]
        [InlineData("http://172.16.0.1/admin")]
        [InlineData("http://192.168.1.1/admin")]
        [InlineData("http://169.254.169.254/latest/meta-data/")]
        [InlineData("http://[::1]/admin")]
        [InlineData("file:///etc/passwd")]
        public async Task TestValidatePublicHttpUrlRejectsUnsafeUrls(string url)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => HttpClientUtils.ValidatePublicHttpUrlAsync(url));
        }
    }
}
