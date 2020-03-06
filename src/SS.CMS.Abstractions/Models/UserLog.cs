using Datory;
using Datory.Annotations;

namespace SS.CMS.Abstractions
{
    [DataTable("siteserver_UserLog")]
    public class UserLog : Entity
    {
        [DataColumn]
        public int UserId { get; set; }

        [DataColumn]
        public string IpAddress { get; set; }

        [DataColumn]
        public string Action { get; set; }

        [DataColumn]
        public string Summary { get; set; }
	}
}
