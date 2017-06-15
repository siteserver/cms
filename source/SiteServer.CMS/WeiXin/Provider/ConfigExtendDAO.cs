using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class ConfigExtendDao : DataProviderBase
    {
        private const string TableName = "wx_ConfigExtend";

        public int Insert(ConfigExtendInfo configExtendInfo)
        {
            var configExtendId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(configExtendInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        configExtendId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return configExtendId;
        }

        public void Update(ConfigExtendInfo configExtendInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(configExtendInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            ExecuteNonQuery(sqlUpdate, parms);
        }

        public void UpdateFuctionId(int publishmentSystemId, int functionId)
        {
            if (functionId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {ConfigExtendAttribute.FunctionId} = {functionId} WHERE {ConfigExtendAttribute.FunctionId} = 0 AND {ConfigExtendAttribute.PublishmentSystemId} = {publishmentSystemId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void DeleteAllNotInIdList(int publishmentSystemId, int functionId, List<int> idList)
        {
            if (functionId > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE {ConfigExtendAttribute.PublishmentSystemId} = {publishmentSystemId} AND {ConfigExtendAttribute.FunctionId} = {functionId}";
                if (idList != null && idList.Count > 0)
                {
                    sqlString =
                        $"DELETE FROM {TableName} WHERE {ConfigExtendAttribute.PublishmentSystemId} = {publishmentSystemId} AND {ConfigExtendAttribute.FunctionId} = {functionId} AND ID NOT IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";
                }
                ExecuteNonQuery(sqlString);
            }
        }

        public List<ConfigExtendInfo> GetConfigExtendInfoList(int publishmentSystemId, int functionId,string keywordType)
        {
            var list = new List<ConfigExtendInfo>();

            string sqlWhere =
                $"WHERE {ConfigExtendAttribute.PublishmentSystemId} = {publishmentSystemId} AND {ConfigExtendAttribute.FunctionId} = {functionId} AND {ConfigExtendAttribute.KeywordType}='{keywordType}' ";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
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
