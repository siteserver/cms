using SSCMS.Cli.Core;
using Xunit;

namespace SSCMS.Cli.Tests.Services
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