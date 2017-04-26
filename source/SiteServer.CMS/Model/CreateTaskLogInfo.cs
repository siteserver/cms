using System;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Model
{
    public class CreateTaskLogInfo
    {
        public CreateTaskLogInfo(int id, ECreateType createType, int publishmentSystemID, string taskName, string timeSpan, bool isSuccess, string errorMessage, DateTime addDate)
        {
            ID = id;
            CreateType = createType;
            PublishmentSystemID = publishmentSystemID;
            TaskName = taskName;
            TimeSpan = timeSpan;
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            AddDate = addDate;
        }

        public int ID { get; set; }
        public ECreateType CreateType { get; set; }
        public int PublishmentSystemID { get; set; }
        public string TaskName { get; set; }
        public string TimeSpan { get; set; }

        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime AddDate { get; set; }
    }
}
