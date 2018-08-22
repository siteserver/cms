using System;

namespace SiteServer.CMS.Model
{
	public class ErrorLogInfo
	{
        public ErrorLogInfo(int id, string pluginId, string message, string stacktrace, string summary, DateTime addDate) 
		{
            Id = id;
            PluginId = pluginId;
            Message = message;
            Stacktrace = stacktrace;
            Summary = summary;
            AddDate = addDate;
        }

        public int Id { get; set; }

        public string PluginId { get; set; }

	    public string Message { get; set; }

	    public string Stacktrace { get; set; }

	    public string Summary { get; set; }

        public DateTime AddDate { get; set; }
    }
}
