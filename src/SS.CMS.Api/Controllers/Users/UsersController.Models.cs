using System;
using System.Collections.Generic;
using System.Drawing;

namespace SS.CMS.Api.Controllers.Users
{
    public partial class UsersController
    {
        public class LoginRequest
        {
            public string UserName { get; set; }
            public string Password { get; set; }
            public string Captcha { get; set; }
            public bool IsAutoLogin { get; set; }
        }

        public class InfoResponse
        {
            public int Id { get; set; }
            public string DisplayName { get; set; }
            public string UserName { get; set; }
            public string AvatarUrl { get; set; }
            public string Bio { get; set; }
            public IList<string> Roles { get; set; }
            public IList<Menu> Menus { get; set; }
        }
    }
}
