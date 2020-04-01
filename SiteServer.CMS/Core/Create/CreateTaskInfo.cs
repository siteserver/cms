using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Core.Create
{
    public class CreateTaskInfo
    {
        public CreateTaskInfo(int id, string name, ECreateType createType, int siteId, int channelId, int contentId, int fileTemplateId, int specialId, int pageCount)
        {
            Id = id;
            Name = name;
            CreateType = createType;
            SiteId = siteId;
            ChannelId = channelId;
            ContentId = contentId;
            FileTemplateId = fileTemplateId;
            SpecialId = specialId;
            PageCount = pageCount;
            Executing = false;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public ECreateType CreateType { get; set; }
        public int SiteId { get; set; }
        public int ChannelId { get; set; }
        public int ContentId { get; set; }
        public int FileTemplateId { get; set; }
        public int SpecialId { get; set; }
        public int PageCount { get; set; }

        public bool Executing { get; set; }
    }
}
