using SS.CMS.Utils.Tests;
using Xunit;

namespace SS.CMS.Core.Tests.Services
{
    [Collection("Database collection")]
    public partial class TableManagerTests
    {
        private readonly IntegrationTestsFixture _fixture;

        public TableManagerTests(IntegrationTestsFixture fixture)
        {
            _fixture = fixture;
        }
    }
}
