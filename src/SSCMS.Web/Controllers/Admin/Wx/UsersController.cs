using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;
using SSCMS.Wx;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UsersController : ControllerBase
    {
        private const string Route = "wx/users";
        private const string RouteWx = "wx/users/wx";
        private const string RouteTenPay = "wx/users/tenPay";

        private readonly IAuthManager _authManager;
        private readonly IWxManager _wxManager;
        private readonly IWxAccountRepository _wxAccountRepository;

        public UsersController(IAuthManager authManager, IWxManager wxManager, IWxAccountRepository wxAccountRepository)
        {
            _authManager = authManager;
            _wxManager = wxManager;
            _wxAccountRepository = wxAccountRepository;
        }

        public class GetResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public IEnumerable<WxUserTag> Tags { get; set; }
            public IEnumerable<WxUser> Users { get; set; }
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
