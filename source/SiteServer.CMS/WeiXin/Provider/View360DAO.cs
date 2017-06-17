using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class View360Dao : DataProviderBase
    {
        private const string TableName = "wx_View360";

        public int Insert(View360Info view360Info)
        {
            var view360Id = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(view360Info.ToNameValueCollection(), ConnectionString, TableName, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        view360Id = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return view360Id;
        }

        public void Update(View360Info view360Info)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(view360Info.ToNameValueCollection(), ConnectionString, TableName, out parms);

            ExecuteNonQuery(sqlUpdate, parms);
        }

        public void AddPvCount(int view360Id)
        {
            if (view360Id > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {View360Attribute.PvCount} = {View360Attribute.PvCount} + 1 WHERE ID = {view360Id}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int view360Id)
        {
            if (view360Id > 0)
            {
                var view360IdList = new List<int>();
                view360IdList.Add(view360Id);
                DataProviderWx.KeywordDao.Delete(GetKeywordIdList(view360IdList));

                string sqlString = $"DELETE FROM {TableName} WHERE ID = {view360Id}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(List<int> view360IdList)
        {
            if (view360IdList != null && view360IdList.Count > 0)
            {
                DataProviderWx.KeywordDao.Delete(GetKeywordIdList(view360IdList));

                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(view360IdList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        private List<int> GetKeywordIdList(List<int> view360IdList)
        {
            var keywordIdList = new List<int>();

            string sqlString =
                $"SELECT {View360Attribute.KeywordId} FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(view360IdList)})";

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

        public View360Info GetView360Info(int view360Id)
        {
            View360Info view360Info = null;

            string sqlWhere = $"WHERE ID = {view360Id}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    view360Info = new View360Info(rdr);
                }
                rdr.Close();
            }

            return view360Info;
        }

        public List<View360Info> GetView360InfoListByKeywordId(int publishmentSystemId, int keywordId)
        {
            var view360InfoList = new List<View360Info>();

            string sqlWhere =
                $"WHERE {View360Attribute.PublishmentSystemId} = {publishmentSystemId} AND {View360Attribute.IsDisabled} <> '{true}'";
            if (keywordId > 0)
            {
                sqlWhere += $" AND {View360Attribute.KeywordId} = {keywordId}";
            }

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var view360Info = new View360Info(rdr);
                    view360InfoList.Add(view360Info);
                }
                rdr.Close();
            }

            return view360InfoList;
        }

        public int GetFirstIdByKeywordId(int publishmentSystemId, int keywordId)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TableName} WHERE {View360Attribute.PublishmentSystemId} = {publishmentSystemId} AND {View360Attribute.IsDisabled} <> '{true}' AND {View360Attribute.KeywordId} = {keywordId}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetTitle(int view360Id)
        {
            var title = string.Empty;

            string sqlWhere = $"WHERE ID = {view360Id}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, View360Attribute.Title, sqlWhere, null);

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
            string whereString = $"WHERE {View360Attribute.PublishmentSystemId} = {publishmentSystemId}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public List<View360Info> GetView360InfoList(int publishmentSystemId)
        {
            var view360InfoList = new List<View360Info>();

            string sqlWhere = $" AND {View360Attribute.PublishmentSystemId} = {publishmentSystemId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var view360Info = new View360Info(rdr);
                    view360InfoList.Add(view360Info);
                }
                rdr.Close();
            }

            return view360InfoList;
        }

    }
}
