using System;

namespace SiteServer.CMS.Model
{
    public class ErrorLogInfo
    {
        public ErrorLogInfo(int id, string category, string pluginId, string message, string stacktrace, string summary,
            DateTime addDate)
        {
            Id = id;
            Category = category;
            PluginId = pluginId;
            Message = message;
            Stacktrace = stacktrace;
            Summary = summary;
            AddDate = addDate;
        }

        public int Id { get; }

        public string Category { get; }

        public string PluginId { get; }

        public string Message { get; }

        public string Stacktrace { get; }

        public string Summary { get; }

        public DateTime AddDate { get; }
    }
}
