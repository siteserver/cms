using System;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Model
{
    public class CreateTaskLogInfo
    {
        public CreateTaskLogInfo(int id, ECreateType createType, int publishmentSystemId, int channelId, int contentId, int templateId, string taskName, string timeSpan, bool isSuccess, string errorMessage, DateTime addDate)
        {
            Id = id;
            CreateType = createType;
            PublishmentSystemId = publishmentSystemId;
            ChannelId = channelId;
            ContentId = contentId;
            TemplateId = templateId;
            TaskName = taskName;
            TimeSpan = timeSpan;
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            AddDate = addDate;
        }

        public int Id { get; set; }
        public ECreateType CreateType { get; set; }
        public int PublishmentSystemId { get; set; }
        public int ChannelId { get; set; }
        public int ContentId { get; set; }
        public int TemplateId { get; set; }
        public string TaskName { get; set; }
        public string TimeSpan { get; set; }

        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime AddDate { get; set; }
    }
}
