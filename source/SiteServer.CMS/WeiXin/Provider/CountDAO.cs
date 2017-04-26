using System;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.WeiXin.Model;
using ECountType = SiteServer.CMS.WeiXin.Model.Enumerations.ECountType;
using ECountTypeUtils = SiteServer.CMS.WeiXin.Model.Enumerations.ECountTypeUtils;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class CountDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_Count";


        public int Insert(CountInfo countInfo)
        {
            var countID = 0;

            var sqlString = @"INSERT INTO wx_Count (PublishmentSystemID, CountYear, CountMonth, CountDay, CountType, Count) VALUES (@PublishmentSystemID, @CountYear, @CountMonth, @CountDay, @CountType, @Count)";

            var parms = new IDataParameter[]
			{
                GetParameter("@PublishmentSystemID", EDataType.Integer, countInfo.PublishmentSystemID),
                GetParameter("@CountYear", EDataType.Integer, countInfo.CountYear),
                GetParameter("@CountMonth", EDataType.Integer, 255, countInfo.CountMonth),
                GetParameter("@CountDay", EDataType.Integer, 200, countInfo.CountDay),        
                GetParameter("@CountType", EDataType.VarChar, 50, ECountTypeUtils.GetValue(countInfo.CountType)), 
                GetParameter("@Count", EDataType.Integer, countInfo.Count),
			};

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, sqlString, parms);
                        countID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, "wx_Count");
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return countID;
        }
       

        public void AddCount(int publishmentSystemID, ECountType countType)
        {
            var count = GetCount(publishmentSystemID, countType);
            var now = DateTime.Now;

            if (count == 0)
            {
                string sqlString =
                    $"INSERT INTO wx_Count (PublishmentSystemID, CountYear, CountMonth, CountDay, CountType, Count) VALUES ({publishmentSystemID}, {now.Year}, {now.Month}, {now.Day}, '{ECountTypeUtils.GetValue(countType)}', 1)";

                ExecuteNonQuery(sqlString);
            }
            else
            {
                string sqlString =
                    $"UPDATE wx_Count SET Count = Count + 1 WHERE PublishmentSystemID = {publishmentSystemID} AND CountYear = {now.Year} AND CountMonth = {now.Month} AND CountDay = {now.Day} AND CountType = '{ECountTypeUtils.GetValue(countType)}'";

                ExecuteNonQuery(sqlString);
            }
        }

        public int GetCount(int publishmentSystemID, ECountType countType)
        {
            var count = 0;

            var now = DateTime.Now;
            string sqlString =
                $"SELECT Count FROM wx_Count WHERE PublishmentSystemID = {publishmentSystemID} AND CountYear = {now.Year} AND CountMonth = {now.Month} AND CountDay = {now.Day} AND CountType = '{ECountTypeUtils.GetValue(countType)}'";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    count = rdr.GetInt32(0);
                }
                rdr.Close();
            }

            return count;
        }

        public int GetCount(int publishmentSystemID, int year, int month, ECountType countType)
        {
            var count = 0;

            string sqlString =
                $"SELECT Count FROM wx_Count WHERE PublishmentSystemID = {publishmentSystemID} AND CountYear = {year} AND CountMonth = {month} AND CountType = '{ECountTypeUtils.GetValue(countType)}'";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    count = rdr.GetInt32(0);
                }
                rdr.Close();
            }

            return count;
        }

        public Dictionary<int, int> GetDayCount(int publishmentSystemID, int year, int month, ECountType countType)
        {
            var dictionary = new Dictionary<int, int>();

            string sqlString =
                $"SELECT CountDay, Count FROM wx_Count WHERE PublishmentSystemID = {publishmentSystemID} AND CountYear = {year} AND CountMonth = {month} AND CountType = '{ECountTypeUtils.GetValue(countType)}'";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    var day = rdr.GetInt32(0);
                    var count = rdr.GetInt32(1);
                    dictionary[day] = count;
                }
                rdr.Close();
            }

            return dictionary;
        }

        public List<CountInfo> GetCountInfoList(int publishmentSystemID)
        {
            var countInfoList = new List<CountInfo>();

            string SQL_WHERE = $"WHERE {CountAttribute.PublishmentSystemID} = {publishmentSystemID}";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var countInfo = new CountInfo(rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetInt32(2), rdr.GetInt32(3), rdr.GetInt32(4), ECountTypeUtils.GetEnumType(rdr.GetValue(5).ToString()), rdr.GetInt32(6));
                    countInfoList.Add(countInfo);
                }
                rdr.Close();
            }

            return countInfoList;
        }
    }
}