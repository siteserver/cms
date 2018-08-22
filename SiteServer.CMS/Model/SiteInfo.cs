using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Plugin;

namespace SiteServer.CMS.Model
{
	[Serializable]
	public class SiteInfo: ISiteInfo
	{
        private string _settingsXml = string.Empty;
        private SiteInfoExtend _additional;

        public SiteInfo()
		{
		}

        public SiteInfo(int id, string siteName, string siteDir, string tableName, bool isRoot, int parentId, int taxis, string settingsXml) 
		{
            Id = id;
            SiteName = siteName;
            SiteDir = siteDir;
            TableName = tableName;
            IsRoot = isRoot;
            ParentId = parentId;
            Taxis = taxis;
            _settingsXml = settingsXml;
		}

		[XmlIgnore]
		public int Id { get; set; }

        [XmlIgnore]
        public string SiteDir { get; set; }

        [XmlIgnore]
		public string SiteName { get; set; }

        [XmlIgnore]
		public string TableName { get; set; }

        [XmlIgnore]
        public bool IsRoot { get; set; }

        [XmlIgnore]
        public int ParentId { get; set; }

        [XmlIgnore]
        public int Taxis { get; set; }

        [JsonIgnore]
        public string SettingsXml
        {
            get => _settingsXml;
	        set
            {
                _additional = null;
                _settingsXml = value;
            }
        }

	    [JsonIgnore]
        public SiteInfoExtend Additional => _additional ?? (_additional = new SiteInfoExtend(SiteDir, _settingsXml));

	    public IAttributes Attributes => Additional;
	}
}
