using System;

namespace SSCMS.Dto
{
    public class SyncTaskLog
    {
        public SyncTaskLog(int siteId, string key, string timeSpan, bool isSuccess, string errorMessage, DateTime addDate)
        {
            SiteId = siteId;
            Key = key;
            TimeSpan = timeSpan;
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            AddDate = addDate;
        }

        public int SiteId { get; set; }

        public string Key { get; set; }

        public string TimeSpan { get; set; }

        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; }

        public DateTime AddDate { get; set; }
    }
}
