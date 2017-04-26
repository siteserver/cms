using System;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Wcm.Model
{
	public class GovInteractLogInfo
	{
        private int logID;
        private int publishmentSystemID;
        private int nodeID;
        private int contentID;
        private int departmentID;
        private string userName;
        private EGovInteractLogType logType;
        private string ipAddress;
        private DateTime addDate;
        private string summary;

		public GovInteractLogInfo()
		{
            logID = 0;
            publishmentSystemID = 0;
            nodeID = 0;
            contentID = 0;
            departmentID = 0;
            userName = string.Empty;
            logType = EGovInteractLogType.New;
            ipAddress = string.Empty;
            addDate = DateTime.Now;
            summary = string.Empty;
		}

        public GovInteractLogInfo(int logID, int publishmentSystemID, int nodeID, int contentID, int departmentID, string userName, EGovInteractLogType logType, string ipAddress, DateTime addDate, string summary)
		{
            this.logID = logID;
            this.publishmentSystemID = publishmentSystemID;
            this.nodeID = nodeID;
            this.contentID = contentID;
            this.departmentID = departmentID;
            this.userName = userName;
            this.logType = logType;
            this.ipAddress = ipAddress;
            this.addDate = addDate;
            this.summary = summary;
		}

        public int LogID
        {
            get { return logID; }
            set { logID = value; }
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

        public EGovInteractLogType LogType
        {
            get { return logType; }
            set { logType = value; }
        }

        public string IPAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }

        public DateTime AddDate
        {
            get { return addDate; }
            set { addDate = value; }
        }

        public string Summary
        {
            get { return summary; }
            set { summary = value; }
        }
	}
}
