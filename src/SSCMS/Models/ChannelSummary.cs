using System;
using System.Collections.Generic;

namespace SSCMS.Models
{
    public class ChannelSummary : IChannelSummary
    {
        public int Id { get; set; }
        public string ChannelName { get; set; }
        public int ParentId { get; set; }
        public List<int> ParentsPath { get; set; }
        public string IndexName { get; set; }
        public string ContentModelPluginId { get; set; }
        public string TableName { get; set; }
        public int Taxis { get; set; }
        public DateTime? AddDate { get; set; }
    }
}
