using System.Collections.Generic;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;

namespace SiteServer.CMS.Database.Repositories
{
    public class ContentTagRepository : GenericRepository<ContentTagInfo>
    {
        private static class Attr
        {
            public const string TagName = nameof(ContentTagInfo.TagName);
            public const string SiteId = nameof(ContentTagInfo.SiteId);
            public const string UseNum = nameof(ContentTagInfo.UseNum);
        }

        public void Insert(ContentTagInfo tagInfo)
        {
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamTagName, contentTag.TagName),
            //    GetParameter(ParamSiteId, contentTag.SiteId),
            //    GetParameter(ParamUseNum, contentTag.UseNum)
            //};
            //var SqlInsert =
            //    "INSERT INTO siteserver_ContentTag (TagName, SiteId, UseNum) VALUES (@TagName, @SiteId, @UseNum)";
            //DatabaseApi.ExecuteNonQuery(ConnectionString, SqlInsert, parameters);

            InsertObject(tagInfo);

            ContentTagManager.ClearCache();
        }

        public void Update(ContentTagInfo tagInfo)
        {
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamUseNum, contentTag.UseNum),
            //    GetParameter(ParamTagName, contentTag.TagName),
            //    GetParameter(ParamSiteId, contentTag.SiteId)
            //};
            //string SqlUpdate = "UPDATE siteserver_ContentTag SET UseNum = @UseNum WHERE TagName = @TagName AND SiteId = @SiteId";
            //DatabaseApi.ExecuteNonQuery(ConnectionString, SqlUpdate, parameters);

            UpdateObject(tagInfo);

            ContentTagManager.ClearCache();
        }

        public void Delete(int siteId, string tagName)
        {
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamTagName, tagName),
            //    GetParameter(ParamSiteId, siteId)
            //};
            //string SqlDelete = "DELETE FROM siteserver_ContentTag WHERE TagName = @TagName AND SiteId = @SiteId";
            //DatabaseApi.ExecuteNonQuery(ConnectionString, SqlDelete, parameters);

            DeleteAll(Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.TagName, tagName));

            ContentTagManager.ClearCache();
        }

        public Dictionary<int, List<ContentTagInfo>> GetAllContentTags()
        {
            var allDict = new Dictionary<int, List<ContentTagInfo>>();

            //var sqlString =
            //    $"SELECT Id, TagName, SiteId, UseNum FROM {TableName} ORDER BY UseNum DESC, TagName";

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    while (rdr.Read())
            //    {
            //        var i = 0;
            //        var tag = new ContentTagInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i));

            //        allDict.TryGetValue(tag.SiteId, out var list);

            //        if (list == null)
            //        {
            //            list = new List<ContentTagInfo>();
            //        }

            //        list.Add(tag);

            //        allDict[tag.SiteId] = list;
            //    }
            //    rdr.Close();
            //}

            var tagList = GetObjectList(Q
                .OrderByDesc(Attr.UseNum)
                .OrderBy(Attr.TagName));

            foreach (var tag in tagList)
            {
                allDict.TryGetValue(tag.SiteId, out var list);

                if (list == null)
                {
                    list = new List<ContentTagInfo>();
                }

                list.Add(tag);

                allDict[tag.SiteId] = list;
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

//namespace SiteServer.CMS.Database.Repositories
//{
//    public class ContentTagDao : DataProviderBase
//    {
//        public override string TableName => "siteserver_ContentTag";

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(ContentTagInfo.Id),
//                DataType = DataType.Integer,
//                IsIdentity = true,
//                IsPrimaryKey = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ContentTagInfo.TagName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ContentTagInfo.SiteId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ContentTagInfo.UseNum),
//                DataType = DataType.Integer
//            }
//        };

//        private const string SqlInsert = "INSERT INTO siteserver_ContentTag (TagName, SiteId, UseNum) VALUES (@TagName, @SiteId, @UseNum)";
//        private const string SqlUpdate = "UPDATE siteserver_ContentTag SET UseNum = @UseNum WHERE TagName = @TagName AND SiteId = @SiteId";
//        private const string SqlDelete = "DELETE FROM siteserver_ContentTag WHERE TagName = @TagName AND SiteId = @SiteId";

//        private const string ParamTagName = "@TagName";
//        private const string ParamSiteId = "@SiteId";
//        private const string ParamUseNum = "@UseNum";

//        public void InsertObject(ContentTagInfo contentTag)
//        {
//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamTagName, contentTag.TagName),
//				GetParameter(ParamSiteId, contentTag.SiteId),
//                GetParameter(ParamUseNum, contentTag.UseNum)
//			};

//            DatabaseApi.ExecuteNonQuery(ConnectionString, SqlInsert, parameters);

//            ContentTagManager.ClearCache();
//        }

//        public void UpdateObject(ContentTagInfo contentTag)
//        {
//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamUseNum, contentTag.UseNum),
//				GetParameter(ParamTagName, contentTag.TagName),
//				GetParameter(ParamSiteId, contentTag.SiteId)
//			};

//            DatabaseApi.ExecuteNonQuery(ConnectionString, SqlUpdate, parameters);

//            ContentTagManager.ClearCache();
//        }

//        public void DeleteById(string tagName, int siteId)
//        {
//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamTagName, tagName),
//				GetParameter(ParamSiteId, siteId)
//			};

//            DatabaseApi.ExecuteNonQuery(ConnectionString, SqlDelete, parameters);

//            ContentTagManager.ClearCache();
//        }

//        public Dictionary<int, List<ContentTagInfo>> GetAllContentTags()
//        {
//            var allDict = new Dictionary<int, List<ContentTagInfo>>();

//            var sqlString =
//                $"SELECT Id, TagName, SiteId, UseNum FROM {TableName} ORDER BY UseNum DESC, TagName";

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                while (rdr.Read())
//                {
//                    var i = 0;
//                    var tag = new ContentTagInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i));

//                    allDict.TryGetValue(tag.SiteId, out var list);

//                    if (list == null)
//                    {
//                        list = new List<ContentTagInfo>();
//                    }

//                    list.Add(tag);

//                    allDict[tag.SiteId] = list;
//                }
//                rdr.Close();
//            }

//            return allDict;
//        }
//    }
//}