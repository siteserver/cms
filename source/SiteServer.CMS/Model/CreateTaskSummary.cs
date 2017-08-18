using System.Collections.Generic;

namespace SiteServer.CMS.Model
{
    public class CreateTaskSummary
    {
        public CreateTaskSummary(List<CreateTaskSummaryItem> tasks, int channelsCount, int contentsCount, int filesCount)
        {
            Tasks = tasks;
            ChannelsCount = channelsCount;
            ContentsCount = contentsCount;
            FilesCount = filesCount;
        }

        public List<CreateTaskSummaryItem> Tasks { get; set; }
        public int ChannelsCount { get; set; }
        public int ContentsCount { get; set; }
        public int FilesCount { get; set; }
    }
}
