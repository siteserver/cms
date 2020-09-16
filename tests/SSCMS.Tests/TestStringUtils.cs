using SSCMS.Utils;
using Xunit;

namespace SSCMS.Tests
{
    public class TestStringUtils
    {
        [Fact]
        public void TestReplaceEndsWith()
        {
            var replaced = StringUtils.ReplaceEndsWith("UserName DESC", " DESC", string.Empty);
            Assert.Equal("UserName", replaced);
        }

        [Fact]
        public void TestReplaceEndsWithIgnoreCase()
        {
            var replaced = StringUtils.ReplaceEndsWithIgnoreCase("UserName desc", " DESC", string.Empty);
            Assert.Equal("UserName", replaced);
        }
    }
}
