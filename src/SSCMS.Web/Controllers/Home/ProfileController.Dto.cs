using System.Collections.Generic;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Home
{
    public partial class ProfileController
    {
        public class GetResult
        {
            public string UserName { get; set; }
            public string DisplayName { get; set; }
            public string AvatarUrl { get; set; }
            public string Mobile { get; set; }
            public string Email { get; set; }
            public Config Config { get; set; }
            public List<TableStyle> Styles { get; set; }
        }

        public class SubmitRequest
        {
            public string DisplayName { get; set; }
            public string AvatarUrl { get; set; }
            public string Mobile { get; set; }
            public string Email { get; set; }
        }
    }
}
