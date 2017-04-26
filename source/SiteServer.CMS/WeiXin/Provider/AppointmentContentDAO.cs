using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class AppointmentContentDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_AppointmentContent";

        public int Insert(AppointmentContentInfo contentInfo)
        {
            var contentID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(contentInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        contentID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            DataProviderWX.AppointmentItemDAO.AddUserCount(contentInfo.AppointmentItemID);
            DataProviderWX.AppointmentDAO.AddUserCount(contentInfo.AppointmentID);

            return contentID;
        }

        public void Update(AppointmentContentInfo contentInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(contentInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        private void UpdateUserCount(int publishmentSystemID)
        {
            var itemIDWithCount = new Dictionary<int, int>();

            string sqlString =
                $"SELECT {AppointmentContentAttribute.AppointmentItemID}, COUNT(*) FROM {TABLE_NAME} WHERE {AppointmentContentAttribute.PublishmentSystemID} = {publishmentSystemID} GROUP BY {AppointmentContentAttribute.AppointmentItemID}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    itemIDWithCount.Add(rdr.GetInt32(0), rdr.GetInt32(1));
                }
                rdr.Close();
            }

            DataProviderWX.AppointmentItemDAO.UpdateUserCount(publishmentSystemID, itemIDWithCount);

            var appointmentIDWithCount = new Dictionary<int, int>();

            sqlString =
                $"SELECT {AppointmentContentAttribute.AppointmentID}, COUNT(*) FROM {TABLE_NAME} WHERE {AppointmentContentAttribute.PublishmentSystemID} = {publishmentSystemID} GROUP BY {AppointmentContentAttribute.AppointmentID}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    appointmentIDWithCount.Add(rdr.GetInt32(0), rdr.GetInt32(1));
                }
                rdr.Close();
            }

            DataProviderWX.AppointmentDAO.UpdateUserCount(publishmentSystemID, appointmentIDWithCount);
        }

        public void Delete(int publishmentSystemID, int contentID)
        {
            if (contentID > 0)
            {
                string sqlString = $"DELETE FROM {TABLE_NAME} WHERE ID = {contentID}";
                ExecuteNonQuery(sqlString);

                UpdateUserCount(publishmentSystemID);
            }
        }

        public void Delete(int publishmentSystemID, List<int> contentIDList)
        {
            if (contentIDList != null && contentIDList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIDList)})";
                ExecuteNonQuery(sqlString);

                UpdateUserCount(publishmentSystemID);
            }
        }

        public void DeleteAll(int appointmentID)
        {
            if (appointmentID > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE {AppointmentContentAttribute.AppointmentID} = {appointmentID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public bool IsExist(int itemID, string cookieSN, string wxOpenID, string userName)
        {
            var isExist = false;

            var statusList = new List<string>();
            statusList.Add(EAppointmentStatusUtils.GetValue(EAppointmentStatus.Handling));
            statusList.Add(EAppointmentStatusUtils.GetValue(EAppointmentStatus.Agree));

            string SQL_WHERE =
                $"WHERE {AppointmentContentAttribute.AppointmentItemID} = {itemID} AND {AppointmentContentAttribute.Status} IN ({TranslateUtils.ToSqlInStringWithQuote(statusList)})";

            SQL_WHERE += $" AND ({AppointmentContentAttribute.CookieSN} = '{PageUtils.FilterSql(cookieSN)}'";

            if (!string.IsNullOrEmpty(wxOpenID))
            {
                SQL_WHERE += $" OR {AppointmentContentAttribute.WXOpenID} = '{PageUtils.FilterSql(wxOpenID)}'";
            }
            else if (!string.IsNullOrEmpty(userName))
            {
                SQL_WHERE += $" OR {AppointmentContentAttribute.UserName} = '{PageUtils.FilterSql(userName)}'";
            }

            SQL_WHERE += ")";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, AppointmentContentAttribute.ID, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    isExist = true;
                }
                rdr.Close();
            }

            return isExist;
        }

        public AppointmentContentInfo GetContentInfo(int contentID)
        {
            AppointmentContentInfo contentInfo = null;

            string SQL_WHERE = $"WHERE ID = {contentID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    contentInfo = new AppointmentContentInfo(rdr);
                }
                rdr.Close();
            }

            return contentInfo;
        }

        public AppointmentContentInfo GetLatestContentInfo(int itemID, string cookieSN, string wxOpenID, string userName)
        {
            AppointmentContentInfo contentInfo = null;

            string SQL_WHERE = $"WHERE {AppointmentContentAttribute.AppointmentItemID} = {itemID}";

            SQL_WHERE += $" AND ({AppointmentContentAttribute.CookieSN} = '{PageUtils.FilterSql(cookieSN)}'";

            if (!string.IsNullOrEmpty(wxOpenID))
            {
                SQL_WHERE += $" AND {AppointmentContentAttribute.WXOpenID} = '{PageUtils.FilterSql(wxOpenID)}'";
            }
            else if (!string.IsNullOrEmpty(userName))
            {
                SQL_WHERE += $" AND {AppointmentContentAttribute.UserName} = '{PageUtils.FilterSql(userName)}'";
            }

            SQL_WHERE += ")";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, "ORDER BY ID DESC");

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    contentInfo = new AppointmentContentInfo(rdr);
                }
                rdr.Close();
            }

            return contentInfo;
        }

        public List<AppointmentContentInfo> GetLatestContentInfoList(int appointmentID, string cookieSN, string wxOpenID, string userName)
        {
            var list = new List<AppointmentContentInfo>();

            string SQL_WHERE = $"WHERE {AppointmentContentAttribute.AppointmentID} = {appointmentID}";

            SQL_WHERE += $" AND ({AppointmentContentAttribute.CookieSN} = '{PageUtils.FilterSql(cookieSN)}'";

            if (!string.IsNullOrEmpty(wxOpenID))
            {
                SQL_WHERE += $" AND {AppointmentContentAttribute.WXOpenID} = '{PageUtils.FilterSql(wxOpenID)}'";
            }
            else if (!string.IsNullOrEmpty(userName))
            {
                SQL_WHERE += $" AND {AppointmentContentAttribute.UserName} = '{PageUtils.FilterSql(userName)}'";
            }

            SQL_WHERE += ")";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, "ORDER BY ID DESC");

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var contentInfo = new AppointmentContentInfo(rdr);

                    var isExists = false;
                    foreach (var theContentInfo in list)
                    {
                        if (theContentInfo.AppointmentItemID == contentInfo.AppointmentItemID)
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

        public string GetSelectString(int publishmentSystemID, int appointmentID)
        {
            string whereString = $"WHERE {AppointmentContentAttribute.PublishmentSystemID} = {publishmentSystemID}";
            if (appointmentID > 0)
            {
                whereString += $" AND {AppointmentContentAttribute.AppointmentID} = {appointmentID}";
            }
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public List<AppointmentContentInfo> GetAppointmentContentInfoList(int publishmentSystemID, int appointmentID)
        {
            var appointmentContentInfolList = new List<AppointmentContentInfo>();


            string SQL_WHERE =
                $"WHERE {AppointmentContentAttribute.PublishmentSystemID} = {publishmentSystemID} AND {AppointmentContentAttribute.AppointmentID} = {appointmentID}";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, "ORDER BY ID DESC");

            using (var rdr = ExecuteReader(SQL_SELECT))
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

        public List<AppointmentContentInfo> GetAppointmentContentInfoList(int publishmentSystemID, int appointmentID, int appointmentItemID)
        {
            var appointmentContentInfolList = new List<AppointmentContentInfo>();


            string SQL_WHERE =
                $"WHERE {AppointmentContentAttribute.PublishmentSystemID} = {publishmentSystemID} AND {AppointmentContentAttribute.AppointmentID} = {appointmentID} AND {AppointmentContentAttribute.AppointmentItemID} = {appointmentItemID}";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, "ORDER BY ID DESC");

            using (var rdr = ExecuteReader(SQL_SELECT))
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
