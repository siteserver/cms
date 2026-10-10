using System.IO;
using SSCMS.Utils;
using Xunit;

namespace SSCMS.Tests
{
    public class TestAssetPathUtils
    {
        private static readonly string Root = Path.Combine(Path.GetTempPath(), "web");

        [Theory]
        [InlineData("css/site.css", true)]
        [InlineData("css/sub/a.css", true)]
        [InlineData("css/../../appsettings.json", false)]
        [InlineData("../appsettings.json", false)]
        public void IsInDirectory_NormalizesTraversal(string relative, bool expected)
        {
            var path = Root + Path.DirectorySeparatorChar + relative.Replace('/', Path.DirectorySeparatorChar);
            Assert.Equal(expected, DirectoryUtils.IsInDirectory(Root, path));
        }

        [Fact]
        public void IsInDirectory_RejectsSiblingPrefix()
        {
            Assert.False(DirectoryUtils.IsInDirectory(Root, Root + "2" + Path.DirectorySeparatorChar + "a.css"));
            Assert.True(DirectoryUtils.IsInDirectory(Root, Root));
        }

        [Theory]
        [InlineData("site.css", true)]
        [InlineData("../site.css", false)]
        [InlineData("....//site.css", false)]
        [InlineData("a/b.css", false)]
        [InlineData("a\\b.css", false)]
        [InlineData("c:site.css", false)]
        [InlineData("..", false)]
        [InlineData("", false)]
        public void IsSafeFileName(string fileName, bool expected)
        {
            Assert.Equal(expected, AssetPathUtils.IsSafeFileName(fileName));
        }

        [Theory]
        [InlineData("", true)]
        [InlineData("css", true)]
        [InlineData("css/sub", true)]
        [InlineData("../", false)]
        [InlineData("css/../..", false)]
        [InlineData("....//", false)] // 仅由点组成的段一律拒绝
        [InlineData("%2e%2e/", false)]
        [InlineData("/etc", false)]
        [InlineData("C:\\Windows", false)]
        [InlineData("css/...", false)]
        public void IsSafeRelativePath(string path, bool expected)
        {
            Assert.Equal(expected, AssetPathUtils.IsSafeRelativePath(path));
        }

        [Fact]
        public void IsUnder_RequiresStrictChild()
        {
            var cssRoot = Path.Combine(Root, "css");
            Assert.True(AssetPathUtils.IsUnder(cssRoot, Path.Combine(cssRoot, "a.css")));
            Assert.False(AssetPathUtils.IsUnder(cssRoot, cssRoot));
            Assert.False(AssetPathUtils.IsUnder(cssRoot, Path.Combine(Root, "404.html")));
            Assert.False(AssetPathUtils.IsUnder(cssRoot, Path.Combine(cssRoot, "..", "404.html")));
        }
    }
}
