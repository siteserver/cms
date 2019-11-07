using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Plugin;

namespace SiteServer.CMS.Model
{
    public class Site: ISiteInfo
	{
        public int Id { get; set; }

        public string SiteDir { get; set; }

		public string SiteName { get; set; }

		public string TableName { get; set; }

	    public bool Root { get; set; }

        public int ParentId { get; set; }

        public int Taxis { get; set; }

        public string Url => Additional.WebUrl;

        public IList<Site> Children { get; set; }

        [JsonIgnore]
        public SiteInfoExtend Additional { get; set; }
    }
}
