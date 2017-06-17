using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class CouponActDao : DataProviderBase
    {
        private const string TableName = "wx_CouponAct";

        public int Insert(CouponActInfo actInfo)
        {
            var actId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(actInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        actId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return actId;
        }

        public void Update(CouponActInfo actInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(actInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            ExecuteNonQuery(sqlUpdate, parms);
        }

        public void UpdateUserCount(int actId, int publishmentSystemId)
        {
            if (actId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} set UserCount= UserCount+1 WHERE ID = {actId} AND publishmentSystemID = {publishmentSystemId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void UpdatePvCount(int actId, int publishmentSystemId)
        {
            if (actId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} set PVCount= PVCount+1 WHERE ID = {actId} AND publishmentSystemID = {publishmentSystemId}";
                ExecuteNonQuery(sqlString);
            }
        }
        
        public void Delete(List<int> actIdList)
        {
            if (actIdList != null && actIdList.Count > 0)
            {
                DataProviderWx.KeywordDao.Delete(GetKeywordIdList(actIdList));

                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(actIdList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        private List<int> GetKeywordIdList(List<int> actIdList)
        {
            var keywordIdList = new List<int>();

            if (actIdList != null && actIdList.Count > 0)
            {
                string sqlString =
                    $"SELECT {CouponActAttribute.KeywordId} FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(actIdList)})";

                using (var rdr = ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        keywordIdList.Add(rdr.GetInt32(0));
                    }
                    rdr.Close();
                }
            }

            return keywordIdList;
        }

        public CouponActInfo GetActInfo(int actId)
        {
            CouponActInfo actInfo = null;

            string sqlWhere = $"WHERE ID = {actId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    actInfo = new CouponActInfo(rdr);
                }
                rdr.Close();
            }

            return actInfo;
        }

        public List<int> GetActIdList(int publishmentSystemId)
        {
            var list = new List<int>();

            string sqlString =
                $"SELECT ID FROM {TableName} WHERE {CouponActAttribute.PublishmentSystemId} = {publishmentSystemId}";

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

        public List<CouponActInfo> GetActInfoListByKeywordId(int publishmentSystemId, int keywordId)
        {
            var actInfoList = new List<CouponActInfo>();

            string sqlWhere =
                $"WHERE {CouponActAttribute.PublishmentSystemId} = {publishmentSystemId} AND {CouponActAttribute.IsDisabled} <> '{true}'";
            if (keywordId > 0)
            {
                sqlWhere += $" AND {CouponActAttribute.KeywordId} = {keywordId}";
            }
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
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

        public int GetKeywordId(int actId)
        {
            var keywordId = 0;

            string sqlWhere = $"WHERE ID = {actId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, CouponActAttribute.KeywordId, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    keywordId = rdr.GetInt32(0);
                }
                rdr.Close();
            }

            return keywordId;
        }

        public string GetTitle(int actId)
        {
            var title = string.Empty;

            string sqlWhere = $"WHERE ID = {actId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, CouponActAttribute.Title, sqlWhere, null);

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
            string whereString = $"WHERE {CouponActAttribute.PublishmentSystemId} = {publishmentSystemId}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public int GetFirstIdByKeywordId(int publishmentSystemId, int keywordId)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TableName} WHERE {CouponActAttribute.PublishmentSystemId} = {publishmentSystemId} AND {CouponActAttribute.IsDisabled} <> '{true}' AND {CouponActAttribute.KeywordId} = {keywordId}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<CouponActInfo> GetCouponActInfoList(int publishmentSystemId)
        {
            var couponActInfoList = new List<CouponActInfo>();

            string sqlWhere = $"WHERE {CouponActAttribute.PublishmentSystemId} = {publishmentSystemId}";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
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
