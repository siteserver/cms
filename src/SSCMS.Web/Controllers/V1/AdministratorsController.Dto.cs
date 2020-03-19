using System;
using System.Collections.Generic;
using SSCMS;

namespace SSCMS.Web.Controllers.V1
{
    public partial class AdministratorsController
    {
        public class ListRequest
        {
            public int Top { get; set; }
            public int Skip { get; set; }
        }

        public class ListResult
        {
            public int Count { get; set; }
            public List<Administrator> Administrators { get; set; }
        }

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
            public Administrator Administrator { get; set; }
            public string AccessToken { get; set; }
            public DateTime? ExpiresAt { get; set; }
            public string SessionId { get; set; }
            public bool IsEnforcePasswordChange { get; set; }
        }

        public class ResetPasswordRequest
        {
            public string Account { get; set; }
            public string Password { get; set; }
            public string NewPassword { get; set; }
        }
    }
}
