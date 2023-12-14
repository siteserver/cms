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

        private readonly IWxAccountRepository _wxAccountRepository;
        private readonly IWxManager _wxManager;

        public TenPayController(IWxAccountRepository wxAccountRepository, IWxManager wxManager)
        {
            _wxAccountRepository = wxAccountRepository;
            _wxManager = wxManager;
        }

        public class GetRequest
        {
            public int ProductId { get; set; }
            public string ReturnUrl { get; set; }
        }
    }
}
