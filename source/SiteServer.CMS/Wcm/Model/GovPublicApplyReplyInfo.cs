using System;

namespace SiteServer.CMS.Wcm.Model
{
	public class GovPublicApplyReplyInfo
	{
        private int replyID;
        private int publishmentSystemID;
        private int applyID;
        private string reply;
        private string fileUrl;
        private int departmentID;
        private string userName;
        private DateTime addDate;

		public GovPublicApplyReplyInfo()
		{
            replyID = 0;
            publishmentSystemID = 0;
            applyID = 0;
            reply = string.Empty;
            fileUrl = string.Empty;
            departmentID = 0;
            userName = string.Empty;
            addDate = DateTime.Now;
		}

        public GovPublicApplyReplyInfo(int replyID, int publishmentSystemID, int applyID, string reply, string fileUrl, int departmentID, string userName, DateTime addDate)
		{
            this.replyID = replyID;
            this.publishmentSystemID = publishmentSystemID;
            this.applyID = applyID;
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

        public int ApplyID
        {
            get { return applyID; }
            set { applyID = value; }
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
