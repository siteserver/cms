using System;
using System.Threading.Tasks;
using Datory.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Datory.Tests
{
    public class DatabaseTests : IClassFixture<UnitTestsFixture>
    {
        private UnitTestsFixture _fixture { get; }
        private readonly ITestOutputHelper _output;

        public DatabaseTests(UnitTestsFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;

            _output.WriteLine(Environment.MachineName);
        }

        public async Task IsConnectionWorksAsync()
        {
            var (isConnectionWorks, errorMessage) = await _fixture.Database.IsConnectionWorksAsync();

            Assert.True(isConnectionWorks);

            _output.WriteLine(_fixture.Database.ConnectionString);
        }

    }
}