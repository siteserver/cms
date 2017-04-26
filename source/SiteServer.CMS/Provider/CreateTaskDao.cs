using System.Collections.Generic;
using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Provider
{
    public class CreateTaskDao : DataProviderBase
    {
        private const string ParmId = "@ID";
        private const string ParmCreateType = "@CreateType";
        private const string ParmPublishmentSystemId = "@PublishmentSystemID";
        private const string ParmChannelId = "@ChannelID";
        private const string ParmContentId = "@ContentID";
        private const string ParmTemplateId = "@TemplateID";

        public void Insert(CreateTaskInfo info)
        {
            const string sqlString = "INSERT INTO siteserver_CreateTask (CreateType, PublishmentSystemID, ChannelID, ContentID, TemplateID) VALUES (@CreateType, @PublishmentSystemID, @ChannelID, @ContentID, @TemplateID)";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmCreateType, EDataType.NVarChar, 50, ECreateTypeUtils.GetValue(info.CreateType)),
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, info.PublishmentSystemID),
                GetParameter(ParmChannelId, EDataType.Integer, info.ChannelID),
                GetParameter(ParmContentId, EDataType.Integer, info.ContentID),
                GetParameter(ParmTemplateId, EDataType.Integer, info.TemplateID),
            };

            ExecuteNonQuery(sqlString, parms);
            ServiceManager.ClearIsPendingCreateCache();
        }

        public void Delete(int taskId)
        {
            const string sqlString = "DELETE FROM siteserver_CreateTask WHERE ID = @ID";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmId, EDataType.Integer, taskId)
			};

            ExecuteNonQuery(sqlString, parms);
        }

        public void DeleteByPublishmentSystemId(int publishmentSystemId)
        {
            const string sqlString = "DELETE FROM siteserver_CreateTask WHERE PublishmentSystemID = @PublishmentSystemID";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
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

            const string sqlString = @"SELECT ID FROM siteserver_CreateTask WHERE CreateType = @CreateType AND PublishmentSystemID = @PublishmentSystemID AND ChannelID = @ChannelID AND ContentID = @ContentID AND TemplateID = @TemplateID";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmCreateType, EDataType.NVarChar, 50, ECreateTypeUtils.GetValue(info.CreateType)),
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, info.PublishmentSystemID),
                GetParameter(ParmChannelId, EDataType.Integer, info.ChannelID),
                GetParameter(ParmContentId, EDataType.Integer, info.ContentID),
                GetParameter(ParmTemplateId, EDataType.Integer, info.TemplateID),
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

        public List<CreateTaskInfo> GetList(int publishmentSystemId, int totalNum)
        {
            var list = new List<CreateTaskInfo>();

            if (publishmentSystemId > 0)
            {
                //string sqlString =
                //    $"SELECT TOP {totalNum} ID, CreateType, PublishmentSystemID, ChannelID, ContentID, TemplateID FROM siteserver_CreateTask WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY ID DESC";
                var sqlString = SqlUtils.GetTopSqlString("siteserver_CreateTask", "ID, CreateType, PublishmentSystemID, ChannelID, ContentID, TemplateID", "WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY ID DESC", totalNum);

                var parms = new IDataParameter[]
                {
                    GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
                };

                using (var rdr = ExecuteReader(sqlString, parms))
                {
                    while (rdr.Read())
                    {
                        var i = 0;
                        var info = new CreateTaskInfo(GetInt(rdr, i++), ECreateTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i));
                        list.Add(info);
                    }
                    rdr.Close();
                }
            }
            else
            {
                //string sqlString =
                //    $"SELECT TOP {totalNum} ID, CreateType, PublishmentSystemID, ChannelID, ContentID, TemplateID FROM siteserver_CreateTask ORDER BY ID DESC";
                var sqlString = SqlUtils.GetTopSqlString("siteserver_CreateTask", "ID, CreateType, PublishmentSystemID, ChannelID, ContentID, TemplateID", "ORDER BY ID DESC", totalNum);

                using (var rdr = ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        var i = 0;
                        var info = new CreateTaskInfo(GetInt(rdr, i++), ECreateTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i));
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

            //var sqlString = "SELECT TOP 1 ID, CreateType, PublishmentSystemID, ChannelID, ContentID, TemplateID FROM siteserver_CreateTask ORDER BY ID";
            var sqlString = SqlUtils.GetTopSqlString("siteserver_CreateTask", "ID, CreateType, PublishmentSystemID, ChannelID, ContentID, TemplateID", "ORDER BY ID", 1);

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    info = new CreateTaskInfo(GetInt(rdr, i++), ECreateTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i));
                }
                rdr.Close();
            }
            return info;
        }

        public bool IsPendingTask()
        {
            var retval = false;

            var sqlString = SqlUtils.GetTopSqlString("siteserver_CreateTask", "ID", "ORDER BY ID", 1);

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    retval = true;
                }
                rdr.Close();
            }
            return retval;
        }

        public void GetCount(int publishmentSystemId, out int indexCount, out int channelsCount, out int contentsCount, out int filesCount)
        {
            indexCount = 0;
            channelsCount = 0;
            contentsCount = 0;
            filesCount = 0;

            if (publishmentSystemId > 0)
            {
                const string sqlString = "SELECT COUNT(*) AS TOTAL, CreateType FROM siteserver_CreateTask WHERE PublishmentSystemID = @PublishmentSystemID GROUP BY CreateType";

                var parms = new IDataParameter[]
                {
                    GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
                };

                using (var rdr = ExecuteReader(sqlString, parms))
                {
                    while (rdr.Read())
                    {
                        var total = GetInt(rdr, 0);
                        var createType = ECreateTypeUtils.GetEnumType(GetString(rdr, 1));

                        if (createType == ECreateType.Index)
                        {
                            indexCount += total;
                        }
                        else if (createType == ECreateType.Channel)
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

                        if (createType == ECreateType.Index)
                        {
                            indexCount += total;
                        }
                        else if (createType == ECreateType.Channel)
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
