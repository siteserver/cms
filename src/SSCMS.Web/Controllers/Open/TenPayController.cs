using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Open
{
    [OpenApiIgnore]
    [Route(Constants.ApiOpenPrefix)]
    public partial class TenPayController : ControllerBase
    {
        public const string Route = "tenPay/{siteId}";
        public const string RouteAuthorize = "tenPay/{siteId}/authorize";

        private readonly IOpenAccountRepository _openAccountRepository;
        private readonly IOpenManager _openManager;

        public TenPayController(IOpenAccountRepository openAccountRepository, IOpenManager openManager)
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
