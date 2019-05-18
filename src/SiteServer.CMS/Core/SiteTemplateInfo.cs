using System;
using System.Xml.Serialization;

namespace SiteServer.CMS.Core
{
	[Serializable]
	[XmlRoot("SiteTemplate")]
	public class SiteTemplateInfo
	{
		public SiteTemplateInfo()
		{
			SiteTemplateName = string.Empty;
			WebSiteUrl = string.Empty;
			PicFileName = string.Empty;
			Description = string.Empty;
		}

        public SiteTemplateInfo(string siteTemplateName, string webSiteUrl, string picFileName, string description) 
		{
			SiteTemplateName = siteTemplateName;
			WebSiteUrl = webSiteUrl;
			PicFileName = picFileName;
			Description = description;
		}

        [XmlElement(ElementName = "SiteTemplateName")]
        public string SiteTemplateName { get; set; }

        [XmlElement(ElementName = "WebSiteUrl")]
		public string WebSiteUrl { get; set; }

        [XmlElement(ElementName = "PicFileName")]
		public string PicFileName { get; set; }

        [XmlElement(ElementName = "Description")]
		public string Description { get; set; }

        public string DirectoryName { get; set; }

    }
}
