using Microsoft.Extensions.DependencyInjection;
using SSCMS;
using Xunit;

namespace SSCMS.Core.Tests.Services
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
