using System.Collections.Generic;
using System.Data;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class CouponSNDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_CouponSN";

        public int Insert(CouponSNInfo couponSNInfo)
        {
            var couponSNID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(couponSNInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);


            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        couponSNID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return couponSNID;
        }

        public void Insert(int publishmentSystemID, int couponID, int totalNum)
        {
            var couponSNList = CouponManager.GetCouponSN(totalNum);
            foreach (var sn in couponSNList)
            {
                string sqlString =
                    $"INSERT INTO {TABLE_NAME} (PublishmentSystemID, CouponID, SN, Status) VALUES ({publishmentSystemID}, {couponID}, '{sn}', '{ECouponStatusUtils.GetValue(ECouponStatus.Unused)}')";

                ExecuteNonQuery(sqlString);

            }

            DataProviderWX.CouponDAO.UpdateTotalNum(couponID, DataProviderWX.CouponSNDAO.GetTotalNum(publishmentSystemID, couponID));
        }

        public void Insert(int publishmentSystemID, int couponID, List<string> snList)
        {

            foreach (var sn in snList)
            {
                if (!string.IsNullOrEmpty(sn))
                {
                    string sqlString =
                        $"INSERT INTO {TABLE_NAME} (PublishmentSystemID, CouponID, SN, Status) VALUES ({publishmentSystemID}, {couponID}, '{sn}', '{ECouponStatusUtils.GetValue(ECouponStatus.Unused)}')";
                    ExecuteNonQuery(sqlString);

                }
            }

            DataProviderWX.CouponDAO.UpdateTotalNum(couponID, DataProviderWX.CouponSNDAO.GetTotalNum(publishmentSystemID, couponID));
        }

        public void Update(CouponSNInfo couponSNInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(couponSNInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            ExecuteNonQuery(SQL_UPDATE, parms);

        }

        public void UpdateStatus(ECouponStatus status, List<int> snIDList)
        {
            string sqlString =
                $"UPDATE {TABLE_NAME} SET {CouponSNAttribute.Status} = '{ECouponStatusUtils.GetValue(status)}' WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(snIDList)})";

            if (status == ECouponStatus.Cash)
            {
                sqlString =
                    $"UPDATE {TABLE_NAME} SET {CouponSNAttribute.Status} = '{ECouponStatusUtils.GetValue(status)}', {CouponSNAttribute.HoldDate} = getdate(), {CouponSNAttribute.CashDate} = getdate() WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(snIDList)})";
            }
            else if (status == ECouponStatus.Hold)
            {
                sqlString =
                    $"UPDATE {TABLE_NAME} SET {CouponSNAttribute.Status} = '{ECouponStatusUtils.GetValue(status)}', {CouponSNAttribute.HoldDate} = getdate() WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(snIDList)})";
            }

            ExecuteNonQuery(sqlString);
        }

        public int Hold(int publishmentSystemID, int actID, string cookieSN)
        {
            var snID = 0;

            string sqlString = $@"SELECT ID FROM wx_CouponSN WHERE 
PublishmentSystemID = {publishmentSystemID} AND 
CookieSN = '{cookieSN}' AND 
Status <> '{ECouponStatusUtils.GetValue(ECouponStatus.Unused)}' AND
CouponID IN (SELECT ID FROM wx_Coupon WHERE ActID = {actID})";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    snID = rdr.GetInt32(0);
                }
                rdr.Close();
            }

            if (snID == 0)
            {
                sqlString = $@"SELECT TOP 1 ID FROM wx_CouponSN WHERE 
PublishmentSystemID = {publishmentSystemID} AND
Status = '{ECouponStatusUtils.GetValue(ECouponStatus.Unused)}' AND
CouponID IN (SELECT ID FROM wx_Coupon WHERE ActID = {actID})
ORDER BY ID";

                using (var rdr = ExecuteReader(sqlString))
                {
                    if (rdr.Read())
                    {
                        snID = rdr.GetInt32(0);
                    }
                    rdr.Close();
                }

            }

            return snID;
        }

        public void Delete(int snID)
        {
            if (snID > 0)
            {
                string sqlString = $"DELETE FROM {TABLE_NAME} WHERE ID = {snID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(List<int> snIDList)
        {
            if (snIDList != null && snIDList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(snIDList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        public CouponSNInfo GetSNInfo(int snID)
        {
            CouponSNInfo snInfo = null;

            string SQL_WHERE = $"WHERE ID = {snID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    snInfo = new CouponSNInfo(rdr);
                }
                rdr.Close();
            }

            return snInfo;
        }

        public List<CouponSNInfo> GetSNInfoByCookieSN(int publishmentSystemID, string cookieSN, string uniqueID)
        {
            var list = new List<CouponSNInfo>();
            StringBuilder builder;
            if (string.IsNullOrEmpty(uniqueID))
            {
                builder = new StringBuilder(
                    $"WHERE {CouponSNAttribute.PublishmentSystemID} = {publishmentSystemID} AND {CouponSNAttribute.CookieSN} = '{PageUtils.FilterSql(cookieSN)}'");
            }
            else
            {
                builder = new StringBuilder(
                    $"WHERE {CouponSNAttribute.PublishmentSystemID} = {publishmentSystemID} AND {CouponSNAttribute.WXOpenID} = '{uniqueID}'");
            }
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, builder.ToString(), "ORDER BY ID");

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var couponSnInfo = new CouponSNInfo(rdr);
                    list.Add(couponSnInfo);
                }
                rdr.Close();
            }

            return list;
        }

        public int GetTotalNum(int publishmentSystemID, int couponID)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TABLE_NAME} WHERE {CouponSNAttribute.PublishmentSystemID} = {publishmentSystemID} AND {CouponSNAttribute.CouponID} = {couponID}";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetHoldNum(int publishmentSystemID, int couponID)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TABLE_NAME} WHERE {CouponSNAttribute.PublishmentSystemID} = {publishmentSystemID} AND {CouponSNAttribute.CouponID} = {couponID} AND ({CouponSNAttribute.Status} = '{ECouponStatusUtils.GetValue(ECouponStatus.Hold)}')";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetCashNum(int publishmentSystemID, int couponID)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TABLE_NAME} WHERE {CouponSNAttribute.PublishmentSystemID} = {publishmentSystemID} AND {CouponSNAttribute.CouponID} = {couponID} AND {CouponSNAttribute.Status} = '{ECouponStatusUtils.GetValue(ECouponStatus.Cash)}'";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetSelectString(int publishmentSystemID, int couponID)
        {
            string whereString =
                $"WHERE {CouponSNAttribute.PublishmentSystemID} = {publishmentSystemID} AND {CouponSNAttribute.CouponID} = {couponID}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public List<CouponSNInfo> GetCouponSNInfoList(int publishmentSystemID, int couponID)
        {
            var couponSNInfoList = new List<CouponSNInfo>();

            string SQL_WHERE =
                $"WHERE {CouponSNAttribute.PublishmentSystemID} = {publishmentSystemID} AND {CouponSNAttribute.CouponID} = {couponID}";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var couponSNInfo = new CouponSNInfo(rdr);
                    couponSNInfoList.Add(couponSNInfo);
                }
                rdr.Close();
            }

            return couponSNInfoList;
        }

    }
}
