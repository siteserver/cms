using System;
using System.Collections.Generic;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.V1
{
    public partial class UsersController
    {
        public class ListRequest
        {
            public int Top { get; set; }
            public int Skip { get; set; }
        }

        public class ListResult
        {
            public int Count { get; set; }
            public List<User> Users { get; set; }
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
            public bool IsPersistent { get; set; }
        }

        public class LoginResult
        {
            public User User { get; set; }
            public string AccessToken { get; set; }
        }

        public class GetLogsRequest
        {
            public int Top { get; set; }
            public int Skip { get; set; }
        }

        public class GetLogsResult
        {
            public int Count { get; set; }
            public List<Log> Logs { get; set; }
        }

        public class ResetPasswordRequest
        {
            public string Password { get; set; }
            public string NewPassword { get; set; }
        }
    }
}
