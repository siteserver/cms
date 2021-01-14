using System;
using Xunit;
using Xunit.Abstractions;

namespace Datory.Tests.Utils
{
    public class TestStringUtils
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TestStringUtils(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void TestWriteEnvironment()
        {
            _testOutputHelper.WriteLine(Environment.MachineName);
        }

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
