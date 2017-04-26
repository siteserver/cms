namespace SiteServer.CMS.StlParser.Model
{
	public class DbItemInfo
	{
        private object dataItem;
        private int itemIndex;

        public DbItemInfo(object dataItem, int itemIndex)
		{
            this.dataItem = dataItem;
            this.itemIndex = itemIndex;
		}

        public object DataItem => dataItem;

	    public int ItemIndex => itemIndex;

	    public void Reload(object theDataItem, int theItemIndex)
        {
            dataItem = theDataItem;
            itemIndex = theItemIndex;
        }
	}
}
