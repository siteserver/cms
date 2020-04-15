using System.Threading.Tasks;
using Datory;
using SSCMS.Models;
using Xunit;

namespace SSCMS.Core.Tests.Services
{
    public partial class TableManagerTests
    {
        [Fact]
        public async Task CreateContentTableAsyncTest()
        {
            const string tableName = "testContentTable";

            var database = _settingsManager.Database;

            await database.DropTableAsync(tableName);

            var contentRepository = new Repository<Content>(database, tableName);

            await database.CreateTableAsync(tableName, contentRepository.TableColumns);

            Assert.True(await database.IsTableExistsAsync(tableName));

            await database.AlterTableAsync(tableName, contentRepository.TableColumns);
        }
    }
}
