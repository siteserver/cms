using System.Threading.Tasks;
using SS.CMS.Utils.Tests;
using Xunit;

namespace SS.CMS.Core.Tests.Services
{
    public partial class TableManagerTests
    {
        [SkippableFact]
        public async Task CreateContentTableAsyncTest()
        {
            Skip.IfNot(TestEnv.IsTestMachine);

            var tableName = "testContentTable";

            await _fixture.DatabaseRepository.CreateContentTableAsync(tableName, _fixture.DatabaseRepository.ContentTableDefaultColumns);

            Assert.True(await _fixture.Database.IsTableExistsAsync(tableName));

            await _fixture.DatabaseRepository.AlterSystemTableAsync(tableName, _fixture.DatabaseRepository.ContentTableDefaultColumns);
        }
    }
}
