using System.Collections.Generic;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Home
{
    public partial class RegisterController
    {
        public class GetResult
        {
            public Config Config { get; set; }
            public List<TableStyle> Styles { get; set; }
            public List<UserGroup> Groups { get; set; }
        }
    }
}
