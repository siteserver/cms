using System.Collections.Generic;
using System.Data;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class CouponDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_Coupon";

        public int Insert(CouponInfo couponInfo)
        {
            var couponID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(couponInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);


            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        couponID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return couponID;
        }

        public void Update(CouponInfo couponInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(couponInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        public void UpdateTotalNum(int couponID, int totalNum)
        {
            string sqlString = $"UPDATE {TABLE_NAME} SET {CouponAttribute.TotalNum} = {totalNum} WHERE ID = {couponID}";

            ExecuteNonQuery(sqlString);
        }

        public void UpdateActID(int couponID, int actID)
        {
            string sqlString = $"UPDATE {TABLE_NAME} SET {CouponAttribute.ActID} = {actID} WHERE ID = {couponID}";

            ExecuteNonQuery(sqlString);
        }

        public void Delete(int couponID)
        {
            if (couponID > 0)
            {
                string sqlString = $"DELETE FROM {TABLE_NAME} WHERE ID = {couponID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(List<int> couponIDList)
        {
            if (couponIDList != null && couponIDList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(couponIDList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        public CouponInfo GetCouponInfo(int couponID)
        {
            CouponInfo couponInfo = null;

            string SQL_WHERE = $"WHERE ID = {couponID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    couponInfo = new CouponInfo(rdr);
                }
                rdr.Close();
            }

            return couponInfo;
        }

        public string GetSelectString(int publishmentSystemID)
        {
            string whereString = $"WHERE {CouponAttribute.PublishmentSystemID} = {publishmentSystemID}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public Dictionary<string, int> GetCouponDictionary(int actID)
        {
            var dictionary = new Dictionary<string, int>();

            string SQL_WHERE = $"WHERE {CouponAttribute.ActID} = {actID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, CouponAttribute.Title + "," + CouponAttribute.TotalNum, SQL_WHERE);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    dictionary.Add(rdr.GetValue(0).ToString(), rdr.GetInt32(1));
                }
                rdr.Close();
            }

            return dictionary;
        }

        public List<CouponInfo> GetCouponInfoList(int publishmentSystemID, int actID)
        {
            var list = new List<CouponInfo>();

            var builder = new StringBuilder(
                $"WHERE {CouponAttribute.PublishmentSystemID} = {publishmentSystemID} AND {CouponAttribute.ActID} = {actID}");
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, builder.ToString(), "ORDER BY ID");

            using (var rdr = ExecuteReader(SQL_SELECT))
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

        public List<CouponInfo> GetAllCouponInfoList(int publishmentSystemID)
        {
            var list = new List<CouponInfo>();

            var builder = new StringBuilder(
                $"WHERE {CouponAttribute.PublishmentSystemID} = {publishmentSystemID} AND {CouponAttribute.TotalNum} > 0");
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, builder.ToString(), "ORDER BY ID");

            using (var rdr = ExecuteReader(SQL_SELECT))
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

        public List<CouponInfo> GetCouponInfoList(int publishmentSystemID)
        {
            var couponInfoList = new List<CouponInfo>();

            string SQL_WHERE = $"WHERE {CouponAttribute.PublishmentSystemID} = {publishmentSystemID}";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
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
