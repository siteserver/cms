using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AccountController : ControllerBase
    {
        private const string Route = "wx/account";
        private const string RouteMp = "wx/account/mp";
        private const string RouteTenPay = "wx/account/tenPay";

        private readonly IAuthManager _authManager;
        private readonly IWxManager _wxManager;
        private readonly IWxAccountRepository _wxAccountRepository;

        public AccountController(IAuthManager authManager, IWxManager wxManager, IWxAccountRepository wxAccountRepository)
        {
            _authManager = authManager;
            _wxManager = wxManager;
            _wxAccountRepository = wxAccountRepository;
        }

        public class GetResult
        {
            public string MpUrl { get; set; }
            public string DefaultTenPayAuthorizeUrl { get; set; }
            public string DefaultTenPayNotifyUrl { get; set; }
            public WxAccount Account { get; set; }
        }

        public class MpSubmitRequest
        {
            public int SiteId { get; set; }
            public string MpAppId { get; set; }
            public string MpAppSecret { get; set; }
        }

        public class MpSubmitResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public WxAccount Account { get; set; }
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
