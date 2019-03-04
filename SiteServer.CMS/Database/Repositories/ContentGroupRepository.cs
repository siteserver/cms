using System.Collections.Generic;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;

namespace SiteServer.CMS.Database.Repositories
{
    public class ContentGroupRepository : GenericRepository<ContentGroupInfo>
    {
        private static class Attr
        {
            public const string GroupName = nameof(ContentGroupInfo.GroupName);
            public const string SiteId = nameof(ContentGroupInfo.SiteId);
            public const string Taxis = nameof(ContentGroupInfo.Taxis);
        }

        public void Insert(ContentGroupInfo groupInfo)
        {
            var maxTaxis = GetMaxTaxis(groupInfo.SiteId);
            groupInfo.Taxis = maxTaxis + 1;

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamGroupName, contentGroup.GroupName),
            //    GetParameter(ParamSiteId, contentGroup.SiteId),
            //    GetParameter(ParamTaxis, contentGroup.Taxis),
            //    GetParameter(ParamDescription,contentGroup.Description)
            //};
            //"INSERT INTO siteserver_ContentGroup (GroupName, SiteId, Taxis, Description) VALUES (@GroupName, @SiteId, @Taxis, @Description)"
            //DatabaseApi.ExecuteNonQuery(ConnectionString, SqlInsert, parameters);

            InsertObject(groupInfo);

            ContentGroupManager.ClearCache();
        }

        public void Update(ContentGroupInfo groupInfo)
        {
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamDescription,contentGroup.Description),
            //    GetParameter(ParamGroupName, contentGroup.GroupName),
            //    GetParameter(ParamSiteId, contentGroup.SiteId)
            //};
            //"UPDATE siteserver_ContentGroup SET Description = @Description WHERE GroupName = @GroupName AND SiteId = @SiteId"
            //DatabaseApi.ExecuteNonQuery(ConnectionString, SqlUpdate, parameters);

            UpdateObject(groupInfo);

            ContentGroupManager.ClearCache();
        }

        public void Delete(int siteId, string groupName)
        {
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamGroupName, groupName),
            //    GetParameter(ParamSiteId, siteId)
            //};
            //"DELETE FROM siteserver_ContentGroup WHERE GroupName = @GroupName AND SiteId = @SiteId"
            //DatabaseApi.ExecuteNonQuery(ConnectionString, SqlDelete, parameters);

            DeleteAll(Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.GroupName, groupName));

            ContentGroupManager.ClearCache();
        }

        private int GetTaxis(int siteId, string groupName)
        {
            //var sqlString =
            //    $"SELECT Taxis FROM siteserver_ContentGroup WHERE (GroupName = @GroupName AND SiteId = {siteId})";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamGroupName, groupName)
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
            //    $"UPDATE {TableName} SET Taxis = {taxis} WHERE (GroupName = @GroupName AND SiteId = {siteId})";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamGroupName, groupName)
            //};
            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            UpdateAll(Q
                .Set(Attr.Taxis, taxis)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.GroupName, groupName)
            );

            ContentGroupManager.ClearCache();
        }

        private int GetMaxTaxis(int siteId)
        {
            //var sqlString =
            //    $"SELECT MAX(Taxis) FROM siteserver_ContentGroup WHERE (SiteId = {siteId})";
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
            //var sqlString = SqlDifferences.GetSqlString("siteserver_ContentGroup", new List<string>
            //    {
            //        nameof(ContentGroupInfo.GroupName),
            //        nameof(ContentGroupInfo.Taxis)
            //    },
            //    $"WHERE (Taxis > (SELECT Taxis FROM siteserver_ContentGroup WHERE GroupName = @GroupName AND SiteId = {siteId}) AND SiteId = {siteId})",
            //    "ORDER BY Taxis", 1);

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamGroupName, groupName)
            //};
            //var higherGroupName = string.Empty;
            //var higherTaxis = 0;

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

            ContentGroupManager.ClearCache();
        }

        public void UpdateTaxisToDown(int siteId, string groupName)
        {
            //var sqlString = SqlDifferences.GetSqlString("siteserver_ContentGroup", new List<string>
            //    {
            //        nameof(ContentGroupInfo.GroupName),
            //        nameof(ContentGroupInfo.Taxis)
            //    },
            //    $"WHERE (Taxis < (SELECT Taxis FROM siteserver_ContentGroup WHERE GroupName = @GroupName AND SiteId = {siteId}) AND SiteId = {siteId})",
            //    "ORDER BY Taxis DESC", 1);

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamGroupName, groupName)
            //};
            //var lowerGroupName = string.Empty;
            //var lowerTaxis = 0;

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

            ContentGroupManager.ClearCache();
        }

        public Dictionary<int, List<ContentGroupInfo>> GetAllContentGroups()
        {
            var allDict = new Dictionary<int, List<ContentGroupInfo>>();

            //var sqlString =
            //    $"SELECT GroupName, SiteId, Taxis, Description FROM {TableName} ORDER BY Taxis DESC, GroupName";

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    while (rdr.Read())
            //    {
            //        var i = 0;
            //        var group = new ContentGroupInfo(DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++),
            //            DatabaseApi.GetString(rdr, i));

            //        allDict.TryGetValue(group.SiteId, out var list);

            //        if (list == null)
            //        {
            //            list = new List<ContentGroupInfo>();
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
                    list = new List<ContentGroupInfo>();
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

//namespace SiteServer.CMS.Database.Repositories
//{
//    public class ContentGroupDao : DataProviderBase
//    {
//        public override string TableName => "siteserver_ContentGroup";

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(ContentGroupInfo.Id),
//                DataType = DataType.Integer,
//                IsIdentity = true,
//                IsPrimaryKey = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ContentGroupInfo.GroupName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ContentGroupInfo.SiteId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ContentGroupInfo.Taxis),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ContentGroupInfo.Description),
//                DataType = DataType.Text
//            }
//        };

//        private const string SqlInsert = "INSERT INTO siteserver_ContentGroup (GroupName, SiteId, Taxis, Description) VALUES (@GroupName, @SiteId, @Taxis, @Description)";
//        private const string SqlUpdate = "UPDATE siteserver_ContentGroup SET Description = @Description WHERE GroupName = @GroupName AND SiteId = @SiteId";
//        private const string SqlDelete = "DELETE FROM siteserver_ContentGroup WHERE GroupName = @GroupName AND SiteId = @SiteId";

//        private const string ParamGroupName = "@GroupName";
//        private const string ParamSiteId = "@SiteId";
//        private const string ParamTaxis = "@Taxis";
//        private const string ParamDescription = "@Description";

//        public void InsertObject(ContentGroupInfo contentGroup)
//        {
//            var maxTaxis = GetMaxTaxis(contentGroup.SiteId);
//            contentGroup.Taxis = maxTaxis + 1;

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamGroupName, contentGroup.GroupName),
//				GetParameter(ParamSiteId, contentGroup.SiteId),
//                GetParameter(ParamTaxis, contentGroup.Taxis),
//				GetParameter(ParamDescription,contentGroup.Description)
//			};

//            DatabaseApi.ExecuteNonQuery(ConnectionString, SqlInsert, parameters);

//            ContentGroupManager.ClearCache();
//        }

//        public void UpdateObject(ContentGroupInfo contentGroup)
//        {
//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamDescription,contentGroup.Description),
//				GetParameter(ParamGroupName, contentGroup.GroupName),
//				GetParameter(ParamSiteId, contentGroup.SiteId)
//			};

//            DatabaseApi.ExecuteNonQuery(ConnectionString, SqlUpdate, parameters);

//            ContentGroupManager.ClearCache();
//        }

//        public void DeleteById(string groupName, int siteId)
//        {
//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamGroupName, groupName),
//				GetParameter(ParamSiteId, siteId)
//			};

//            DatabaseApi.ExecuteNonQuery(ConnectionString, SqlDelete, parameters);

//            ContentGroupManager.ClearCache();
//        }

//        private void SetTaxis(int siteId, string groupName, int taxis)
//        {
//            var sqlString =
//                $"UPDATE {TableName} SET Taxis = {taxis} WHERE (GroupName = @GroupName AND SiteId = {siteId})";

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamGroupName, groupName)
//            };
//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

//            ContentGroupManager.ClearCache();
//        }

//        public void UpdateTaxisToUp(int siteId, string groupName)
//        {
//            var sqlString = SqlDifferences.GetSqlString("siteserver_ContentGroup", new List<string>
//                {
//                    nameof(ContentGroupInfo.GroupName),
//                    nameof(ContentGroupInfo.Taxis)
//                }, 
//                $"WHERE (Taxis > (SELECT Taxis FROM siteserver_ContentGroup WHERE GroupName = @GroupName AND SiteId = {siteId}) AND SiteId = {siteId})",
//                "ORDER BY Taxis", 1);

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamGroupName, groupName)			 
//			};
//            var higherGroupName = string.Empty;
//            var higherTaxis = 0;

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

//            ContentGroupManager.ClearCache();
//        }

//        public void UpdateTaxisToDown(int siteId, string groupName)
//        {
//            var sqlString = SqlDifferences.GetSqlString("siteserver_ContentGroup", new List<string>
//                {
//                    nameof(ContentGroupInfo.GroupName),
//                    nameof(ContentGroupInfo.Taxis)
//                }, 
//                $"WHERE (Taxis < (SELECT Taxis FROM siteserver_ContentGroup WHERE GroupName = @GroupName AND SiteId = {siteId}) AND SiteId = {siteId})",
//                "ORDER BY Taxis DESC", 1);

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamGroupName, groupName)			 
//			};
//            var lowerGroupName = string.Empty;
//            var lowerTaxis = 0;

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

//            ContentGroupManager.ClearCache();
//        }

//        public Dictionary<int, List<ContentGroupInfo>> GetAllContentGroups()
//        {
//            var allDict = new Dictionary<int, List<ContentGroupInfo>>();

//            var sqlString =
//                $"SELECT GroupName, SiteId, Taxis, Description FROM {TableName} ORDER BY Taxis DESC, GroupName";

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                while (rdr.Read())
//                {
//                    var i = 0;
//                    var group = new ContentGroupInfo(DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++),
//                        DatabaseApi.GetString(rdr, i));

//                    allDict.TryGetValue(group.SiteId, out var list);

//                    if (list == null)
//                    {
//                        list = new List<ContentGroupInfo>();
//                    }

//                    list.Add(group);

//                    allDict[group.SiteId] = list;
//                }
//                rdr.Close();
//            }

//            return allDict;
//        }

//        private int GetTaxis(int siteId, string groupName)
//        {
//            var sqlString =
//                $"SELECT Taxis FROM siteserver_ContentGroup WHERE (GroupName = @GroupName AND SiteId = {siteId})";

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamGroupName, groupName)
//            };

//            return DatabaseApi.Instance.GetIntResult(sqlString, parameters);
//        }

//        private int GetMaxTaxis(int siteId)
//        {
//            var sqlString =
//                $"SELECT MAX(Taxis) FROM siteserver_ContentGroup WHERE (SiteId = {siteId})";
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

//    }
//}