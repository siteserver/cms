using System;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Wcm.Model
{
	public class GovPublicApplyLogInfo
	{
        private int logID;
        private int publishmentSystemID;
        private int applyID;
        private int departmentID;
        private string userName;
        private EGovPublicApplyLogType logType;
        private string ipAddress;
        private DateTime addDate;
        private string summary;

		public GovPublicApplyLogInfo()
		{
            logID = 0;
            publishmentSystemID = 0;
            applyID = 0;
            departmentID = 0;
            userName = string.Empty;
            logType = EGovPublicApplyLogType.New;
            ipAddress = string.Empty;
            addDate = DateTime.Now;
            summary = string.Empty;
		}

        public GovPublicApplyLogInfo(int logID, int publishmentSystemID, int applyID, int departmentID, string userName, EGovPublicApplyLogType logType, string ipAddress, DateTime addDate, string summary)
		{
            this.logID = logID;
            this.publishmentSystemID = publishmentSystemID;
            this.applyID = applyID;
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

        public int ApplyID
        {
            get { return applyID; }
            set { applyID = value; }
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

        public EGovPublicApplyLogType LogType
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
