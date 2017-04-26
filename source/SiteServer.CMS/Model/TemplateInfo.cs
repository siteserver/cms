using System;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Model
{
	[Serializable]
	public class TemplateInfo
	{
		private int _templateId;
		private int _publishmentSystemId;
		private string _templateName;
		private ETemplateType _templateType;
		private string _relatedFileName;
        private string _createdFileFullName;
        private string _createdFileExtName;
		private ECharset _charset;
		private bool _isDefault;

		public TemplateInfo()
		{
			_templateId = 0;
			_publishmentSystemId = 0;
			_templateName = string.Empty;
			_templateType = ETemplateType.ContentTemplate;
			_relatedFileName = string.Empty;
            _createdFileFullName = string.Empty;
            _createdFileExtName = string.Empty;
			_charset = ECharset.utf_8;
			_isDefault = false;
		}

        public TemplateInfo(int templateId, int publishmentSystemId, string templateName, ETemplateType templateType, string relatedFileName, string createdFileFullName, string createdFileExtName, ECharset charset, bool isDefault) 
		{
			_templateId = templateId;
			_publishmentSystemId = publishmentSystemId;
			_templateName = templateName;
			_templateType = templateType;
            _relatedFileName = relatedFileName;
            _createdFileFullName = createdFileFullName;
            _createdFileExtName = createdFileExtName;
			_charset = charset;
			_isDefault = isDefault;
		}

		public int TemplateId
		{
			get{ return _templateId; }
			set{ _templateId = value; }
		}

		public int PublishmentSystemId
		{
			get{ return _publishmentSystemId; }
			set{ _publishmentSystemId = value; }
		}

		public string TemplateName
		{
			get{ return _templateName; }
			set{ _templateName = value; }
		}

		public ETemplateType TemplateType
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

        //ÐéÄâ×Ö¶Î
        private string _content;
        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }
	}
}
