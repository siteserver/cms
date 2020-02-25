using SS.CMS.Abstractions;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace SS.CMS.Tests.Services
{
    [Collection("Database collection")]
    public partial class TableManagerTests
    {
        private readonly IntegrationTestsFixture _fixture;
        private readonly IContentRepository _contentRepository;

        public TableManagerTests(IntegrationTestsFixture fixture)
        {
            _fixture = fixture;
            _contentRepository = _fixture.Provider.GetService<IContentRepository>();
        }
    }
}
