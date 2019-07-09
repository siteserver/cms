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

            await _fixture.TableManager.CreateContentTableAsync(tableName, _fixture.TableManager.ContentTableDefaultColumns);

            Assert.True(await _fixture.Database.IsTableExistsAsync(tableName));

            await _fixture.TableManager.AlterSystemTableAsync(tableName, _fixture.TableManager.ContentTableDefaultColumns);
        }
    }
}
