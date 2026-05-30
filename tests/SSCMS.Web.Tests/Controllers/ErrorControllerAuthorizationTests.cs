using System.Linq;
using Microsoft.AspNetCore.Authorization;
using SSCMS.Configuration;
using Xunit;

namespace SSCMS.Web.Tests.Controllers
{
    public class ErrorControllerAuthorizationTests
    {
        [Fact]
        public void AdminErrorControllerRequiresAdministratorRole()
        {
            var attributes = typeof(Web.Controllers.Admin.ErrorController)
                .GetCustomAttributes(typeof(AuthorizeAttribute), true)
                .Cast<AuthorizeAttribute>();

            Assert.Contains(attributes, attribute => attribute.Roles == Types.Roles.Administrator);
        }

        [Fact]
        public void HomeErrorControllerRequiresUserRole()
        {
            var attributes = typeof(Web.Controllers.Home.ErrorController)
                .GetCustomAttributes(typeof(AuthorizeAttribute), true)
                .Cast<AuthorizeAttribute>();

            Assert.Contains(attributes, attribute => attribute.Roles == Types.Roles.User);
        }
    }
}
