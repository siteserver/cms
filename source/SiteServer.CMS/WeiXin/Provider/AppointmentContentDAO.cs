using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class AppointmentContentDao : DataProviderBase
    {
        private const string TableName = "wx_AppointmentContent";

        public int Insert(AppointmentContentInfo contentInfo)
        {
            var contentId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(contentInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        contentId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            DataProviderWx.AppointmentItemDao.AddUserCount(contentInfo.AppointmentItemId);
            DataProviderWx.AppointmentDao.AddUserCount(contentInfo.AppointmentId);

            return contentId;
        }

        public void Update(AppointmentContentInfo contentInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(contentInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            ExecuteNonQuery(sqlUpdate, parms);
        }

        private void UpdateUserCount(int publishmentSystemId)
        {
            var itemIdWithCount = new Dictionary<int, int>();

            string sqlString =
                $"SELECT {AppointmentContentAttribute.AppointmentItemId}, COUNT(*) FROM {TableName} WHERE {AppointmentContentAttribute.PublishmentSystemId} = {publishmentSystemId} GROUP BY {AppointmentContentAttribute.AppointmentItemId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    itemIdWithCount.Add(rdr.GetInt32(0), rdr.GetInt32(1));
                }
                rdr.Close();
            }

            DataProviderWx.AppointmentItemDao.UpdateUserCount(publishmentSystemId, itemIdWithCount);

            var appointmentIdWithCount = new Dictionary<int, int>();

            sqlString =
                $"SELECT {AppointmentContentAttribute.AppointmentId}, COUNT(*) FROM {TableName} WHERE {AppointmentContentAttribute.PublishmentSystemId} = {publishmentSystemId} GROUP BY {AppointmentContentAttribute.AppointmentId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    appointmentIdWithCount.Add(rdr.GetInt32(0), rdr.GetInt32(1));
                }
                rdr.Close();
            }

            DataProviderWx.AppointmentDao.UpdateUserCount(publishmentSystemId, appointmentIdWithCount);
        }

        public void Delete(int publishmentSystemId, int contentId)
        {
            if (contentId > 0)
            {
                string sqlString = $"DELETE FROM {TableName} WHERE ID = {contentId}";
                ExecuteNonQuery(sqlString);

                UpdateUserCount(publishmentSystemId);
            }
        }

        public void Delete(int publishmentSystemId, List<int> contentIdList)
        {
            if (contentIdList != null && contentIdList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";
                ExecuteNonQuery(sqlString);

                UpdateUserCount(publishmentSystemId);
            }
        }

        public void DeleteAll(int appointmentId)
        {
            if (appointmentId > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE {AppointmentContentAttribute.AppointmentId} = {appointmentId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public bool IsExist(int itemId, string cookieSn, string wxOpenId, string userName)
        {
            var isExist = false;

            var statusList = new List<string>();
            statusList.Add(EAppointmentStatusUtils.GetValue(EAppointmentStatus.Handling));
            statusList.Add(EAppointmentStatusUtils.GetValue(EAppointmentStatus.Agree));

            string sqlWhere =
                $"WHERE {AppointmentContentAttribute.AppointmentItemId} = {itemId} AND {AppointmentContentAttribute.Status} IN ({TranslateUtils.ToSqlInStringWithQuote(statusList)})";

            sqlWhere += $" AND ({AppointmentContentAttribute.CookieSn} = '{PageUtils.FilterSql(cookieSn)}'";

            if (!string.IsNullOrEmpty(wxOpenId))
            {
                sqlWhere += $" OR {AppointmentContentAttribute.WxOpenId} = '{PageUtils.FilterSql(wxOpenId)}'";
            }
            else if (!string.IsNullOrEmpty(userName))
            {
                sqlWhere += $" OR {AppointmentContentAttribute.UserName} = '{PageUtils.FilterSql(userName)}'";
            }

            sqlWhere += ")";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, AppointmentContentAttribute.Id, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    isExist = true;
                }
                rdr.Close();
            }

            return isExist;
        }

        public AppointmentContentInfo GetContentInfo(int contentId)
        {
            AppointmentContentInfo contentInfo = null;

            string sqlWhere = $"WHERE ID = {contentId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    contentInfo = new AppointmentContentInfo(rdr);
                }
                rdr.Close();
            }

            return contentInfo;
        }

        public AppointmentContentInfo GetLatestContentInfo(int itemId, string cookieSn, string wxOpenId, string userName)
        {
            AppointmentContentInfo contentInfo = null;

            string sqlWhere = $"WHERE {AppointmentContentAttribute.AppointmentItemId} = {itemId}";

            sqlWhere += $" AND ({AppointmentContentAttribute.CookieSn} = '{PageUtils.FilterSql(cookieSn)}'";

            if (!string.IsNullOrEmpty(wxOpenId))
            {
                sqlWhere += $" AND {AppointmentContentAttribute.WxOpenId} = '{PageUtils.FilterSql(wxOpenId)}'";
            }
            else if (!string.IsNullOrEmpty(userName))
            {
                sqlWhere += $" AND {AppointmentContentAttribute.UserName} = '{PageUtils.FilterSql(userName)}'";
            }

            sqlWhere += ")";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, "ORDER BY ID DESC");

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    contentInfo = new AppointmentContentInfo(rdr);
                }
                rdr.Close();
            }

            return contentInfo;
        }

        public List<AppointmentContentInfo> GetLatestContentInfoList(int appointmentId, string cookieSn, string wxOpenId, string userName)
        {
            var list = new List<AppointmentContentInfo>();

            string sqlWhere = $"WHERE {AppointmentContentAttribute.AppointmentId} = {appointmentId}";

            sqlWhere += $" AND ({AppointmentContentAttribute.CookieSn} = '{PageUtils.FilterSql(cookieSn)}'";

            if (!string.IsNullOrEmpty(wxOpenId))
            {
                sqlWhere += $" AND {AppointmentContentAttribute.WxOpenId} = '{PageUtils.FilterSql(wxOpenId)}'";
            }
            else if (!string.IsNullOrEmpty(userName))
            {
                sqlWhere += $" AND {AppointmentContentAttribute.UserName} = '{PageUtils.FilterSql(userName)}'";
            }

            sqlWhere += ")";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, "ORDER BY ID DESC");

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var contentInfo = new AppointmentContentInfo(rdr);

                    var isExists = false;
                    foreach (var theContentInfo in list)
                    {
                        if (theContentInfo.AppointmentItemId == contentInfo.AppointmentItemId)
                        {
                            isExists = true;
                        }
                    }

                    if (!isExists)
                    {
                        list.Add(contentInfo);
                    }
                }
                rdr.Close();
            }

            return list;
        }

        public string GetSelectString(int publishmentSystemId, int appointmentId)
        {
            string whereString = $"WHERE {AppointmentContentAttribute.PublishmentSystemId} = {publishmentSystemId}";
            if (appointmentId > 0)
            {
                whereString += $" AND {AppointmentContentAttribute.AppointmentId} = {appointmentId}";
            }
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public List<AppointmentContentInfo> GetAppointmentContentInfoList(int publishmentSystemId, int appointmentId)
        {
            var appointmentContentInfolList = new List<AppointmentContentInfo>();


            string sqlWhere =
                $"WHERE {AppointmentContentAttribute.PublishmentSystemId} = {publishmentSystemId} AND {AppointmentContentAttribute.AppointmentId} = {appointmentId}";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, "ORDER BY ID DESC");

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var appointmentContentInfo = new AppointmentContentInfo(rdr);
                    appointmentContentInfolList.Add(appointmentContentInfo);
                }
                rdr.Close();
            }

            return appointmentContentInfolList;
        }

        public List<AppointmentContentInfo> GetAppointmentContentInfoList(int publishmentSystemId, int appointmentId, int appointmentItemId)
        {
            var appointmentContentInfolList = new List<AppointmentContentInfo>();


            string sqlWhere =
                $"WHERE {AppointmentContentAttribute.PublishmentSystemId} = {publishmentSystemId} AND {AppointmentContentAttribute.AppointmentId} = {appointmentId} AND {AppointmentContentAttribute.AppointmentItemId} = {appointmentItemId}";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, "ORDER BY ID DESC");

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var appointmentContentInfo = new AppointmentContentInfo(rdr);
                    appointmentContentInfolList.Add(appointmentContentInfo);
                }
                rdr.Close();
            }

            return appointmentContentInfolList;
        }
    }
}
