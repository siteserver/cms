using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class SearchNavigationDao : DataProviderBase
    {
        private const string TableName = "wx_SearchNavigation";

        public int Insert(SearchNavigationInfo searchNavigationInfo)
        {
            var searchNavigationId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(searchNavigationInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        searchNavigationId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return searchNavigationId;
        }

        public void Update(SearchNavigationInfo searchNavigationInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(searchNavigationInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            ExecuteNonQuery(sqlUpdate, parms);
        }

        public void UpdateSearchId(int publishmentSystemId, int searchId)
        {
            if (searchId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {SearchNavigationAttribute.SearchId} = {searchId} WHERE {SearchNavigationAttribute.SearchId} = 0 AND {SearchNavigationAttribute.PublishmentSystemId} = {publishmentSystemId}";
                ExecuteNonQuery(sqlString);
            }
        }


        public void Delete(int publishmentSystemId, int searchNavigationId)
        {
            if (searchNavigationId > 0)
            {
                string sqlString = $"DELETE FROM {TableName} WHERE ID = {searchNavigationId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemId, List<int> searchNavigationIdList)
        {
            if (searchNavigationIdList != null && searchNavigationIdList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(searchNavigationIdList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        public void DeleteAllNotInIdList(int publishmentSystemId, int searchId, List<int> idList)
        {
            if (searchId > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE {SearchNavigationAttribute.PublishmentSystemId} = {publishmentSystemId} AND {SearchNavigationAttribute.SearchId} = {searchId}";
                if (idList != null && idList.Count > 0)
                {
                    sqlString =
                        $"DELETE FROM {TableName} WHERE {SearchNavigationAttribute.PublishmentSystemId} = {publishmentSystemId} AND {SearchNavigationAttribute.SearchId} = {searchId} AND ID NOT IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";
                }
                ExecuteNonQuery(sqlString);
            }
        }

        public SearchNavigationInfo GetSearchNavigationInfo(int searchNavigationId)
        {
            SearchNavigationInfo searchNavigationInfo = null;

            string sqlWhere = $"WHERE ID = {searchNavigationId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    searchNavigationInfo = new SearchNavigationInfo(rdr);
                }
                rdr.Close();
            }

            return searchNavigationInfo;
        }

        public List<SearchNavigationInfo> GetSearchNavigationInfoList(int publishmentSystemId, int searchId)
        {
            var searchNavigationInfoList = new List<SearchNavigationInfo>();

            string sqlWhere =
                $"WHERE {SearchNavigationAttribute.PublishmentSystemId} = {publishmentSystemId} AND {SearchNavigationAttribute.SearchId} = {searchId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var searchNavigationInfo = new SearchNavigationInfo(rdr);
                    searchNavigationInfoList.Add(searchNavigationInfo);
                }
                rdr.Close();
            }

            return searchNavigationInfoList;
        }

        public string GetSelectString(int publishmentSystemId)
        {
            string whereString = $"WHERE {SearchNavigationAttribute.PublishmentSystemId} = {publishmentSystemId}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public List<SearchNavigationInfo> GetSearchNavigationInfoList(int publishmentSystemId)
        {
            var searchNavigationInfoList = new List<SearchNavigationInfo>();

            string sqlWhere = $"WHERE {SearchNavigationAttribute.PublishmentSystemId} = {publishmentSystemId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var searchNavigationInfo = new SearchNavigationInfo(rdr);
                    searchNavigationInfoList.Add(searchNavigationInfo);
                }
                rdr.Close();
            }

            return searchNavigationInfoList;
        }

    }
}
