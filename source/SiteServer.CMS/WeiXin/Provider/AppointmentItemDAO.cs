using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class AppointmentItemDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_AppointmentItem";

         
        public int Insert(AppointmentItemInfo appointmentItemInfo)
        {
            var appointmentItemID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(appointmentItemInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);
             
            
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        appointmentItemID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return appointmentItemID;
        }

        public void Update(AppointmentItemInfo appointmentItemInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(appointmentItemInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);
             

            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        public void UpdateAppointmentID(int publishmentSystemID, int appointmentID)
        {
            if (appointmentID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {AppointmentItemAttribute.AppointmentID} = {appointmentID} WHERE {AppointmentItemAttribute.AppointmentID} = 0 AND {AppointmentItemAttribute.PublishmentSystemID} = {publishmentSystemID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemID, int appointmentItemID)
        {
            if (appointmentItemID > 0)
            {  
                string sqlString = $"DELETE FROM {TABLE_NAME} WHERE ID = {appointmentItemID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemID, List<int> appointmentItemIDList)
        {
            if (appointmentItemIDList != null && appointmentItemIDList.Count > 0)
            { 
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(appointmentItemIDList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        public void DeleteAll(int appointmentID)
        {
            if (appointmentID > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE {AppointmentItemAttribute.AppointmentID} = {appointmentID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void AddUserCount(int itemID)
        {
            if (itemID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {AppointmentItemAttribute.UserCount} = {AppointmentItemAttribute.UserCount} + 1 WHERE ID = {itemID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void UpdateUserCount(int publishmentSystemID, Dictionary<int, int> itemIDWithCount)
        {
            if (itemIDWithCount.Count == 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {AppointmentItemAttribute.UserCount} = 0 WHERE {AppointmentItemAttribute.PublishmentSystemID} = {publishmentSystemID}";
                ExecuteNonQuery(sqlString);
            }
            else
            {
                var itemIDList = GetItemIDList(publishmentSystemID);
                foreach (var itemID in itemIDList)
                {
                    if (itemIDWithCount.ContainsKey(itemID))
                    {
                        string sqlString =
                            $"UPDATE {TABLE_NAME} SET {AppointmentItemAttribute.UserCount} = {itemIDWithCount[itemID]} WHERE ID = {itemID}";
                        ExecuteNonQuery(sqlString);
                    }
                    else
                    {
                        string sqlString =
                            $"UPDATE {TABLE_NAME} SET {AppointmentItemAttribute.UserCount} = 0 WHERE ID = {itemID}";
                        ExecuteNonQuery(sqlString);
                    }
                }
            }
        }
 
        public AppointmentItemInfo GetItemInfo(int appointmentItemID)
        {
            AppointmentItemInfo appointmentItemInfo = null;

            string SQL_WHERE = $"WHERE ID = {appointmentItemID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    appointmentItemInfo = new AppointmentItemInfo(rdr);
                }
                rdr.Close();
            }

            return appointmentItemInfo;
        }

        public AppointmentItemInfo GetItemInfo(int publishmentSystemID, int appointmentID)
        {
            AppointmentItemInfo appointmentItemInfo = null;

            string SQL_WHERE =
                $"WHERE {AppointmentItemAttribute.PublishmentSystemID} = {publishmentSystemID} AND {AppointmentItemAttribute.AppointmentID} = {appointmentID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    appointmentItemInfo = new AppointmentItemInfo(rdr);
                }
                rdr.Close();
            }

            return appointmentItemInfo;
        }

        public int GetItemID(int publishmentSystemID, int appointmentID)
        {
            var itemID = 0;

            string SQL_WHERE =
                $"WHERE {AppointmentItemAttribute.PublishmentSystemID} = {publishmentSystemID} AND {AppointmentItemAttribute.AppointmentID} = {appointmentID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, AppointmentItemAttribute.ID, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    itemID = rdr.GetInt32(0);
                }
                rdr.Close();
            }

            return itemID;
        }

        private List<int> GetItemIDList(int publishmentSystemID)
        {
            var itemIDList = new List<int>();

            string SQL_WHERE = $"WHERE {AppointmentItemAttribute.PublishmentSystemID} = {publishmentSystemID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, AppointmentItemAttribute.ID, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    itemIDList.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return itemIDList;
        }

        public List<AppointmentItemInfo> GetItemInfoList(int publishmentSystemID, int appointmentID)
        {
            var list = new List<AppointmentItemInfo>();

            string SQL_WHERE =
                $"WHERE {AppointmentItemAttribute.PublishmentSystemID} = {publishmentSystemID} AND {AppointmentItemAttribute.AppointmentID} = {appointmentID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var appointmentItemInfo = new AppointmentItemInfo(rdr);
                    list.Add(appointmentItemInfo);
                }
                rdr.Close();
            }

            return list;
        }

        public List<AppointmentItemInfo> GetItemInfoList(string wxOpenID,string userName)
        {
            var list = new List<AppointmentItemInfo>();

            string SQL_WHERE =
                $"WHERE  {AppointmentItemAttribute.ID} IN (SELECT AppointmentItemID FROM wx_AppointmentContent WHERE UserName = '{PageUtils.FilterSql(userName)}')";
            if (!string.IsNullOrEmpty(wxOpenID))
            {
                SQL_WHERE =
                    $"WHERE  {AppointmentItemAttribute.ID} IN (SELECT AppointmentItemID FROM wx_AppointmentContent WHERE WXOpenID = '{PageUtils.FilterSql(wxOpenID)}')";
            }
            
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var appointmentItemInfo = new AppointmentItemInfo(rdr);
                    list.Add(appointmentItemInfo);
                }
                rdr.Close();
            }

            return list;
        }

         
        public string GetTitle(int appointmentItemID)
        {
            var title = string.Empty;

            string SQL_WHERE = $"WHERE ID = {appointmentItemID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, AppointmentItemAttribute.Title, SQL_WHERE, null);

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
         
        public string GetSelectString(int publishmentSystemID,int appointmentID)
        {
            string whereString =
                $"WHERE {AppointmentItemAttribute.PublishmentSystemID} = {publishmentSystemID} AND {AppointmentItemAttribute.AppointmentID} = {appointmentID}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public List<AppointmentItemInfo> GetAppointmentItemInfoList(int publishmentSystemID)
        {
            var list = new List<AppointmentItemInfo>();

            string SQL_WHERE = $"WHERE {AppointmentItemAttribute.PublishmentSystemID} = {publishmentSystemID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var appointmentItemInfo = new AppointmentItemInfo(rdr);
                    list.Add(appointmentItemInfo);
                }
                rdr.Close();
            }

            return list;
        }
    }
}
