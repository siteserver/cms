using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SiteServer.API.Controllers
{
    public static class RequestExtensions
    {
        public const string ConstUnauthorized = "权限不足，访问被禁止";
        public const string ConstNotFound = "请求的资源不存在";

        public static T Unauthorized<T>(this HttpRequestMessage request) where T : class
        {
            throw new HttpResponseException(request.CreateErrorResponse(
                HttpStatusCode.Unauthorized,
                ConstUnauthorized
            ));
        }

        public static void Unauthorized(this HttpRequestMessage request)
        {
            throw new HttpResponseException(request.CreateErrorResponse(
                HttpStatusCode.Unauthorized,
                ConstUnauthorized
            ));
        }

        public static T NotFound<T>(this HttpRequestMessage request) where T : class
        {
            throw new HttpResponseException(request.CreateErrorResponse(
                HttpStatusCode.NotFound,
                ConstNotFound
            ));
        }

        public static void NotFound(this HttpRequestMessage request)
        {
            throw new HttpResponseException(request.CreateErrorResponse(
                HttpStatusCode.NotFound,
                ConstNotFound
            ));
        }

        public static T BadRequest<T>(this HttpRequestMessage request, string errorMessage) where T : class
        {
            throw new HttpResponseException(request.CreateErrorResponse(
                HttpStatusCode.BadRequest,
                errorMessage
            ));
        }

        public static void BadRequest(this HttpRequestMessage request, string errorMessage)
        {
            throw new HttpResponseException(request.CreateErrorResponse(
                HttpStatusCode.BadRequest,
                errorMessage
            ));
        }
    }
}
