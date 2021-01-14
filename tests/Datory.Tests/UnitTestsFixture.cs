using System.IO;
using System.Threading.Tasks;
using Datory.Tests.Mocks;
using Datory.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Datory.Tests
{
    // https://mderriey.com/2017/09/04/async-lifetime-with-xunit/
    public class UnitTestsFixture : IAsyncLifetime
    {
        public IDatabase Database { get; }

        public UnitTestsFixture()
        {
            var contentRootPath = Directory.GetCurrentDirectory();

            var config = new ConfigurationBuilder()
                .SetBasePath(contentRootPath)
                .AddJsonFile("config.json")
                .Build();
            Database = new Database(Utilities.ToEnum(config["Database:Type"], DatabaseType.SQLite), config["Database:ConnectionString"]);
        }

        public async Task InitializeAsync()
        {
            var repository = new Repository<TestTableInfo>(Database);
            var isExists = await Database.IsTableExistsAsync(repository.TableName);
            if (!isExists)
            {
                await Database.CreateTableAsync(repository.TableName, repository.TableColumns);
            }
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
