using System;

namespace BaiRong.Core.Model
{
	public class ErrorLogInfo
	{
	    public ErrorLogInfo()
		{
            Id = 0;
            AddDate = DateTime.Now;
            Message = string.Empty;
            Stacktrace = string.Empty;
            Summary = string.Empty;
		}

        public ErrorLogInfo(int id, DateTime addDate, string message, string stacktrace, string summary) 
		{
            Id = id;
            AddDate = addDate;
            Message = message;
            Stacktrace = stacktrace;
            Summary = summary;
		}

        public int Id { get; set; }

	    public DateTime AddDate { get; set; }

	    public string Message { get; set; }

	    public string Stacktrace { get; set; }

	    public string Summary { get; set; }
	}
}
