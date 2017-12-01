using System.Net;
using System.Net.Http;
using System.Text;
using BaiRong.Core;

namespace SiteServer.API.Core
{
    public class WebApiUtils
    {
        public static HttpResponseMessage CreateJsonResponse(object obj)
        {
            return new HttpResponseMessage
            {
                Content =
                    new StringContent(TranslateUtils.JsonSerialize(obj ?? new {}),
                        Encoding.GetEncoding("UTF-8"), "application/json"),
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}