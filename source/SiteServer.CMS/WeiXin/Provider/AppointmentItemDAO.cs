using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class AppointmentItemDao : DataProviderBase
    {
        private const string TableName = "wx_AppointmentItem";

         
        public int Insert(AppointmentItemInfo appointmentItemInfo)
        {
            var appointmentItemId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(appointmentItemInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);
             
            
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        appointmentItemId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return appointmentItemId;
        }

        public void Update(AppointmentItemInfo appointmentItemInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(appointmentItemInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);
             

            ExecuteNonQuery(sqlUpdate, parms);
        }

        public void UpdateAppointmentId(int publishmentSystemId, int appointmentId)
        {
            if (appointmentId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {AppointmentItemAttribute.AppointmentId} = {appointmentId} WHERE {AppointmentItemAttribute.AppointmentId} = 0 AND {AppointmentItemAttribute.PublishmentSystemId} = {publishmentSystemId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemId, int appointmentItemId)
        {
            if (appointmentItemId > 0)
            {  
                string sqlString = $"DELETE FROM {TableName} WHERE ID = {appointmentItemId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemId, List<int> appointmentItemIdList)
        {
            if (appointmentItemIdList != null && appointmentItemIdList.Count > 0)
            { 
                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(appointmentItemIdList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        public void DeleteAll(int appointmentId)
        {
            if (appointmentId > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE {AppointmentItemAttribute.AppointmentId} = {appointmentId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void AddUserCount(int itemId)
        {
            if (itemId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {AppointmentItemAttribute.UserCount} = {AppointmentItemAttribute.UserCount} + 1 WHERE ID = {itemId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void UpdateUserCount(int publishmentSystemId, Dictionary<int, int> itemIdWithCount)
        {
            if (itemIdWithCount.Count == 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {AppointmentItemAttribute.UserCount} = 0 WHERE {AppointmentItemAttribute.PublishmentSystemId} = {publishmentSystemId}";
                ExecuteNonQuery(sqlString);
            }
            else
            {
                var itemIdList = GetItemIdList(publishmentSystemId);
                foreach (var itemId in itemIdList)
                {
                    if (itemIdWithCount.ContainsKey(itemId))
                    {
                        string sqlString =
                            $"UPDATE {TableName} SET {AppointmentItemAttribute.UserCount} = {itemIdWithCount[itemId]} WHERE ID = {itemId}";
                        ExecuteNonQuery(sqlString);
                    }
                    else
                    {
                        string sqlString =
                            $"UPDATE {TableName} SET {AppointmentItemAttribute.UserCount} = 0 WHERE ID = {itemId}";
                        ExecuteNonQuery(sqlString);
                    }
                }
            }
        }
 
        public AppointmentItemInfo GetItemInfo(int appointmentItemId)
        {
            AppointmentItemInfo appointmentItemInfo = null;

            string sqlWhere = $"WHERE ID = {appointmentItemId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    appointmentItemInfo = new AppointmentItemInfo(rdr);
                }
                rdr.Close();
            }

            return appointmentItemInfo;
        }

        public AppointmentItemInfo GetItemInfo(int publishmentSystemId, int appointmentId)
        {
            AppointmentItemInfo appointmentItemInfo = null;

            string sqlWhere =
                $"WHERE {AppointmentItemAttribute.PublishmentSystemId} = {publishmentSystemId} AND {AppointmentItemAttribute.AppointmentId} = {appointmentId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    appointmentItemInfo = new AppointmentItemInfo(rdr);
                }
                rdr.Close();
            }

            return appointmentItemInfo;
        }

        public int GetItemId(int publishmentSystemId, int appointmentId)
        {
            var itemId = 0;

            string sqlWhere =
                $"WHERE {AppointmentItemAttribute.PublishmentSystemId} = {publishmentSystemId} AND {AppointmentItemAttribute.AppointmentId} = {appointmentId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, AppointmentItemAttribute.Id, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    itemId = rdr.GetInt32(0);
                }
                rdr.Close();
            }

            return itemId;
        }

        private List<int> GetItemIdList(int publishmentSystemId)
        {
            var itemIdList = new List<int>();

            string sqlWhere = $"WHERE {AppointmentItemAttribute.PublishmentSystemId} = {publishmentSystemId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, AppointmentItemAttribute.Id, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    itemIdList.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return itemIdList;
        }

        public List<AppointmentItemInfo> GetItemInfoList(int publishmentSystemId, int appointmentId)
        {
            var list = new List<AppointmentItemInfo>();

            string sqlWhere =
                $"WHERE {AppointmentItemAttribute.PublishmentSystemId} = {publishmentSystemId} AND {AppointmentItemAttribute.AppointmentId} = {appointmentId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
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

        public List<AppointmentItemInfo> GetItemInfoList(string wxOpenId,string userName)
        {
            var list = new List<AppointmentItemInfo>();

            string sqlWhere =
                $"WHERE  {AppointmentItemAttribute.Id} IN (SELECT AppointmentItemID FROM wx_AppointmentContent WHERE UserName = '{PageUtils.FilterSql(userName)}')";
            if (!string.IsNullOrEmpty(wxOpenId))
            {
                sqlWhere =
                    $"WHERE  {AppointmentItemAttribute.Id} IN (SELECT AppointmentItemID FROM wx_AppointmentContent WHERE WXOpenID = '{PageUtils.FilterSql(wxOpenId)}')";
            }
            
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
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

         
        public string GetTitle(int appointmentItemId)
        {
            var title = string.Empty;

            string sqlWhere = $"WHERE ID = {appointmentItemId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, AppointmentItemAttribute.Title, sqlWhere, null);

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
         
        public string GetSelectString(int publishmentSystemId,int appointmentId)
        {
            string whereString =
                $"WHERE {AppointmentItemAttribute.PublishmentSystemId} = {publishmentSystemId} AND {AppointmentItemAttribute.AppointmentId} = {appointmentId}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public List<AppointmentItemInfo> GetAppointmentItemInfoList(int publishmentSystemId)
        {
            var list = new List<AppointmentItemInfo>();

            string sqlWhere = $"WHERE {AppointmentItemAttribute.PublishmentSystemId} = {publishmentSystemId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
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
