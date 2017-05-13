using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Provider
{
    public class PublishmentSystemDao : DataProviderBase
    {
        public string TableName => "siteserver_PublishmentSystem";

        private const string SqlSelectPublishmentSystemAll = "SELECT PublishmentSystemID, PublishmentSystemName, PublishmentSystemType, AuxiliaryTableForContent, AuxiliaryTableForGovPublic, AuxiliaryTableForGovInteract, AuxiliaryTableForVote, AuxiliaryTableForJob, IsCheckContentUseLevel, CheckContentLevel, PublishmentSystemDir, PublishmentSystemUrl, IsHeadquarters, ParentPublishmentSystemID, Taxis, SettingsXML FROM siteserver_PublishmentSystem ORDER BY Taxis";

        private const string SqlSelectAllWithNode = "SELECT p.PublishmentSystemID, p.PublishmentSystemName, p.PublishmentSystemType, p.AuxiliaryTableForContent, p.AuxiliaryTableForGovPublic, p.AuxiliaryTableForGovInteract, p.AuxiliaryTableForVote, p.AuxiliaryTableForJob, p.IsCheckContentUseLevel, p.CheckContentLevel, p.PublishmentSystemDir, p.PublishmentSystemUrl, p.IsHeadquarters, p.ParentPublishmentSystemID, p.Taxis, n.NodeName FROM siteserver_PublishmentSystem p INNER JOIN siteserver_Node n ON (p.PublishmentSystemID = n.NodeID) ORDER BY p.IsHeadquarters DESC, p.ParentPublishmentSystemID, p.Taxis DESC, n.NodeID";

        private const string SqlInsertPublishmentSystem = "INSERT INTO siteserver_PublishmentSystem (PublishmentSystemID, PublishmentSystemName, PublishmentSystemType, AuxiliaryTableForContent, AuxiliaryTableForGovPublic, AuxiliaryTableForGovInteract, AuxiliaryTableForVote, AuxiliaryTableForJob, IsCheckContentUseLevel, CheckContentLevel, PublishmentSystemDir, PublishmentSystemUrl, IsHeadquarters, ParentPublishmentSystemID, Taxis, SettingsXML) VALUES (@PublishmentSystemID, @PublishmentSystemName, @PublishmentSystemType, @AuxiliaryTableForContent, @AuxiliaryTableForGovPublic, @AuxiliaryTableForGovInteract, @AuxiliaryTableForVote, @AuxiliaryTableForJob, @IsCheckContentUseLevel, @CheckContentLevel, @PublishmentSystemDir, @PublishmentSystemUrl, @IsHeadquarters, @ParentPublishmentSystemID, @Taxis, @SettingsXML)";

        private const string SqlUpdatePublishmentSystem = "UPDATE siteserver_PublishmentSystem SET PublishmentSystemName = @PublishmentSystemName, PublishmentSystemType = @PublishmentSystemType, AuxiliaryTableForContent = @AuxiliaryTableForContent, AuxiliaryTableForGovPublic = @AuxiliaryTableForGovPublic, AuxiliaryTableForGovInteract = @AuxiliaryTableForGovInteract, AuxiliaryTableForVote = @AuxiliaryTableForVote, AuxiliaryTableForJob = @AuxiliaryTableForJob, IsCheckContentUseLevel = @IsCheckContentUseLevel, CheckContentLevel = @CheckContentLevel, PublishmentSystemDir = @PublishmentSystemDir, PublishmentSystemUrl = @PublishmentSystemUrl, IsHeadquarters = @IsHeadquarters, ParentPublishmentSystemID = @ParentPublishmentSystemID, Taxis = @Taxis, SettingsXML = @SettingsXML WHERE  PublishmentSystemID = @PublishmentSystemID";

        private const string SqlUpdateAllIsHeadquarters = "UPDATE siteserver_PublishmentSystem SET IsHeadquarters = @IsHeadquarters";

        private const string SqlSelectPublishmentsystemDirByIsHeadquarters = "SELECT PublishmentSystemDir FROM siteserver_PublishmentSystem WHERE IsHeadquarters = @IsHeadquarters";

        private const string SqlSelectPublishmentsystemIdByParent = "SELECT PublishmentSystemID FROM siteserver_PublishmentSystem WHERE ParentPublishmentSystemID = @ParentPublishmentSystemID";

        private const string SqlSelectPublishmentsystemIdByIsHeadquarters = "SELECT PublishmentSystemID FROM siteserver_PublishmentSystem WHERE IsHeadquarters = @IsHeadquarters";

        private const string SqlSelectPublishmentsystemIdByPublishmentsystemDir = "SELECT PublishmentSystemID FROM siteserver_PublishmentSystem WHERE PublishmentSystemDir = @PublishmentSystemDir";

        private const string ParmPublishmentsystemId = "@PublishmentSystemID";
        private const string ParmPublishmentsystemName = "@PublishmentSystemName";
        private const string ParmPublishmentsystemType = "@PublishmentSystemType";
        private const string ParmAuxiliaryTableForContent = "@AuxiliaryTableForContent";
        private const string ParmAuxiliaryTableForGovpublic = "@AuxiliaryTableForGovPublic";
        private const string ParmAuxiliaryTableForGovinteract = "@AuxiliaryTableForGovInteract";
        private const string ParmAuxiliaryTableForVote = "@AuxiliaryTableForVote";
        private const string ParmAuxiliaryTableForJob = "@AuxiliaryTableForJob";
        private const string ParmIsCheckContentUseLevel = "@IsCheckContentUseLevel";
        private const string ParmCheckContentLevel = "@CheckContentLevel";
        private const string ParmPublishmentsystemDir = "@PublishmentSystemDir";
        private const string ParmPublishmentsystemUrl = "@PublishmentSystemUrl";
        private const string ParmIsHeadquarters = "@IsHeadquarters";
        private const string ParmParentPublishmentsystemid = "@ParentPublishmentSystemID";
        private const string ParmTaxis = "@Taxis";
        private const string ParmSettingsXml = "@SettingsXML";

        public void InsertWithTrans(PublishmentSystemInfo info, IDbTransaction trans)
        {
            //获取排序值
            var taxis = GetMaxTaxis() + 1;
            var insertParms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentsystemId, EDataType.Integer, info.PublishmentSystemId),
				GetParameter(ParmPublishmentsystemName, EDataType.NVarChar, 50, info.PublishmentSystemName),
                GetParameter(ParmPublishmentsystemType, EDataType.VarChar, 50, EPublishmentSystemTypeUtils.GetValue(info.PublishmentSystemType)),
				GetParameter(ParmAuxiliaryTableForContent, EDataType.VarChar, 50, info.AuxiliaryTableForContent),
                GetParameter(ParmAuxiliaryTableForGovpublic, EDataType.VarChar, 50, info.AuxiliaryTableForGovPublic),
                GetParameter(ParmAuxiliaryTableForGovinteract, EDataType.VarChar, 50, info.AuxiliaryTableForGovInteract),
                GetParameter(ParmAuxiliaryTableForVote, EDataType.VarChar, 50, info.AuxiliaryTableForVote),
                GetParameter(ParmAuxiliaryTableForJob, EDataType.VarChar, 50, info.AuxiliaryTableForJob),
				GetParameter(ParmIsCheckContentUseLevel, EDataType.VarChar, 18, info.IsCheckContentUseLevel.ToString()),
				GetParameter(ParmCheckContentLevel, EDataType.Integer, info.CheckContentLevel),
				GetParameter(ParmPublishmentsystemDir, EDataType.VarChar, 50, info.PublishmentSystemDir),
				GetParameter(ParmPublishmentsystemUrl, EDataType.VarChar, 200, info.PublishmentSystemUrl),
				GetParameter(ParmIsHeadquarters, EDataType.VarChar, 18, info.IsHeadquarters.ToString()),
                GetParameter(ParmParentPublishmentsystemid, EDataType.Integer, info.ParentPublishmentSystemId),
                GetParameter(ParmTaxis, EDataType.Integer, taxis),
				GetParameter(ParmSettingsXml, EDataType.NText, info.Additional.ToString())
			};

            ExecuteNonQuery(trans, SqlInsertPublishmentSystem, insertParms);
            PublishmentSystemManager.ClearCache(true);
        }

        public void Delete(int publishmentSystemId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var list = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(publishmentSystemId);
            BaiRongDataProvider.TableStyleDao.Delete(list, publishmentSystemInfo.AuxiliaryTableForContent);

            BaiRongDataProvider.TagDao.DeleteTags(publishmentSystemId);

            DataProvider.NodeDao.Delete(publishmentSystemId);

            UpdateParentPublishmentSystemIdToZero(publishmentSystemId);

            ExecuteNonQuery($"DELETE FROM siteserver_PublishmentSystem WHERE PublishmentSystemID  = {publishmentSystemId}");

            PublishmentSystemManager.ClearCache(true);
            NodeManager.RemoveCache(publishmentSystemId);
            ProductPermissionsManager.Current.ClearCache();
        }


        public void Update(PublishmentSystemInfo info)
        {
            var updateParms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentsystemName, EDataType.NVarChar, 50, info.PublishmentSystemName),
                GetParameter(ParmPublishmentsystemType, EDataType.VarChar, 50, EPublishmentSystemTypeUtils.GetValue(info.PublishmentSystemType)),
				GetParameter(ParmAuxiliaryTableForContent, EDataType.VarChar, 50, info.AuxiliaryTableForContent),
                GetParameter(ParmAuxiliaryTableForGovpublic, EDataType.VarChar, 50, info.AuxiliaryTableForGovPublic),
                GetParameter(ParmAuxiliaryTableForGovinteract, EDataType.VarChar, 50, info.AuxiliaryTableForGovInteract),
                GetParameter(ParmAuxiliaryTableForVote, EDataType.VarChar, 50, info.AuxiliaryTableForVote),
                GetParameter(ParmAuxiliaryTableForJob, EDataType.VarChar, 50, info.AuxiliaryTableForJob),
				GetParameter(ParmIsCheckContentUseLevel, EDataType.VarChar, 18, info.IsCheckContentUseLevel.ToString()),
				GetParameter(ParmCheckContentLevel, EDataType.Integer, info.CheckContentLevel),
				GetParameter(ParmPublishmentsystemDir, EDataType.VarChar, 50, info.PublishmentSystemDir),
				GetParameter(ParmPublishmentsystemUrl, EDataType.VarChar, 200, info.PublishmentSystemUrl),
				GetParameter(ParmIsHeadquarters, EDataType.VarChar, 18, info.IsHeadquarters.ToString()),
                GetParameter(ParmParentPublishmentsystemid, EDataType.Integer, info.ParentPublishmentSystemId),
                GetParameter(ParmTaxis, EDataType.Integer, info.Taxis),
				GetParameter(ParmSettingsXml, EDataType.NText, info.Additional.ToString()),
				GetParameter(ParmPublishmentsystemId, EDataType.Integer, info.PublishmentSystemId)
			};

            if (info.IsHeadquarters)
            {
                UpdateAllIsHeadquarters();
            }

            ExecuteNonQuery(SqlUpdatePublishmentSystem, updateParms);
            PublishmentSystemManager.ClearCache(true);
        }

        public void UpdateParentPublishmentSystemIdToZero(int parentPublishmentSystemId)
        {
            var sqlString = "UPDATE siteserver_PublishmentSystem SET ParentPublishmentSystemID = 0 WHERE ParentPublishmentSystemID = " + parentPublishmentSystemId;

            ExecuteNonQuery(sqlString);
            PublishmentSystemManager.ClearCache(true);
        }

        public List<string> GetLowerPublishmentSystemDirListThatNotIsHeadquarters()
        {
            var list = new List<string>();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmIsHeadquarters, EDataType.VarChar, 18, false.ToString())
			};

            using (var rdr = ExecuteReader(SqlSelectPublishmentsystemDirByIsHeadquarters, parms))
            {
                while (rdr.Read())
                {
                    list.Add(GetString(rdr, 0).ToLower());
                }
                rdr.Close();
            }
            return list;
        }

        public List<int> GetPublishmentSystemIdListByParent(int parentPublishmentSystemId)
        {
            var list = new List<int>();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmParentPublishmentsystemid, EDataType.Integer, parentPublishmentSystemId)
			};

            using (var rdr = ExecuteReader(SqlSelectPublishmentsystemIdByParent, parms))
            {
                while (rdr.Read())
                {
                    list.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }
            return list;
        }

        private void UpdateAllIsHeadquarters()
        {
            var updateParms = new IDataParameter[]
			{
				GetParameter(ParmIsHeadquarters, EDataType.VarChar, 18, false.ToString())
			};

            ExecuteNonQuery(SqlUpdateAllIsHeadquarters, updateParms);
            PublishmentSystemManager.ClearCache(true);
        }

        public List<KeyValuePair<int, PublishmentSystemInfo>> GetPublishmentSystemInfoKeyValuePairList()
        {
            var list = new List<KeyValuePair<int, PublishmentSystemInfo>>();

            var publishmentSystemInfoList = GetPublishmentSystemInfoList();
            foreach (var publishmentSystemInfo in publishmentSystemInfoList)
            {
                var entry = new KeyValuePair<int, PublishmentSystemInfo>(publishmentSystemInfo.PublishmentSystemId, publishmentSystemInfo);
                list.Add(entry);
            }

            return list;
        }

        protected List<int> GetPublishmentSystemIdList(DateTime sinceDate)
        {
            var list = new List<int>();

            string sqlString =
                $"SELECT p.PublishmentSystemID FROM siteserver_PublishmentSystem p INNER JOIN siteserver_Node n ON (p.PublishmentSystemID = n.NodeID AND (n.AddDate BETWEEN '{sinceDate.ToShortDateString()}' AND '{DateTime.Now.ToShortDateString()}')) ORDER BY p.IsHeadquarters DESC, p.ParentPublishmentSystemID, p.Taxis DESC, n.NodeID";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }
            return list;
        }

        private List<PublishmentSystemInfo> GetPublishmentSystemInfoList()
        {
            var list = new List<PublishmentSystemInfo>();

            using (var rdr = ExecuteReader(SqlSelectPublishmentSystemAll))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var publishmentSystemInfo = new PublishmentSystemInfo(GetInt(rdr, i++), GetString(rdr, i++), EPublishmentSystemTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i));
                    list.Add(publishmentSystemInfo);
                }
                rdr.Close();
            }
            return list;
        }

        public IEnumerable GetDataSource()
        {
            return (IEnumerable)ExecuteReader(SqlSelectAllWithNode);
        }

        public string GetSelectCommand()
        {
            return SqlSelectAllWithNode;
        }

        public string GetDatabasePublishmentSystemUrl(int publishmentSystemId)
        {
            var publishmentSystemUrl = string.Empty;

            var sqlString = "SELECT PublishmentSystemUrl FROM siteserver_PublishmentSystem WHERE PublishmentSystemID = " + publishmentSystemId;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    publishmentSystemUrl = GetString(rdr, 0);
                }
                rdr.Close();
            }
            return publishmentSystemUrl;
        }

        public int GetPublishmentSystemCount()
        {
            var count = 0;

            var sqlString = "SELECT Count(*) FROM siteserver_PublishmentSystem";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    count = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return count;
        }

        public int GetPublishmentSystemIdByIsHeadquarters()
        {
            var publishmentSystemId = 0;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmIsHeadquarters, EDataType.VarChar, 18, true.ToString())
			};

            using (var rdr = ExecuteReader(SqlSelectPublishmentsystemIdByIsHeadquarters, parms))
            {
                if (rdr.Read())
                {
                    publishmentSystemId = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return publishmentSystemId;
        }

        public int GetPublishmentSystemIdByPublishmentSystemDir(string publishmentSystemDir)
        {
            var publishmentSystemId = 0;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentsystemDir, EDataType.VarChar, 50, publishmentSystemDir)
			};

            using (var rdr = ExecuteReader(SqlSelectPublishmentsystemIdByPublishmentsystemDir, parms))
            {
                if (rdr.Read())
                {
                    publishmentSystemId = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return publishmentSystemId;
        }

        public IEnumerable GetStlDataSource(string siteName, string siteDir, int startNum, int totalNum, string whereString, EScopeType scopeType, string orderByString, string since)
        {
            IEnumerable ie = null;

            var sqlWhereString = string.Empty;

            PublishmentSystemInfo publishmentSystemInfo = null;
            if (!string.IsNullOrEmpty(siteName))
            {
                publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfoBySiteName(siteName);
            }
            else if (!string.IsNullOrEmpty(siteDir))
            {
                publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfoByDirectory(siteDir);
            }

            if (publishmentSystemInfo != null)
            {
                sqlWhereString = $"WHERE (ParentPublishmentSystemID = {publishmentSystemInfo.PublishmentSystemId})";
            }
            else
            {
                if (scopeType == EScopeType.Children)
                {
                    sqlWhereString = "WHERE (ParentPublishmentSystemID = 0 AND IsHeadquarters = 'False')";
                }
                else if (scopeType == EScopeType.Descendant)
                {
                    sqlWhereString = "WHERE (IsHeadquarters = 'False')";
                }
            }

            if (!string.IsNullOrEmpty(whereString))
            {
                sqlWhereString = string.IsNullOrEmpty(sqlWhereString) ? $"WHERE ({whereString})" : $"{sqlWhereString} AND ({whereString})";
            }

            if (string.IsNullOrEmpty(orderByString) || StringUtils.EqualsIgnoreCase(orderByString, "default"))
            {
                orderByString = "ORDER BY IsHeadquarters DESC, ParentPublishmentSystemID, Taxis DESC, PublishmentSystemID";

                var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, startNum, totalNum, SqlUtils.Asterisk, sqlWhereString, orderByString);

                ie = (IEnumerable)ExecuteReader(sqlSelect);
            }

            return ie;
        }

        public bool UpdateTaxisToDown(int publishmentSystemId)
        {
            SetTaxisNotZero();
            //var sbSql = new StringBuilder();
            //sbSql.AppendFormat("SELECT TOP 1 PublishmentSystemID, Taxis FROM siteserver_PublishmentSystem ");
            //sbSql.AppendFormat(" WHERE Taxis > (SELECT Taxis FROM siteserver_PublishmentSystem WHERE PublishmentSystemID = {0}) ", publishmentSystemId);
            //sbSql.AppendFormat(" ORDER BY Taxis ");

            var sqlString = SqlUtils.GetTopSqlString("siteserver_PublishmentSystem", "PublishmentSystemID, Taxis", $"WHERE Taxis > (SELECT Taxis FROM siteserver_PublishmentSystem WHERE PublishmentSystemID = {publishmentSystemId}) ORDER BY Taxis", 1);

            var lowerId = 0;
            var lowerTaxis = 0;

            using (var reader = ExecuteReader(sqlString))
            {
                if (reader.Read())
                {
                    lowerId = Convert.ToInt32(reader[0]);
                    lowerTaxis = Convert.ToInt32(reader[1]);
                }
                reader.Close();
            }

            var selectedTaxis = GetTaxis(publishmentSystemId);
            if (lowerId == 0) return false;

            SetTaxis(publishmentSystemId, lowerTaxis);
            SetTaxis(lowerId, selectedTaxis);
            return true;
        }

        public bool UpdateTaxisToUp(int publishmentSystemId)
        {
            SetTaxisNotZero();
            //var sbSql = new StringBuilder();
            //sbSql.AppendFormat("SELECT TOP 1 PublishmentSystemID, Taxis FROM siteserver_PublishmentSystem ");
            //sbSql.AppendFormat(" WHERE Taxis < (SELECT Taxis FROM siteserver_PublishmentSystem WHERE PublishmentSystemID = {0}) ", publishmentSystemId);
            //sbSql.AppendFormat(" ORDER BY Taxis DESC");

            var sqlString = SqlUtils.GetTopSqlString("siteserver_PublishmentSystem", "PublishmentSystemID, Taxis", $"WHERE Taxis < (SELECT Taxis FROM siteserver_PublishmentSystem WHERE PublishmentSystemID = {publishmentSystemId}) ORDER BY Taxis DESC", 1);

            var higherId = 0;
            var higherTaxis = 0;

            using (var reader = ExecuteReader(sqlString))
            {
                if (reader.Read())
                {
                    higherId = Convert.ToInt32(reader[0]);
                    higherTaxis = Convert.ToInt32(reader[1]);
                }
                reader.Close();
            }

            var selectedTaxis = GetTaxis(publishmentSystemId);
            if (higherId == 0) return false;

            SetTaxis(publishmentSystemId, higherTaxis);
            SetTaxis(higherId, selectedTaxis);
            return true;
        }

        private void SetTaxis(int publishmentSystemId, int taxis)
        {
            var sbSql = new StringBuilder();
            sbSql.AppendFormat(" UPDATE siteserver_PublishmentSystem SET Taxis = {0} ", taxis);
            sbSql.AppendFormat(" WHERE PublishmentSystemID = {0} ", publishmentSystemId);
            ExecuteNonQuery(sbSql.ToString());
        }

        private int GetTaxis(int publishmentSystemId)
        {
            var sbSql = new StringBuilder();
            sbSql.AppendFormat(" SELECT Taxis FROM siteserver_PublishmentSystem ");
            sbSql.AppendFormat(" WHERE PublishmentSystemID = {0} ", publishmentSystemId);

            var taxis = 0;
            using (var reader = ExecuteReader(sbSql.ToString()))
            {
                if (reader.Read())
                {
                    taxis = Convert.ToInt32(reader[0]);
                }
                reader.Close();
            }
            return taxis;
        }

        private static int GetMaxTaxis()
        {
            const string sqlString = "SELECT MAX(Taxis) FROM siteserver_PublishmentSystem";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        private void SetTaxisNotZero()
        {
            const string sqlString = @"UPDATE siteserver_PublishmentSystem SET Taxis = PublishmentSystemID where Taxis = 0";
            ExecuteNonQuery(sqlString);
        }
    }
}
