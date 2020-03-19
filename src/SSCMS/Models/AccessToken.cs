using System.Collections.Generic;
using Datory;
using Datory.Annotations;

namespace SSCMS
{
    [DataTable("siteserver_AccessToken")]
    public class AccessToken : Entity
    {
        [DataColumn]
        public string Title { get; set; }

        [DataColumn]
        public string Token { get; set; }

        [DataColumn]
        public string AdminName { get; set; }

        [DataColumn]
        public List<string> Scopes { get; set; }
    }
}
