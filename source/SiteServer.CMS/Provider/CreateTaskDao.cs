using System.Collections.Generic;
using System.Data;
using BaiRong.Core.Data;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Plugin;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Provider
{
    public class CreateTaskDao : DataProviderBase
    {
        private const string ParmId = "@Id";
        private const string ParmCreateType = "@CreateType";
        private const string ParmPublishmentSystemId = "@PublishmentSystemId";
        private const string ParmChannelId = "@ChannelId";
        private const string ParmContentId = "@ContentId";
        private const string ParmTemplateId = "@TemplateId";

        public void Insert(CreateTaskInfo info)
        {
            const string sqlString = "INSERT INTO siteserver_CreateTask (CreateType, PublishmentSystemId, ChannelId, ContentId, TemplateId) VALUES (@CreateType, @PublishmentSystemId, @ChannelId, @ContentId, @TemplateId)";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmCreateType, DataType.NVarChar, 50, ECreateTypeUtils.GetValue(info.CreateType)),
                GetParameter(ParmPublishmentSystemId, DataType.Integer, info.PublishmentSystemId),
                GetParameter(ParmChannelId, DataType.Integer, info.ChannelId),
                GetParameter(ParmContentId, DataType.Integer, info.ContentId),
                GetParameter(ParmTemplateId, DataType.Integer, info.TemplateId)
            };

            ExecuteNonQuery(sqlString, parms);
            ServiceManager.ClearIsPendingCreateCache();
        }

        public void Delete(int taskId)
        {
            const string sqlString = "DELETE FROM siteserver_CreateTask WHERE Id = @Id";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmId, DataType.Integer, taskId)
			};

            ExecuteNonQuery(sqlString, parms);
        }

        public void DeleteAll()
        {
            const string sqlString = "DELETE FROM siteserver_CreateTask";

            ExecuteNonQuery(sqlString);
        }

        public void DeleteByPublishmentSystemId(int publishmentSystemId)
        {
            const string sqlString = "DELETE FROM siteserver_CreateTask WHERE PublishmentSystemId = @PublishmentSystemId";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
            };

            ExecuteNonQuery(sqlString, parms);
        }

        public bool IsExists(CreateTaskInfo info)
        {
            if (info == null)
            {
                return false;
            }

            var exists = false;

            const string sqlString = @"SELECT Id FROM siteserver_CreateTask WHERE CreateType = @CreateType AND PublishmentSystemId = @PublishmentSystemId AND ChannelId = @ChannelId AND ContentId = @ContentId AND TemplateId = @TemplateId";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmCreateType, DataType.NVarChar, 50, ECreateTypeUtils.GetValue(info.CreateType)),
                GetParameter(ParmPublishmentSystemId, DataType.Integer, info.PublishmentSystemId),
                GetParameter(ParmChannelId, DataType.Integer, info.ChannelId),
                GetParameter(ParmContentId, DataType.Integer, info.ContentId),
                GetParameter(ParmTemplateId, DataType.Integer, info.TemplateId)
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

        public List<CreateTaskInfo> GetList(int siteId, int totalNum)
        {
            var list = new List<CreateTaskInfo>();

            if (siteId > 0)
            {
                //string sqlString =
                //    $"SELECT TOP {totalNum} Id, CreateType, PublishmentSystemId, ChannelId, ContentId, TemplateId FROM siteserver_CreateTask WHERE PublishmentSystemId = @PublishmentSystemId ORDER BY Id DESC";
                var sqlString = SqlUtils.GetTopSqlString("siteserver_CreateTask", "Id, CreateType, PublishmentSystemId, ChannelId, ContentId, TemplateId", "WHERE PublishmentSystemId = @PublishmentSystemId ORDER BY Id DESC", totalNum);

                var parms = new IDataParameter[]
                {
                    GetParameter(ParmPublishmentSystemId, DataType.Integer, siteId)
                };

                using (var rdr = ExecuteReader(sqlString, parms))
                {
                    while (rdr.Read())
                    {
                        var i = 0;

                        var id = GetInt(rdr, i++);
                        var createType = ECreateTypeUtils.GetEnumType(GetString(rdr, i++));
                        var publishmentSystemId = GetInt(rdr, i++);
                        var channelId = GetInt(rdr, i++);
                        var contentId = GetInt(rdr, i++);
                        var templateId = GetInt(rdr, i);
                        int pageCount;
                        var name = CreateManager.GetTaskName(createType, publishmentSystemId, channelId, contentId,
                            templateId, out pageCount);

                        var info = new CreateTaskInfo(id, name, createType, publishmentSystemId, channelId, contentId, templateId, pageCount);
                        list.Add(info);
                    }
                    rdr.Close();
                }
            }
            else
            {
                //string sqlString =
                //    $"SELECT TOP {totalNum} Id, CreateType, PublishmentSystemId, ChannelId, ContentId, TemplateId FROM siteserver_CreateTask ORDER BY Id DESC";
                var sqlString = SqlUtils.GetTopSqlString("siteserver_CreateTask", "Id, CreateType, PublishmentSystemId, ChannelId, ContentId, TemplateId", "ORDER BY Id DESC", totalNum);

                using (var rdr = ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        var i = 0;

                        var id = GetInt(rdr, i++);
                        var createType = ECreateTypeUtils.GetEnumType(GetString(rdr, i++));
                        var publishmentSystemId = GetInt(rdr, i++);
                        var channelId = GetInt(rdr, i++);
                        var contentId = GetInt(rdr, i++);
                        var templateId = GetInt(rdr, i);
                        int pageCount;
                        var name = CreateManager.GetTaskName(createType, publishmentSystemId, channelId, contentId,
                            templateId, out pageCount);

                        var info = new CreateTaskInfo(id, name, createType, publishmentSystemId, channelId, contentId, templateId, pageCount);
                        list.Add(info);
                    }
                    rdr.Close();
                }
            }

            return list;
        }

        public CreateTaskInfo GetLastPendingTask()
        {
            CreateTaskInfo info = null;

            //var sqlString = "SELECT TOP 1 Id, CreateType, PublishmentSystemId, ChannelId, ContentId, TemplateId FROM siteserver_CreateTask ORDER BY Id";
            var sqlString = SqlUtils.GetTopSqlString("siteserver_CreateTask", "Id, CreateType, PublishmentSystemId, ChannelId, ContentId, TemplateId", "ORDER BY Id", 1);

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    var id = GetInt(rdr, i++);
                    var createType = ECreateTypeUtils.GetEnumType(GetString(rdr, i++));
                    var publishmentSystemId = GetInt(rdr, i++);
                    var channelId = GetInt(rdr, i++);
                    var contentId = GetInt(rdr, i++);
                    var templateId = GetInt(rdr, i);
                    int pageCount;
                    var name = CreateManager.GetTaskName(createType, publishmentSystemId, channelId, contentId,
                        templateId, out pageCount);

                    info = new CreateTaskInfo(id, name, createType, publishmentSystemId, channelId, contentId, templateId, pageCount);
                }
                rdr.Close();
            }

            return info;
        }

        /// <summary>
        /// 一次获取多个任务
        /// </summary>
        /// <param name="topNum"></param>
        /// <returns></returns>
        public List<CreateTaskInfo> GetLastPendingTasks(int topNum)
        {
            var list = new List<CreateTaskInfo>();

            var sqlString = SqlUtils.GetTopSqlString("siteserver_CreateTask", "Id, CreateType, PublishmentSystemId, ChannelId, ContentId, TemplateId", "ORDER BY Id", topNum);

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var id = GetInt(rdr, i++);
                    var createType = ECreateTypeUtils.GetEnumType(GetString(rdr, i++));
                    var publishmentSystemId = GetInt(rdr, i++);
                    var channelId = GetInt(rdr, i++);
                    var contentId = GetInt(rdr, i++);
                    var templateId = GetInt(rdr, i);
                    int pageCount;
                    var name = CreateManager.GetTaskName(createType, publishmentSystemId, channelId, contentId,
                        templateId, out pageCount);
                    var info = new CreateTaskInfo(id, name, createType, publishmentSystemId, channelId, contentId, templateId, pageCount);

                    list.Add(info);
                }
                rdr.Close();
            }
            return list;
        }

        public bool IsPendingTask()
        {
            var retval = false;

            var sqlString = SqlUtils.GetTopSqlString("siteserver_CreateTask", "Id", "ORDER BY Id", 1);

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    retval = true;
                }
                rdr.Close();
            }
            return retval;
        }

        public void GetCount(int publishmentSystemId, out int channelsCount, out int contentsCount, out int filesCount)
        {
            channelsCount = 0;
            contentsCount = 0;
            filesCount = 0;

            if (publishmentSystemId > 0)
            {
                const string sqlString = "SELECT COUNT(*) AS TOTAL, CreateType FROM siteserver_CreateTask WHERE PublishmentSystemId = @PublishmentSystemId GROUP BY CreateType";

                var parms = new IDataParameter[]
                {
                    GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
                };

                using (var rdr = ExecuteReader(sqlString, parms))
                {
                    while (rdr.Read() && !rdr.IsDBNull(0))
                    {
                        var total = GetInt(rdr, 0);
                        var createType = ECreateTypeUtils.GetEnumType(GetString(rdr, 1));

                        if (createType == ECreateType.Channel)
                        {
                            channelsCount += total;
                        }
                        else if (createType == ECreateType.Content)
                        {
                            contentsCount += total;
                        }
                        else if (createType == ECreateType.File)
                        {
                            filesCount += total;
                        }
                        else if (createType == ECreateType.AllContent)
                        {
                            contentsCount += total;
                        }
                    }
                    rdr.Close();
                }
            }
            else
            {
                const string sqlString = "SELECT COUNT(*) AS TOTAL, CreateType FROM siteserver_CreateTask GROUP BY CreateType";

                using (var rdr = ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        var total = GetInt(rdr, 0);
                        var createType = ECreateTypeUtils.GetEnumType(GetString(rdr, 1));

                        if (createType == ECreateType.Channel)
                        {
                            channelsCount += total;
                        }
                        else if (createType == ECreateType.Content)
                        {
                            contentsCount += total;
                        }
                        else if (createType == ECreateType.File)
                        {
                            filesCount += total;
                        }
                        else if (createType == ECreateType.AllContent)
                        {
                            contentsCount += total;
                        }
                    }
                    rdr.Close();
                }
            }
        }
    }
}
