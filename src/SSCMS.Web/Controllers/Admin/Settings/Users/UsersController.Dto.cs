using System.Collections.Generic;
using SSCMS;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersController
    {
        public class GetRequest
        {
            public bool? State { get; set; }
            public int GroupId { get; set; }
            public string Order { get; set; }
            public int LastActivityDate { get; set; }
            public string Keyword { get; set; }
            public int Offset { get; set; }
            public int Limit { get; set; }
        }

        public class GetResults
        {
            public List<User> Users { get; set; }
            public int Count { get; set; }
            public List<UserGroup> Groups { get; set; }
        }

        public class ImportResult
        {
            public bool Value { set; get; }
            public int Success { set; get; }
            public int Failure { set; get; }
            public string ErrorMessage { set; get; }
        }
    }
}
