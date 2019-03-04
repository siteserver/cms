using System.Collections.Generic;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Database.Repositories
{
    public class ChannelGroupRepository : GenericRepository<ChannelGroupInfo>
    {
        private static class Attr
        {
            public const string GroupName = nameof(ChannelGroupInfo.GroupName);
            public const string SiteId = nameof(ChannelGroupInfo.SiteId);
            public const string Taxis = nameof(ChannelGroupInfo.Taxis);
        }

        public void Insert(ChannelGroupInfo groupInfo)
        {
            var maxTaxis = GetMaxTaxis(groupInfo.SiteId);
            groupInfo.Taxis = maxTaxis + 1;

            //var sqlString = $"INSERT INTO {TableName} (GroupName, SiteId, Taxis, Description) VALUES (@GroupName, @SiteId, @Taxis, @Description)";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamGroupName, groupInfo.GroupName),
            //    GetParameter(ParamSiteId, groupInfo.SiteId),
            //    GetParameter(ParamTaxis, groupInfo.Taxis),
            //    GetParameter(ParamDescription,groupInfo.Description)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            InsertObject(groupInfo);

            ChannelGroupManager.ClearCache();
        }

        public void Update(ChannelGroupInfo groupInfo)
        {
            //var sqlString = $"UPDATE {TableName} SET Description = @Description WHERE GroupName = @GroupName AND SiteId = @SiteId";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamDescription,groupInfo.Description),
            //    GetParameter(ParamGroupName, groupInfo.GroupName),
            //    GetParameter(ParamSiteId, groupInfo.SiteId)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
            UpdateObject(groupInfo);

            ChannelGroupManager.ClearCache();
        }

        public void Delete(int siteId, string groupName)
        {
            //var sqlString = $"DELETE FROM {TableName} WHERE GroupName = @GroupName AND SiteId = @SiteId";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamGroupName, groupName),
            //    GetParameter(ParamSiteId, siteId)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            DeleteAll(Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.GroupName, groupName));

            var channelIdList = ChannelManager.GetChannelIdList(ChannelManager.GetChannelInfo(siteId, siteId), EScopeType.All, groupName, string.Empty, string.Empty);
            foreach (var channelId in channelIdList)
            {
                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                var groupNameList = TranslateUtils.StringCollectionToStringList(channelInfo.GroupNameCollection);
                groupNameList.Remove(groupName);
                channelInfo.GroupNameCollection = TranslateUtils.ObjectCollectionToString(groupNameList);
                DataProvider.Channel.Update(channelInfo);
            }

            ChannelGroupManager.ClearCache();
        }

        private int GetTaxis(int siteId, string groupName)
        {
            //const string sqlString = "SELECT Taxis FROM siteserver_ChannelGroup WHERE (GroupName = @GroupName AND SiteId = @SiteId)";
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamGroupName, groupName),
            //    GetParameter(ParamSiteId, siteId)
            //};
            //return DatabaseApi.Instance.GetIntResult(sqlString, parameters);

            return GetValue<int>(Q
                .Select(Attr.Taxis)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.GroupName, groupName));
        }

        private void SetTaxis(int siteId, string groupName, int taxis)
        {
            //var sqlString =
            //    $"UPDATE siteserver_ChannelGroup SET Taxis = {taxis} WHERE (GroupName = @GroupName AND SiteId = @SiteId)";
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamGroupName, groupName),
            //    GetParameter(ParamSiteId, siteId)
            //};
            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            UpdateAll(Q
                .Set(Attr.Taxis, taxis)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.GroupName, groupName)
            );

            ChannelGroupManager.ClearCache();
        }

        private int GetMaxTaxis(int siteId)
        {
            //var sqlString =
            //    $"SELECT MAX(Taxis) FROM {TableName} WHERE (SiteId = {siteId})";
            //var maxTaxis = 0;

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    if (rdr.Read())
            //    {
            //        maxTaxis = DatabaseApi.GetInt(rdr, 0);
            //    }
            //    rdr.Close();
            //}
            //return maxTaxis;

            return Max(Attr.Taxis, Q.Where(Attr.SiteId, siteId));
        }

        public void UpdateTaxisToUp(int siteId, string groupName)
        {
            //var sqlString = SqlDifferences.GetSqlString("siteserver_ChannelGroup", new List<string>
            //    {
            //        nameof(ChannelGroupInfo.GroupName),
            //        nameof(ChannelGroupInfo.Taxis)
            //    },
            //    "WHERE (Taxis > (SELECT Taxis FROM siteserver_ChannelGroup WHERE GroupName = @GroupName AND SiteId = @SiteId) AND SiteId = @SiteId)",
            //    "ORDER BY Taxis", 1);

            //var higherGroupName = string.Empty;
            //var higherTaxis = 0;

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamGroupName, groupName),
            //    GetParameter(ParamSiteId, siteId)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        higherGroupName = DatabaseApi.GetString(rdr, 0);
            //        higherTaxis = DatabaseApi.GetInt(rdr, 1);
            //    }
            //    rdr.Close();
            //}

            var taxis = GetTaxis(siteId, groupName);
            var result = GetValue<(string GroupName, int Taxis)?> (Q
                .Select(Attr.GroupName, Attr.Taxis)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.Taxis, ">", taxis)
                .OrderBy(Attr.Taxis));

            var higherGroupName = string.Empty;
            var higherTaxis = 0;
            if (result != null)
            {
                higherGroupName = result.Value.GroupName;
                higherTaxis = result.Value.Taxis;
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
            //var sqlString = SqlDifferences.GetSqlString("siteserver_ChannelGroup", new List<string>
            //    {
            //        nameof(ChannelGroupInfo.GroupName),
            //        nameof(ChannelGroupInfo.Taxis)
            //    },
            //    "WHERE (Taxis < (SELECT Taxis FROM siteserver_ChannelGroup WHERE GroupName = @GroupName AND SiteId = @SiteId) AND SiteId = @SiteId)",
            //    "ORDER BY Taxis DESC", 1);

            //var lowerGroupName = string.Empty;
            //var lowerTaxis = 0;
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamGroupName, groupName),
            //    GetParameter(ParamSiteId, siteId)
            //};
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        lowerGroupName = DatabaseApi.GetString(rdr, 0);
            //        lowerTaxis = DatabaseApi.GetInt(rdr, 1);
            //    }
            //    rdr.Close();
            //}
            var taxis = GetTaxis(siteId, groupName);
            var result = GetValue<(string GroupName, int Taxis)?>(Q
                .Select(Attr.GroupName, Attr.Taxis)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.Taxis, "<", taxis)
                .OrderByDesc(Attr.Taxis));

            var lowerGroupName = string.Empty;
            var lowerTaxis = 0;
            if (result != null)
            {
                lowerGroupName = result.Value.GroupName;
                lowerTaxis = result.Value.Taxis;
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

            //var sqlString =
            //    $"SELECT GroupName, SiteId, Taxis, Description FROM {TableName} ORDER BY Taxis DESC, GroupName";

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    while (rdr.Read())
            //    {
            //        var i = 0;
            //        var group = new ChannelGroupInfo(DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++),
            //            DatabaseApi.GetString(rdr, i));

            //        allDict.TryGetValue(group.SiteId, out var list);

            //        if (list == null)
            //        {
            //            list = new List<ChannelGroupInfo>();
            //        }

            //        list.Add(group);

            //        allDict[group.SiteId] = list;
            //    }
            //    rdr.Close();
            //}

            var groupList = GetObjectList(Q
                .OrderByDesc(Attr.Taxis)
                .OrderBy(Attr.GroupName));

            foreach (var group in groupList)
            {
                allDict.TryGetValue(group.SiteId, out var list);

                if (list == null)
                {
                    list = new List<ChannelGroupInfo>();
                }

                list.Add(group);

                allDict[group.SiteId] = list;
            }

            return allDict;
        }
    }
}


//using System.Collections.Generic;
//using System.Data;
//using SiteServer.CMS.Database.Caches;
//using SiteServer.CMS.Database.Core;
//using SiteServer.CMS.Database.Models;
//using SiteServer.Plugin;
//using SiteServer.Utils;
//using SiteServer.Utils.Enumerations;

//namespace SiteServer.CMS.Database.Repositories
//{
//	public class ChannelGroup : DataProviderBase
//	{
//        public override string TableName => "siteserver_ChannelGroup";

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelGroupInfo.Id),
//                DataType = DataType.Integer,
//                IsIdentity = true,
//                IsPrimaryKey = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelGroupInfo.GroupName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelGroupInfo.SiteId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelGroupInfo.Taxis),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelGroupInfo.Description),
//                DataType = DataType.Text
//            }
//        };

//		private const string ParamGroupName = "@GroupName";
//		private const string ParamSiteId = "@SiteId";
//        private const string ParamTaxis = "@Taxis";
//		private const string ParamDescription = "@Description";


//		public void InsertObject(ChannelGroupInfo groupInfo) 
//		{
//            var maxTaxis = GetMaxTaxis(groupInfo.SiteId);
//            groupInfo.Taxis = maxTaxis + 1;

//            var sqlString = $"INSERT INTO {TableName} (GroupName, SiteId, Taxis, Description) VALUES (@GroupName, @SiteId, @Taxis, @Description)";

//		    IDataParameter[] parameters =
//			{
//				GetParameter(ParamGroupName, groupInfo.GroupName),
//				GetParameter(ParamSiteId, groupInfo.SiteId),
//                GetParameter(ParamTaxis, groupInfo.Taxis),
//				GetParameter(ParamDescription,groupInfo.Description)
//			};

//		    DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

//		    ChannelGroupManager.ClearCache();
//        }

//		public void UpdateObject(ChannelGroupInfo groupInfo) 
//		{
//            var sqlString = $"UPDATE {TableName} SET Description = @Description WHERE GroupName = @GroupName AND SiteId = @SiteId";

//		    IDataParameter[] parameters =
//			{
//				GetParameter(ParamDescription,groupInfo.Description),
//				GetParameter(ParamGroupName, groupInfo.GroupName),
//				GetParameter(ParamSiteId, groupInfo.SiteId)
//			};

//		    DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

//		    ChannelGroupManager.ClearCache();
//        }

//        public void DeleteById(int siteId, string groupName)
//		{
//            var sqlString = $"DELETE FROM {TableName} WHERE GroupName = @GroupName AND SiteId = @SiteId";

//		    IDataParameter[] parameters =
//			{
//				GetParameter(ParamGroupName, groupName),
//				GetParameter(ParamSiteId, siteId)
//			};

//		    DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

//		    var channelIdList = ChannelManager.GetChannelIdList(ChannelManager.GetChannelInfo(siteId, siteId), EScopeType.All, groupName, string.Empty, string.Empty);
//		    foreach (var channelId in channelIdList)
//		    {
//		        var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
//		        var groupNameList = TranslateUtils.StringCollectionToStringList(channelInfo.GroupNameCollection);
//		        groupNameList.Remove(groupName);
//                channelInfo.GroupNameCollection = TranslateUtils.ObjectCollectionToString(groupNameList);
//                DataProvider.Channel.UpdateObject(channelInfo);
//		    }

//		    ChannelGroupManager.ClearCache();
//        }

//        private int GetTaxis(int siteId, string groupName)
//        {
//            const string sqlString = "SELECT Taxis FROM siteserver_ChannelGroup WHERE (GroupName = @GroupName AND SiteId = @SiteId)";
//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamGroupName, groupName),
//				GetParameter(ParamSiteId, siteId)
//			};
//            return DatabaseApi.Instance.GetIntResult(sqlString, parameters);
//        }

//        private void SetTaxis(int siteId, string groupName, int taxis)
//        {
//            var sqlString =
//                $"UPDATE siteserver_ChannelGroup SET Taxis = {taxis} WHERE (GroupName = @GroupName AND SiteId = @SiteId)";
//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamGroupName, groupName),
//				GetParameter(ParamSiteId, siteId)
//			};
//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
//        }

//        private int GetMaxTaxis(int siteId)
//        {
//            var sqlString =
//                $"SELECT MAX(Taxis) FROM {TableName} WHERE (SiteId = {siteId})";
//            var maxTaxis = 0;

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                if (rdr.Read())
//                {
//                    maxTaxis = DatabaseApi.GetInt(rdr, 0);
//                }
//                rdr.Close();
//            }
//            return maxTaxis;
//        }

//        public void UpdateTaxisToUp(int siteId, string groupName)
//        {
//            var sqlString = SqlDifferences.GetSqlString("siteserver_ChannelGroup", new List<string>
//                {
//                    nameof(ChannelGroupInfo.GroupName),
//                    nameof(ChannelGroupInfo.Taxis)
//                },
//                "WHERE (Taxis > (SELECT Taxis FROM siteserver_ChannelGroup WHERE GroupName = @GroupName AND SiteId = @SiteId) AND SiteId = @SiteId)",
//                "ORDER BY Taxis", 1);

//            var higherGroupName = string.Empty;
//            var higherTaxis = 0;

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamGroupName, groupName),
//				GetParameter(ParamSiteId, siteId)
//			};

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                if (rdr.Read())
//                {
//                    higherGroupName = DatabaseApi.GetString(rdr, 0);
//                    higherTaxis = DatabaseApi.GetInt(rdr, 1);
//                }
//                rdr.Close();
//            }

//            if (!string.IsNullOrEmpty(higherGroupName))
//            {
//                //Get Taxis Of Selected ID
//                var selectedTaxis = GetTaxis(siteId, groupName);

//                //Set The Selected Class Taxis To Higher Level
//                SetTaxis(siteId, groupName, higherTaxis);
//                //Set The Higher Class Taxis To Lower Level
//                SetTaxis(siteId, higherGroupName, selectedTaxis);
//            }

//            ChannelGroupManager.ClearCache();
//        }

//        public void UpdateTaxisToDown(int siteId, string groupName)
//        {
//            var sqlString = SqlDifferences.GetSqlString("siteserver_ChannelGroup", new List<string>
//                {
//                    nameof(ChannelGroupInfo.GroupName),
//                    nameof(ChannelGroupInfo.Taxis)
//                },
//                "WHERE (Taxis < (SELECT Taxis FROM siteserver_ChannelGroup WHERE GroupName = @GroupName AND SiteId = @SiteId) AND SiteId = @SiteId)",
//                "ORDER BY Taxis DESC", 1);

//            var lowerGroupName = string.Empty;
//            var lowerTaxis = 0;
//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamGroupName, groupName),
//				GetParameter(ParamSiteId, siteId)
//			};
//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                if (rdr.Read())
//                {
//                    lowerGroupName = DatabaseApi.GetString(rdr, 0);
//                    lowerTaxis = DatabaseApi.GetInt(rdr, 1);
//                }
//                rdr.Close();
//            }

//            if (!string.IsNullOrEmpty(lowerGroupName))
//            {
//                //Get Taxis Of Selected Class
//                var selectedTaxis = GetTaxis(siteId, groupName);

//                //Set The Selected Class Taxis To Lower Level
//                SetTaxis(siteId, groupName, lowerTaxis);
//                //Set The Lower Class Taxis To Higher Level
//                SetTaxis(siteId, lowerGroupName, selectedTaxis);
//            }

//            ChannelGroupManager.ClearCache();
//        }

//	    public Dictionary<int, List<ChannelGroupInfo>> GetAllChannelGroups()
//	    {
//	        var allDict = new Dictionary<int, List<ChannelGroupInfo>>();

//	        var sqlString =
//	            $"SELECT GroupName, SiteId, Taxis, Description FROM {TableName} ORDER BY Taxis DESC, GroupName";

//	        using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//	        {
//	            while (rdr.Read())
//	            {
//	                var i = 0;
//	                var group = new ChannelGroupInfo(DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++),
//	                    DatabaseApi.GetString(rdr, i));

//	                allDict.TryGetValue(group.SiteId, out var list);

//	                if (list == null)
//	                {
//	                    list = new List<ChannelGroupInfo>();
//	                }

//	                list.Add(group);

//	                allDict[group.SiteId] = list;
//	            }
//	            rdr.Close();
//	        }

//	        return allDict;
//	    }
//    }
//}
