using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.CMS.Api.V1;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.V1
{
    public partial class AdministratorsController
    {
        public class LoginRequest
        {
            /// <summary>
            /// 账号
            /// </summary>
            public string Account { get; set; }

            /// <summary>
            /// 密码
            /// </summary>
            public string Password { get; set; }

            /// <summary>
            /// 下次自动登录
            /// </summary>
            public bool IsAutoLogin { get; set; }
        }

        public class LoginResult
        {
            public Administrator Value { get; set; }
            public string AccessToken { get; set; }
            public DateTime? ExpiresAt { get; set; }
            public string SessionId { get; set; }
            public bool IsEnforcePasswordChange { get; set; }
        }
    }
}
