using System.Collections.Generic;
using SSCMS.Configuration;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Home
{
    public partial class IndexController
    {
        public class GetResult
        {
            public User User { get; set; }
            public string HomeTitle { get; set; }
            public string HomeLogoUrl { get; set; }
            public List<Menu> Menus { get; set; }
            public string DefaultPageUrl { get; set; }
        }
    }
}
