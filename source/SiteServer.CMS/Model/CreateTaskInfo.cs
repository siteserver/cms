using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Model
{
    public class CreateTaskInfo
    {
        public CreateTaskInfo(int id, ECreateType createType, int publishmentSystemID, int channelID, int contentID, int templateID)
        {
            ID = id;
            CreateType = createType;
            PublishmentSystemID = publishmentSystemID;
            ChannelID = channelID;
            ContentID = contentID;
            TemplateID = templateID;
        }

        public int ID { get; set; }
        public ECreateType CreateType { get; set; }
        public int PublishmentSystemID { get; set; }
        public int ChannelID { get; set; }
        public int ContentID { get; set; }
        public int TemplateID { get; set; }

        public bool Equals(CreateTaskInfo taskInfo)
        {
            if (taskInfo == null)
            {
                return false;
            }
            if (CreateType != taskInfo.CreateType || PublishmentSystemID != taskInfo.PublishmentSystemID || ChannelID != taskInfo.ChannelID || ContentID != taskInfo.ContentID || TemplateID != taskInfo.TemplateID)
            {
                return false;
            }
            return true;
        }
    }
}
