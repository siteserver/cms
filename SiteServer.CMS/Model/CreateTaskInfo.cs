using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Model
{
    public class CreateTaskInfo
    {
        public CreateTaskInfo(int id, string name, ECreateType createType, int publishmentSystemId, int channelId, int contentId, int templateId, int pageCount)
        {
            Id = id;
            Name = name;
            CreateType = createType;
            PublishmentSystemId = publishmentSystemId;
            ChannelId = channelId;
            ContentId = contentId;
            TemplateId = templateId;
            PageCount = pageCount;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public ECreateType CreateType { get; set; }
        public int PublishmentSystemId { get; set; }
        public int ChannelId { get; set; }
        public int ContentId { get; set; }
        public int TemplateId { get; set; }
        public int PageCount { get; set; }

        public bool Equals(CreateTaskInfo taskInfo)
        {
            return CreateType == taskInfo?.CreateType && PublishmentSystemId == taskInfo.PublishmentSystemId && ChannelId == taskInfo.ChannelId && ContentId == taskInfo.ContentId && TemplateId == taskInfo.TemplateId;
        }
    }
}
