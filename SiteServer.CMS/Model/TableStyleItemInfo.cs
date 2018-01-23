namespace SiteServer.CMS.Model
{
	public class TableStyleItemInfo
	{
	    public TableStyleItemInfo()
		{
            Id = 0;
            TableStyleId = 0;
            ItemTitle = string.Empty;
            ItemValue = string.Empty;
            IsSelected = false;
		}

        public TableStyleItemInfo(int id, int tableStyleId, string itemTitle, string itemValue, bool isSelected) 
		{
            Id = id;
            TableStyleId = tableStyleId;
            ItemTitle = itemTitle;
            ItemValue = itemValue;
            IsSelected = isSelected;
		}

        public int Id { get; set; }

	    public int TableStyleId { get; set; }

	    public string ItemTitle { get; set; }

	    public string ItemValue { get; set; }

	    public bool IsSelected { get; set; }
	}
}
