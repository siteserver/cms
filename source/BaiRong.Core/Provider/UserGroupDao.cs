using System.Collections.Generic;
using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Provider
{
    public class UserGroupDao : DataProviderBase
    {
        private const string SqlUpdate = "UPDATE bairong_UserGroup SET GroupName = @GroupName, IsDefault = @IsDefault, Description = @Description, ExtendValues = @ExtendValues WHERE GroupID = @GroupID";

        private const string SqlDelete = "DELETE FROM bairong_UserGroup WHERE GroupID = @GroupID";

        private const string SqlSelectById = "SELECT GroupID, GroupName, IsDefault, Description, ExtendValues FROM bairong_UserGroup WHERE GroupID = @GroupID";

        private const string SqlSelectAllId = "SELECT GroupID FROM bairong_UserGroup ORDER BY GroupID";

        private const string ParmGroupId = "@GroupID";
        private const string ParmGroupName = "@GroupName";
        private const string ParmIsDefault = "@IsDefault";
        private const string ParmDescription = "@Description";
        private const string ParmExtendValues = "@ExtendValues";

        public int Insert(UserGroupInfo groupInfo)
        {
            int groupId;

            var sqlString = "INSERT INTO bairong_UserGroup (GroupName, IsDefault, Description, ExtendValues) VALUES (@GroupName, @IsDefault, @Description, @ExtendValues)";

            var insertParms = new IDataParameter[]
			{
                GetParameter(ParmGroupName, EDataType.NVarChar, 50, groupInfo.GroupName),
                GetParameter(ParmIsDefault, EDataType.VarChar, 18, groupInfo.IsDefault.ToString()),
                GetParameter(ParmDescription, EDataType.NVarChar, 255, groupInfo.Description),
                GetParameter(ParmExtendValues, EDataType.NText, groupInfo.Additional.ToString())
			};

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        groupId = ExecuteNonQueryAndReturnId(trans, sqlString, insertParms);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            UserGroupManager.ClearCache();

            return groupId;
        }

        public void Update(UserGroupInfo groupInfo)
        {
            var updateParms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, EDataType.NVarChar, 50, groupInfo.GroupName),
                GetParameter(ParmIsDefault, EDataType.VarChar, 18, groupInfo.IsDefault.ToString()),
                GetParameter(ParmDescription, EDataType.NVarChar, 255, groupInfo.Description),
                GetParameter(ParmExtendValues, EDataType.NText, groupInfo.Additional.ToString()),
				GetParameter(ParmGroupId, EDataType.Integer, groupInfo.GroupId)
			};

            ExecuteNonQuery(SqlUpdate, updateParms);

            UserGroupManager.ClearCache();
        }

        public void Delete(int groupId)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmGroupId, EDataType.Integer, groupId)
			};

            ExecuteNonQuery(SqlDelete, parms);

            UserGroupManager.ClearCache();
        }

        public UserGroupInfo GetUserGroupInfo(int groupId)
        {
            UserGroupInfo groupInfo = null;

            if (groupId > 0)
            {
                var parms = new IDataParameter[]
                {
                    GetParameter(ParmGroupId, EDataType.Integer, groupId)
                };

                using (var rdr = ExecuteReader(SqlSelectById, parms))
                {
                    if (rdr.Read())
                    {
                        var i = 0;
                        groupInfo = new UserGroupInfo(GetInt(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                    }
                    rdr.Close();
                }
            }

            return groupInfo;
        }

        public bool IsExists(string groupName)
        {
            var exists = false;

            var sqlString = "SELECT GroupName FROM bairong_UserGroup WHERE GroupName = @GroupName";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmGroupName, EDataType.NVarChar, 50, groupName)
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    exists = true;
                }
                rdr.Close();
            }

            return exists;
        }

        public UserGroupInfo GetUserGroupByGroupName(string groupName)
        {
            var sqlString = "SELECT GroupID, GroupName, IsDefault, Description, ExtendValues FROM bairong_UserGroup WHERE GroupName = @GroupName";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmGroupName, EDataType.NVarChar, 50, groupName)
			};

            UserGroupInfo groupInfo = null;
            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    groupInfo = new UserGroupInfo(GetInt(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

            return groupInfo;
        }

        private List<int> GetUserGroupIdList()
        {
            var list = new List<int>();

            using (var rdr = ExecuteReader(SqlSelectAllId))
            {
                while (rdr.Read())
                {
                    list.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }

            return list;
        }

        public List<KeyValuePair<int, UserGroupInfo>> GetUserGroupInfoPairList()
        {
            var pairList = new List<KeyValuePair<int, UserGroupInfo>>();

            var groupIdList = GetUserGroupIdList();
            foreach (var groupId in groupIdList)
            {
                var groupInfo = GetUserGroupInfo(groupId);
                var pair = new KeyValuePair<int, UserGroupInfo>(groupId, groupInfo);
                pairList.Add(pair);
            }

            return pairList;
        }

        public List<int> GetGroupIdList()
        {
            var list = new List<int>();

            var sqlString = "SELECT GroupID FROM bairong_UserGroup";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }

            return list;
        }

        private void SetAllDefaultToFalse()
        {
            var sqlString = "UPDATE bairong_UserGroup SET IsDefault = @IsDefault";

            var updateParms = new IDataParameter[]
            {
                GetParameter(ParmIsDefault, EDataType.VarChar, 18, false.ToString())
            };

            ExecuteNonQuery(sqlString, updateParms);
        }

        public void SetDefault(int groupId)
        {
            SetAllDefaultToFalse();

            var sqlString = "UPDATE bairong_UserGroup SET IsDefault = @IsDefault WHERE GroupID = @GroupID";

            var updateParms = new IDataParameter[]
            {
                GetParameter(ParmIsDefault, EDataType.VarChar, 18, true.ToString()),
                GetParameter(ParmGroupId, EDataType.Integer, groupId)
            };

            ExecuteNonQuery(sqlString, updateParms);
        }

        public UserGroupInfo AddDefaultGroup()
        {
            var groupInfo = new UserGroupInfo(0, "默认用户组", true, "系统默认用户组", string.Empty);
            groupInfo.GroupId = Insert(groupInfo);
            return groupInfo;
        }
    }
}
