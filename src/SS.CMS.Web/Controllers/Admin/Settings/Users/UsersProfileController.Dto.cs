using Microsoft.AspNetCore.Http;

namespace SS.CMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersProfileController
    {
        public class GetResult
        {
            public string UserName { get; set; }
            public string DisplayName { get; set; }
            public string AvatarUrl { get; set; }
            public string Mobile { get; set; }
            public string Email { get; set; }
        }

        public class UploadRequest
        {
            public int UserId { get; set; }
            public IFormFile File { set; get; }
        }

        public class SubmitRequest
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public string DisplayName { get; set; }
            public string AvatarUrl { get; set; }
            public string Mobile { get; set; }
            public string Email { get; set; }
        }
    }
}
