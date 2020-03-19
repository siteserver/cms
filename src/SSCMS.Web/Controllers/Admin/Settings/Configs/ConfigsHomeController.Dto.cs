using System.Collections.Generic;
using SSCMS;

namespace SSCMS.Web.Controllers.Admin.Settings.Configs
{
    public partial class ConfigsHomeController
    {
        public class GetResult
        {
            public Config Config { get; set; }
            public string HomeDirectory { get; set; }
            public string AdminToken { get; set; }
            public List<TableStyle> Styles { get; set; }
        }

        public class SubmitRequest
        {
            public bool IsHomeClosed { get; set; }
            public string HomeTitle { get; set; }
            public bool IsHomeLogo { get; set; }
            public string HomeLogoUrl { get; set; }
            public string HomeDefaultAvatarUrl { get; set; }
            public List<string> UserRegistrationAttributes { get; set; }
            public bool IsUserRegistrationGroup { get; set; }
            public bool IsHomeAgreement { get; set; }
            public string HomeAgreementHtml { get; set; }
        }
    }
}
