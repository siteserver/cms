using System;
using System.Collections.Generic;
using System.Text;

namespace SiteServer.Abstractions
{
    public interface IChannelSummary
    {
        int Id { get; set; }
        string ChannelName { get; set; }
        int ParentId { get; set; }
        string ParentsPath { get; set; }
        string IndexName { get; set; }
        string ContentModelPluginId { get; set; }
        int Taxis { get; set; }
        DateTime? AddDate { get; set; }
    }
}
