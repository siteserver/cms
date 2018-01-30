namespace SiteServer.CMS.Model
{
    public class TableInfo
    {
	    public TableInfo()
	    {
	        Id = 0;
            TableName = string.Empty;
			DisplayName = string.Empty;
			AttributeNum = 0;
			IsCreatedInDb = false;
			IsChangedAfterCreatedInDb = false;
            IsDefault = false;
			Description = string.Empty;
		}

        public TableInfo(int id, string tableName, string displayName, int attributeNum, bool isCreatedInDb, bool isChangedAfterCreatedInDb, bool isDefault, string description)
        {
            Id = id;
            TableName = tableName;
			DisplayName = displayName;
			AttributeNum = attributeNum;
			IsCreatedInDb = isCreatedInDb;
			IsChangedAfterCreatedInDb = isChangedAfterCreatedInDb;
            IsDefault = isDefault;
			Description = description;
		}

        public int Id { get; set; }

        public string TableName { get; set; }

	    public string DisplayName { get; set; }

	    public int AttributeNum { get; set; }

	    public bool IsCreatedInDb { get; set; }

	    public bool IsChangedAfterCreatedInDb { get; set; }

	    public bool IsDefault { get; set; }

	    public string Description { get; set; }
	}
}
