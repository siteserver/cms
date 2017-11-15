using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.Plugin;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class KeywordGroupDao : DataProviderBase
	{
        private const string SqlUpdate = "UPDATE wx_KeywordGroup SET PublishmentSystemID = @PublishmentSystemID, GroupName = @GroupName, Taxis = @Taxis WHERE GroupID = @GroupID";

        private const string SqlDelete = "DELETE FROM wx_KeywordGroup WHERE GroupID = @GroupID";

        private const string SqlSelect = "SELECT GroupID, PublishmentSystemID, GroupName, Taxis FROM wx_KeywordGroup WHERE GroupID = @GroupID";

        private const string SqlSelectAll = "SELECT GroupID, PublishmentSystemID, GroupName, Taxis FROM wx_KeywordGroup ORDER BY Taxis";

        private const string ParmGroupId = "@GroupID";
        private const string ParmPublishmentSystemId = "@PublishmentSystemID";
        private const string ParmGroupName = "@GroupName";
        private const string ParmTaxis = "@Taxis";

		public int Insert(KeywordGroupInfo groupInfo) 
		{
            var groupId = 0;

            var sqlString = "INSERT INTO wx_KeywordGroup (PublishmentSystemID, GroupName, Taxis) VALUES (@PublishmentSystemID, @GroupName, @Taxis)";

            var taxis = GetMaxTaxis() + 1;
			var parms = new IDataParameter[]
			{
                GetParameter(ParmPublishmentSystemId, DataType.Integer, groupInfo.PublishmentSystemId),
                GetParameter(ParmGroupName, DataType.NVarChar, 255, groupInfo.GroupName),
                GetParameter(ParmTaxis, DataType.Integer, taxis)
			};

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        groupId = ExecuteNonQueryAndReturnId(trans, sqlString, parms);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return groupId;
		}

        public void Update(KeywordGroupInfo groupInfo) 
		{
			var parms = new IDataParameter[]
			{
                GetParameter(ParmPublishmentSystemId, DataType.Integer, groupInfo.PublishmentSystemId),
                GetParameter(ParmGroupName, DataType.NVarChar, 255, groupInfo.GroupName),
                GetParameter(ParmTaxis, DataType.Integer, groupInfo.Taxis),
                GetParameter(ParmGroupId, DataType.Integer, groupInfo.GroupId)
			};

            ExecuteNonQuery(SqlUpdate, parms);
		}

		public void Delete(int groupId)
		{
            var parms = new IDataParameter[]
			{
				GetParameter(ParmGroupId, DataType.Integer, groupId)
			};

            ExecuteNonQuery(SqlDelete, parms);
		}

        public KeywordGroupInfo GetKeywordGroupInfo(int groupId)
		{
            KeywordGroupInfo groupInfo = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmGroupId, DataType.Integer, groupId)
			};

            using (var rdr = ExecuteReader(SqlSelect, parms))
            {
                if (rdr.Read())
                {
                    groupInfo = new KeywordGroupInfo(rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetValue(2).ToString(), rdr.GetInt32(3));
                }
                rdr.Close();
            }

            return groupInfo;
		}

		public IEnumerable GetDataSource()
		{
            var enumerable = (IEnumerable)ExecuteReader(SqlSelectAll);
			return enumerable;
		}

        public int GetCount(int parentId)
        {
            var sqlString = "SELECT COUNT(*) FROM wx_KeywordGroup WHERE ParentID = " + parentId;
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<KeywordGroupInfo> GetKeywordGroupInfoList()
        {
            var list = new List<KeywordGroupInfo>();

            using (var rdr = ExecuteReader(SqlSelectAll))
            {
                while (rdr.Read())
                {
                    var groupInfo = new KeywordGroupInfo(rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetValue(2).ToString(), rdr.GetInt32(3));
                    list.Add(groupInfo);
                }
                rdr.Close();
            }

            return list;
        }

        public bool UpdateTaxisToUp(int parentId, int groupId)
        {
            string sqlString =
                $"SELECT TOP 1 GroupID, Taxis FROM wx_KeywordGroup WHERE (Taxis > (SELECT Taxis FROM wx_KeywordGroup WHERE GroupID = {groupId} AND ParentID = {parentId})) AND ParentID = {parentId} ORDER BY Taxis";
            var higherId = 0;
            var higherTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    higherId = rdr.GetInt32(0);
                    higherTaxis = rdr.GetInt32(1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(groupId);

            if (higherId > 0)
            {
                SetTaxis(groupId, higherTaxis);
                SetTaxis(higherId, selectedTaxis);
                return true;
            }
            return false;
        }

        public bool UpdateTaxisToDown(int parentId, int groupId)
        {
            string sqlString =
                $"SELECT TOP 1 GroupID, Taxis FROM wx_KeywordGroup WHERE (Taxis < (SELECT Taxis FROM wx_KeywordGroup WHERE GroupID = {groupId} AND ParentID = {parentId})) AND ParentID = {parentId} ORDER BY Taxis DESC";
            var lowerId = 0;
            var lowerTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    lowerId = rdr.GetInt32(0);
                    lowerTaxis = rdr.GetInt32(1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(groupId);

            if (lowerId > 0)
            {
                SetTaxis(groupId, lowerTaxis);
                SetTaxis(lowerId, selectedTaxis);
                return true;
            }
            return false;
        }

        private int GetMaxTaxis()
        {
            var sqlString = "SELECT MAX(Taxis) FROM wx_KeywordGroup";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        private int GetTaxis(int groupId)
        {
            string sqlString = $"SELECT Taxis FROM wx_KeywordGroup WHERE GroupID = {groupId}";
            var taxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    taxis = Convert.ToInt32(rdr[0]);
                }
                rdr.Close();
            }

            return taxis;
        }

        private void SetTaxis(int groupId, int taxis)
        {
            string sqlString = $"UPDATE wx_KeywordGroup SET Taxis = {taxis} WHERE GroupID = {groupId}";
            ExecuteNonQuery(sqlString);
        }
	}
}