using System.Collections.Generic;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersStyleController
    {
        public class GetResult
        {
            public List<InputStyle> Styles { get; set; }
            public string TableName { get; set; }
            public List<int> RelatedIdentities { get; set; }
        }

        public class DeleteRequest
        {
            public string AttributeName { get; set; }
        }

        public class DeleteResult
        {
            public List<InputStyle> Styles { get; set; }
        }

        public class ResetResult
        {
            public List<InputStyle> Styles { get; set; }
        }
    }
}
