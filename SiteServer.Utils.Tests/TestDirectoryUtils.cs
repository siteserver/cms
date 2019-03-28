using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace SiteServer.Utils.Tests
{
    public class TestDirectoryUtils
    {
        [Fact]
        public void TestGetParentPath()
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);

            dirPath = DirectoryUtils.GetParentPath(dirPath);
            Assert.Equal("Bin", PathUtils.GetDirectoryName(dirPath, false), StringComparer.OrdinalIgnoreCase);

            dirPath = DirectoryUtils.GetParentPath(dirPath);
            Assert.Equal("SiteServer.Utils.Tests", PathUtils.GetDirectoryName(dirPath, false), StringComparer.OrdinalIgnoreCase);
        }
    }
}
