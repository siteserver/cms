using System.Linq;
using Microsoft.AspNetCore.Authorization;
using SSCMS.Configuration;
using SSCMS.Web.Controllers.Preview;
using Xunit;

namespace SSCMS.Web.Tests.Controllers
{
    public class PreviewControllerAuthorizationTests
    {
        [Fact]
        public void PreviewControllerRequiresAuthenticatedAdminOrUser()
        {
            var attributes = typeof(PreviewController)
                .GetCustomAttributes(typeof(AuthorizeAttribute), true)
                .Cast<AuthorizeAttribute>();

            Assert.Contains(attributes, attribute =>
                attribute.Roles == $"{Types.Roles.Administrator},{Types.Roles.User}");
        }
    }
}
