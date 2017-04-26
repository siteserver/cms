namespace BaiRong.Core.Model
{
	public class TableStyleItemInfo
	{
	    public TableStyleItemInfo()
		{
            TableStyleItemId = 0;
            TableStyleId = 0;
            ItemTitle = string.Empty;
            ItemValue = string.Empty;
            IsSelected = false;
		}

        public TableStyleItemInfo(int tableStyleItemId, int tableStyleId, string itemTitle, string itemValue, bool isSelected) 
		{
            TableStyleItemId = tableStyleItemId;
            TableStyleId = tableStyleId;
            ItemTitle = itemTitle;
            ItemValue = itemValue;
            IsSelected = isSelected;
		}

        public int TableStyleItemId { get; set; }

	    public int TableStyleId { get; set; }

	    public string ItemTitle { get; set; }

	    public string ItemValue { get; set; }

	    public bool IsSelected { get; set; }
	}
}
