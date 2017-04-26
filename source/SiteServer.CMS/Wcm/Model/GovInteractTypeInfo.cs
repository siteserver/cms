namespace SiteServer.CMS.Wcm.Model
{
	public class GovInteractTypeInfo
	{
        private int typeID;
        private string typeName;
        private int nodeID;
        private int publishmentSystemID;
        private int taxis;

		public GovInteractTypeInfo()
		{
            typeID = 0;
            typeName = string.Empty;
            nodeID = 0;
            publishmentSystemID = 0;
            taxis = 0;
		}

        public GovInteractTypeInfo(int typeID, string typeName, int nodeID, int publishmentSystemID, int taxis)
		{
            this.typeID = typeID;
            this.typeName = typeName;
            this.nodeID = nodeID;
            this.publishmentSystemID = publishmentSystemID;
            this.taxis = taxis;
		}

        public int TypeID
        {
            get { return typeID; }
            set { typeID = value; }
        }

        public string TypeName
        {
            get { return typeName; }
            set { typeName = value; }
        }

        public int NodeID
        {
            get { return nodeID; }
            set { nodeID = value; }
        }

        public int PublishmentSystemID
        {
            get { return publishmentSystemID; }
            set { publishmentSystemID = value; }
        }

        public int Taxis
        {
            get { return taxis; }
            set { taxis = value; }
        }
	}
}
