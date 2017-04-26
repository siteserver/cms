using System;
using System.Xml.Serialization;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.CMS.Model
{
    public class PublishmentSystemAttribute
    {
        protected PublishmentSystemAttribute()
        {
        }

        public const string PublishmentSystemId = "PublishmentSystemId";
        public const string PublishmentSystemName = "PublishmentSystemName";
        public const string PublishmentSystemType = "PublishmentSystemType";
        public const string AuxiliaryTableForContent = "AuxiliaryTableForContent";
        public const string AuxiliaryTableForGovPublic = "AuxiliaryTableForGovPublic";
        public const string AuxiliaryTableForGovInteract = "AuxiliaryTableForGovInteract";
        public const string AuxiliaryTableForJob = "AuxiliaryTableForJob";
        public const string AuxiliaryTableForVote = "AuxiliaryTableForVote";
        public const string IsCheckContentUseLevel = "IsCheckContentUseLevel";
        public const string CheckContentLevel = "CheckContentLevel";
        public const string PublishmentSystemDir = "PublishmentSystemDir";
        public const string PublishmentSystemUrl = "PublishmentSystemUrl";
        public const string IsHeadquarters = "IsHeadquarters";
        public const string ParentPublishmentSystemId = "ParentPublishmentSystemId";
        public const string Taxis = "Taxis";
        public const string SettingsXml = "SettingsXml";
    }

	[Serializable]
	public class PublishmentSystemInfo
	{
		private int _publishmentSystemId;
		private string _publishmentSystemName = string.Empty;
        private EPublishmentSystemType _publishmentSystemType = EPublishmentSystemType.CMS;
		private string _auxiliaryTableForContent = string.Empty;
        private string _auxiliaryTableForGovPublic = string.Empty;
        private string _auxiliaryTableForGovInteract = string.Empty;
        private string _auxiliaryTableForVote = string.Empty;
        private string _auxiliaryTableForJob = string.Empty;
		private bool _isCheckContentUseLevel;
		private int _checkContentLevel;
		private string _publishmentSystemDir = string.Empty;
		private string _publishmentSystemUrl = string.Empty;
        private bool _isHeadquarters;
        private int _parentPublishmentSystemId;
        private int _taxis;
        private string _settingsXml = string.Empty;
        private PublishmentSystemInfoExtend _additional;

        public PublishmentSystemInfo()
		{
		}

        public PublishmentSystemInfo(int publishmentSystemId, string publishmentSystemName, EPublishmentSystemType publishmentSystemType, string auxiliaryTableForContent, string auxiliaryTableForGovPublic, string auxiliaryTableForGovInteract, string auxiliaryTableForVote, string auxiliaryTableForJob, bool isCheckContentUseLevel, int checkContentLevel, string publishmentSystemDir, string publishmentSystemUrl, bool isHeadquarters, int parentPublishmentSystemId, int taxis, string settingsXml) 
		{
			_publishmentSystemId = publishmentSystemId;
			_publishmentSystemName = publishmentSystemName;
            _publishmentSystemType = publishmentSystemType;
			_auxiliaryTableForContent = auxiliaryTableForContent;
            _auxiliaryTableForGovPublic = auxiliaryTableForGovPublic;
            _auxiliaryTableForGovInteract = auxiliaryTableForGovInteract;
            _auxiliaryTableForVote = auxiliaryTableForVote;
            _auxiliaryTableForJob = auxiliaryTableForJob;
			_isCheckContentUseLevel = isCheckContentUseLevel;
			_checkContentLevel = checkContentLevel;
			_publishmentSystemDir = publishmentSystemDir;
			_publishmentSystemUrl = publishmentSystemUrl;
			_isHeadquarters = isHeadquarters;
            _parentPublishmentSystemId = parentPublishmentSystemId;
            _taxis = taxis;
            _settingsXml = settingsXml;
		}

		[XmlIgnore]
		public int PublishmentSystemId
		{
			get{ return _publishmentSystemId; }
			set{ _publishmentSystemId = value; }
		}

		[XmlIgnore]
		public string PublishmentSystemName
		{
			get{ return _publishmentSystemName; }
			set{ _publishmentSystemName = value; }
		}

        [XmlIgnore]
        public EPublishmentSystemType PublishmentSystemType
        {
            get { return _publishmentSystemType; }
            set { _publishmentSystemType = value; }
        }

		[XmlIgnore]
		public string AuxiliaryTableForContent
		{
			get{ return _auxiliaryTableForContent; }
			set{ _auxiliaryTableForContent = value; }
		}

        [XmlIgnore]
        public string AuxiliaryTableForGovPublic
        {
            get { return _auxiliaryTableForGovPublic; }
            set { _auxiliaryTableForGovPublic = value; }
        }

        [XmlIgnore]
        public string AuxiliaryTableForGovInteract
        {
            get { return _auxiliaryTableForGovInteract; }
            set { _auxiliaryTableForGovInteract = value; }
        }

        [XmlIgnore]
        public string AuxiliaryTableForJob
        {
            get { return _auxiliaryTableForJob; }
            set { _auxiliaryTableForJob = value; }
        }

        [XmlIgnore]
        public string AuxiliaryTableForVote
        {
            get { return _auxiliaryTableForVote; }
            set { _auxiliaryTableForVote = value; }
        }

        [XmlIgnore]
        public bool IsCheckContentUseLevel
		{
			get{ return _isCheckContentUseLevel; }
			set{ _isCheckContentUseLevel = value; }
		}

		[XmlIgnore]
		public int CheckContentLevel
		{
            get {
                return _isCheckContentUseLevel ? _checkContentLevel : 1;
            }
			set{ _checkContentLevel = value; }
		}

		[XmlIgnore]
		public string PublishmentSystemDir
		{
            get{ return _publishmentSystemDir; }
			set{ _publishmentSystemDir = value; }
		}

		[XmlIgnore]
		public string PublishmentSystemUrl
		{
            get { return _publishmentSystemUrl; }
			set{ _publishmentSystemUrl = value; }
		}

		[XmlIgnore]
        public bool IsHeadquarters
		{
			get{ return _isHeadquarters; }
			set{ _isHeadquarters = value; }
		}

        [XmlIgnore]
        public int ParentPublishmentSystemId
        {
            get { return _parentPublishmentSystemId; }
            set { _parentPublishmentSystemId = value; }
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

        public PublishmentSystemInfoExtend Additional => _additional ?? (_additional = new PublishmentSystemInfoExtend(_settingsXml));
	}
}
