using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using SSCMS.Configuration;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersProfileController
    {
        public class GetResult
        {
            public User User { get; set; }
            public IEnumerable<InputStyle> Styles { get; set; }
        }

        public class UploadRequest
        {
            public int UserId { get; set; }
            public IFormFile File { set; get; }
        }
    }
}
