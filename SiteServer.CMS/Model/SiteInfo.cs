using System;
using System.Xml.Serialization;
using SiteServer.Plugin;

namespace SiteServer.CMS.Model
{
	[Serializable]
	public class SiteInfo: ISiteInfo
	{
		private int _id;
		private string _siteName = string.Empty;
        private string _siteDir = string.Empty;
        private string _tableName = string.Empty;
        private bool _isRoot;
        private int _parentId;
        private int _taxis;
        private string _settingsXml = string.Empty;
        private SiteInfoExtend _additional;

        public SiteInfo()
		{
		}

        public SiteInfo(int id, string siteName, string siteDir, string tableName, bool isRoot, int parentId, int taxis, string settingsXml) 
		{
            _id = id;
            _siteName = siteName;
            _siteDir = siteDir;
            _tableName = tableName;
            _isRoot = isRoot;
            _parentId = parentId;
            _taxis = taxis;
            _settingsXml = settingsXml;
		}

		[XmlIgnore]
		public int Id
		{
			get{ return _id; }
			set{ _id = value; }
		}

        [XmlIgnore]
        public string SiteDir
        {
            get { return _siteDir; }
            set { _siteDir = value; }
        }

        [XmlIgnore]
		public string SiteName
		{
			get{ return _siteName; }
			set{ _siteName = value; }
		}

		[XmlIgnore]
		public string TableName
        {
			get{ return _tableName; }
			set{ _tableName = value; }
		}

		[XmlIgnore]
        public bool IsRoot
        {
			get{ return _isRoot; }
			set{ _isRoot = value; }
		}

        [XmlIgnore]
        public int ParentId
        {
            get { return _parentId; }
            set { _parentId = value; }
        }

        [XmlIgnore]
        public int Taxis
        {
            get { return _taxis; }
            set { _taxis = value; }
        }

        public string SettingsXml
        {
            get { return _settingsXml; }
            set
            {
                _additional = null;
                _settingsXml = value;
            }
        }

        public SiteInfoExtend Additional => _additional ?? (_additional = new SiteInfoExtend(SiteDir, _settingsXml));

	    public IAttributes Attributes => Additional;
	}
}
