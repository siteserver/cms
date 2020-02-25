using System.Threading.Tasks;
using SS.CMS.Abstractions.Tests;
using Xunit;

namespace SS.CMS.Tests.Services
{
    public partial class TableManagerTests
    {
        [SkippableFact]
        public async Task CreateContentTableAsyncTest()
        {
            Skip.IfNot(TestEnv.IsTestMachine);

            var tableName = "testContentTable";

            var database = _fixture.SettingsManager.Database;

            await database.CreateTableAsync(tableName, _contentRepository.GetDefaultTableColumns(tableName));

            Assert.True(await database.IsTableExistsAsync(tableName));

            await database.AlterTableAsync(tableName, _contentRepository.GetDefaultTableColumns(tableName));
        }
    }
}
