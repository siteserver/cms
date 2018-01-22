namespace SiteServer.CMS.Model
{
    public class TableCollectionInfo
    {
	    public TableCollectionInfo()
		{
			TableEnName = string.Empty;
			TableCnName = string.Empty;
			AttributeNum = 0;
			IsCreatedInDb = false;
			IsChangedAfterCreatedInDb = false;
            IsDefault = false;
			Description = string.Empty;
		}

        public TableCollectionInfo(string tableEnName, string tableCnName, int attributeNum, bool isCreatedInDb, bool isChangedAfterCreatedInDb, bool isDefault, string description) 
		{
			TableEnName = tableEnName;
			TableCnName = tableCnName;
			AttributeNum = attributeNum;
			IsCreatedInDb = isCreatedInDb;
			IsChangedAfterCreatedInDb = isChangedAfterCreatedInDb;
            IsDefault = isDefault;
			Description = description;
		}

		public string TableEnName { get; set; }

	    public string TableCnName { get; set; }

	    public int AttributeNum { get; set; }

	    public bool IsCreatedInDb { get; set; }

	    public bool IsChangedAfterCreatedInDb { get; set; }

	    public bool IsDefault { get; set; }

	    public string Description { get; set; }
	}
}
