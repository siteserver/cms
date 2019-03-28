using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace SiteServer.API.Tests.Utils
{
    public static class ControllerUtils
    {
        public static T NewAnonymousController<T>() where T : ApiController, new()
        {
            var controller = new T
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
            return controller;
        }

        public static T NewAdminController<T>(string accessToken) where T : ApiController, new()
        {
            var controller = new T();
            var controllerContext = new HttpControllerContext();
            var request = new HttpRequestMessage();
            request.Headers.Add(Rest.AuthKeyAdminHeader, accessToken);

            // Don't forget these lines, if you do then the request will be null.
            controllerContext.Request = request;
            controller.ControllerContext = controllerContext;

            return controller;
        }
    }
}
