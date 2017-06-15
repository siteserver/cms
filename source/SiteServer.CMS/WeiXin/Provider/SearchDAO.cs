using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class SearchDao : DataProviderBase
    {
        private const string TableName = "wx_Search";

        public int Insert(SearchInfo searchInfo)
        {
            var searchId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(searchInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        searchId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return searchId;
        }

        public void Update(SearchInfo searchInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(searchInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            ExecuteNonQuery(sqlUpdate, parms);
        }
  
        public void AddPvCount(int searchId)
        {
            if (searchId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {SearchAttribute.PvCount} = {SearchAttribute.PvCount} + 1 WHERE ID = {searchId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemId, int searchId)
        {
            if (searchId > 0)
            {
                var searchIdList = new List<int>();
                searchIdList.Add(searchId);
                DataProviderWx.KeywordDao.Delete(GetKeywordIdList(searchIdList));

                string sqlString = $"DELETE FROM {TableName} WHERE ID = {searchId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemId, List<int> searchIdList)
        {
            if (searchIdList != null && searchIdList.Count > 0)
            {
                DataProviderWx.KeywordDao.Delete(GetKeywordIdList(searchIdList));

                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(searchIdList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        private List<int> GetKeywordIdList(List<int> searchIdList)
        {
            var keywordIdList = new List<int>();

            string sqlString =
                $"SELECT {SearchAttribute.KeywordId} FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(searchIdList)})";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    keywordIdList.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return keywordIdList;
        }

        public SearchInfo GetSearchInfo(int searchId)
        {
            SearchInfo searchInfo = null;

            string sqlWhere = $"WHERE ID = {searchId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    searchInfo = new SearchInfo(rdr);
                }
                rdr.Close();
            }

            return searchInfo;
        }

        public List<SearchInfo> GetSearchInfoListByKeywordId(int publishmentSystemId, int keywordId)
        {
            var searchInfoList = new List<SearchInfo>();

            string sqlWhere =
                $"WHERE {SearchAttribute.PublishmentSystemId} = {publishmentSystemId} AND {SearchAttribute.IsDisabled} <> '{true}'";
            if (keywordId > 0)
            {
                sqlWhere += $" AND {SearchAttribute.KeywordId} = {keywordId}";
            }

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var searchInfo = new SearchInfo(rdr);
                    searchInfoList.Add(searchInfo);
                }
                rdr.Close();
            }

            return searchInfoList;
        }

        public string GetTitle(int searchId)
        {
            var title = string.Empty;

            string sqlWhere = $"WHERE ID = {searchId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SearchAttribute.Title, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    title = rdr.GetValue(0).ToString();
                }
                rdr.Close();
            }

            return title;
        }

        public string GetSelectString(int publishmentSystemId)
        {
            string whereString = $"WHERE {SearchAttribute.PublishmentSystemId} = {publishmentSystemId}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public int GetFirstIdByKeywordId(int publishmentSystemId, int keywordId)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TableName} WHERE {SearchAttribute.PublishmentSystemId} = {publishmentSystemId} AND {SearchAttribute.IsDisabled} <> '{true}' AND {SearchAttribute.KeywordId} = {keywordId}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<SearchInfo> GetSearchInfoList(int publishmentSystemId)
        {
            var searchInfoList = new List<SearchInfo>();

            string sqlWhere = $"WHERE {SearchAttribute.PublishmentSystemId} = {publishmentSystemId}";
            
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var searchInfo = new SearchInfo(rdr);
                    searchInfoList.Add(searchInfo);
                }
                rdr.Close();
            }

            return searchInfoList;
        }
        
    }
}
