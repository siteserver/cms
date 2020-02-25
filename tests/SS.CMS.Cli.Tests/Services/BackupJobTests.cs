using SS.CMS.Cli.Services;
using Xunit;

namespace SS.CMS.Cli.Tests.Services
{
    public class BackupJobTests
    {
        [Fact]
        public void TestReplaceEndsWith()
        {
            Assert.Equal("backup", BackupJob.CommandName);
        }
    }
}