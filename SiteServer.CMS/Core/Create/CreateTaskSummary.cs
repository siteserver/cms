using System.Collections.Generic;

namespace SiteServer.CMS.Core.Create
{
    public class CreateTaskSummary
    {
        public CreateTaskSummary(List<CreateTaskSummaryItem> tasks, int channelsCount, int contentsCount,
            int filesCount, int specialsCount)
        {
            Tasks = tasks;
            ChannelsCount = channelsCount;
            ContentsCount = contentsCount;
            FilesCount = filesCount;
            SpecialsCount = specialsCount;
        }

        public List<CreateTaskSummaryItem> Tasks { get; }

        public int ChannelsCount { get; }

        public int ContentsCount { get; }

        public int FilesCount { get; }

        public int SpecialsCount { get; }
    }
}
