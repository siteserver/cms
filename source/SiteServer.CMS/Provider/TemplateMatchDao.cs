using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Provider
{
    public class TemplateMatchDao : DataProviderBase
    {
        private const string SqlSelect = "SELECT NodeID, PublishmentSystemID, ChannelTemplateID, ContentTemplateID, FilePath, ChannelFilePathRule, ContentFilePathRule FROM siteserver_TemplateMatch WHERE NodeID = @NodeID";

        private const string SqlInsert = "INSERT INTO siteserver_TemplateMatch (NodeID, PublishmentSystemID, ChannelTemplateID, ContentTemplateID, FilePath, ChannelFilePathRule, ContentFilePathRule) VALUES (@NodeID, @PublishmentSystemID, @ChannelTemplateID, @ContentTemplateID, @FilePath, @ChannelFilePathRule, @ContentFilePathRule)";

        private const string SqlUpdate = "UPDATE siteserver_TemplateMatch SET ChannelTemplateID = @ChannelTemplateID, ContentTemplateID = @ContentTemplateID, FilePath = @FilePath, ChannelFilePathRule = @ChannelFilePathRule, ContentFilePathRule = @ContentFilePathRule WHERE NodeID = @NodeID";

        private const string SqlDelete = "DELETE FROM siteserver_TemplateMatch WHERE NodeID = @NodeID";

        private const string ParmNodeId = "@NodeID";
        private const string ParmPublishmentSystemId = "@PublishmentSystemID";
        private const string ParmChannelTemplateId = "@ChannelTemplateID";
        private const string ParmContentTemplateId = "@ContentTemplateID";
        private const string ParmFilepath = "@FilePath";
        private const string ParmChannelFilepathRule = "@ChannelFilePathRule";
        private const string ParmContentFilepathRule = "@ContentFilePathRule";

        public void Insert(TemplateMatchInfo info)
        {
            var insertParms = new IDataParameter[]
		    {
                GetParameter(ParmNodeId, EDataType.Integer, info.NodeId),
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, info.PublishmentSystemId),
			    GetParameter(ParmChannelTemplateId, EDataType.Integer, info.ChannelTemplateId),
                GetParameter(ParmContentTemplateId, EDataType.Integer, info.ContentTemplateId),
                GetParameter(ParmFilepath, EDataType.VarChar, 200, info.FilePath),
                GetParameter(ParmChannelFilepathRule, EDataType.VarChar, 200, info.ChannelFilePathRule),
                GetParameter(ParmContentFilepathRule, EDataType.VarChar, 200, info.ContentFilePathRule)
		    };

            ExecuteNonQuery(SqlInsert, insertParms);
        }

        public void Update(TemplateMatchInfo info)
        {
            var updateParms = new IDataParameter[]
		    {
			    GetParameter(ParmChannelTemplateId, EDataType.Integer, info.ChannelTemplateId),
                GetParameter(ParmContentTemplateId, EDataType.Integer, info.ContentTemplateId),
                GetParameter(ParmFilepath, EDataType.VarChar, 200, info.FilePath),
                GetParameter(ParmChannelFilepathRule, EDataType.VarChar, 200, info.ChannelFilePathRule),
                GetParameter(ParmContentFilepathRule, EDataType.VarChar, 200, info.ContentFilePathRule),
                GetParameter(ParmNodeId, EDataType.Integer, info.NodeId)
		    };

            ExecuteNonQuery(SqlUpdate, updateParms);
        }

        public void Delete(int nodeId)
        {

            var parms = new IDataParameter[]
			{
				GetParameter(ParmNodeId, EDataType.Integer, nodeId)
			};

            ExecuteNonQuery(SqlDelete, parms);
        }

        public TemplateMatchInfo GetTemplateMatchInfo(int nodeId)
        {
            TemplateMatchInfo info = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmNodeId, EDataType.Integer, nodeId)
			};

            using (var rdr = ExecuteReader(SqlSelect, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    info = new TemplateMatchInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

            return info;
        }

        public bool IsExists(int nodeId)
        {
            var isExists = false;

            string sqlString = $"SELECT NodeID FROM siteserver_TemplateMatch WHERE NodeID = {nodeId}";

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

        public Hashtable GetChannelTemplateIdHashtable(int publishmentSystemId)
        {
            var hashtable = new Hashtable();

            string sqlString =
                $"SELECT NodeID, ChannelTemplateID FROM siteserver_TemplateMatch WHERE PublishmentSystemID = {publishmentSystemId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    hashtable.Add(GetInt(rdr, 0), GetInt(rdr, 1));
                }
                rdr.Close();
            }

            return hashtable;
        }

        public Hashtable GetContentTemplateIdHashtable(int publishmentSystemId)
        {
            var hashtable = new Hashtable();

            string sqlString =
                $"SELECT NodeID, ContentTemplateID FROM siteserver_TemplateMatch WHERE PublishmentSystemID = {publishmentSystemId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    hashtable.Add(GetInt(rdr, 0), GetInt(rdr, 1));
                }
                rdr.Close();
            }

            return hashtable;
        }

        public int GetChannelTemplateId(int nodeId)
        {
            var templateId = 0;

            string sqlString = $"SELECT ChannelTemplateID FROM siteserver_TemplateMatch WHERE NodeID = {nodeId}";

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

        public int GetContentTemplateId(int nodeId)
        {
            var templateId = 0;

            string sqlString = $"SELECT ContentTemplateID FROM siteserver_TemplateMatch WHERE NodeID = {nodeId}";

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

        public string GetFilePath(int nodeId)
        {
            var filePath = string.Empty;

            string sqlString = $"SELECT FilePath FROM siteserver_TemplateMatch WHERE NodeID = {nodeId}";

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

        public string GetChannelFilePathRule(int nodeId)
        {
            var filePathRule = string.Empty;

            string sqlString = $"SELECT ChannelFilePathRule FROM siteserver_TemplateMatch WHERE NodeID = {nodeId}";

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

        public string GetContentFilePathRule(int nodeId)
        {
            var filePathRule = string.Empty;

            string sqlString = $"SELECT ContentFilePathRule FROM siteserver_TemplateMatch WHERE NodeID = {nodeId}";

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

        public List<string> GetAllFilePathByPublishmentSystemId(int publishmentSystemId)
        {
            var list = new List<string>();

            string sqlString =
                $"SELECT FilePath FROM siteserver_TemplateMatch WHERE FilePath <> '' AND PublishmentSystemID = {publishmentSystemId}";

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
