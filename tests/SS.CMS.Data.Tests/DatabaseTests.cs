using System.IO;
using SS.CMS.Core.Common.Office;
using SS.CMS.Utils;
using SS.CMS.Utils.Tests;
using Xunit;
using Xunit.Abstractions;

namespace SS.CMS.Data.Tests
{
    public class DatabaseTests : IClassFixture<EnvironmentFixture>
    {
        private EnvironmentFixture _fixture { get; }
        private readonly ITestOutputHelper _output;

        public DatabaseTests(EnvironmentFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

        [Fact]
        public void WordParseTest()
        {
            var projDirectoryPath = _fixture.SettingsManager.ContentRootPath;

            _output.WriteLine(projDirectoryPath);

            _output.WriteLine(_fixture.SettingsManager.DatabaseType.Value);
            _output.WriteLine(_fixture.SettingsManager.DatabaseConnectionString);

            var db = new Database(_fixture.SettingsManager.DatabaseType,
                _fixture.SettingsManager.DatabaseConnectionString);

            _output.WriteLine(db.DatabaseType.Value);
            _output.WriteLine(db.ConnectionString);

            Assert.NotEqual(_fixture.SettingsManager.DatabaseConnectionString, db.ConnectionString);
        }
    }
}