using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Open
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AccountController : ControllerBase
    {
        private const string Route = "cms/open/account";
        private const string RouteWx = "cms/open/account/wx";
        private const string RouteTenPay = "cms/open/account/tenPay";

        private readonly IAuthManager _authManager;
        private readonly IOpenAccountRepository _openAccountRepository;

        public AccountController(IAuthManager authManager, IOpenAccountRepository openAccountRepository)
        {
            _authManager = authManager;
            _openAccountRepository = openAccountRepository;
        }

        public class GetResult
        {
            public string DefaultWxUrl { get; set; }
            public string DefaultTenPayAuthorizeUrl { get; set; }
            public string DefaultTenPayNotifyUrl { get; set; }
            public OpenAccount Account { get; set; }
        }

        public class WxSubmitRequest
        {
            public int SiteId { get; set; }
            public string WxAppId { get; set; }
            public string WxAppSecret { get; set; }
            public string WxUrl { get; set; }
            public string WxToken { get; set; }
            public bool WxIsEncrypt { get; set; }
            public string WxEncodingAESKey { get; set; }
        }

        public class TenPaySubmitRequest
        {
            public int SiteId { get; set; }
            public string TenPayAppId { get; set; }
            public string TenPayAppSecret { get; set; }
            public string TenPayMchId { get; set; }
            public string TenPayKey { get; set; }
            public string TenPayAuthorizeUrl { get; set; }
            public string TenPayNotifyUrl { get; set; }
        }
    }
}
