using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class SearchDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_Search";

        public int Insert(SearchInfo searchInfo)
        {
            var searchID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(searchInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        searchID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return searchID;
        }

        public void Update(SearchInfo searchInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(searchInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            ExecuteNonQuery(SQL_UPDATE, parms);
        }
  
        public void AddPVCount(int searchID)
        {
            if (searchID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {SearchAttribute.PVCount} = {SearchAttribute.PVCount} + 1 WHERE ID = {searchID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemID, int searchID)
        {
            if (searchID > 0)
            {
                var SearchIDList = new List<int>();
                SearchIDList.Add(searchID);
                DataProviderWX.KeywordDAO.Delete(GetKeywordIDList(SearchIDList));

                string sqlString = $"DELETE FROM {TABLE_NAME} WHERE ID = {searchID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemID, List<int> searchIDList)
        {
            if (searchIDList != null && searchIDList.Count > 0)
            {
                DataProviderWX.KeywordDAO.Delete(GetKeywordIDList(searchIDList));

                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(searchIDList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        private List<int> GetKeywordIDList(List<int> searchIDList)
        {
            var keywordIDList = new List<int>();

            string sqlString =
                $"SELECT {SearchAttribute.KeywordID} FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(searchIDList)})";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    keywordIDList.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return keywordIDList;
        }

        public SearchInfo GetSearchInfo(int SearchID)
        {
            SearchInfo searchInfo = null;

            string SQL_WHERE = $"WHERE ID = {SearchID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    searchInfo = new SearchInfo(rdr);
                }
                rdr.Close();
            }

            return searchInfo;
        }

        public List<SearchInfo> GetSearchInfoListByKeywordID(int publishmentSystemID, int keywordID)
        {
            var searchInfoList = new List<SearchInfo>();

            string SQL_WHERE =
                $"WHERE {SearchAttribute.PublishmentSystemID} = {publishmentSystemID} AND {SearchAttribute.IsDisabled} <> '{true}'";
            if (keywordID > 0)
            {
                SQL_WHERE += $" AND {SearchAttribute.KeywordID} = {keywordID}";
            }

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
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

        public string GetTitle(int searchID)
        {
            var title = string.Empty;

            string SQL_WHERE = $"WHERE ID = {searchID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SearchAttribute.Title, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    title = rdr.GetValue(0).ToString();
                }
                rdr.Close();
            }

            return title;
        }

        public string GetSelectString(int publishmentSystemID)
        {
            string whereString = $"WHERE {SearchAttribute.PublishmentSystemID} = {publishmentSystemID}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public int GetFirstIDByKeywordID(int publishmentSystemID, int keywordID)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TABLE_NAME} WHERE {SearchAttribute.PublishmentSystemID} = {publishmentSystemID} AND {SearchAttribute.IsDisabled} <> '{true}' AND {SearchAttribute.KeywordID} = {keywordID}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<SearchInfo> GetSearchInfoList(int publishmentSystemID)
        {
            var searchInfoList = new List<SearchInfo>();

            string SQL_WHERE = $"WHERE {SearchAttribute.PublishmentSystemID} = {publishmentSystemID}";
            
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
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
