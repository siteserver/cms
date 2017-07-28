using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class KeywordGroupDAO : DataProviderBase
	{
        private const string SQL_UPDATE = "UPDATE wx_KeywordGroup SET PublishmentSystemID = @PublishmentSystemID, GroupName = @GroupName, Taxis = @Taxis WHERE GroupID = @GroupID";

        private const string SQL_DELETE = "DELETE FROM wx_KeywordGroup WHERE GroupID = @GroupID";

        private const string SQL_SELECT = "SELECT GroupID, PublishmentSystemID, GroupName, Taxis FROM wx_KeywordGroup WHERE GroupID = @GroupID";

        private const string SQL_SELECT_ALL = "SELECT GroupID, PublishmentSystemID, GroupName, Taxis FROM wx_KeywordGroup ORDER BY Taxis";

        private const string PARM_GROUP_ID = "@GroupID";
        private const string PARM_PUBLISHMENT_SYSTEM_ID = "@PublishmentSystemID";
        private const string PARM_GROUP_NAME = "@GroupName";
        private const string PARM_TAXIS = "@Taxis";

		public int Insert(KeywordGroupInfo groupInfo) 
		{
            var groupID = 0;

            var sqlString = "INSERT INTO wx_KeywordGroup (PublishmentSystemID, GroupName, Taxis) VALUES (@PublishmentSystemID, @GroupName, @Taxis)";

            var taxis = GetMaxTaxis() + 1;
			var parms = new IDataParameter[]
			{
                GetParameter(PARM_PUBLISHMENT_SYSTEM_ID, EDataType.Integer, groupInfo.PublishmentSystemID),
                GetParameter(PARM_GROUP_NAME, EDataType.NVarChar, 255, groupInfo.GroupName),
                GetParameter(PARM_TAXIS, EDataType.Integer, taxis)
			};

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, sqlString, parms);
                        groupID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, "wx_KeywordGroup");
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return groupID;
		}

        public void Update(KeywordGroupInfo groupInfo) 
		{
			var parms = new IDataParameter[]
			{
                GetParameter(PARM_PUBLISHMENT_SYSTEM_ID, EDataType.Integer, groupInfo.PublishmentSystemID),
                GetParameter(PARM_GROUP_NAME, EDataType.NVarChar, 255, groupInfo.GroupName),
                GetParameter(PARM_TAXIS, EDataType.Integer, groupInfo.Taxis),
                GetParameter(PARM_GROUP_ID, EDataType.Integer, groupInfo.GroupID)
			};

            ExecuteNonQuery(SQL_UPDATE, parms);
		}

		public void Delete(int groupID)
		{
            var parms = new IDataParameter[]
			{
				GetParameter(PARM_GROUP_ID, EDataType.Integer, groupID)
			};

            ExecuteNonQuery(SQL_DELETE, parms);
		}

        public KeywordGroupInfo GetKeywordGroupInfo(int groupID)
		{
            KeywordGroupInfo groupInfo = null;

            var parms = new IDataParameter[]
			{
				GetParameter(PARM_GROUP_ID, EDataType.Integer, groupID)
			};

            using (var rdr = ExecuteReader(SQL_SELECT, parms))
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
            var enumerable = (IEnumerable)ExecuteReader(SQL_SELECT_ALL);
			return enumerable;
		}

        public int GetCount(int parentID)
        {
            var sqlString = "SELECT COUNT(*) FROM wx_KeywordGroup WHERE ParentID = " + parentID;
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<KeywordGroupInfo> GetKeywordGroupInfoList()
        {
            var list = new List<KeywordGroupInfo>();

            using (var rdr = ExecuteReader(SQL_SELECT_ALL))
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

        public bool UpdateTaxisToUp(int parentID, int groupID)
        {
            string sqlString =
                $"SELECT TOP 1 GroupID, Taxis FROM wx_KeywordGroup WHERE (Taxis > (SELECT Taxis FROM wx_KeywordGroup WHERE GroupID = {groupID} AND ParentID = {parentID})) AND ParentID = {parentID} ORDER BY Taxis";
            var higherID = 0;
            var higherTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    higherID = rdr.GetInt32(0);
                    higherTaxis = rdr.GetInt32(1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(groupID);

            if (higherID > 0)
            {
                SetTaxis(groupID, higherTaxis);
                SetTaxis(higherID, selectedTaxis);
                return true;
            }
            return false;
        }

        public bool UpdateTaxisToDown(int parentID, int groupID)
        {
            string sqlString =
                $"SELECT TOP 1 GroupID, Taxis FROM wx_KeywordGroup WHERE (Taxis < (SELECT Taxis FROM wx_KeywordGroup WHERE GroupID = {groupID} AND ParentID = {parentID})) AND ParentID = {parentID} ORDER BY Taxis DESC";
            var lowerID = 0;
            var lowerTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    lowerID = rdr.GetInt32(0);
                    lowerTaxis = rdr.GetInt32(1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(groupID);

            if (lowerID > 0)
            {
                SetTaxis(groupID, lowerTaxis);
                SetTaxis(lowerID, selectedTaxis);
                return true;
            }
            return false;
        }

        private int GetMaxTaxis()
        {
            var sqlString = "SELECT MAX(Taxis) FROM wx_KeywordGroup";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        private int GetTaxis(int groupID)
        {
            string sqlString = $"SELECT Taxis FROM wx_KeywordGroup WHERE GroupID = {groupID}";
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

        private void SetTaxis(int groupID, int taxis)
        {
            string sqlString = $"UPDATE wx_KeywordGroup SET Taxis = {taxis} WHERE GroupID = {groupID}";
            ExecuteNonQuery(sqlString);
        }
	}
}