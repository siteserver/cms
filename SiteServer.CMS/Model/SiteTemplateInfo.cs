using System;
using System.Xml.Serialization;

namespace SiteServer.CMS.Model
{
	[Serializable]
	[XmlRoot("SiteTemplate")]
	public class SiteTemplateInfo
	{
		private string _siteTemplateName;
		private string _webSiteUrl;
		private string _picFileName;
		private string _description;

		public SiteTemplateInfo()
		{
			_siteTemplateName = string.Empty;
			_webSiteUrl = string.Empty;
			_picFileName = string.Empty;
			_description = string.Empty;
		}

        public SiteTemplateInfo(string siteTemplateName, string webSiteUrl, string picFileName, string description) 
		{
			_siteTemplateName = siteTemplateName;
			_webSiteUrl = webSiteUrl;
			_picFileName = picFileName;
			_description = description;
		}

		[XmlElement(ElementName = "SiteTemplateName")]
		public string SiteTemplateName
		{
			get { return _siteTemplateName; }
			set { _siteTemplateName = value; }
		}

		[XmlElement(ElementName = "WebSiteUrl")]
		public string WebSiteUrl
		{
			get { return _webSiteUrl; }
			set { _webSiteUrl = value; }
		}

		[XmlElement(ElementName = "PicFileName")]
		public string PicFileName
		{
			get { return _picFileName; }
			set { _picFileName = value; }
		}

		[XmlElement(ElementName = "Description")]
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

	}
}
