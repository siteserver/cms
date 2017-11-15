namespace SiteServer.CMS.Model
{
	public class RelatedFieldItemInfo
	{
	    public RelatedFieldItemInfo()
		{
            Id = 0;
            RelatedFieldId = 0;
            ItemName = string.Empty;
            ItemValue = string.Empty;
            ParentId = 0;
            Taxis = 0;
		}

        public RelatedFieldItemInfo(int id, int relatedFieldId, string itemName, string itemValue, int parentId, int taxis)
		{
            Id = id;
            RelatedFieldId = relatedFieldId;
            ItemName = itemName;
            ItemValue = itemValue;
            ParentId = parentId;
            Taxis = taxis;
		}

        public int Id { get; set; }

	    public int RelatedFieldId { get; set; }

	    public string ItemName { get; set; }

	    public string ItemValue { get; set; }

	    public int ParentId { get; set; }

	    public int Taxis { get; set; }
	}
}
