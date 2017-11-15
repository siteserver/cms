using System;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Model
{
    public class TaskInfo
    {
        public TaskInfo()
        {
            TaskId = 0;
            TaskName = string.Empty;
            IsSystemTask = false;
            PublishmentSystemId = 0;
            ServiceType = EServiceType.Backup;
            ServiceParameters = string.Empty;
            FrequencyType = EFrequencyType.Week;
            PeriodIntervalMinute = 0;
            StartDay = 0;
            StartWeekday = 0;
            StartHour = 0;
            IsEnabled = false;
            AddDate = DateTime.Now;
            LastExecuteDate = DateTime.Now;
            Description = string.Empty;
        }

        public TaskInfo(int taskId, string taskName, bool isSystemTask, int publishmentSystemId, EServiceType serviceType, string serviceParameters, EFrequencyType frequencyType, int periodIntervalMinute, int startDay, int startWeekday, int startHour, bool isEnabled, DateTime addDate, DateTime lastExecuteDate, string description)
        {
            TaskId = taskId;
            TaskName = taskName;
            IsSystemTask = isSystemTask;
            PublishmentSystemId = publishmentSystemId;
            ServiceType = serviceType;
            ServiceParameters = serviceParameters;
            FrequencyType = frequencyType;
            PeriodIntervalMinute = periodIntervalMinute;
            StartDay = startDay;
            StartWeekday = startWeekday;
            StartHour = startHour;
            IsEnabled = isEnabled;
            AddDate = addDate;
            LastExecuteDate = lastExecuteDate;
            Description = description;
        }

        public TaskInfo(int taskId, string taskName, bool isSystemTask, int publishmentSystemId, EServiceType serviceType, string serviceParameters, EFrequencyType frequencyType, int periodIntervalMinute, int startDay, int startWeekday, int startHour, bool isEnabled, DateTime addDate, DateTime lastExecuteDate, string description,DateTime onlyOnceDate)
        {
            TaskId = taskId;
            TaskName = taskName;
            IsSystemTask = isSystemTask;
            PublishmentSystemId = publishmentSystemId;
            ServiceType = serviceType;
            ServiceParameters = serviceParameters;
            FrequencyType = frequencyType;
            PeriodIntervalMinute = periodIntervalMinute;
            StartDay = startDay;
            StartWeekday = startWeekday;
            StartHour = startHour;
            IsEnabled = isEnabled;
            AddDate = addDate;
            LastExecuteDate = lastExecuteDate;
            Description = description;
            OnlyOnceDate = onlyOnceDate;
        }

        public int TaskId { get; set; }

        public string TaskName { get; set; }

        public bool IsSystemTask { get; set; }

        public int PublishmentSystemId { get; set; }

        public EServiceType ServiceType { get; set; }

        public string ServiceParameters { get; set; }

        public EFrequencyType FrequencyType { get; set; }

        public int PeriodIntervalMinute { get; set; }

        public int StartDay { get; set; }

        public int StartWeekday { get; set; }

        public int StartHour { get; set; }

        public bool IsEnabled { get; set; }

        public DateTime AddDate { get; set; }

        public DateTime LastExecuteDate { get; set; }

        public string Description { get; set; }

        public DateTime OnlyOnceDate { get; set; }
    }
}
