using System;

namespace SiteServer.CMS.Model
{
    public class TaskLogInfo
    {
        private int id;
        private int taskID;
        private bool isSuccess;
        private string errorMessage;
        private DateTime addDate;

        public TaskLogInfo()
        {
            id = 0;
            taskID = 0;
            isSuccess = false;
            errorMessage = string.Empty;
            addDate = DateTime.Now;
        }

        public TaskLogInfo(int id, int taskID, bool isSuccess, string errorMessage, DateTime addDate)
        {
            this.id = id;
            this.taskID = taskID;
            this.isSuccess = isSuccess;
            this.errorMessage = errorMessage;
            this.addDate = addDate;
        }

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public int TaskID
        {
            get { return taskID; }
            set { taskID = value; }
        }

        public bool IsSuccess
        {
            get { return isSuccess; }
            set { isSuccess = value; }
        }

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }

        public DateTime AddDate
        {
            get { return addDate; }
            set { addDate = value; }
        }
    }
}
