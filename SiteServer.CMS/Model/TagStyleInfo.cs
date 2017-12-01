namespace SiteServer.CMS.Model
{
	public class TagStyleInfo
	{
	    public TagStyleInfo()
		{
            StyleId = 0;
            StyleName = string.Empty;
            ElementName = string.Empty;
			PublishmentSystemId = 0;
            IsTemplate = false;
            StyleTemplate = string.Empty;
            ScriptTemplate = string.Empty;
            ContentTemplate = string.Empty;
            SuccessTemplate = string.Empty;
            FailureTemplate = string.Empty;
            SettingsXml = string.Empty;
		}

        public TagStyleInfo(int styleId, string styleName, string elementName, int publishmentSystemId, bool isTemplate, string styleTemplate, string scriptTemplate, string contentTemplate, string successTemplate, string failureTemplate, string settingsXml) 
		{
            StyleId = styleId;
            StyleName = styleName;
            ElementName = elementName;
            PublishmentSystemId = publishmentSystemId;
            IsTemplate = isTemplate;
            StyleTemplate = styleTemplate;
            ScriptTemplate = scriptTemplate;
            ContentTemplate = contentTemplate;
            SuccessTemplate = successTemplate;
            FailureTemplate = failureTemplate;
            SettingsXml = settingsXml;
		}

        public int StyleId { get; set; }

	    public string StyleName { get; set; }

	    public string ElementName { get; set; }

	    public int PublishmentSystemId { get; set; }

	    public bool IsTemplate { get; set; }

	    public string StyleTemplate { get; set; }

	    public string ScriptTemplate { get; set; }

	    public string ContentTemplate { get; set; }

	    public string SuccessTemplate { get; set; }

	    public string FailureTemplate { get; set; }

	    public string SettingsXml { get; set; }
	}
}
