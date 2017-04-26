namespace SiteServer.CMS.Model
{
	public class TagStyleInfo
	{
        private int styleID;
        private string styleName;
        private string elementName;
		private int publishmentSystemID;
        private bool isTemplate;
        private string styleTemplate;
        private string scriptTemplate;
        private string contentTemplate;
        private string successTemplate;
        private string failureTemplate;
        private string settingsXML;

		public TagStyleInfo()
		{
            styleID = 0;
            styleName = string.Empty;
            elementName = string.Empty;
			publishmentSystemID = 0;
            isTemplate = false;
            styleTemplate = string.Empty;
            scriptTemplate = string.Empty;
            contentTemplate = string.Empty;
            successTemplate = string.Empty;
            failureTemplate = string.Empty;
            settingsXML = string.Empty;
		}

        public TagStyleInfo(int styleID, string styleName, string elementName, int publishmentSystemID, bool isTemplate, string styleTemplate, string scriptTemplate, string contentTemplate, string successTemplate, string failureTemplate, string settingsXML) 
		{
            this.styleID = styleID;
            this.styleName = styleName;
            this.elementName = elementName;
            this.publishmentSystemID = publishmentSystemID;
            this.isTemplate = isTemplate;
            this.styleTemplate = styleTemplate;
            this.scriptTemplate = scriptTemplate;
            this.contentTemplate = contentTemplate;
            this.successTemplate = successTemplate;
            this.failureTemplate = failureTemplate;
            this.settingsXML = settingsXML;
		}

        public int StyleID
        {
            get { return styleID; }
            set { styleID = value; }
        }

        public string StyleName
		{
            get { return styleName; }
            set { styleName = value; }
		}

        public string ElementName
        {
            get { return elementName; }
            set { elementName = value; }
        }

		public int PublishmentSystemID
		{
			get{ return publishmentSystemID; }
			set{ publishmentSystemID = value; }
		}

        public bool IsTemplate
        {
            get { return isTemplate; }
            set { isTemplate = value; }
        }

        public string StyleTemplate
        {
            get { return styleTemplate; }
            set { styleTemplate = value; }
        }

        public string ScriptTemplate
        {
            get { return scriptTemplate; }
            set { scriptTemplate = value; }
        }

        public string ContentTemplate
        {
            get { return contentTemplate; }
            set { contentTemplate = value; }
        }

        public string SuccessTemplate
        {
            get { return successTemplate; }
            set { successTemplate = value; }
        }

        public string FailureTemplate
        {
            get { return failureTemplate; }
            set { failureTemplate = value; }
        }

        public string SettingsXML
        {
            get { return settingsXML; }
            set { settingsXML = value; }
        }
	}
}
