using System;

namespace SiteServer.CMS.Model
{
    public class TaskLogInfo
    {
        public TaskLogInfo()
        {
            Id = 0;
            TaskId = 0;
            IsSuccess = false;
            ErrorMessage = string.Empty;
            AddDate = DateTime.Now;
        }

        public TaskLogInfo(int id, int taskId, bool isSuccess, string errorMessage, DateTime addDate)
        {
            Id = id;
            TaskId = taskId;
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            AddDate = addDate;
        }

        public int Id { get; set; }

        public int TaskId { get; set; }

        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; }

        public DateTime AddDate { get; set; }
    }
}
