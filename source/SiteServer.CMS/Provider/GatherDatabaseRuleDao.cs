using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Provider
{
    public class GatherDatabaseRuleDao : DataProviderBase
    {
        private const string SqlSelectGatherRule = "SELECT GatherRuleName, PublishmentSystemID, ConnectionString, RelatedTableName, RelatedIdentity, RelatedOrderBy, WhereString, TableMatchID, NodeID, GatherNum, IsChecked, IsOrderByDesc, LastGatherDate, IsAutoCreate FROM siteserver_GatherDatabaseRule WHERE GatherRuleName = @GatherRuleName AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelectTableMatchId = "SELECT TableMatchID FROM siteserver_GatherDatabaseRule WHERE GatherRuleName = @GatherRuleName AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelectAllGatherRuleByPsId = "SELECT GatherRuleName, PublishmentSystemID, ConnectionString, RelatedTableName, RelatedIdentity, RelatedOrderBy, WhereString, TableMatchID, NodeID, GatherNum, IsChecked, IsOrderByDesc, LastGatherDate, IsAutoCreate FROM siteserver_GatherDatabaseRule WHERE PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelectGatherRuleNameByPsId = "SELECT GatherRuleName FROM siteserver_GatherDatabaseRule WHERE PublishmentSystemID = @PublishmentSystemID";

        private const string SqlInsertGatherRule = @"
INSERT INTO siteserver_GatherDatabaseRule 
(GatherRuleName, PublishmentSystemID, ConnectionString, RelatedTableName, RelatedIdentity, RelatedOrderBy, WhereString, TableMatchID, NodeID, GatherNum, IsChecked, IsOrderByDesc, LastGatherDate, IsAutoCreate) VALUES (@GatherRuleName, @PublishmentSystemID, @ConnectionString, @RelatedTableName, @RelatedIdentity, @RelatedOrderBy, @WhereString, @TableMatchID, @NodeID, @GatherNum, @IsChecked, @IsOrderByDesc, @LastGatherDate, @IsAutoCreate)";

        private const string SqlUpdateGatherRule = @"
UPDATE siteserver_GatherDatabaseRule SET ConnectionString = @ConnectionString, RelatedTableName = @RelatedTableName, RelatedIdentity = @RelatedIdentity, RelatedOrderBy = @RelatedOrderBy, WhereString = @WhereString, TableMatchID = @TableMatchID, NodeID = @NodeID, GatherNum = @GatherNum, IsChecked = @IsChecked, IsOrderByDesc = @IsOrderByDesc, LastGatherDate = @LastGatherDate, IsAutoCreate = @IsAutoCreate WHERE GatherRuleName = @GatherRuleName AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlUpdateLastGatherDate = "UPDATE siteserver_GatherDatabaseRule SET LastGatherDate = @LastGatherDate WHERE GatherRuleName = @GatherRuleName AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlDeleteGatherRule = "DELETE FROM siteserver_GatherDatabaseRule WHERE GatherRuleName = @GatherRuleName AND PublishmentSystemID = @PublishmentSystemID";

        private const string ParmGatherRuleName = "@GatherRuleName";
        private const string ParmPublishmentSystemId = "@PublishmentSystemID";
        private const string ParmConnectionString = "@ConnectionString";
        private const string ParmRelatedTableName = "@RelatedTableName";
        private const string ParmRelatedIdentity = "@RelatedIdentity";
        private const string ParmRelatedOrderBy = "@RelatedOrderBy";
        private const string ParmWhereString = "@WhereString";
        private const string ParmTableMatchId = "@TableMatchID";
        private const string ParmNodeId = "@NodeID";
        private const string ParmGatherNum = "@GatherNum";
        private const string ParmIsChecked = "@IsChecked";
        private const string ParmIsOrderByDesc = "@IsOrderByDesc";
        private const string ParmLastGatherDate = "@LastGatherDate";
        private const string ParmIsAutoCreate = "@IsAutoCreate";

        public void Insert(GatherDatabaseRuleInfo gatherDatabaseRuleInfo)
        {
            var insertParms = new IDataParameter[]
            {
                GetParameter(ParmGatherRuleName, EDataType.NVarChar, 50, gatherDatabaseRuleInfo.GatherRuleName),
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, gatherDatabaseRuleInfo.PublishmentSystemId),
                GetParameter(ParmConnectionString, EDataType.VarChar, 255, gatherDatabaseRuleInfo.ConnectionString),
                GetParameter(ParmRelatedTableName, EDataType.VarChar, 255, gatherDatabaseRuleInfo.RelatedTableName),
                GetParameter(ParmRelatedIdentity, EDataType.VarChar, 255, gatherDatabaseRuleInfo.RelatedIdentity),
                GetParameter(ParmRelatedOrderBy, EDataType.VarChar, 255, gatherDatabaseRuleInfo.RelatedOrderBy),
                GetParameter(ParmWhereString, EDataType.NVarChar, 255, gatherDatabaseRuleInfo.WhereString),
                GetParameter(ParmTableMatchId, EDataType.Integer, gatherDatabaseRuleInfo.TableMatchId),
                GetParameter(ParmNodeId, EDataType.Integer, gatherDatabaseRuleInfo.NodeId),
                GetParameter(ParmGatherNum, EDataType.Integer, gatherDatabaseRuleInfo.GatherNum),
                GetParameter(ParmIsChecked, EDataType.VarChar, 18, gatherDatabaseRuleInfo.IsChecked.ToString()),
                GetParameter(ParmIsOrderByDesc, EDataType.VarChar, 18, gatherDatabaseRuleInfo.IsOrderByDesc.ToString()),
                GetParameter(ParmLastGatherDate, EDataType.DateTime, gatherDatabaseRuleInfo.LastGatherDate),
                GetParameter(ParmIsAutoCreate, EDataType.VarChar, 18, gatherDatabaseRuleInfo.IsAutoCreate.ToString())
            };

            ExecuteNonQuery(SqlInsertGatherRule, insertParms);
        }

        public void UpdateLastGatherDate(string gatherRuleName, int publishmentSystemId)
        {
            var parms = new IDataParameter[]
            {
                GetParameter(ParmLastGatherDate, EDataType.DateTime, DateTime.Now),
                GetParameter(ParmGatherRuleName, EDataType.NVarChar, 50, gatherRuleName),
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
            };

            ExecuteNonQuery(SqlUpdateLastGatherDate, parms);
        }

        public void Update(GatherDatabaseRuleInfo gatherDatabaseRuleInfo)
        {
            var updateParms = new IDataParameter[]
            {
                GetParameter(ParmConnectionString, EDataType.VarChar, 255, gatherDatabaseRuleInfo.ConnectionString),
                GetParameter(ParmRelatedTableName, EDataType.VarChar, 255, gatherDatabaseRuleInfo.RelatedTableName),
                GetParameter(ParmRelatedIdentity, EDataType.VarChar, 255, gatherDatabaseRuleInfo.RelatedIdentity),
                GetParameter(ParmRelatedOrderBy, EDataType.VarChar, 255, gatherDatabaseRuleInfo.RelatedOrderBy),
                GetParameter(ParmWhereString, EDataType.NVarChar, 255, gatherDatabaseRuleInfo.WhereString),
                GetParameter(ParmTableMatchId, EDataType.Integer, gatherDatabaseRuleInfo.TableMatchId),
                GetParameter(ParmNodeId, EDataType.Integer, gatherDatabaseRuleInfo.NodeId),
                GetParameter(ParmGatherNum, EDataType.Integer, gatherDatabaseRuleInfo.GatherNum),
                GetParameter(ParmIsChecked, EDataType.VarChar, 18, gatherDatabaseRuleInfo.IsChecked.ToString()),
                GetParameter(ParmIsOrderByDesc, EDataType.VarChar, 18, gatherDatabaseRuleInfo.IsOrderByDesc.ToString()),
                GetParameter(ParmLastGatherDate, EDataType.DateTime, gatherDatabaseRuleInfo.LastGatherDate),
                GetParameter(ParmGatherRuleName, EDataType.NVarChar, 50, gatherDatabaseRuleInfo.GatherRuleName),
                GetParameter(ParmIsAutoCreate, EDataType.VarChar, 18, gatherDatabaseRuleInfo.IsAutoCreate.ToString()),
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, gatherDatabaseRuleInfo.PublishmentSystemId)
            };

            ExecuteNonQuery(SqlUpdateGatherRule, updateParms);
        }


        public void Delete(string gatherRuleName, int publishmentSystemId)
        {
            var parms = new IDataParameter[]
            {
                GetParameter(ParmGatherRuleName, EDataType.NVarChar, 50, gatherRuleName),
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
            };

            ExecuteNonQuery(SqlDeleteGatherRule, parms);
        }

        public GatherDatabaseRuleInfo GetGatherDatabaseRuleInfo(string gatherRuleName, int publishmentSystemId)
        {
            GatherDatabaseRuleInfo gatherDatabaseRuleInfo = null;

            var parms = new IDataParameter[]
            {
                GetParameter(ParmGatherRuleName, EDataType.NVarChar, 50, gatherRuleName),
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
            };

            using (var rdr = ExecuteReader(SqlSelectGatherRule, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    gatherDatabaseRuleInfo = new GatherDatabaseRuleInfo(GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetDateTime(rdr, i++), GetBool(rdr, i));
                }
                rdr.Close();
            }

            return gatherDatabaseRuleInfo;
        }

        public string GetImportGatherRuleName(int publishmentSystemId, string gatherRuleName)
        {
            string importGatherRuleName;
            if (gatherRuleName.IndexOf("_", StringComparison.Ordinal) != -1)
            {
                var gatherRuleNameCount = 0;
                var lastGatherRuleName = gatherRuleName.Substring(gatherRuleName.LastIndexOf("_", StringComparison.Ordinal) + 1);
                var firstGatherRuleName = gatherRuleName.Substring(0, gatherRuleName.Length - lastGatherRuleName.Length);
                try
                {
                    gatherRuleNameCount = int.Parse(lastGatherRuleName);
                }
                catch
                {
                    // ignored
                }
                gatherRuleNameCount++;
                importGatherRuleName = firstGatherRuleName + gatherRuleNameCount;
            }
            else
            {
                importGatherRuleName = gatherRuleName + "_1";
            }

            var parms = new IDataParameter[]
            {
                GetParameter(ParmGatherRuleName, EDataType.NVarChar, 50, importGatherRuleName),
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
            };

            using (var rdr = ExecuteReader(SqlSelectGatherRule, parms))
            {
                if (rdr.Read())
                {
                    importGatherRuleName = GetImportGatherRuleName(publishmentSystemId, importGatherRuleName);
                }
                rdr.Close();
            }

            return importGatherRuleName;
        }

        public IEnumerable GetDataSource(int publishmentSystemId)
        {
            var parms = new IDataParameter[]
            {
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
            };

            var enumerable = (IEnumerable)ExecuteReader(SqlSelectAllGatherRuleByPsId, parms);
            return enumerable;
        }

        public ArrayList GetGatherDatabaseRuleInfoArrayList(int publishmentSystemId)
        {
            var list = new ArrayList();

            var parms = new IDataParameter[]
            {
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
            };

            using (var rdr = ExecuteReader(SqlSelectAllGatherRuleByPsId, parms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var gatherDatabaseRuleInfo = new GatherDatabaseRuleInfo(GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetDateTime(rdr, i++), GetBool(rdr, i));
                    list.Add(gatherDatabaseRuleInfo);
                }
                rdr.Close();
            }

            return list;
        }

        public List<string> GetGatherRuleNameArrayList(int publishmentSystemId)
        {
            var list = new List<string>();

            var parms = new IDataParameter[]
            {
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
            };

            using (var rdr = ExecuteReader(SqlSelectGatherRuleNameByPsId, parms))
            {
                while (rdr.Read())
                {
                    list.Add(GetString(rdr, 0));
                }
                rdr.Close();
            }

            return list;
        }

        public int GetTableMatchId(string gatherRuleName, int publishmentSystemId)
        {
            var tableMatchId = 0;

            var parms = new IDataParameter[]
            {
                GetParameter(ParmGatherRuleName, EDataType.NVarChar, 50, gatherRuleName),
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
            };

            using (var rdr = ExecuteReader(SqlSelectTableMatchId, parms))
            {
                if (rdr.Read())
                {
                    tableMatchId = GetInt(rdr, 0);
                }
                rdr.Close();
            }

            return tableMatchId;
        }


        public void OpenAuto(int publishmentSystemId, List<string> gatherRuleNameCollection)
        {
            string sql =
                $"UPDATE siteserver_GatherDatabaseRule SET IsAutoCreate = 'True' WHERE PublishmentSystemID = {publishmentSystemId} AND GatherRuleName in ({TranslateUtils.ToSqlInStringWithQuote(gatherRuleNameCollection)})";
            ExecuteNonQuery(sql);
        }

        public void CloseAuto(int publishmentSystemId, List<string> gatherRuleNameCollection)
        {
            string sql =
                $"UPDATE siteserver_GatherDatabaseRule SET IsAutoCreate = 'False' WHERE PublishmentSystemID = {publishmentSystemId} AND GatherRuleName in ({TranslateUtils.ToSqlInStringWithQuote(gatherRuleNameCollection)})";
            ExecuteNonQuery(sql);
        }

    }
}
