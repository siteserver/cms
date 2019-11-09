using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Plugin;
using SiteServer.Utils;

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

        public bool IsSeparatedWeb => Additional.IsSeparatedWeb;

        public string SeparatedWebUrl => Additional.SeparatedWebUrl;

        public string WebUrl => Additional.IsSeparatedWeb ? Additional.SeparatedWebUrl : PageUtils.ParseNavigationUrl($"~/{ SiteDir}");

        public string AssetsDir => Additional.AssetsDir;

        public bool IsSeparatedAssets => Additional.IsSeparatedAssets;

        public string SeparatedAssetsUrl => Additional.SeparatedAssetsUrl;

        public string AssetsUrl => Additional.IsSeparatedAssets ? Additional.SeparatedAssetsUrl : PageUtils.Combine(WebUrl, Additional.AssetsDir);

        public IList<Site> Children { get; set; }

        [JsonIgnore]
        public SiteInfoExtend Additional { get; set; }
    }
}
