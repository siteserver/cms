namespace SiteServer.CMS.StlParser.Model
{
	public class DbItemInfo
	{
	    public DbItemInfo(object dataItem, int itemIndex)
		{
            DataItem = dataItem;
            ItemIndex = itemIndex;
		}

        public object DataItem { get; private set; }

	    public int ItemIndex { get; private set; }

	    public void Reload(object theDataItem, int theItemIndex)
        {
            DataItem = theDataItem;
            ItemIndex = theItemIndex;
        }
	}
}
