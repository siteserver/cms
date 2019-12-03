using System;
using Datory;


namespace SiteServer.Abstractions
{
    [Serializable]
    [DataTable("siteserver_AdministratorsInRoles")]
    public class AdministratorsInRoles : Entity
    {
        [DataColumn]
        public string RoleName { get; set; }

        [DataColumn]
        public string UserName { get; set; }
	}
}
