using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class View360DAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_View360";

        public int Insert(View360Info view360Info)
        {
            var view360ID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(view360Info.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        view360ID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return view360ID;
        }

        public void Update(View360Info view360Info)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(view360Info.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        public void AddPVCount(int view360ID)
        {
            if (view360ID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {View360Attribute.PVCount} = {View360Attribute.PVCount} + 1 WHERE ID = {view360ID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int view360ID)
        {
            if (view360ID > 0)
            {
                var view360IDList = new List<int>();
                view360IDList.Add(view360ID);
                DataProviderWX.KeywordDAO.Delete(GetKeywordIDList(view360IDList));

                string sqlString = $"DELETE FROM {TABLE_NAME} WHERE ID = {view360ID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(List<int> view360IDList)
        {
            if (view360IDList != null && view360IDList.Count > 0)
            {
                DataProviderWX.KeywordDAO.Delete(GetKeywordIDList(view360IDList));

                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(view360IDList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        private List<int> GetKeywordIDList(List<int> view360IDList)
        {
            var keywordIDList = new List<int>();

            string sqlString =
                $"SELECT {View360Attribute.KeywordID} FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(view360IDList)})";

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

        public View360Info GetView360Info(int view360ID)
        {
            View360Info view360Info = null;

            string SQL_WHERE = $"WHERE ID = {view360ID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    view360Info = new View360Info(rdr);
                }
                rdr.Close();
            }

            return view360Info;
        }

        public List<View360Info> GetView360InfoListByKeywordID(int publishmentSystemID, int keywordID)
        {
            var view360InfoList = new List<View360Info>();

            string SQL_WHERE =
                $"WHERE {View360Attribute.PublishmentSystemID} = {publishmentSystemID} AND {View360Attribute.IsDisabled} <> '{true}'";
            if (keywordID > 0)
            {
                SQL_WHERE += $" AND {View360Attribute.KeywordID} = {keywordID}";
            }

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
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

        public int GetFirstIDByKeywordID(int publishmentSystemID, int keywordID)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TABLE_NAME} WHERE {View360Attribute.PublishmentSystemID} = {publishmentSystemID} AND {View360Attribute.IsDisabled} <> '{true}' AND {View360Attribute.KeywordID} = {keywordID}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetTitle(int view360ID)
        {
            var title = string.Empty;

            string SQL_WHERE = $"WHERE ID = {view360ID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, View360Attribute.Title, SQL_WHERE, null);

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
            string whereString = $"WHERE {View360Attribute.PublishmentSystemID} = {publishmentSystemID}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public List<View360Info> GetView360InfoList(int publishmentSystemID)
        {
            var view360InfoList = new List<View360Info>();

            string SQL_WHERE = $" AND {View360Attribute.PublishmentSystemID} = {publishmentSystemID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
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
