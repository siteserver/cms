using System;
using SiteServer.Plugin;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Model
{
	[Serializable]
	public class TemplateInfo
	{
		private int _id;
		private int _siteId;
		private string _templateName;
		private TemplateType _templateType;
		private string _relatedFileName;
        private string _createdFileFullName;
        private string _createdFileExtName;
		private ECharset _charset;
		private bool _isDefault;

		public TemplateInfo()
		{
            _id = 0;
			_siteId = 0;
			_templateName = string.Empty;
			_templateType = TemplateType.ContentTemplate;
			_relatedFileName = string.Empty;
            _createdFileFullName = string.Empty;
            _createdFileExtName = string.Empty;
			_charset = ECharset.utf_8;
			_isDefault = false;
		}

        public TemplateInfo(int id, int siteId, string templateName, TemplateType templateType, string relatedFileName, string createdFileFullName, string createdFileExtName, ECharset charset, bool isDefault) 
		{
            _id = id;
			_siteId = siteId;
			_templateName = templateName;
			_templateType = templateType;
            _relatedFileName = relatedFileName;
            _createdFileFullName = createdFileFullName;
            _createdFileExtName = createdFileExtName;
			_charset = charset;
			_isDefault = isDefault;
		}

		public int Id
		{
			get{ return _id; }
			set{ _id = value; }
		}

		public int SiteId
		{
			get{ return _siteId; }
			set{ _siteId = value; }
		}

		public string TemplateName
		{
			get{ return _templateName; }
			set{ _templateName = value; }
		}

		public TemplateType TemplateType
		{
			get{ return _templateType; }
			set{ _templateType = value; }
		}

        public string RelatedFileName
		{
            get { return _relatedFileName; }
            set { _relatedFileName = value; }
		}

        public string CreatedFileFullName
        {
            get { return _createdFileFullName; }
            set { _createdFileFullName = value; }
        }

        public string CreatedFileExtName
        {
            get { return _createdFileExtName; }
            set { _createdFileExtName = value; }
        }

		public ECharset Charset
		{
			get { return _charset; }
			set { _charset = value; }
		}

		public bool IsDefault
		{
			get{ return _isDefault; }
			set{ _isDefault = value; }
		}

        private string _content;
        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }
	}
}
