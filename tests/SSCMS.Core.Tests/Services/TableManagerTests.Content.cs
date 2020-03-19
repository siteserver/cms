using System.Threading.Tasks;
using SSCMS.Tests;
using Xunit;

namespace SSCMS.Core.Tests.Services
{
    public partial class TableManagerTests
    {
        [SkippableFact]
        public async Task CreateContentTableAsyncTest()
        {
            Skip.IfNot(TestEnv.IsTestMachine);

            var tableName = "testContentTable";

            var database = _fixture.SettingsManager.Database;

            await database.CreateTableAsync(tableName, _contentRepository.GetTableColumns(tableName));

            Assert.True(await database.IsTableExistsAsync(tableName));

            await database.AlterTableAsync(tableName, _contentRepository.GetTableColumns(tableName));
        }
    }
}
