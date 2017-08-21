using System;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.Plugin;
using SiteServer.Plugin.Models;
using ECountType = SiteServer.CMS.WeiXin.Model.Enumerations.ECountType;
using ECountTypeUtils = SiteServer.CMS.WeiXin.Model.Enumerations.ECountTypeUtils;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class CountDao : DataProviderBase
    {
        private const string TableName = "wx_Count";


        public int Insert(CountInfo countInfo)
        {
            int countId;

            var sqlString = @"INSERT INTO wx_Count (PublishmentSystemID, CountYear, CountMonth, CountDay, CountType, Count) VALUES (@PublishmentSystemID, @CountYear, @CountMonth, @CountDay, @CountType, @Count)";

            var parms = new IDataParameter[]
			{
                GetParameter("@PublishmentSystemID", DataType.Integer, countInfo.PublishmentSystemId),
                GetParameter("@CountYear", DataType.Integer, countInfo.CountYear),
                GetParameter("@CountMonth", DataType.Integer, 255, countInfo.CountMonth),
                GetParameter("@CountDay", DataType.Integer, 200, countInfo.CountDay),        
                GetParameter("@CountType", DataType.VarChar, 50, ECountTypeUtils.GetValue(countInfo.CountType)), 
                GetParameter("@Count", DataType.Integer, countInfo.Count),
			};

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        countId = ExecuteNonQueryAndReturnId(trans, sqlString, parms);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return countId;
        }
       

        public void AddCount(int publishmentSystemId, ECountType countType)
        {
            var count = GetCount(publishmentSystemId, countType);
            var now = DateTime.Now;

            if (count == 0)
            {
                string sqlString =
                    $"INSERT INTO wx_Count (PublishmentSystemID, CountYear, CountMonth, CountDay, CountType, Count) VALUES ({publishmentSystemId}, {now.Year}, {now.Month}, {now.Day}, '{ECountTypeUtils.GetValue(countType)}', 1)";

                ExecuteNonQuery(sqlString);
            }
            else
            {
                string sqlString =
                    $"UPDATE wx_Count SET Count = Count + 1 WHERE PublishmentSystemID = {publishmentSystemId} AND CountYear = {now.Year} AND CountMonth = {now.Month} AND CountDay = {now.Day} AND CountType = '{ECountTypeUtils.GetValue(countType)}'";

                ExecuteNonQuery(sqlString);
            }
        }

        public int GetCount(int publishmentSystemId, ECountType countType)
        {
            var count = 0;

            var now = DateTime.Now;
            string sqlString =
                $"SELECT Count FROM wx_Count WHERE PublishmentSystemID = {publishmentSystemId} AND CountYear = {now.Year} AND CountMonth = {now.Month} AND CountDay = {now.Day} AND CountType = '{ECountTypeUtils.GetValue(countType)}'";

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

        public int GetCount(int publishmentSystemId, int year, int month, ECountType countType)
        {
            var count = 0;

            string sqlString =
                $"SELECT Count FROM wx_Count WHERE PublishmentSystemID = {publishmentSystemId} AND CountYear = {year} AND CountMonth = {month} AND CountType = '{ECountTypeUtils.GetValue(countType)}'";

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

        public Dictionary<int, int> GetDayCount(int publishmentSystemId, int year, int month, ECountType countType)
        {
            var dictionary = new Dictionary<int, int>();

            string sqlString =
                $"SELECT CountDay, Count FROM wx_Count WHERE PublishmentSystemID = {publishmentSystemId} AND CountYear = {year} AND CountMonth = {month} AND CountType = '{ECountTypeUtils.GetValue(countType)}'";

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

        public List<CountInfo> GetCountInfoList(int publishmentSystemId)
        {
            var countInfoList = new List<CountInfo>();

            string sqlWhere = $"WHERE {CountAttribute.PublishmentSystemId} = {publishmentSystemId}";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
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