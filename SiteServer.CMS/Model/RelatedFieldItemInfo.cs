namespace SiteServer.CMS.Model
{
	public class RelatedFieldItemInfo
	{
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
