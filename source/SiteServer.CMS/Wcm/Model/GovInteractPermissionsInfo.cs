namespace SiteServer.CMS.Wcm.Model
{
    public class GovInteractPermissionsInfo
    {
        private string userName;
        private int nodeID;
        private string permissions;

        public GovInteractPermissionsInfo()
        {
            userName = string.Empty;
            nodeID = 0;
            permissions = string.Empty;
        }

        public GovInteractPermissionsInfo(string userName, int nodeID, string permissions)
        {
            this.userName = userName;
            this.nodeID = nodeID;
            this.permissions = permissions;
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        public int NodeID
        {
            get { return nodeID; }
            set { nodeID = value; }
        }

        public string Permissions
        {
            get { return permissions; }
            set { permissions = value; }
        }
    }
}
