using System.Collections.Generic;
using System.Data;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Provider
{
	public class ChannelGroupDao : DataProviderBase
	{
        public override string TableName => "siteserver_ChannelGroup";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(ChannelGroupInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelGroupInfo.GroupName),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelGroupInfo.SiteId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelGroupInfo.Taxis),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelGroupInfo.Description),
                DataType = DataType.Text
            }
        };

		private const string ParmGroupName = "@GroupName";
		private const string ParmSiteId = "@SiteId";
        private const string ParmTaxis = "@Taxis";
		private const string ParmDescription = "@Description";


		public void Insert(ChannelGroupInfo groupInfo) 
		{
            var maxTaxis = GetMaxTaxis(groupInfo.SiteId);
            groupInfo.Taxis = maxTaxis + 1;

            var sqlString = $"INSERT INTO {TableName} (GroupName, SiteId, Taxis, Description) VALUES (@GroupName, @SiteId, @Taxis, @Description)";

            var insertParms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, DataType.VarChar, 255, groupInfo.GroupName),
				GetParameter(ParmSiteId, DataType.Integer, groupInfo.SiteId),
                GetParameter(ParmTaxis, DataType.Integer, groupInfo.Taxis),
				GetParameter(ParmDescription, DataType.Text, groupInfo.Description)
			};

            ExecuteNonQuery(sqlString, insertParms);

		    ChannelGroupManager.ClearCache();
        }

		public void Update(ChannelGroupInfo groupInfo) 
		{
            var sqlString = $"UPDATE {TableName} SET Description = @Description WHERE GroupName = @GroupName AND SiteId = @SiteId";

            var updateParms = new IDataParameter[]
			{
				GetParameter(ParmDescription, DataType.Text, groupInfo.Description),
				GetParameter(ParmGroupName, DataType.VarChar, 255, groupInfo.GroupName),
				GetParameter(ParmSiteId, DataType.Integer, groupInfo.SiteId)
			};

            ExecuteNonQuery(sqlString, updateParms);

		    ChannelGroupManager.ClearCache();
        }

        public void Delete(int siteId, string groupName)
		{
            var sqlString = $"DELETE FROM {TableName} WHERE GroupName = @GroupName AND SiteId = @SiteId";

            var groupParms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, DataType.VarChar, 255, groupName),
				GetParameter(ParmSiteId, DataType.Integer, siteId)
			};

            ExecuteNonQuery(sqlString, groupParms);

		    var channelIdList = ChannelManager.GetChannelIdList(ChannelManager.GetChannelInfo(siteId, siteId), EScopeType.All, groupName, string.Empty, string.Empty);
		    foreach (var channelId in channelIdList)
		    {
		        var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
		        var groupNameList = TranslateUtils.StringCollectionToStringList(channelInfo.GroupNameCollection);
		        groupNameList.Remove(groupName);
                channelInfo.GroupNameCollection = TranslateUtils.ObjectCollectionToString(groupNameList);
                DataProvider.ChannelDao.Update(channelInfo);
		    }

		    ChannelGroupManager.ClearCache();
        }

  //      public ChannelGroupInfo GetGroupInfo(int siteId, string groupName)
		//{
		//	ChannelGroupInfo group = null;

  //          const string sqlString = "SELECT GroupName, SiteId, Taxis, Description FROM siteserver_ChannelGroup WHERE GroupName = @GroupName AND SiteId = @SiteId";

  //          var parms = new IDataParameter[]
		//	{
		//		GetParameter(ParmGroupName, DataType.VarChar, 255, groupName),
		//		GetParameter(ParmSiteId, DataType.Integer, siteId)
		//	};
			
		//	using (var rdr = ExecuteReader(sqlString, parms))
		//	{
		//		if (rdr.Read())
		//		{
		//		    var i = 0;
  //                  group = new ChannelGroupInfo(GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i));
		//		}
		//		rdr.Close();
		//	}

		//	return group;
		//}

  //      public bool IsExists(int siteId, string groupName)
		//{
		//	var exists = false;

  //          var sqlString = "SELECT GroupName FROM siteserver_ChannelGroup WHERE GroupName = @GroupName AND SiteId = @SiteId";

  //          var parms = new IDataParameter[]
		//	{
		//		GetParameter(ParmGroupName, DataType.VarChar, 255, groupName),
		//		GetParameter(ParmSiteId, DataType.Integer, siteId)
		//	};
			
		//	using (var rdr = ExecuteReader(sqlString, parms)) 
		//	{
		//		if (rdr.Read()) 
		//		{					
		//			exists = true;
		//		}
		//		rdr.Close();
		//	}

		//	return exists;
		//}

		//public IDataReader GetDataSource(int siteId)
		//{
  //          string sqlString =
  //              $"SELECT GroupName, SiteId, Taxis, Description FROM siteserver_ChannelGroup WHERE SiteId = {siteId} ORDER BY Taxis DESC, GroupName";
		//	var enumerable = ExecuteReader(sqlString);
		//	return enumerable;
		//}

		//public List<ChannelGroupInfo> GetGroupInfoList(int siteId)
		//{
		//	var list = new List<ChannelGroupInfo>();
  //          string sqlString =
  //              $"SELECT GroupName, SiteId, Taxis, Description FROM siteserver_ChannelGroup WHERE SiteId = {siteId} ORDER BY Taxis DESC, GroupName";

		//	using (var rdr = ExecuteReader(sqlString)) 
		//	{
		//		while (rdr.Read())
		//		{
		//		    var i = 0;
  //                  list.Add(new ChannelGroupInfo(GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i)));
		//		}
		//		rdr.Close();
		//	}

		//	return list;
		//}

		//public List<string> GetGroupNameList(int siteId)
		//{
		//	var list = new List<string>();
  //          var sqlString =
  //              $"SELECT GroupName FROM siteserver_ChannelGroup WHERE SiteId = {siteId} ORDER BY Taxis DESC, GroupName";
			
		//	using (var rdr = ExecuteReader(sqlString)) 
		//	{
		//		while (rdr.Read()) 
		//		{
  //                  list.Add(GetString(rdr, 0));
		//		}
		//		rdr.Close();
		//	}

		//	return list;
		//}

        private int GetTaxis(int siteId, string groupName)
        {
            var sqlString = "SELECT Taxis FROM siteserver_ChannelGroup WHERE (GroupName = @GroupName AND SiteId = @SiteId)";
            var parms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, DataType.VarChar, 255, groupName),
				GetParameter(ParmSiteId, DataType.Integer, siteId)
			};
            return DataProvider.DatabaseDao.GetIntResult(sqlString, parms);
        }

        private void SetTaxis(int siteId, string groupName, int taxis)
        {
            string sqlString =
                $"UPDATE siteserver_ChannelGroup SET Taxis = {taxis} WHERE (GroupName = @GroupName AND SiteId = @SiteId)";
            var parms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, DataType.VarChar, 255, groupName),
				GetParameter(ParmSiteId, DataType.Integer, siteId)
			};
            ExecuteNonQuery(sqlString, parms);
        }

        private int GetMaxTaxis(int siteId)
        {
            var sqlString =
                $"SELECT MAX(Taxis) FROM {TableName} WHERE (SiteId = {siteId})";
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

        public void UpdateTaxisToUp(int siteId, string groupName)
        {
            //Get Higher Taxis and ID
            //var sqlString = "SELECT TOP 1 GroupName, Taxis FROM siteserver_ChannelGroup WHERE (Taxis > (SELECT Taxis FROM siteserver_ChannelGroup WHERE GroupName = @GroupName AND SiteId = @SiteId) AND SiteId = @SiteId) ORDER BY Taxis";
            var sqlString = SqlUtils.ToTopSqlString("siteserver_ChannelGroup", "GroupName, Taxis",
                "WHERE (Taxis > (SELECT Taxis FROM siteserver_ChannelGroup WHERE GroupName = @GroupName AND SiteId = @SiteId) AND SiteId = @SiteId)",
                "ORDER BY Taxis", 1);

            var higherGroupName = string.Empty;
            var higherTaxis = 0;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, DataType.VarChar, 255, groupName),
				GetParameter(ParmSiteId, DataType.Integer, siteId)
			};

            using (var rdr = ExecuteReader(sqlString, parms))
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
            }

            ChannelGroupManager.ClearCache();
        }

        public void UpdateTaxisToDown(int siteId, string groupName)
        {
            //Get Lower Taxis and ID
            //var sqlString = "SELECT TOP 1 GroupName, Taxis FROM siteserver_ChannelGroup WHERE (Taxis < (SELECT Taxis FROM siteserver_ChannelGroup WHERE GroupName = @GroupName AND SiteId = @SiteId) AND SiteId = @SiteId) ORDER BY Taxis DESC";
            var sqlString = SqlUtils.ToTopSqlString("siteserver_ChannelGroup", "GroupName, Taxis",
                "WHERE (Taxis < (SELECT Taxis FROM siteserver_ChannelGroup WHERE GroupName = @GroupName AND SiteId = @SiteId) AND SiteId = @SiteId)",
                "ORDER BY Taxis DESC", 1);

            var lowerGroupName = string.Empty;
            var lowerTaxis = 0;
            var parms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, DataType.VarChar, 255, groupName),
				GetParameter(ParmSiteId, DataType.Integer, siteId)
			};
            using (var rdr = ExecuteReader(sqlString, parms))
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
            }
            
            ChannelGroupManager.ClearCache();
        }

	    public Dictionary<int, List<ChannelGroupInfo>> GetAllChannelGroups()
	    {
	        var allDict = new Dictionary<int, List<ChannelGroupInfo>>();

	        var sqlString =
	            $"SELECT GroupName, SiteId, Taxis, Description FROM {TableName} ORDER BY Taxis DESC, GroupName";

	        using (var rdr = ExecuteReader(sqlString))
	        {
	            while (rdr.Read())
	            {
	                var i = 0;
	                var group = new ChannelGroupInfo(GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++),
	                    GetString(rdr, i));

	                List<ChannelGroupInfo> list;
	                allDict.TryGetValue(group.SiteId, out list);

	                if (list == null)
	                {
	                    list = new List<ChannelGroupInfo>();
	                }

	                list.Add(group);

	                allDict[group.SiteId] = list;
	            }
	            rdr.Close();
	        }

	        return allDict;
	    }
    }
}
