using System.Collections.Generic;
using System.Data;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class CouponDao : DataProviderBase
    {
        private const string TableName = "wx_Coupon";

        public int Insert(CouponInfo couponInfo)
        {
            var couponId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(couponInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);


            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        couponId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return couponId;
        }

        public void Update(CouponInfo couponInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(couponInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            ExecuteNonQuery(sqlUpdate, parms);
        }

        public void UpdateTotalNum(int couponId, int totalNum)
        {
            string sqlString = $"UPDATE {TableName} SET {CouponAttribute.TotalNum} = {totalNum} WHERE ID = {couponId}";

            ExecuteNonQuery(sqlString);
        }

        public void UpdateActId(int couponId, int actId)
        {
            string sqlString = $"UPDATE {TableName} SET {CouponAttribute.ActId} = {actId} WHERE ID = {couponId}";

            ExecuteNonQuery(sqlString);
        }

        public void Delete(int couponId)
        {
            if (couponId > 0)
            {
                string sqlString = $"DELETE FROM {TableName} WHERE ID = {couponId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(List<int> couponIdList)
        {
            if (couponIdList != null && couponIdList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(couponIdList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        public CouponInfo GetCouponInfo(int couponId)
        {
            CouponInfo couponInfo = null;

            string sqlWhere = $"WHERE ID = {couponId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    couponInfo = new CouponInfo(rdr);
                }
                rdr.Close();
            }

            return couponInfo;
        }

        public string GetSelectString(int publishmentSystemId)
        {
            string whereString = $"WHERE {CouponAttribute.PublishmentSystemId} = {publishmentSystemId}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public Dictionary<string, int> GetCouponDictionary(int actId)
        {
            var dictionary = new Dictionary<string, int>();

            string sqlWhere = $"WHERE {CouponAttribute.ActId} = {actId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, CouponAttribute.Title + "," + CouponAttribute.TotalNum, sqlWhere);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    dictionary.Add(rdr.GetValue(0).ToString(), rdr.GetInt32(1));
                }
                rdr.Close();
            }

            return dictionary;
        }

        public List<CouponInfo> GetCouponInfoList(int publishmentSystemId, int actId)
        {
            var list = new List<CouponInfo>();

            var builder = new StringBuilder(
                $"WHERE {CouponAttribute.PublishmentSystemId} = {publishmentSystemId} AND {CouponAttribute.ActId} = {actId}");
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, builder.ToString(), "ORDER BY ID");

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var couponInfo = new CouponInfo(rdr);
                    list.Add(couponInfo);
                }
                rdr.Close();
            }

            return list;
        }

        public List<CouponInfo> GetAllCouponInfoList(int publishmentSystemId)
        {
            var list = new List<CouponInfo>();

            var builder = new StringBuilder(
                $"WHERE {CouponAttribute.PublishmentSystemId} = {publishmentSystemId} AND {CouponAttribute.TotalNum} > 0");
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, builder.ToString(), "ORDER BY ID");

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var couponInfo = new CouponInfo(rdr);
                    list.Add(couponInfo);
                }
                rdr.Close();
            }

            return list;
        }

        public List<CouponInfo> GetCouponInfoList(int publishmentSystemId)
        {
            var couponInfoList = new List<CouponInfo>();

            string sqlWhere = $"WHERE {CouponAttribute.PublishmentSystemId} = {publishmentSystemId}";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var couponInfo = new CouponInfo(rdr);
                    couponInfoList.Add(couponInfo);
                }
                rdr.Close();
            }

            return couponInfoList;
        }
    }
}
