using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace SS.CMS.Api.Tests.IntegrationTests.Cms
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
