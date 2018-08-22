using System.Collections.Generic;
using System.Data;
using SiteServer.CMS.Data;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.CMS.Provider
{
    public class TemplateMatchDao : DataProviderBase
    {
        public override string TableName => "siteserver_TemplateMatch";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(TemplateMatchInfo.Id),
                DataType = DataType.Integer,
                IsPrimaryKey = true,
                IsIdentity = true
            },
            new TableColumn
            {
                AttributeName = nameof(TemplateMatchInfo.ChannelId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(TemplateMatchInfo.SiteId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(TemplateMatchInfo.ChannelTemplateId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(TemplateMatchInfo.ContentTemplateId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(TemplateMatchInfo.FilePath),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(TemplateMatchInfo.ChannelFilePathRule),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(TemplateMatchInfo.ContentFilePathRule),
                DataType = DataType.VarChar,
                DataLength = 200
            }
        };

        private const string SqlSelect = "SELECT Id, ChannelId, SiteId, ChannelTemplateId, ContentTemplateId, FilePath, ChannelFilePathRule, ContentFilePathRule FROM siteserver_TemplateMatch WHERE ChannelId = @ChannelId";

        private const string SqlInsert = "INSERT INTO siteserver_TemplateMatch (ChannelId, SiteId, ChannelTemplateId, ContentTemplateId, FilePath, ChannelFilePathRule, ContentFilePathRule) VALUES (@ChannelId, @SiteId, @ChannelTemplateId, @ContentTemplateId, @FilePath, @ChannelFilePathRule, @ContentFilePathRule)";

        private const string SqlUpdate = "UPDATE siteserver_TemplateMatch SET ChannelTemplateId = @ChannelTemplateId, ContentTemplateId = @ContentTemplateId, FilePath = @FilePath, ChannelFilePathRule = @ChannelFilePathRule, ContentFilePathRule = @ContentFilePathRule WHERE ChannelId = @ChannelId";

        private const string SqlDelete = "DELETE FROM siteserver_TemplateMatch WHERE ChannelId = @ChannelId";

        private const string ParmChannelId = "@ChannelId";
        private const string ParmSiteId = "@SiteId";
        private const string ParmChannelTemplateId = "@ChannelTemplateId";
        private const string ParmContentTemplateId = "@ContentTemplateId";
        private const string ParmFilepath = "@FilePath";
        private const string ParmChannelFilepathRule = "@ChannelFilePathRule";
        private const string ParmContentFilepathRule = "@ContentFilePathRule";

        public void Insert(TemplateMatchInfo info)
        {
            var insertParms = new IDataParameter[]
		    {
                GetParameter(ParmChannelId, DataType.Integer, info.ChannelId),
                GetParameter(ParmSiteId, DataType.Integer, info.SiteId),
			    GetParameter(ParmChannelTemplateId, DataType.Integer, info.ChannelTemplateId),
                GetParameter(ParmContentTemplateId, DataType.Integer, info.ContentTemplateId),
                GetParameter(ParmFilepath, DataType.VarChar, 200, info.FilePath),
                GetParameter(ParmChannelFilepathRule, DataType.VarChar, 200, info.ChannelFilePathRule),
                GetParameter(ParmContentFilepathRule, DataType.VarChar, 200, info.ContentFilePathRule)
		    };

            ExecuteNonQuery(SqlInsert, insertParms);
        }

        public void Update(TemplateMatchInfo info)
        {
            var updateParms = new IDataParameter[]
		    {
			    GetParameter(ParmChannelTemplateId, DataType.Integer, info.ChannelTemplateId),
                GetParameter(ParmContentTemplateId, DataType.Integer, info.ContentTemplateId),
                GetParameter(ParmFilepath, DataType.VarChar, 200, info.FilePath),
                GetParameter(ParmChannelFilepathRule, DataType.VarChar, 200, info.ChannelFilePathRule),
                GetParameter(ParmContentFilepathRule, DataType.VarChar, 200, info.ContentFilePathRule),
                GetParameter(ParmChannelId, DataType.Integer, info.ChannelId)
		    };

            ExecuteNonQuery(SqlUpdate, updateParms);
        }

        public void Delete(int channelId)
        {

            var parms = new IDataParameter[]
			{
				GetParameter(ParmChannelId, DataType.Integer, channelId)
			};

            ExecuteNonQuery(SqlDelete, parms);
        }

        public TemplateMatchInfo GetTemplateMatchInfo(int channelId)
        {
            TemplateMatchInfo info = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmChannelId, DataType.Integer, channelId)
			};

            using (var rdr = ExecuteReader(SqlSelect, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    info = new TemplateMatchInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

            return info;
        }

        public bool IsExists(int channelId)
        {
            var isExists = false;

            string sqlString = $"SELECT ChannelId FROM siteserver_TemplateMatch WHERE ChannelId = {channelId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    isExists = true;
                }
                rdr.Close();
            }

            return isExists;
        }

        public Dictionary<int, int> GetChannelTemplateIdDict(int siteId)
        {
            var dict = new Dictionary<int, int>();

            string sqlString =
                $"SELECT ChannelId, ChannelTemplateId FROM siteserver_TemplateMatch WHERE SiteId = {siteId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    dict[GetInt(rdr, 0)] = GetInt(rdr, 1);
                }
                rdr.Close();
            }

            return dict;
        }

        public Dictionary<int, int> GetContentTemplateIdDict(int siteId)
        {
            var dict = new Dictionary<int, int>();

            string sqlString =
                $"SELECT ChannelId, ContentTemplateId FROM siteserver_TemplateMatch WHERE SiteId = {siteId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    dict[GetInt(rdr, 0)] = GetInt(rdr, 1);
                }
                rdr.Close();
            }

            return dict;
        }

        public int GetChannelTemplateId(int channelId)
        {
            var templateId = 0;

            string sqlString = $"SELECT ChannelTemplateId FROM siteserver_TemplateMatch WHERE ChannelId = {channelId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    templateId = GetInt(rdr, 0);
                }
                rdr.Close();
            }

            return templateId;
        }

        public int GetContentTemplateId(int channelId)
        {
            var templateId = 0;

            string sqlString = $"SELECT ContentTemplateId FROM siteserver_TemplateMatch WHERE ChannelId = {channelId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    templateId = GetInt(rdr, 0);
                }
                rdr.Close();
            }

            return templateId;
        }

        public string GetFilePath(int channelId)
        {
            var filePath = string.Empty;

            string sqlString = $"SELECT FilePath FROM siteserver_TemplateMatch WHERE ChannelId = {channelId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    filePath = GetString(rdr, 0);
                }
                rdr.Close();
            }

            return filePath;
        }

        public string GetChannelFilePathRule(int channelId)
        {
            var filePathRule = string.Empty;

            string sqlString = $"SELECT ChannelFilePathRule FROM siteserver_TemplateMatch WHERE ChannelId = {channelId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    filePathRule = GetString(rdr, 0);
                }
                rdr.Close();
            }

            return filePathRule;
        }

        public string GetContentFilePathRule(int channelId)
        {
            var filePathRule = string.Empty;

            string sqlString = $"SELECT ContentFilePathRule FROM siteserver_TemplateMatch WHERE ChannelId = {channelId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    filePathRule = GetString(rdr, 0);
                }
                rdr.Close();
            }

            return filePathRule;
        }

        public List<string> GetAllFilePathBySiteId(int siteId)
        {
            var list = new List<string>();

            string sqlString =
                $"SELECT FilePath FROM siteserver_TemplateMatch WHERE FilePath <> '' AND SiteId = {siteId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetString(rdr, 0));
                }
                rdr.Close();
            }
            return list;
        }
    }
}
