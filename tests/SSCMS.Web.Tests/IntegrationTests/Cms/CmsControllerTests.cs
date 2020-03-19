using Microsoft.AspNetCore.Mvc.Testing;
using SSCMS.Web;
using Xunit;

namespace SSCMS.Tests.IntegrationTests.Cms
{
    public partial class CmsControllerTests
        : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public CmsControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }
    }
}
