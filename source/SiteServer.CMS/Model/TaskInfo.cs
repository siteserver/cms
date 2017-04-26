using System;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Model
{
    public class TaskInfo
    {
        private int taskID;
        private string taskName;
        private bool isSystemTask;
        private int publishmentSystemID;
        private EServiceType serviceType;
        private string serviceParameters;
        private EFrequencyType frequencyType;
        private int periodIntervalMinute;
        private int startDay;
        private int startWeekday;
        private int startHour;
        private bool isEnabled;
        private DateTime addDate;
        private DateTime lastExecuteDate;
        private string description;
        private DateTime onlyOnceDate;

        public TaskInfo()
        {
            taskID = 0;
            taskName = string.Empty;
            isSystemTask = false;
            publishmentSystemID = 0;
            serviceType = EServiceType.Backup;
            serviceParameters = string.Empty;
            frequencyType = EFrequencyType.Week;
            periodIntervalMinute = 0;
            startDay = 0;
            startWeekday = 0;
            startHour = 0;
            isEnabled = false;
            addDate = DateTime.Now;
            lastExecuteDate = DateTime.Now;
            description = string.Empty;
        }

        public TaskInfo(int taskID, string taskName, bool isSystemTask, int publishmentSystemID, EServiceType serviceType, string serviceParameters, EFrequencyType frequencyType, int periodIntervalMinute, int startDay, int startWeekday, int startHour, bool isEnabled, DateTime addDate, DateTime lastExecuteDate, string description)
        {
            this.taskID = taskID;
            this.taskName = taskName;
            this.isSystemTask = isSystemTask;
            this.publishmentSystemID = publishmentSystemID;
            this.serviceType = serviceType;
            this.serviceParameters = serviceParameters;
            this.frequencyType = frequencyType;
            this.periodIntervalMinute = periodIntervalMinute;
            this.startDay = startDay;
            this.startWeekday = startWeekday;
            this.startHour = startHour;
            this.isEnabled = isEnabled;
            this.addDate = addDate;
            this.lastExecuteDate = lastExecuteDate;
            this.description = description;
        }

        public TaskInfo(int taskID, string taskName, bool isSystemTask, int publishmentSystemID, EServiceType serviceType, string serviceParameters, EFrequencyType frequencyType, int periodIntervalMinute, int startDay, int startWeekday, int startHour, bool isEnabled, DateTime addDate, DateTime lastExecuteDate, string description,DateTime onlyOnceDate)
        {
            this.taskID = taskID;
            this.taskName = taskName;
            this.isSystemTask = isSystemTask;
            this.publishmentSystemID = publishmentSystemID;
            this.serviceType = serviceType;
            this.serviceParameters = serviceParameters;
            this.frequencyType = frequencyType;
            this.periodIntervalMinute = periodIntervalMinute;
            this.startDay = startDay;
            this.startWeekday = startWeekday;
            this.startHour = startHour;
            this.isEnabled = isEnabled;
            this.addDate = addDate;
            this.lastExecuteDate = lastExecuteDate;
            this.description = description;
            OnlyOnceDate = onlyOnceDate;
        }

        public int TaskID
        {
            get { return taskID; }
            set { taskID = value; }
        }

        public string TaskName
        {
            get { return taskName; }
            set { taskName = value; }
        }

        public bool IsSystemTask
        {
            get { return isSystemTask; }
            set { isSystemTask = value; }
        }

        public int PublishmentSystemID
        {
            get { return publishmentSystemID; }
            set { publishmentSystemID = value; }
        }

        public EServiceType ServiceType
        {
            get { return serviceType; }
            set { serviceType = value; }
        }

        public string ServiceParameters
        {
            get { return serviceParameters; }
            set { serviceParameters = value; }
        }

        public EFrequencyType FrequencyType
        {
            get { return frequencyType; }
            set { frequencyType = value; }
        }

        public int PeriodIntervalMinute
        {
            get { return periodIntervalMinute; }
            set { periodIntervalMinute = value; }
        }

        public int StartDay
        {
            get { return startDay; }
            set { startDay = value; }
        }

        public int StartWeekday
        {
            get { return startWeekday; }
            set { startWeekday = value; }
        }

        public int StartHour
        {
            get { return startHour; }
            set { startHour = value; }
        }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set { isEnabled = value; }
        }

        public DateTime AddDate
        {
            get { return addDate; }
            set { addDate = value; }
        }

        public DateTime LastExecuteDate
        {
            get { return lastExecuteDate; }
            set { lastExecuteDate = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public DateTime OnlyOnceDate
        {
            get { return onlyOnceDate; }
            set { onlyOnceDate = value; }
        }
    }
}
