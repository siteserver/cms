using System;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Provider
{
    public class TaskDao : DataProviderBase
    {
        public override string TableName => "siteserver_Task";

        public override List<TableColumnInfo> TableColumns => new List<TableColumnInfo>
        {
            new TableColumnInfo
            {
                ColumnName = nameof(TaskInfo.TaskId),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TaskInfo.TaskName),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TaskInfo.IsSystemTask),
                DataType = DataType.VarChar,
                Length = 18
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TaskInfo.PublishmentSystemId),
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TaskInfo.ServiceType),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TaskInfo.ServiceParameters),
                DataType = DataType.Text
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TaskInfo.FrequencyType),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TaskInfo.PeriodIntervalMinute),
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TaskInfo.StartDay),
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TaskInfo.StartWeekday),
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TaskInfo.StartHour),
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TaskInfo.IsEnabled),
                DataType = DataType.VarChar,
                Length = 18
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TaskInfo.AddDate),
                DataType = DataType.DateTime
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TaskInfo.LastExecuteDate),
                DataType = DataType.DateTime
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TaskInfo.Description),
                DataType = DataType.VarChar,
                Length = 255
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TaskInfo.OnlyOnceDate),
                DataType = DataType.DateTime
            }
        };

        private const string SqlSelectById = "SELECT TaskID, TaskName, IsSystemTask, PublishmentSystemID, ServiceType, ServiceParameters, FrequencyType, PeriodIntervalMinute, StartDay, StartWeekday, StartHour, IsEnabled, AddDate, LastExecuteDate, Description, OnlyOnceDate  FROM siteserver_Task WHERE TaskID = @TaskID";

        private const string SqlSelectByName = "SELECT TaskID, TaskName, IsSystemTask, PublishmentSystemID, ServiceType, ServiceParameters, FrequencyType, PeriodIntervalMinute, StartDay, StartWeekday, StartHour, IsEnabled, AddDate, LastExecuteDate, Description, OnlyOnceDate FROM siteserver_Task WHERE TaskName = @TaskName";

        private const string SqlUpdateTask = "UPDATE siteserver_Task SET TaskName = @TaskName, IsSystemTask = @IsSystemTask, PublishmentSystemID = @PublishmentSystemID, ServiceType = @ServiceType, ServiceParameters = @ServiceParameters, FrequencyType = @FrequencyType, PeriodIntervalMinute = @PeriodIntervalMinute, StartDay = @StartDay, StartWeekday = @StartWeekday, StartHour = @StartHour, IsEnabled = @IsEnabled, AddDate = @AddDate, LastExecuteDate = @LastExecuteDate, Description = @Description, OnlyOnceDate = @OnlyOnceDate WHERE TaskID = @TaskID";

        private const string SqlUpdateTaskState = "UPDATE siteserver_Task SET IsEnabled = @IsEnabled WHERE TaskID = @TaskID";

        private const string SqlUpdateTaskLastExecuteDate = "UPDATE siteserver_Task SET LastExecuteDate = @LastExecuteDate WHERE TaskID = @TaskID";

        private const string SqlDelete = "DELETE FROM siteserver_Task WHERE TaskID = @TaskID";

        private const string ParmTaskId = "@TaskID";
        private const string ParmTaskName = "@TaskName";
        private const string ParmIsSystemTask = "@IsSystemTask";
        private const string ParmPublishmentSystemId = "@PublishmentSystemID";
        private const string ParmServiceType = "@ServiceType";
        private const string ParmServiceParameters = "@ServiceParameters";
        private const string ParmFrequencyType = "@FrequencyType";
        private const string ParmPeriodIntervalMinute = "@PeriodIntervalMinute";
        private const string ParmStartDay = "@StartDay";
        private const string ParmStartWeekday = "@StartWeekday";
        private const string ParmStartHour = "@StartHour";
        private const string ParmIsEnabled = "@IsEnabled";
        private const string ParmAddDate = "@AddDate";
        private const string ParmLastExecuteDate = "@LastExecuteDate";
        private const string ParmDescription = "@Description";
        private const string ParmOnlyOnceDate = "@OnlyOnceDate";

        public int Insert(TaskInfo info)
        {
            const string sqlString = "INSERT INTO siteserver_Task (TaskName, IsSystemTask, PublishmentSystemID, ServiceType, ServiceParameters, FrequencyType, PeriodIntervalMinute, StartDay, StartWeekday, StartHour, IsEnabled, AddDate, LastExecuteDate, Description, OnlyOnceDate) VALUES (@TaskName, @IsSystemTask, @PublishmentSystemID, @ServiceType, @ServiceParameters, @FrequencyType, @PeriodIntervalMinute, @StartDay, @StartWeekday, @StartHour, @IsEnabled, @AddDate, @LastExecuteDate, @Description, @OnlyOnceDate)";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmTaskName, DataType.VarChar, 50, info.TaskName),
                GetParameter(ParmIsSystemTask, DataType.VarChar, 18, info.IsSystemTask.ToString()),
                GetParameter(ParmPublishmentSystemId, DataType.Integer, info.PublishmentSystemId),
                GetParameter(ParmServiceType, DataType.VarChar, 50, EServiceTypeUtils.GetValue(info.ServiceType)),
				GetParameter(ParmServiceParameters, DataType.Text, info.ServiceParameters),
				GetParameter(ParmFrequencyType, DataType.VarChar, 50, EFrequencyTypeUtils.GetValue(info.FrequencyType)),
				GetParameter(ParmPeriodIntervalMinute, DataType.Integer, info.PeriodIntervalMinute),
                GetParameter(ParmStartDay, DataType.Integer, info.StartDay),
                GetParameter(ParmStartWeekday, DataType.Integer, info.StartWeekday),
                GetParameter(ParmStartHour, DataType.Integer, info.StartHour),
                GetParameter(ParmIsEnabled, DataType.VarChar, 18, info.IsEnabled.ToString()),
                GetParameter(ParmAddDate, DataType.DateTime, info.AddDate),
                GetParameter(ParmLastExecuteDate, DataType.DateTime, info.LastExecuteDate),
                GetParameter(ParmDescription, DataType.VarChar, 255, info.Description),
                GetParameter(ParmOnlyOnceDate,DataType.DateTime,info.OnlyOnceDate)
			};

            var id = ExecuteNonQueryAndReturnId(TableName, nameof(TaskInfo.TaskId), sqlString, parms);
            ServiceManager.ClearTaskCache();
            return id;
        }

        public void Update(TaskInfo info)
        {
            var updateParms = new IDataParameter[]
			{
                GetParameter(ParmTaskName, DataType.VarChar, 50, info.TaskName),
                GetParameter(ParmIsSystemTask, DataType.VarChar, 18, info.IsSystemTask.ToString()),
                GetParameter(ParmPublishmentSystemId, DataType.Integer, info.PublishmentSystemId),
                GetParameter(ParmServiceType, DataType.VarChar, 50, EServiceTypeUtils.GetValue(info.ServiceType)),
				GetParameter(ParmServiceParameters, DataType.Text, info.ServiceParameters),
				GetParameter(ParmFrequencyType, DataType.VarChar, 50, EFrequencyTypeUtils.GetValue(info.FrequencyType)),
				GetParameter(ParmPeriodIntervalMinute, DataType.Integer, info.PeriodIntervalMinute),
                GetParameter(ParmStartDay, DataType.Integer, info.StartDay),
                GetParameter(ParmStartWeekday, DataType.Integer, info.StartWeekday),
                GetParameter(ParmStartHour, DataType.Integer, info.StartHour),
                GetParameter(ParmIsEnabled, DataType.VarChar, 18, info.IsEnabled.ToString()),
                GetParameter(ParmAddDate, DataType.DateTime, info.AddDate),
                GetParameter(ParmLastExecuteDate, DataType.DateTime, info.LastExecuteDate),
                GetParameter(ParmDescription, DataType.VarChar, 255, info.Description),
                GetParameter(ParmTaskId, DataType.Integer, info.TaskId),
                GetParameter(ParmOnlyOnceDate,DataType.DateTime,info.OnlyOnceDate)
			};

            ExecuteNonQuery(SqlUpdateTask, updateParms);
            ServiceManager.ClearTaskCache();
        }

        public void UpdateState(int taskId, bool isEnabled)
        {
            var updateParms = new IDataParameter[]
			{
                GetParameter(ParmIsEnabled, DataType.VarChar, 18, isEnabled.ToString()),
                GetParameter(ParmTaskId, DataType.Integer, taskId)
			};

            ExecuteNonQuery(SqlUpdateTaskState, updateParms);
            ServiceManager.ClearTaskCache();
        }

        public void UpdateLastExecuteDate(int taskId)
        {
            var updateParms = new IDataParameter[]
			{
                GetParameter(ParmLastExecuteDate, DataType.DateTime, DateTime.Now),
                GetParameter(ParmTaskId, DataType.Integer, taskId)
			};

            ExecuteNonQuery(SqlUpdateTaskLastExecuteDate, updateParms);
            ServiceManager.ClearTaskCache();
        }

        public void Delete(int taskId)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmTaskId, DataType.Integer, taskId)
			};

            ExecuteNonQuery(SqlDelete, parms);
            ServiceManager.ClearTaskCache();
        }

        public TaskInfo GetTaskInfo(int taskId)
        {
            TaskInfo info = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTaskId, DataType.Integer, taskId)
			};

            using (var rdr = ExecuteReader(SqlSelectById, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    info = new TaskInfo(GetInt(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), EServiceTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), EFrequencyTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetDateTime(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i));
                }
                rdr.Close();
            }

            return info;
        }

        public List<int> GetTaskIdList(EServiceType serviceType)
        {
            var list = new List<int>();

            const string sqlString = "SELECT TaskID FROM siteserver_Task WHERE ServiceType = @ServiceType";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmServiceType, DataType.VarChar, 50, EServiceTypeUtils.GetValue(serviceType))
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                while (rdr.Read())
                {
                    list.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }
            return list;
        }

        public List<TaskInfo> GetTaskInfoList()
        {
            var list = new List<TaskInfo>();

            const string sqlString = "SELECT TaskID, TaskName, IsSystemTask, PublishmentSystemID, ServiceType, ServiceParameters, FrequencyType, PeriodIntervalMinute, StartDay, StartWeekday, StartHour, IsEnabled, AddDate, LastExecuteDate, Description, OnlyOnceDate FROM siteserver_Task ORDER BY LastExecuteDate DESC";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var info = new TaskInfo(GetInt(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), EServiceTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), EFrequencyTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetDateTime(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i));
                    list.Add(info);
                }
                rdr.Close();
            }
            return list;
        }

        public List<TaskInfo> GetTaskInfoList(EServiceType serviceType)
        {
            var list = new List<TaskInfo>();

            const string sqlString = "SELECT TaskID, TaskName, IsSystemTask, PublishmentSystemID, ServiceType, ServiceParameters, FrequencyType, PeriodIntervalMinute, StartDay, StartWeekday, StartHour, IsEnabled, AddDate, LastExecuteDate, Description, OnlyOnceDate FROM siteserver_Task WHERE ServiceType = @ServiceType";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmServiceType, DataType.VarChar, 50, EServiceTypeUtils.GetValue(serviceType))
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var info = new TaskInfo(GetInt(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), EServiceTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), EFrequencyTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetDateTime(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i));
                    list.Add(info);
                }
                rdr.Close();
            }
            return list;
        }

        public List<TaskInfo> GetTaskInfoList(EServiceType serviceType, int publishmentSystemId)
        {
            var list = new List<TaskInfo>();

            const string sqlString = "SELECT TaskID, TaskName, IsSystemTask, PublishmentSystemID, ServiceType, ServiceParameters, FrequencyType, PeriodIntervalMinute, StartDay, StartWeekday, StartHour, IsEnabled, AddDate, LastExecuteDate, Description, OnlyOnceDate FROM siteserver_Task WHERE ServiceType = @ServiceType AND PublishmentSystemID = @PublishmentSystemID";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmServiceType, DataType.VarChar, 50, EServiceTypeUtils.GetValue(serviceType)),
                GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var info = new TaskInfo(GetInt(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), EServiceTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), EFrequencyTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetDateTime(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i));
                    list.Add(info);
                }
                rdr.Close();
            }
            return list;
        }

        public List<TaskInfo> GetAllTaskInfoList()
        {
            var list = new List<TaskInfo>();

            const string sqlString = "SELECT TaskID, TaskName, IsSystemTask, PublishmentSystemID, ServiceType, ServiceParameters, FrequencyType, PeriodIntervalMinute, StartDay, StartWeekday, StartHour, IsEnabled, AddDate, LastExecuteDate, Description, OnlyOnceDate FROM siteserver_Task";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var info = new TaskInfo(GetInt(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), EServiceTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), EFrequencyTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetDateTime(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i));
                    list.Add(info);
                }
                rdr.Close();
            }
            return list;
        }

        public bool IsExists(string taskName)
        {
            var exists = false;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTaskName, DataType.VarChar, 50, taskName)
			};

            using (var rdr = ExecuteReader(SqlSelectByName, parms))
            {
                if (rdr.Read())
                {
                    exists = true;
                }
                rdr.Close();
            }

            return exists;
        }

        public bool IsSystemTaskExists(int publishmentSystemId, EServiceType serviceType)
        {
            var exists = false;

            const string sqlString = "SELECT TaskID FROM siteserver_Task WHERE PublishmentSystemID = @PublishmentSystemID AND IsSystemTask = @IsSystemTask AND ServiceType = @ServiceType";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
                GetParameter(ParmIsSystemTask, DataType.VarChar, 18, true.ToString()),
                GetParameter(ParmServiceType, DataType.VarChar, 50, EServiceTypeUtils.GetValue(serviceType))
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    exists = true;
                }
                rdr.Close();
            }

            return exists;
        }

        public void DeleteSystemTask(int publishmentSystemId, EServiceType serviceType)
        {
            const string sqlString = "DELETE FROM siteserver_Task WHERE PublishmentSystemID = @PublishmentSystemID AND IsSystemTask = @IsSystemTask AND ServiceType = @ServiceType";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
                GetParameter(ParmIsSystemTask, DataType.VarChar, 18, true.ToString()),
                GetParameter(ParmServiceType, DataType.VarChar, 50, EServiceTypeUtils.GetValue(serviceType))
			};

            ExecuteNonQuery(sqlString, parms);
            ServiceManager.ClearTaskCache();
        }
    }
}
