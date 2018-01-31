using System.Collections.Generic;
using System.Data;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.Utils;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.CMS.Provider
{
    public class ContentGroupDao : DataProviderBase
    {
        public override string TableName => "siteserver_ContentGroup";

        public override List<TableColumnInfo> TableColumns => new List<TableColumnInfo>
        {
            new TableColumnInfo
            {
                ColumnName = nameof(ContentGroupInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumnInfo
            {
                ColumnName = nameof(ContentGroupInfo.GroupName),
                DataType = DataType.VarChar,
                Length = 255
            },
            new TableColumnInfo
            {
                ColumnName = nameof(ContentGroupInfo.SiteId),
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = nameof(ContentGroupInfo.Taxis),
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = nameof(ContentGroupInfo.Description),
                DataType = DataType.Text
            }
        };

        private const string SqlInsert = "INSERT INTO siteserver_ContentGroup (GroupName, SiteId, Taxis, Description) VALUES (@GroupName, @SiteId, @Taxis, @Description)";
        private const string SqlUpdate = "UPDATE siteserver_ContentGroup SET Description = @Description WHERE GroupName = @GroupName AND SiteId = @SiteId";
        private const string SqlDelete = "DELETE FROM siteserver_ContentGroup WHERE GroupName = @GroupName AND SiteId = @SiteId";

        private const string ParmGroupName = "@GroupName";
        private const string ParmSiteId = "@SiteId";
        private const string ParmTaxis = "@Taxis";
        private const string ParmDescription = "@Description";

        public void Insert(ContentGroupInfo contentGroup)
        {
            var maxTaxis = GetMaxTaxis(contentGroup.SiteId);
            contentGroup.Taxis = maxTaxis + 1;

            var insertParms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, DataType.VarChar, 255, contentGroup.GroupName),
				GetParameter(ParmSiteId, DataType.Integer, contentGroup.SiteId),
                GetParameter(ParmTaxis, DataType.Integer, contentGroup.Taxis),
				GetParameter(ParmDescription, DataType.Text, contentGroup.Description)
			};

            ExecuteNonQuery(SqlInsert, insertParms);
        }

        public void Update(ContentGroupInfo contentGroup)
        {
            var updateParms = new IDataParameter[]
			{
				GetParameter(ParmDescription, DataType.Text, contentGroup.Description),
				GetParameter(ParmGroupName, DataType.VarChar, 255, contentGroup.GroupName),
				GetParameter(ParmSiteId, DataType.Integer, contentGroup.SiteId)
			};

            ExecuteNonQuery(SqlUpdate, updateParms);
        }

        public void Delete(string groupName, int siteId)
        {
            var contentGroupParms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, DataType.VarChar, 255, groupName),
				GetParameter(ParmSiteId, DataType.Integer, siteId)
			};

            ExecuteNonQuery(SqlDelete, contentGroupParms);
        }

        public ContentGroupInfo GetContentGroupInfo(string groupName, int siteId)
        {
            ContentGroupInfo contentGroup = null;

            string sqlString =
                $"SELECT GroupName, SiteId, Taxis, Description FROM siteserver_ContentGroup WHERE GroupName = @GroupName AND SiteId = {siteId}";

            var selectParms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, DataType.VarChar, 255, groupName)			 
			};

            using (var rdr = ExecuteReader(sqlString, selectParms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    contentGroup = new ContentGroupInfo(GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

            return contentGroup;
        }

        public bool IsExists(string groupName, int siteId)
        {
            var exists = false;

            string sqlString =
                $"SELECT GroupName FROM siteserver_ContentGroup WHERE GroupName = @GroupName AND SiteId = {siteId}";

            var selectParms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, DataType.VarChar, 255, groupName)			 
			};

            using (var rdr = ExecuteReader(sqlString, selectParms))
            {
                if (rdr.Read())
                {
                    exists = true;
                }
                rdr.Close();
            }

            return exists;
        }

        public List<ContentGroupInfo> GetContentGroupInfoList(int siteId)
        {
            var list = new List<ContentGroupInfo>();
            string sqlString =
                $"SELECT GroupName, SiteId, Taxis, Description FROM siteserver_ContentGroup WHERE SiteId = {siteId} ORDER BY Taxis DESC, GroupName";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    list.Add(new ContentGroupInfo(GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i)));
                }
                rdr.Close();
            }

            return list;
        }

        public List<string> GetGroupNameList(int siteId)
        {
            var list = new List<string>();
            string sqlString =
                $"SELECT GroupName FROM siteserver_ContentGroup WHERE SiteId = {siteId} ORDER BY Taxis DESC, GroupName";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetString(rdr, 0));
                }
                rdr.Close();
            }

            return list;
        }

        private int GetTaxis(int siteId, string groupName)
        {
            string sqlString =
                $"SELECT Taxis FROM siteserver_ContentGroup WHERE (GroupName = @GroupName AND SiteId = {siteId})";

            var selectParms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, DataType.VarChar, 255, groupName)			 
			};

            return DataProvider.DatabaseDao.GetIntResult(sqlString, selectParms);
        }

        private void SetTaxis(int siteId, string groupName, int taxis)
        {
            string sqlString =
                $"UPDATE siteserver_ContentGroup SET Taxis = {taxis} WHERE (GroupName = @GroupName AND SiteId = {siteId})";
            var updateParms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, DataType.VarChar, 255, groupName)			 
			};
            ExecuteNonQuery(sqlString, updateParms);
        }

        private int GetMaxTaxis(int siteId)
        {
            string sqlString =
                $"SELECT MAX(Taxis) FROM siteserver_ContentGroup WHERE (SiteId = {siteId})";
            var maxTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    maxTaxis = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return maxTaxis;
        }

        public bool UpdateTaxisToUp(int siteId, string groupName)
        {
            //Get Higher Taxis and ID
            //string sqlString =
            //    $"SELECT TOP 1 GroupName, Taxis FROM siteserver_ContentGroup WHERE (Taxis > (SELECT Taxis FROM siteserver_ContentGroup WHERE GroupName = @GroupName AND SiteId = {siteId}) AND SiteId = {siteId}) ORDER BY Taxis";
            var sqlString = SqlUtils.ToTopSqlString("siteserver_ContentGroup", "GroupName, Taxis",
                $"WHERE (Taxis > (SELECT Taxis FROM siteserver_ContentGroup WHERE GroupName = @GroupName AND SiteId = {siteId}) AND SiteId = {siteId})",
                "ORDER BY Taxis", 1);

            var selectParms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, DataType.VarChar, 255, groupName)			 
			};
            var higherGroupName = string.Empty;
            var higherTaxis = 0;

            using (var rdr = ExecuteReader(sqlString, selectParms))
            {
                if (rdr.Read())
                {
                    higherGroupName = GetString(rdr, 0);
                    higherTaxis = GetInt(rdr, 1);
                }
                rdr.Close();
            }

            if (!string.IsNullOrEmpty(higherGroupName))
            {
                //Get Taxis Of Selected ID
                var selectedTaxis = GetTaxis(siteId, groupName);

                //Set The Selected Class Taxis To Higher Level
                SetTaxis(siteId, groupName, higherTaxis);
                //Set The Higher Class Taxis To Lower Level
                SetTaxis(siteId, higherGroupName, selectedTaxis);
                return true;
            }
            return false;
        }

        public bool UpdateTaxisToDown(int siteId, string groupName)
        {
            //Get Lower Taxis and ID
            //string sqlString =
            //    $"SELECT TOP 1 GroupName, Taxis FROM siteserver_ContentGroup WHERE (Taxis < (SELECT Taxis FROM siteserver_ContentGroup WHERE GroupName = @GroupName AND SiteId = {siteId}) AND SiteId = {siteId}) ORDER BY Taxis DESC";
            var sqlString = SqlUtils.ToTopSqlString("siteserver_ContentGroup", "GroupName, Taxis",
                $"WHERE (Taxis < (SELECT Taxis FROM siteserver_ContentGroup WHERE GroupName = @GroupName AND SiteId = {siteId}) AND SiteId = {siteId})",
                "ORDER BY Taxis DESC", 1);

            var selectParms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, DataType.VarChar, 255, groupName)			 
			};
            var lowerGroupName = string.Empty;
            var lowerTaxis = 0;

            using (var rdr = ExecuteReader(sqlString, selectParms))
            {
                if (rdr.Read())
                {
                    lowerGroupName = GetString(rdr, 0);
                    lowerTaxis = GetInt(rdr, 1);
                }
                rdr.Close();
            }

            if (!string.IsNullOrEmpty(lowerGroupName))
            {
                //Get Taxis Of Selected Class
                var selectedTaxis = GetTaxis(siteId, groupName);

                //Set The Selected Class Taxis To Lower Level
                SetTaxis(siteId, groupName, lowerTaxis);
                //Set The Lower Class Taxis To Higher Level
                SetTaxis(siteId, lowerGroupName, selectedTaxis);
                return true;
            }
            return false;
        }

    }
}