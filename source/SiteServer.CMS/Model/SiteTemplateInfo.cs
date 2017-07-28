using System;
using System.Xml.Serialization;

namespace SiteServer.CMS.Model
{
	[Serializable]
	[XmlRoot("SiteTemplate")]
	public class SiteTemplateInfo
	{
		private string siteTemplateName;
        private string publishmentSystemType;
		private string webSiteUrl;
		private string picFileName;
		private string description;

		public SiteTemplateInfo()
		{
			siteTemplateName = string.Empty;
            publishmentSystemType = string.Empty;
			webSiteUrl = string.Empty;
			picFileName = string.Empty;
			description = string.Empty;
		}

        public SiteTemplateInfo(string siteTemplateName, string publishmentSystemType, string webSiteUrl, string picFileName, string description) 
		{
			this.siteTemplateName = siteTemplateName;
            this.publishmentSystemType = publishmentSystemType;
			this.webSiteUrl = webSiteUrl;
			this.picFileName = picFileName;
			this.description = description;
		}

		[XmlElement(ElementName = "SiteTemplateName")]
		public string SiteTemplateName
		{
			get { return siteTemplateName; }
			set { siteTemplateName = value; }
		}

        [XmlElement(ElementName = "PublishmentSystemType")]
        public string PublishmentSystemType
        {
            get { return publishmentSystemType; }
            set { publishmentSystemType = value; }
        }

		[XmlElement(ElementName = "WebSiteUrl")]
		public string WebSiteUrl
		{
			get { return webSiteUrl; }
			set { webSiteUrl = value; }
		}

		[XmlElement(ElementName = "PicFileName")]
		public string PicFileName
		{
			get { return picFileName; }
			set { picFileName = value; }
		}

		[XmlElement(ElementName = "Description")]
		public string Description
		{
			get { return description; }
			set { description = value; }
		}

	}
}
