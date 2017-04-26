using System.Collections.Generic;

namespace SiteServer.CMS.Model
{
    public class CreateTaskSummary
    {
        public CreateTaskSummary(List<CreateTaskSummaryItem> items, int indexCount, int channelsCount, int contentsCount, int filesCount)
        {
            Items = items;
            IndexCount = indexCount;
            ChannelsCount = channelsCount;
            ContentsCount = contentsCount;
            FilesCount = filesCount;
        }

        public List<CreateTaskSummaryItem> Items { get; set; }
        public int IndexCount { get; set; }
        public int ChannelsCount { get; set; }
        public int ContentsCount { get; set; }
        public int FilesCount { get; set; }
    }
}
