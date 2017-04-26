namespace SiteServer.CMS.Model
{
	public class RelatedFieldItemInfo
	{
        private int id;
        private int relatedFieldID;
        private string itemName;
        private string itemValue;
        private int parentID;
        private int taxis;

		public RelatedFieldItemInfo()
		{
            id = 0;
            relatedFieldID = 0;
            itemName = string.Empty;
            itemValue = string.Empty;
            parentID = 0;
            taxis = 0;
		}

        public RelatedFieldItemInfo(int id, int relatedFieldID, string itemName, string itemValue, int parentID, int taxis)
		{
            this.id = id;
            this.relatedFieldID = relatedFieldID;
            this.itemName = itemName;
            this.itemValue = itemValue;
            this.parentID = parentID;
            this.taxis = taxis;
		}

        public int ID
		{
            get { return id; }
            set { id = value; }
		}

        public int RelatedFieldID
        {
            get { return relatedFieldID; }
            set { relatedFieldID = value; }
        }

        public string ItemName
        {
            get { return itemName; }
            set { itemName = value; }
        }

        public string ItemValue
        {
            get { return itemValue; }
            set { itemValue = value; }
        }

        public int ParentID
		{
            get { return parentID; }
            set { parentID = value; }
		}

        public int Taxis
        {
            get { return taxis; }
            set { taxis = value; }
        }
	}
}
