using System.Collections.Generic;
using SSCMS.Configuration;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Home
{
    public partial class RegisterController
    {
        public class GetResult
        {
            public bool IsUserRegistrationGroup { get; set; }
            public bool IsHomeAgreement { get; set; }
            public string HomeAgreementHtml { get; set; }
            public IEnumerable<InputStyle> Styles { get; set; }
            public IEnumerable<UserGroup> Groups { get; set; }
        }

        public class CheckRequest
        {
            public string Token { get; set; }
            public string Value { get; set; }
        }
    }
}
