using Microsoft.Extensions.DependencyInjection;
using SSCMS.Repositories;
using SSCMS.Services;
using Xunit;

namespace SSCMS.Core.Tests.Services
{
    [Collection("Database collection")]
    public partial class TableManagerTests
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IContentRepository _contentRepository;

        public TableManagerTests(IntegrationTestsFixture fixture)
        {
            _settingsManager = fixture.Provider.GetService<ISettingsManager>();
            _contentRepository = fixture.Provider.GetService<IContentRepository>();
        }
    }
}
