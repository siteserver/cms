using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Core.Create
{
    public class CreateTaskSummaryItem
    {
        public CreateTaskSummaryItem(CreateTaskInfo taskInfo, string timeSpan, bool isPending, bool isSuccess, string errorMessage)
        {
            siteId = taskInfo.SiteId;
            channelId = taskInfo.ChannelId;
            contentId = taskInfo.ContentId;
            fileTemplateId = taskInfo.FileTemplateId;
            specialId = taskInfo.SpecialId;
            type = ECreateTypeUtils.GetText(taskInfo.CreateType);
            name = taskInfo.Name;
            this.timeSpan = timeSpan;
            this.isPending = isPending;
            this.isSuccess = isSuccess;
            this.errorMessage = errorMessage;
        }

        public CreateTaskSummaryItem(CreateTaskLogInfo logInfo)
        {
            siteId = logInfo.SiteId;
            channelId = logInfo.ChannelId;
            contentId = logInfo.ContentId;
            fileTemplateId = logInfo.FileTemplateId;
            specialId = logInfo.SpecialId;
            type = ECreateTypeUtils.GetText(logInfo.CreateType);
            name = logInfo.TaskName;
            timeSpan = logInfo.TimeSpan;
            isPending = false;
            isSuccess = logInfo.IsSuccess;
            errorMessage = logInfo.ErrorMessage;
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
