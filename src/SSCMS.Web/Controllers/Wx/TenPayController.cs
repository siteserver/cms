using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Wx
{
    [OpenApiIgnore]
    [Route(Constants.ApiWxPrefix)]
    public partial class TenPayController : ControllerBase
    {
        public const string Route = "tenPay/{siteId}";
        public const string RouteAuthorize = "tenPay/{siteId}/authorize";

        private readonly IWxAccountRepository _openAccountRepository;
        private readonly IWxManager _openManager;

        public TenPayController(IWxAccountRepository openAccountRepository, IWxManager openManager)
        {
            _openAccountRepository = openAccountRepository;
            _openManager = openManager;
        }

        public class GetRequest
        {
            public int ProductId { get; set; }
            public string ReturnUrl { get; set; }
        }
    }
}
