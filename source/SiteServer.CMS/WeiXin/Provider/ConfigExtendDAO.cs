using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class ConfigExtendDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_ConfigExtend";

        public int Insert(ConfigExtendInfo configExtendInfo)
        {
            var configExtendID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(configExtendInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        configExtendID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return configExtendID;
        }

        public void Update(ConfigExtendInfo configExtendInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(configExtendInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        public void UpdateFuctionID(int publishmentSystemID, int functionID)
        {
            if (functionID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {ConfigExtendAttribute.FunctionID} = {functionID} WHERE {ConfigExtendAttribute.FunctionID} = 0 AND {ConfigExtendAttribute.PublishmentSystemID} = {publishmentSystemID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void DeleteAllNotInIDList(int publishmentSystemID, int functionID, List<int> idList)
        {
            if (functionID > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE {ConfigExtendAttribute.PublishmentSystemID} = {publishmentSystemID} AND {ConfigExtendAttribute.FunctionID} = {functionID}";
                if (idList != null && idList.Count > 0)
                {
                    sqlString =
                        $"DELETE FROM {TABLE_NAME} WHERE {ConfigExtendAttribute.PublishmentSystemID} = {publishmentSystemID} AND {ConfigExtendAttribute.FunctionID} = {functionID} AND ID NOT IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";
                }
                ExecuteNonQuery(sqlString);
            }
        }

        public List<ConfigExtendInfo> GetConfigExtendInfoList(int publishmentSystemID, int functionID,string keywordType)
        {
            var list = new List<ConfigExtendInfo>();

            string SQL_WHERE =
                $"WHERE {ConfigExtendAttribute.PublishmentSystemID} = {publishmentSystemID} AND {ConfigExtendAttribute.FunctionID} = {functionID} AND {ConfigExtendAttribute.KeywordType}='{keywordType}' ";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var configExtendInfo = new ConfigExtendInfo(rdr);
                    list.Add(configExtendInfo);
                }
                rdr.Close();
            }

            return list;
        }
    }
}
