using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class SearchNavigationDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_SearchNavigation";

        public int Insert(SearchNavigationInfo searchNavigationInfo)
        {
            var searchNavigationID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(searchNavigationInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        searchNavigationID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return searchNavigationID;
        }

        public void Update(SearchNavigationInfo searchNavigationInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(searchNavigationInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        public void UpdateSearchID(int publishmentSystemID, int searchID)
        {
            if (searchID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {SearchNavigationAttribute.SearchID} = {searchID} WHERE {SearchNavigationAttribute.SearchID} = 0 AND {SearchNavigationAttribute.PublishmentSystemID} = {publishmentSystemID}";
                ExecuteNonQuery(sqlString);
            }
        }


        public void Delete(int publishmentSystemID, int searchNavigationID)
        {
            if (searchNavigationID > 0)
            {
                string sqlString = $"DELETE FROM {TABLE_NAME} WHERE ID = {searchNavigationID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemID, List<int> searchNavigationIDList)
        {
            if (searchNavigationIDList != null && searchNavigationIDList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(searchNavigationIDList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        public void DeleteAllNotInIDList(int publishmentSystemID, int searchID, List<int> idList)
        {
            if (searchID > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE {SearchNavigationAttribute.PublishmentSystemID} = {publishmentSystemID} AND {SearchNavigationAttribute.SearchID} = {searchID}";
                if (idList != null && idList.Count > 0)
                {
                    sqlString =
                        $"DELETE FROM {TABLE_NAME} WHERE {SearchNavigationAttribute.PublishmentSystemID} = {publishmentSystemID} AND {SearchNavigationAttribute.SearchID} = {searchID} AND ID NOT IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";
                }
                ExecuteNonQuery(sqlString);
            }
        }

        public SearchNavigationInfo GetSearchNavigationInfo(int SearchNavigationID)
        {
            SearchNavigationInfo searchNavigationInfo = null;

            string SQL_WHERE = $"WHERE ID = {SearchNavigationID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    searchNavigationInfo = new SearchNavigationInfo(rdr);
                }
                rdr.Close();
            }

            return searchNavigationInfo;
        }

        public List<SearchNavigationInfo> GetSearchNavigationInfoList(int publishmentSystemID, int searchID)
        {
            var searchNavigationInfoList = new List<SearchNavigationInfo>();

            string SQL_WHERE =
                $"WHERE {SearchNavigationAttribute.PublishmentSystemID} = {publishmentSystemID} AND {SearchNavigationAttribute.SearchID} = {searchID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
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

        public string GetSelectString(int publishmentSystemID)
        {
            string whereString = $"WHERE {SearchNavigationAttribute.PublishmentSystemID} = {publishmentSystemID}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public List<SearchNavigationInfo> GetSearchNavigationInfoList(int publishmentSystemID)
        {
            var searchNavigationInfoList = new List<SearchNavigationInfo>();

            string SQL_WHERE = $"WHERE {SearchNavigationAttribute.PublishmentSystemID} = {publishmentSystemID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
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
