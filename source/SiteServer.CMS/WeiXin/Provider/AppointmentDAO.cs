using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class AppointmentDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_Appointment";
         
        public int Insert(AppointmentInfo appointmentInfo)
        {
            var appointmentID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(appointmentInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);
             
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        appointmentID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return appointmentID;
        }

        public void Update(AppointmentInfo appointmentInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(appointmentInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        public void AddUserCount(int appointmentID)
        {
            if (appointmentID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {AppointmentAttribute.UserCount} = {AppointmentAttribute.UserCount} + 1 WHERE ID = {appointmentID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void UpdateUserCount(int publishmentSystemID, Dictionary<int, int> appointmentIDWithCount)
        {
            if (appointmentIDWithCount.Count == 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {AppointmentAttribute.UserCount} = 0 WHERE {AppointmentAttribute.PublishmentSystemID} = {publishmentSystemID}";
                ExecuteNonQuery(sqlString);
            }
            else
            {
                var appointmentIDList = GetAppointmentIDList(publishmentSystemID);
                foreach (var appointmentID in appointmentIDList)
                {
                    if (appointmentIDWithCount.ContainsKey(appointmentID))
                    {
                        string sqlString =
                            $"UPDATE {TABLE_NAME} SET {AppointmentAttribute.UserCount} = {appointmentIDWithCount[appointmentID]} WHERE ID = {appointmentID}";
                        ExecuteNonQuery(sqlString);
                    }
                    else
                    {
                        string sqlString =
                            $"UPDATE {TABLE_NAME} SET {AppointmentAttribute.UserCount} = 0 WHERE ID = {appointmentID}";
                        ExecuteNonQuery(sqlString);
                    }
                }
            }
        }
  
        public void AddPVCount(int appointmentID)
        {
            if (appointmentID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {AppointmentAttribute.PVCount} = {AppointmentAttribute.PVCount} + 1 WHERE ID = {appointmentID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemID, int appointmentID)
        {
            if (appointmentID > 0)
            {
                var appointmentIDList = new List<int>();
                appointmentIDList.Add(appointmentID);
                DataProviderWX.KeywordDAO.Delete(GetKeywordIDList(appointmentIDList));

                DataProviderWX.AppointmentContentDAO.DeleteAll(appointmentID);
                DataProviderWX.AppointmentItemDAO.DeleteAll(appointmentID);

                string sqlString = $"DELETE FROM {TABLE_NAME} WHERE ID = {appointmentID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemID, List<int> appointmentIDList)
        {
            if (appointmentIDList != null && appointmentIDList.Count > 0)
            {
                DataProviderWX.KeywordDAO.Delete(GetKeywordIDList(appointmentIDList));

                foreach (var appointmentID in appointmentIDList)
                {
                    DataProviderWX.AppointmentContentDAO.DeleteAll(appointmentID);
                    DataProviderWX.AppointmentItemDAO.DeleteAll(appointmentID);
                }

                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(appointmentIDList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        private List<int> GetKeywordIDList(List<int> appointmentIDList)
        {
            var keywordIDList = new List<int>();

            string sqlString =
                $"SELECT {AppointmentAttribute.KeywordID} FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(appointmentIDList)})";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    keywordIDList.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return keywordIDList;
        }

        private List<int> GetAppointmentIDList(int publishmentSystemID)
        {
            var idList = new List<int>();

            string SQL_WHERE = $"WHERE {AppointmentAttribute.PublishmentSystemID} = {publishmentSystemID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, AppointmentAttribute.ID, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    idList.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return idList;
        }

        public AppointmentInfo GetAppointmentInfo(int appointmentID)
        {
            AppointmentInfo appointmentInfo = null;

            string SQL_WHERE = $"WHERE ID = {appointmentID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    appointmentInfo = new AppointmentInfo(rdr);
                }
                rdr.Close();
            }

            return appointmentInfo;
        }

        public List<AppointmentInfo> GetAppointmentInfoListByKeywordID(int publishmentSystemID, int keywordID)
        {
            var appointmentInfoList = new List<AppointmentInfo>();

            string SQL_WHERE =
                $"WHERE {AppointmentAttribute.PublishmentSystemID} = {publishmentSystemID} AND {AppointmentAttribute.IsDisabled} <> '{true}'";
            if (keywordID > 0)
            {
                SQL_WHERE += $" AND {AppointmentAttribute.KeywordID} = {keywordID}";
            }

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
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

        public int GetFirstIDByKeywordID(int publishmentSystemID, int keywordID)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TABLE_NAME} WHERE {AppointmentAttribute.PublishmentSystemID} = {publishmentSystemID} AND {AppointmentAttribute.IsDisabled} <> '{true}' AND {AppointmentAttribute.KeywordID} = {keywordID}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetTitle(int appointmentID)
        {
            var title = string.Empty;

            string SQL_WHERE = $"WHERE ID = {appointmentID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, AppointmentAttribute.Title, SQL_WHERE, null);

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
            string whereString = $"WHERE {AppointmentAttribute.PublishmentSystemID} = {publishmentSystemID}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public List<AppointmentInfo> GetAppointmentInfoList(int publishmentSystemID)
        {
            var appointmentInfoList = new List<AppointmentInfo>();

            string SQL_WHERE = $" AND {AppointmentAttribute.PublishmentSystemID} = {publishmentSystemID}";         

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
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
