namespace SSCMS.Dto
{
    public class SyncTaskSummaryItem
    {
        public SyncTaskSummaryItem(int siteId, string key, string timeSpan, bool isPending, bool isSuccess, string errorMessage)
        {
            this.siteId = siteId;
            this.key = key;
            this.timeSpan = timeSpan;
            this.isPending = isPending;
            this.isSuccess = isSuccess;
            this.errorMessage = errorMessage;
        }

        public SyncTaskSummaryItem(SyncTaskLog log)
        {
            siteId = log.SiteId;
            key = log.Key;
            timeSpan = log.TimeSpan;
            isPending = false;
            isSuccess = log.IsSuccess;
            errorMessage = log.ErrorMessage;
        }

        public int siteId { get; set; }
        public string key { get; set; }
        public string timeSpan { get; set; }
        public bool isPending { get; set; }
        public bool isSuccess { get; set; }
        public string errorMessage { get; set; }
    }
}
