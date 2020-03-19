using Datory;

namespace SSCMS.Dto.Create
{
    public class CreateTaskSummaryItem
    {
        public CreateTaskSummaryItem(CreateTask task, string timeSpan, bool isPending, bool isSuccess, string errorMessage)
        {
            siteId = task.SiteId;
            channelId = task.ChannelId;
            contentId = task.ContentId;
            fileTemplateId = task.FileTemplateId;
            specialId = task.SpecialId;
            type = task.CreateType.GetDisplayName();
            name = task.Name;
            this.timeSpan = timeSpan;
            this.isPending = isPending;
            this.isSuccess = isSuccess;
            this.errorMessage = errorMessage;
        }

        public CreateTaskSummaryItem(CreateTaskLog log)
        {
            siteId = log.SiteId;
            channelId = log.ChannelId;
            contentId = log.ContentId;
            fileTemplateId = log.FileTemplateId;
            specialId = log.SpecialId;
            type = log.CreateType.GetDisplayName();
            name = log.TaskName;
            timeSpan = log.TimeSpan;
            isPending = false;
            isSuccess = log.IsSuccess;
            errorMessage = log.ErrorMessage;
        }

        public int siteId { get; set; }
        public int channelId { get; set; }
        public int contentId { get; set; }
        public int fileTemplateId { get; set; }
        public int specialId { get; set; }

        public string type { get; set; }
        public string name { get; set; }
        public string timeSpan { get; set; }
        public bool isPending { get; set; }
        public bool isSuccess { get; set; }
        public string errorMessage { get; set; }
    }
}
