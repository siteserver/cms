using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace SSCMS.Web.Controllers
{
    public partial class PingController
    {
        [HttpGet, Route(RouteIp)]
        public string Ip()
        {
            var client = new RestClient("https://api.open.21ds.cn/apiv1/iptest?apkey=iptest") { Timeout = -1 };
            var request = new RestRequest(Method.GET);

            var response = client.Execute(request);
            return response.Content;
        }
    }
}