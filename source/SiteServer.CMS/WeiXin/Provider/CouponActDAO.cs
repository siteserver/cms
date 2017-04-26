using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class CouponActDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_CouponAct";

        public int Insert(CouponActInfo actInfo)
        {
            var actID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(actInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        actID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return actID;
        }

        public void Update(CouponActInfo actInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(actInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        public void UpdateUserCount(int actID, int publishmentSystemID)
        {
            if (actID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} set UserCount= UserCount+1 WHERE ID = {actID} AND publishmentSystemID = {publishmentSystemID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void UpdatePVCount(int actID, int publishmentSystemID)
        {
            if (actID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} set PVCount= PVCount+1 WHERE ID = {actID} AND publishmentSystemID = {publishmentSystemID}";
                ExecuteNonQuery(sqlString);
            }
        }
        
        public void Delete(List<int> actIDList)
        {
            if (actIDList != null && actIDList.Count > 0)
            {
                DataProviderWX.KeywordDAO.Delete(GetKeywordIDList(actIDList));

                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(actIDList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        private List<int> GetKeywordIDList(List<int> actIDList)
        {
            var keywordIDList = new List<int>();

            if (actIDList != null && actIDList.Count > 0)
            {
                string sqlString =
                    $"SELECT {CouponActAttribute.KeywordID} FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(actIDList)})";

                using (var rdr = ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        keywordIDList.Add(rdr.GetInt32(0));
                    }
                    rdr.Close();
                }
            }

            return keywordIDList;
        }

        public CouponActInfo GetActInfo(int actID)
        {
            CouponActInfo actInfo = null;

            string SQL_WHERE = $"WHERE ID = {actID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    actInfo = new CouponActInfo(rdr);
                }
                rdr.Close();
            }

            return actInfo;
        }

        public List<int> GetActIDList(int publishmentSystemID)
        {
            var list = new List<int>();

            string sqlString =
                $"SELECT ID FROM {TABLE_NAME} WHERE {CouponActAttribute.PublishmentSystemID} = {publishmentSystemID}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read() && !rdr.IsDBNull(0))
                {
                    list.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return list;
        }

        public List<CouponActInfo> GetActInfoListByKeywordID(int publishmentSystemID, int keywordID)
        {
            var actInfoList = new List<CouponActInfo>();

            string SQL_WHERE =
                $"WHERE {CouponActAttribute.PublishmentSystemID} = {publishmentSystemID} AND {CouponActAttribute.IsDisabled} <> '{true}'";
            if (keywordID > 0)
            {
                SQL_WHERE += $" AND {CouponActAttribute.KeywordID} = {keywordID}";
            }
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var actInfo = new CouponActInfo(rdr);
                    actInfoList.Add(actInfo);
                }
                rdr.Close();
            }

            return actInfoList;
        }

        public int GetKeywordID(int actID)
        {
            var keywordID = 0;

            string SQL_WHERE = $"WHERE ID = {actID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, CouponActAttribute.KeywordID, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    keywordID = rdr.GetInt32(0);
                }
                rdr.Close();
            }

            return keywordID;
        }

        public string GetTitle(int actID)
        {
            var title = string.Empty;

            string SQL_WHERE = $"WHERE ID = {actID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, CouponActAttribute.Title, SQL_WHERE, null);

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
            string whereString = $"WHERE {CouponActAttribute.PublishmentSystemID} = {publishmentSystemID}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public int GetFirstIDByKeywordID(int publishmentSystemID, int keywordID)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TABLE_NAME} WHERE {CouponActAttribute.PublishmentSystemID} = {publishmentSystemID} AND {CouponActAttribute.IsDisabled} <> '{true}' AND {CouponActAttribute.KeywordID} = {keywordID}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<CouponActInfo> GetCouponActInfoList(int publishmentSystemID)
        {
            var couponActInfoList = new List<CouponActInfo>();

            string SQL_WHERE = $"WHERE {CouponActAttribute.PublishmentSystemID} = {publishmentSystemID}";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var couponActInfo = new CouponActInfo(rdr);
                    couponActInfoList.Add(couponActInfo);
                }
                rdr.Close();
            }

            return couponActInfoList;
        }
    }
}
