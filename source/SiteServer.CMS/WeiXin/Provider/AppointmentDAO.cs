using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class AppointmentDao : DataProviderBase
    {
        private const string TableName = "wx_Appointment";
         
        public int Insert(AppointmentInfo appointmentInfo)
        {
            var appointmentId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(appointmentInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);
             
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        appointmentId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return appointmentId;
        }

        public void Update(AppointmentInfo appointmentInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(appointmentInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            ExecuteNonQuery(sqlUpdate, parms);
        }

        public void AddUserCount(int appointmentId)
        {
            if (appointmentId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {AppointmentAttribute.UserCount} = {AppointmentAttribute.UserCount} + 1 WHERE ID = {appointmentId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void UpdateUserCount(int publishmentSystemId, Dictionary<int, int> appointmentIdWithCount)
        {
            if (appointmentIdWithCount.Count == 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {AppointmentAttribute.UserCount} = 0 WHERE {AppointmentAttribute.PublishmentSystemId} = {publishmentSystemId}";
                ExecuteNonQuery(sqlString);
            }
            else
            {
                var appointmentIdList = GetAppointmentIdList(publishmentSystemId);
                foreach (var appointmentId in appointmentIdList)
                {
                    if (appointmentIdWithCount.ContainsKey(appointmentId))
                    {
                        string sqlString =
                            $"UPDATE {TableName} SET {AppointmentAttribute.UserCount} = {appointmentIdWithCount[appointmentId]} WHERE ID = {appointmentId}";
                        ExecuteNonQuery(sqlString);
                    }
                    else
                    {
                        string sqlString =
                            $"UPDATE {TableName} SET {AppointmentAttribute.UserCount} = 0 WHERE ID = {appointmentId}";
                        ExecuteNonQuery(sqlString);
                    }
                }
            }
        }
  
        public void AddPvCount(int appointmentId)
        {
            if (appointmentId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {AppointmentAttribute.PvCount} = {AppointmentAttribute.PvCount} + 1 WHERE ID = {appointmentId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemId, int appointmentId)
        {
            if (appointmentId > 0)
            {
                var appointmentIdList = new List<int>();
                appointmentIdList.Add(appointmentId);
                DataProviderWx.KeywordDao.Delete(GetKeywordIdList(appointmentIdList));

                DataProviderWx.AppointmentContentDao.DeleteAll(appointmentId);
                DataProviderWx.AppointmentItemDao.DeleteAll(appointmentId);

                string sqlString = $"DELETE FROM {TableName} WHERE ID = {appointmentId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemId, List<int> appointmentIdList)
        {
            if (appointmentIdList != null && appointmentIdList.Count > 0)
            {
                DataProviderWx.KeywordDao.Delete(GetKeywordIdList(appointmentIdList));

                foreach (var appointmentId in appointmentIdList)
                {
                    DataProviderWx.AppointmentContentDao.DeleteAll(appointmentId);
                    DataProviderWx.AppointmentItemDao.DeleteAll(appointmentId);
                }

                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(appointmentIdList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        private List<int> GetKeywordIdList(List<int> appointmentIdList)
        {
            var keywordIdList = new List<int>();

            string sqlString =
                $"SELECT {AppointmentAttribute.KeywordId} FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(appointmentIdList)})";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    keywordIdList.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return keywordIdList;
        }

        private List<int> GetAppointmentIdList(int publishmentSystemId)
        {
            var idList = new List<int>();

            string sqlWhere = $"WHERE {AppointmentAttribute.PublishmentSystemId} = {publishmentSystemId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, AppointmentAttribute.Id, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    idList.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return idList;
        }

        public AppointmentInfo GetAppointmentInfo(int appointmentId)
        {
            AppointmentInfo appointmentInfo = null;

            string sqlWhere = $"WHERE ID = {appointmentId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    appointmentInfo = new AppointmentInfo(rdr);
                }
                rdr.Close();
            }

            return appointmentInfo;
        }

        public List<AppointmentInfo> GetAppointmentInfoListByKeywordId(int publishmentSystemId, int keywordId)
        {
            var appointmentInfoList = new List<AppointmentInfo>();

            string sqlWhere =
                $"WHERE {AppointmentAttribute.PublishmentSystemId} = {publishmentSystemId} AND {AppointmentAttribute.IsDisabled} <> '{true}'";
            if (keywordId > 0)
            {
                sqlWhere += $" AND {AppointmentAttribute.KeywordId} = {keywordId}";
            }

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var appointmentInfo = new AppointmentInfo(rdr);
                    appointmentInfoList.Add(appointmentInfo);
                }
                rdr.Close();
            }

            return appointmentInfoList;
        }

        public int GetFirstIdByKeywordId(int publishmentSystemId, int keywordId)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TableName} WHERE {AppointmentAttribute.PublishmentSystemId} = {publishmentSystemId} AND {AppointmentAttribute.IsDisabled} <> '{true}' AND {AppointmentAttribute.KeywordId} = {keywordId}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetTitle(int appointmentId)
        {
            var title = string.Empty;

            string sqlWhere = $"WHERE ID = {appointmentId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, AppointmentAttribute.Title, sqlWhere, null);

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
            string whereString = $"WHERE {AppointmentAttribute.PublishmentSystemId} = {publishmentSystemId}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public List<AppointmentInfo> GetAppointmentInfoList(int publishmentSystemId)
        {
            var appointmentInfoList = new List<AppointmentInfo>();

            string sqlWhere = $" AND {AppointmentAttribute.PublishmentSystemId} = {publishmentSystemId}";         

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var appointmentInfo = new AppointmentInfo(rdr);
                    appointmentInfoList.Add(appointmentInfo);
                }
                rdr.Close();
            }

            return appointmentInfoList;
        }
    }
}
