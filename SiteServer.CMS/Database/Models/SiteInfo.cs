using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using SiteServer.CMS.Database.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Extends;
using SiteServer.CMS.Database.Repositories.Contents;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_Site")]
    public class SiteInfo: IDataInfo, ISiteInfo
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string SiteDir { get; set; }

		public string SiteName { get; set; }

		public string TableName { get; set; }

        private string IsRoot { get; set; }

        [Computed]
        public bool Root
        {
            get => TranslateUtils.ToBool(IsRoot);
            set => IsRoot = value.ToString();
        }

        public int ParentId { get; set; }

        public int Taxis { get; set; }

        [Text]
        private string SettingsXml { get; set; }

        private SiteInfoExtend _extend;

        [JsonIgnore]
        [Computed]
        public IAttributes Attributes => Extend;

        [JsonIgnore]
        [Computed]
        public SiteInfoExtend Extend => _extend ?? (_extend = new SiteInfoExtend(this));

        public string GetSettingsXml()
        {
            return SettingsXml;
        }

        public void SetSettingsXml(string json)
        {
            SettingsXml = json;
            _extend = null;
        }
    }
}
