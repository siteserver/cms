using System.Collections.Generic;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services.ICacheManager;
using SS.CMS.Services.ISettingsManager;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class ContentGroupRepository : IContentGroupRepository
    {
        private static readonly string CacheKey = StringUtils.GetCacheKey(nameof(ContentGroupRepository));
        private readonly Repository<ContentGroupInfo> _repository;
        private readonly ISettingsManager _settingsManager;
        private readonly ICacheManager _cacheManager;
        public ContentGroupRepository(ISettingsManager settingsManager, ICacheManager cacheManager)
        {
            _repository = new Repository<ContentGroupInfo>(new Db(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
            _settingsManager = settingsManager;
            _cacheManager = cacheManager;
        }

        public IDb Db => _repository.Db;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string GroupName = nameof(ContentGroupInfo.GroupName);
            public const string SiteId = nameof(ContentGroupInfo.SiteId);
            public const string Taxis = nameof(ContentGroupInfo.Taxis);
        }

        public int Insert(ContentGroupInfo groupInfo)
        {
            var maxTaxis = GetMaxTaxis(groupInfo.SiteId);
            groupInfo.Taxis = maxTaxis + 1;

            groupInfo.Id = _repository.Insert(groupInfo);

            if (groupInfo.Id > 0)
            {
                ClearCache();
            }

            return groupInfo.Id;
        }

        public bool Update(ContentGroupInfo groupInfo)
        {
            var success = _repository.Update(groupInfo);

            if (success)
            {
                ClearCache();
            }

            return success;
        }

        public void Delete(int siteId, string groupName)
        {
            _repository.Delete(Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.GroupName, groupName));

            ClearCache();
        }

        private int GetTaxis(int siteId, string groupName)
        {
            return _repository.Get<int>(Q
                .Select(Attr.Taxis)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.GroupName, groupName));
        }

        private void SetTaxis(int siteId, string groupName, int taxis)
        {
            _repository.Update(Q
                .Set(Attr.Taxis, taxis)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.GroupName, groupName)
            );

            ClearCache();
        }

        private int GetMaxTaxis(int siteId)
        {
            return _repository.Max(Attr.Taxis, Q
                       .Where(Attr.SiteId, siteId)
                   ) ?? 0;
        }

        public void UpdateTaxisToUp(int siteId, string groupName)
        {
            var taxis = GetTaxis(siteId, groupName);
            var result = _repository.Get<(string GroupName, int Taxis)?>(Q
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
                SetTaxis(siteId, groupName, higherTaxis);
                SetTaxis(siteId, higherGroupName, taxis);
            }

            ClearCache();
        }

        public void UpdateTaxisToDown(int siteId, string groupName)
        {
            var taxis = GetTaxis(siteId, groupName);
            var result = _repository.Get<(string GroupName, int Taxis)?>(Q
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
                SetTaxis(siteId, groupName, lowerTaxis);
                SetTaxis(siteId, lowerGroupName, taxis);
            }

            ClearCache();
        }

        private Dictionary<int, List<ContentGroupInfo>> GetAllContentGroupsToCache()
        {
            var allDict = new Dictionary<int, List<ContentGroupInfo>>();

            var groupList = _repository.GetAll(Q
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

// using System.Collections.Generic;
// using System.Data;
// using Datory;
// using SiteServer.CMS.Core;
// using SiteServer.Utils;
// using SiteServer.CMS.Model;
// using SiteServer.CMS.DataCache;

// namespace SiteServer.CMS.Provider
// {
//     public class ContentGroupDao
//     {
//         public override string TableName => "siteserver_ContentGroup";

//         public override List<TableColumn> TableColumns => new List<TableColumn>
//         {
//             new TableColumn
//             {
//                 AttributeName = nameof(ContentGroupInfo.Id),
//                 DataType = DataType.Integer,
//                 IsIdentity = true,
//                 IsPrimaryKey = true
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(ContentGroupInfo.GroupName),
//                 DataType = DataType.VarChar,
//                 DataLength = 255
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(ContentGroupInfo.SiteId),
//                 DataType = DataType.Integer
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(ContentGroupInfo.Taxis),
//                 DataType = DataType.Integer
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(ContentGroupInfo.Description),
//                 DataType = DataType.Text
//             }
//         };

//         private const string SqlInsert = "INSERT INTO siteserver_ContentGroup (GroupName, SiteId, Taxis, Description) VALUES (@GroupName, @SiteId, @Taxis, @Description)";
//         private const string SqlUpdate = "UPDATE siteserver_ContentGroup SET Description = @Description WHERE GroupName = @GroupName AND SiteId = @SiteId";
//         private const string SqlDelete = "DELETE FROM siteserver_ContentGroup WHERE GroupName = @GroupName AND SiteId = @SiteId";

//         private const string ParmGroupName = "@GroupName";
//         private const string ParmSiteId = "@SiteId";
//         private const string ParmTaxis = "@Taxis";
//         private const string ParmDescription = "@Description";

//         public void Insert(ContentGroupInfo contentGroup)
//         {
//             var maxTaxis = GetMaxTaxis(contentGroup.SiteId);
//             contentGroup.Taxis = maxTaxis + 1;

//             var insertParms = new IDataParameter[]
// 			{
// 				GetParameter(ParmGroupName, DataType.VarChar, 255, contentGroup.GroupName),
// 				GetParameter(ParmSiteId, DataType.Integer, contentGroup.SiteId),
//                 GetParameter(ParmTaxis, DataType.Integer, contentGroup.Taxis),
// 				GetParameter(ParmDescription, DataType.Text, contentGroup.Description)
// 			};

//             ExecuteNonQuery(SqlInsert, insertParms);

//             ContentGroupManager.ClearCache();
//         }

//         public void Update(ContentGroupInfo contentGroup)
//         {
//             var updateParms = new IDataParameter[]
// 			{
// 				GetParameter(ParmDescription, DataType.Text, contentGroup.Description),
// 				GetParameter(ParmGroupName, DataType.VarChar, 255, contentGroup.GroupName),
// 				GetParameter(ParmSiteId, DataType.Integer, contentGroup.SiteId)
// 			};

//             ExecuteNonQuery(SqlUpdate, updateParms);

//             ContentGroupManager.ClearCache();
//         }

//         public void Delete(string groupName, int siteId)
//         {
//             var contentGroupParms = new IDataParameter[]
// 			{
// 				GetParameter(ParmGroupName, DataType.VarChar, 255, groupName),
// 				GetParameter(ParmSiteId, DataType.Integer, siteId)
// 			};

//             ExecuteNonQuery(SqlDelete, contentGroupParms);

//             ContentGroupManager.ClearCache();
//         }

//    //     public ContentGroupInfo GetContentGroupInfo(string groupName, int siteId)
//    //     {
//    //         ContentGroupInfo contentGroup = null;

//    //         string sqlString =
//    //             $"SELECT GroupName, SiteId, Taxis, Description FROM siteserver_ContentGroup WHERE GroupName = @GroupName AND SiteId = {siteId}";

//    //         var selectParms = new IDataParameter[]
// 			//{
// 			//	GetParameter(ParmGroupName, DataType.VarChar, 255, groupName)			 
// 			//};

//    //         using (var rdr = ExecuteReader(sqlString, selectParms))
//    //         {
//    //             if (rdr.Read())
//    //             {
//    //                 var i = 0;
//    //                 contentGroup = new ContentGroupInfo(GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i));
//    //             }
//    //             rdr.Close();
//    //         }

//    //         return contentGroup;
//    //     }

//         private void SetTaxis(int siteId, string groupName, int taxis)
//         {
//             var sqlString =
//                 $"UPDATE {TableName} SET Taxis = {taxis} WHERE (GroupName = @GroupName AND SiteId = {siteId})";
//             var updateParms = new IDataParameter[]
//             {
//                 GetParameter(ParmGroupName, DataType.VarChar, 255, groupName)
//             };
//             ExecuteNonQuery(sqlString, updateParms);

//             ContentGroupManager.ClearCache();
//         }

//         public void UpdateTaxisToUp(int siteId, string groupName)
//         {
//             //Get Higher Taxis and ID
//             //string sqlString =
//             //    $"SELECT TOP 1 GroupName, Taxis FROM siteserver_ContentGroup WHERE (Taxis > (SELECT Taxis FROM siteserver_ContentGroup WHERE GroupName = @GroupName AND SiteId = {siteId}) AND SiteId = {siteId}) ORDER BY Taxis";
//             var sqlString = SqlUtils.ToTopSqlString("siteserver_ContentGroup", "GroupName, Taxis",
//                 $"WHERE (Taxis > (SELECT Taxis FROM siteserver_ContentGroup WHERE GroupName = @GroupName AND SiteId = {siteId}) AND SiteId = {siteId})",
//                 "ORDER BY Taxis", 1);

//             var selectParms = new IDataParameter[]
// 			{
// 				GetParameter(ParmGroupName, DataType.VarChar, 255, groupName)			 
// 			};
//             var higherGroupName = string.Empty;
//             var higherTaxis = 0;

//             using (var rdr = ExecuteReader(sqlString, selectParms))
//             {
//                 if (rdr.Read())
//                 {
//                     higherGroupName = GetString(rdr, 0);
//                     higherTaxis = GetInt(rdr, 1);
//                 }
//                 rdr.Close();
//             }

//             if (!string.IsNullOrEmpty(higherGroupName))
//             {
//                 //Get Taxis Of Selected ID
//                 var selectedTaxis = GetTaxis(siteId, groupName);

//                 //Set The Selected Class Taxis To Higher Level
//                 SetTaxis(siteId, groupName, higherTaxis);
//                 //Set The Higher Class Taxis To Lower Level
//                 SetTaxis(siteId, higherGroupName, selectedTaxis);
//             }

//             ContentGroupManager.ClearCache();
//         }

//         public void UpdateTaxisToDown(int siteId, string groupName)
//         {
//             //Get Lower Taxis and ID
//             //string sqlString =
//             //    $"SELECT TOP 1 GroupName, Taxis FROM siteserver_ContentGroup WHERE (Taxis < (SELECT Taxis FROM siteserver_ContentGroup WHERE GroupName = @GroupName AND SiteId = {siteId}) AND SiteId = {siteId}) ORDER BY Taxis DESC";
//             var sqlString = SqlUtils.ToTopSqlString("siteserver_ContentGroup", "GroupName, Taxis",
//                 $"WHERE (Taxis < (SELECT Taxis FROM siteserver_ContentGroup WHERE GroupName = @GroupName AND SiteId = {siteId}) AND SiteId = {siteId})",
//                 "ORDER BY Taxis DESC", 1);

//             var selectParms = new IDataParameter[]
// 			{
// 				GetParameter(ParmGroupName, DataType.VarChar, 255, groupName)			 
// 			};
//             var lowerGroupName = string.Empty;
//             var lowerTaxis = 0;

//             using (var rdr = ExecuteReader(sqlString, selectParms))
//             {
//                 if (rdr.Read())
//                 {
//                     lowerGroupName = GetString(rdr, 0);
//                     lowerTaxis = GetInt(rdr, 1);
//                 }
//                 rdr.Close();
//             }

//             if (!string.IsNullOrEmpty(lowerGroupName))
//             {
//                 //Get Taxis Of Selected Class
//                 var selectedTaxis = GetTaxis(siteId, groupName);

//                 //Set The Selected Class Taxis To Lower Level
//                 SetTaxis(siteId, groupName, lowerTaxis);
//                 //Set The Lower Class Taxis To Higher Level
//                 SetTaxis(siteId, lowerGroupName, selectedTaxis);
//             }

//             ContentGroupManager.ClearCache();
//         }

//         public Dictionary<int, List<ContentGroupInfo>> GetAllContentGroups()
//         {
//             var allDict = new Dictionary<int, List<ContentGroupInfo>>();

//             var sqlString =
//                 $"SELECT GroupName, SiteId, Taxis, Description FROM {TableName} ORDER BY Taxis DESC, GroupName";

//             using (var rdr = ExecuteReader(sqlString))
//             {
//                 while (rdr.Read())
//                 {
//                     var i = 0;
//                     var group = new ContentGroupInfo(GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++),
//                         GetString(rdr, i));

//                     List<ContentGroupInfo> list;
//                     allDict.TryGetValue(group.SiteId, out list);

//                     if (list == null)
//                     {
//                         list = new List<ContentGroupInfo>();
//                     }

//                     list.Add(group);

//                     allDict[group.SiteId] = list;
//                 }
//                 rdr.Close();
//             }

//             return allDict;
//         }

//         private int GetTaxis(int siteId, string groupName)
//         {
//             string sqlString =
//                 $"SELECT Taxis FROM siteserver_ContentGroup WHERE (GroupName = @GroupName AND SiteId = {siteId})";

//             var selectParms = new IDataParameter[]
//             {
//                 GetParameter(ParmGroupName, DataType.VarChar, 255, groupName)
//             };

//             return DatabaseUtils.GetIntResult(sqlString, selectParms);
//         }

//         private int GetMaxTaxis(int siteId)
//         {
//             string sqlString =
//                 $"SELECT MAX(Taxis) FROM siteserver_ContentGroup WHERE (SiteId = {siteId})";
//             var maxTaxis = 0;

//             using (var rdr = ExecuteReader(sqlString))
//             {
//                 if (rdr.Read())
//                 {
//                     maxTaxis = GetInt(rdr, 0);
//                 }
//                 rdr.Close();
//             }
//             return maxTaxis;
//         }

//     }
// }