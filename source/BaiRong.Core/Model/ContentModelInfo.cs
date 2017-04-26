using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Model
{
	public class ContentModelInfo
	{
	    public ContentModelInfo()
		{
            ModelId = string.Empty;
            SiteId = 0;
            ModelName = string.Empty;
            IsSystem = false;
            TableName = string.Empty;
            TableType = EAuxiliaryTableType.BackgroundContent;
            IconUrl = string.Empty;
			Description = string.Empty;
		}

        public ContentModelInfo(string modelId, int siteId, string modelName, bool isSystem, string tableName, EAuxiliaryTableType tableType, string iconUrl, string description)
		{
            ModelId = modelId;
            SiteId = siteId;
            ModelName = modelName;
            IsSystem = isSystem;
            TableName = tableName;
            TableType = tableType;
            IconUrl = iconUrl;
            Description = description;
		}

        public string ModelId { get; set; }

	    public int SiteId { get; set; }

	    public string ModelName { get; set; }

	    public bool IsSystem { get; set; }

	    public string TableName { get; set; }

	    public EAuxiliaryTableType TableType { get; set; }

	    public string IconUrl { get; set; }

	    public string Description { get; set; }
	}
}
