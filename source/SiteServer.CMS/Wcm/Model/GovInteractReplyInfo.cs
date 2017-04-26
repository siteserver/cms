using System;

namespace SiteServer.CMS.Wcm.Model
{
	public class GovInteractReplyInfo
	{
        private int replyID;
        private int publishmentSystemID;
        private int nodeID;
        private int contentID;
        private string reply;
        private string fileUrl;
        private int departmentID;
        private string userName;
        private DateTime addDate;

		public GovInteractReplyInfo()
		{
            replyID = 0;
            publishmentSystemID = 0;
            nodeID = 0;
            contentID = 0;
            reply = string.Empty;
            fileUrl = string.Empty;
            departmentID = 0;
            userName = string.Empty;
            addDate = DateTime.Now;
		}

        public GovInteractReplyInfo(int replyID, int publishmentSystemID, int nodeID, int contentID, string reply, string fileUrl, int departmentID, string userName, DateTime addDate)
		{
            this.replyID = replyID;
            this.publishmentSystemID = publishmentSystemID;
            this.nodeID = nodeID;
            this.contentID = contentID;
            this.reply = reply;
            this.fileUrl = fileUrl;
            this.departmentID = departmentID;
            this.userName = userName;
            this.addDate = addDate;
		}

        public int AcceptID
        {
            get { return replyID; }
            set { replyID = value; }
        }

        public int PublishmentSystemID
        {
            get { return publishmentSystemID; }
            set { publishmentSystemID = value; }
        }

        public int NodeID
        {
            get { return nodeID; }
            set { nodeID = value; }
        }

        public int ContentID
        {
            get { return contentID; }
            set { contentID = value; }
        }

        public string Reply
        {
            get { return reply; }
            set { reply = value; }
        }

        public string FileUrl
        {
            get { return fileUrl; }
            set { fileUrl = value; }
        }
        
        public int DepartmentID
        {
            get { return departmentID; }
            set { departmentID = value; }
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        public DateTime AddDate
        {
            get { return addDate; }
            set { addDate = value; }
        }
	}
}
