using SS.CMS.Cli.Core;
using SS.CMS.Cli.Services;
using Xunit;

namespace SS.CMS.Cli.Tests.Services
{
    public class BackupJobTests
    {
        [Fact]
        public void TestReplaceEndsWith()
        {
            var backup = CliUtils.GetJobService("backup");
            Assert.Equal("backup", backup.CommandName);
        }
    }
}