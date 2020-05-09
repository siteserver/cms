using System.Collections.Generic;
using SSCMS.Configuration;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Home
{
    public partial class ProfileController
    {
        public class GetResult
        {
            public User User { get; set; }
            public IEnumerable<InputStyle> Styles { get; set; }
        }
    }
}
