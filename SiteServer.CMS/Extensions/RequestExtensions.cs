using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Extensions
{
    public static class RequestExtensions
    {
        public static T Unauthorized<T>(this HttpRequestMessage request) where T : class
        {
            throw new HttpResponseException(request.CreateErrorResponse(
                HttpStatusCode.Unauthorized,
                Constants.Unauthorized
            ));
        }

        public static void Unauthorized(this HttpRequestMessage request)
        {
            throw new HttpResponseException(request.CreateErrorResponse(
                HttpStatusCode.Unauthorized,
                Constants.Unauthorized
            ));
        }

        public static T NotFound<T>(this HttpRequestMessage request) where T : class
        {
            throw new HttpResponseException(request.CreateErrorResponse(
                HttpStatusCode.NotFound,
                Constants.NotFound
            ));
        }

        public static void NotFound(this HttpRequestMessage request)
        {
            throw new HttpResponseException(request.CreateErrorResponse(
                HttpStatusCode.NotFound,
                Constants.NotFound
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
