using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Provider
{
    public class TaskDao : DataProviderBase
    {
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
            int id;
            const string sqlString = "INSERT INTO siteserver_Task (TaskName, IsSystemTask, PublishmentSystemID, ServiceType, ServiceParameters, FrequencyType, PeriodIntervalMinute, StartDay, StartWeekday, StartHour, IsEnabled, AddDate, LastExecuteDate, Description, OnlyOnceDate) VALUES (@TaskName, @IsSystemTask, @PublishmentSystemID, @ServiceType, @ServiceParameters, @FrequencyType, @PeriodIntervalMinute, @StartDay, @StartWeekday, @StartHour, @IsEnabled, @AddDate, @LastExecuteDate, @Description, @OnlyOnceDate)";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmTaskName, EDataType.NVarChar, 50, info.TaskName),
                GetParameter(ParmIsSystemTask, EDataType.VarChar, 18, info.IsSystemTask.ToString()),
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, info.PublishmentSystemID),
                GetParameter(ParmServiceType, EDataType.VarChar, 50, EServiceTypeUtils.GetValue(info.ServiceType)),
				GetParameter(ParmServiceParameters, EDataType.NText, info.ServiceParameters),
				GetParameter(ParmFrequencyType, EDataType.VarChar, 50, EFrequencyTypeUtils.GetValue(info.FrequencyType)),
				GetParameter(ParmPeriodIntervalMinute, EDataType.Integer, info.PeriodIntervalMinute),
                GetParameter(ParmStartDay, EDataType.Integer, info.StartDay),
                GetParameter(ParmStartWeekday, EDataType.Integer, info.StartWeekday),
                GetParameter(ParmStartHour, EDataType.Integer, info.StartHour),
                GetParameter(ParmIsEnabled, EDataType.VarChar, 18, info.IsEnabled.ToString()),
                GetParameter(ParmAddDate, EDataType.DateTime, info.AddDate),
                GetParameter(ParmLastExecuteDate, EDataType.DateTime, info.LastExecuteDate),
                GetParameter(ParmDescription, EDataType.NVarChar, 255, info.Description),
                GetParameter(ParmOnlyOnceDate,EDataType.DateTime,info.OnlyOnceDate)
			};

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        id = ExecuteNonQueryAndReturnId(trans, sqlString, parms);
                        ServiceManager.ClearTaskCache();
                        trans.Commit();
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        throw;
                    }

                }
            }
            return id;
        }

        public void Update(TaskInfo info)
        {
            var updateParms = new IDataParameter[]
			{
                GetParameter(ParmTaskName, EDataType.NVarChar, 50, info.TaskName),
                GetParameter(ParmIsSystemTask, EDataType.VarChar, 18, info.IsSystemTask.ToString()),
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, info.PublishmentSystemID),
                GetParameter(ParmServiceType, EDataType.VarChar, 50, EServiceTypeUtils.GetValue(info.ServiceType)),
				GetParameter(ParmServiceParameters, EDataType.NText, info.ServiceParameters),
				GetParameter(ParmFrequencyType, EDataType.VarChar, 50, EFrequencyTypeUtils.GetValue(info.FrequencyType)),
				GetParameter(ParmPeriodIntervalMinute, EDataType.Integer, info.PeriodIntervalMinute),
                GetParameter(ParmStartDay, EDataType.Integer, info.StartDay),
                GetParameter(ParmStartWeekday, EDataType.Integer, info.StartWeekday),
                GetParameter(ParmStartHour, EDataType.Integer, info.StartHour),
                GetParameter(ParmIsEnabled, EDataType.VarChar, 18, info.IsEnabled.ToString()),
                GetParameter(ParmAddDate, EDataType.DateTime, info.AddDate),
                GetParameter(ParmLastExecuteDate, EDataType.DateTime, info.LastExecuteDate),
                GetParameter(ParmDescription, EDataType.NVarChar, 255, info.Description),
                GetParameter(ParmTaskId, EDataType.Integer, info.TaskID),
                GetParameter(ParmOnlyOnceDate,EDataType.DateTime,info.OnlyOnceDate)
			};

            ExecuteNonQuery(SqlUpdateTask, updateParms);
            ServiceManager.ClearTaskCache();
        }

        public void UpdateState(int taskId, bool isEnabled)
        {
            var updateParms = new IDataParameter[]
			{
                GetParameter(ParmIsEnabled, EDataType.VarChar, 18, isEnabled.ToString()),
                GetParameter(ParmTaskId, EDataType.Integer, taskId)
			};

            ExecuteNonQuery(SqlUpdateTaskState, updateParms);
            ServiceManager.ClearTaskCache();
        }

        public void UpdateLastExecuteDate(int taskId)
        {
            var updateParms = new IDataParameter[]
			{
                GetParameter(ParmLastExecuteDate, EDataType.DateTime, DateTime.Now),
                GetParameter(ParmTaskId, EDataType.Integer, taskId)
			};

            ExecuteNonQuery(SqlUpdateTaskLastExecuteDate, updateParms);
            ServiceManager.ClearTaskCache();
        }

        public void Delete(int taskId)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmTaskId, EDataType.Integer, taskId)
			};

            ExecuteNonQuery(SqlDelete, parms);
            ServiceManager.ClearTaskCache();
        }

        public TaskInfo GetTaskInfo(int taskId)
        {
            TaskInfo info = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTaskId, EDataType.Integer, taskId)
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
                GetParameter(ParmServiceType, EDataType.VarChar, 50, EServiceTypeUtils.GetValue(serviceType))
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
                GetParameter(ParmServiceType, EDataType.VarChar, 50, EServiceTypeUtils.GetValue(serviceType))
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
                GetParameter(ParmServiceType, EDataType.VarChar, 50, EServiceTypeUtils.GetValue(serviceType)),
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
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
				GetParameter(ParmTaskName, EDataType.NVarChar, 50, taskName)
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
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
                GetParameter(ParmIsSystemTask, EDataType.VarChar, 18, true.ToString()),
                GetParameter(ParmServiceType, EDataType.VarChar, 50, EServiceTypeUtils.GetValue(serviceType))
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
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
                GetParameter(ParmIsSystemTask, EDataType.VarChar, 18, true.ToString()),
                GetParameter(ParmServiceType, EDataType.VarChar, 50, EServiceTypeUtils.GetValue(serviceType))
			};

            ExecuteNonQuery(sqlString, parms);
            ServiceManager.ClearTaskCache();
        }
    }
}
