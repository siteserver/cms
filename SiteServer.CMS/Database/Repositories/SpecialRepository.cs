using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;

namespace SiteServer.CMS.Database.Repositories
{
    public class SpecialRepository : GenericRepository<SpecialInfo>
    {
        private static class Attr
        {
            public const string Id = nameof(SpecialInfo.Id);
            public const string SiteId = nameof(SpecialInfo.SiteId);
            public const string Title = nameof(SpecialInfo.Title);
            public const string Url = nameof(SpecialInfo.Url);
        }

        public void Insert(SpecialInfo specialInfo)
        {
     //       var sqlString = $@"INSERT INTO {TableName}
     //      ({nameof(SpecialInfo.SiteId)}, 
     //       {nameof(SpecialInfo.Title)}, 
     //       {nameof(SpecialInfo.Url)},
     //       {nameof(SpecialInfo.AddDate)})
     //VALUES
     //      (@{nameof(SpecialInfo.SiteId)}, 
     //       @{nameof(SpecialInfo.Title)}, 
     //       @{nameof(SpecialInfo.Url)}, 
     //       @{nameof(SpecialInfo.AddDate)})";

     //       IDataParameter[] parameters =
     //       {
     //           GetParameter(nameof(specialInfo.SiteId), specialInfo.SiteId),
     //           GetParameter(nameof(specialInfo.Title), specialInfo.Title),
     //           GetParameter(nameof(specialInfo.Url), specialInfo.Url),
     //           GetParameter(nameof(specialInfo.AddDate),specialInfo.AddDate)
     //       };

     //       DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            InsertObject(specialInfo);

            SpecialManager.RemoveCache(specialInfo.SiteId);
        }

        public void Update(SpecialInfo specialInfo)
        {
            //var sqlString = $@"UPDATE {TableName} SET
            //    {nameof(SpecialInfo.SiteId)} = @{nameof(SpecialInfo.SiteId)},  
            //    {nameof(SpecialInfo.Title)} = @{nameof(SpecialInfo.Title)}, 
            //    {nameof(SpecialInfo.Url)} = @{nameof(SpecialInfo.Url)},
            //    {nameof(SpecialInfo.AddDate)} = @{nameof(SpecialInfo.AddDate)}
            //WHERE {nameof(SpecialInfo.Id)} = @{nameof(SpecialInfo.Id)}";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(nameof(specialInfo.SiteId), specialInfo.SiteId),
            //    GetParameter(nameof(specialInfo.Title), specialInfo.Title),
            //    GetParameter(nameof(specialInfo.Url), specialInfo.Url),
            //    GetParameter(nameof(specialInfo.AddDate),specialInfo.AddDate),
            //    GetParameter(nameof(specialInfo.Id), specialInfo.Id)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            UpdateObject(specialInfo);

            SpecialManager.RemoveCache(specialInfo.SiteId);
        }

        public void Delete(int siteId, int specialId)
        {
            if (specialId <= 0) return;

            //var sqlString = $"DELETE FROM {TableName} WHERE {nameof(SpecialInfo.Id)} = {specialId}";
            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

            DeleteById(specialId);

            SpecialManager.RemoveCache(siteId);
        }

        public bool IsTitleExists(int siteId, string title)
        {
    //        var exists = false;

    //        var sqlString = $@"SELECT {nameof(SpecialInfo.Id)} FROM {TableName} WHERE 
    //{nameof(SpecialInfo.SiteId)} = @{nameof(SpecialInfo.SiteId)} AND {nameof(SpecialInfo.Title)} = @{nameof(SpecialInfo.Title)}";

    //        IDataParameter[] parameters =
    //        {
    //            GetParameter(nameof(SpecialInfo.SiteId), siteId),
    //            GetParameter(nameof(SpecialInfo.Title), title)
    //        };

    //        using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
    //        {
    //            if (rdr.Read() && !rdr.IsDBNull(0))
    //            {
    //                exists = true;
    //            }
    //            rdr.Close();
    //        }

    //        return exists;

            return Exists(Q.Where(Attr.SiteId, siteId).Where(Attr.Title, title));
        }

        public bool IsUrlExists(int siteId, string url)
        {
            //        var exists = false;

            //        var sqlString = $@"SELECT {nameof(SpecialInfo.Id)} FROM {TableName} WHERE 
            //{nameof(SpecialInfo.SiteId)} = @{nameof(SpecialInfo.SiteId)} AND {nameof(SpecialInfo.Url)} = @{nameof(SpecialInfo.Url)}";

            //        IDataParameter[] parameters =
            //        {
            //            GetParameter(nameof(SpecialInfo.SiteId), siteId),
            //            GetParameter(nameof(SpecialInfo.Url), url)
            //        };

            //        using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
            //        {
            //            if (rdr.Read() && !rdr.IsDBNull(0))
            //            {
            //                exists = true;
            //            }
            //            rdr.Close();
            //        }

            //        return exists;

            return Exists(Q.Where(Attr.SiteId, siteId).Where(Attr.Url, url));
        }

        public IList<SpecialInfo> GetSpecialInfoList(int siteId)
        {
            //var list = new List<SpecialInfo>();

            //var sqlString = $@"SELECT {nameof(SpecialInfo.Id)}, 
            //    {nameof(SpecialInfo.SiteId)},
            //    {nameof(SpecialInfo.Title)}, 
            //    {nameof(SpecialInfo.Url)}, 
            //    {nameof(SpecialInfo.AddDate)}
            //FROM {TableName} WHERE {nameof(SpecialInfo.SiteId)} = {siteId} ORDER BY {nameof(SpecialInfo.Id)} DESC";

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    while (rdr.Read())
            //    {
            //        list.Add(GetSpecialInfo(rdr));
            //    }
            //    rdr.Close();
            //}

            //return list;

            return GetObjectList(Q.Where(Attr.SiteId, siteId).OrderByDesc(Attr.Id));
        }

        public IList<SpecialInfo> GetSpecialInfoList(int siteId, string keyword)
        {
            //var list = new List<SpecialInfo>();

            //keyword = AttackUtils.FilterSql(keyword);

            //var sqlString = $@"SELECT {nameof(SpecialInfo.Id)}, 
            //    {nameof(SpecialInfo.SiteId)},
            //    {nameof(SpecialInfo.Title)}, 
            //    {nameof(SpecialInfo.Url)}, 
            //    {nameof(SpecialInfo.AddDate)}
            //FROM {TableName} WHERE {nameof(SpecialInfo.SiteId)} = {siteId} AND ({nameof(SpecialInfo.Title)} LIKE '%{keyword}%' OR {nameof(SpecialInfo.Url)} LIKE '%{keyword}%')  ORDER BY {nameof(SpecialInfo.Id)} DESC";

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    while (rdr.Read())
            //    {
            //        list.Add(GetSpecialInfo(rdr));
            //    }
            //    rdr.Close();
            //}

            //return list;

            return GetObjectList(Q
                .Where(Attr.SiteId, siteId)
                .OrWhereContains(Attr.Title, keyword)
                .OrWhereContains(Attr.Url, keyword)
                .OrderByDesc(Attr.Id));
        }

        public Dictionary<int, SpecialInfo> GetSpecialInfoDictionaryBySiteId(int siteId)
        {
            //var sqlString = $@"SELECT {nameof(SpecialInfo.Id)}, 
            //{nameof(SpecialInfo.SiteId)},
            //{nameof(SpecialInfo.Title)},
            //{nameof(SpecialInfo.Url)},
            //{nameof(SpecialInfo.AddDate)}
            //FROM {TableName} WHERE {nameof(SpecialInfo.SiteId)} = {siteId}";

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    while (rdr.Read())
            //    {
            //        var specialInfo = GetSpecialInfo(rdr);
            //        dictionary.Add(specialInfo.Id, specialInfo);
            //    }
            //    rdr.Close();
            //}

            var specialInfoList = GetSpecialInfoList(siteId);

            return specialInfoList.ToDictionary(specialInfo => specialInfo.Id);
        }

        //private static SpecialInfo GetSpecialInfo(IDataRecord rdr)
        //{
        //    if (rdr == null) return null;

        //    var specialInfo = new SpecialInfo();

        //    var i = 0;
        //    specialInfo.Id = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
        //    i++;
        //    specialInfo.SiteId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
        //    i++;
        //    specialInfo.Title = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
        //    i++;
        //    specialInfo.Url = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
        //    i++;
        //    specialInfo.AddDate = rdr.IsDBNull(i) ? DateTime.Now : rdr.GetDateTime(i);

        //    return specialInfo;
        //}
    }
}


//using System;
//using System.Collections.Generic;
//using System.Data;
//using SiteServer.CMS.Database.Caches;
//using SiteServer.CMS.Database.Core;
//using SiteServer.CMS.Database.Models;
//using SiteServer.Plugin;
//using SiteServer.Utils;

//namespace SiteServer.CMS.Database.Repositories
//{
//    public class Special : DataProviderBase
//    {
//        public override string TableName => "siteserver_Special";

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(SpecialInfo.Id),
//                DataType = DataType.Integer,
//                IsIdentity = true,
//                IsPrimaryKey = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(SpecialInfo.SiteId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(SpecialInfo.Title),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(SpecialInfo.Url),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(SpecialInfo.AddDate),
//                DataType = DataType.DateTime
//            }
//        };

//        public void InsertObject(SpecialInfo specialInfo)
//        {
//            var sqlString = $@"INSERT INTO {TableName}
//           ({nameof(SpecialInfo.SiteId)}, 
//            {nameof(SpecialInfo.Title)}, 
//            {nameof(SpecialInfo.Url)},
//            {nameof(SpecialInfo.AddDate)})
//     VALUES
//           (@{nameof(SpecialInfo.SiteId)}, 
//            @{nameof(SpecialInfo.Title)}, 
//            @{nameof(SpecialInfo.Url)}, 
//            @{nameof(SpecialInfo.AddDate)})";

//            IDataParameter[] parameters = 
//            {
//                GetParameter(nameof(specialInfo.SiteId), specialInfo.SiteId),
//                GetParameter(nameof(specialInfo.Title), specialInfo.Title),
//                GetParameter(nameof(specialInfo.Url), specialInfo.Url),
//                GetParameter(nameof(specialInfo.AddDate),specialInfo.AddDate)
//            };

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

//            SpecialManager.RemoveCache(specialInfo.SiteId);
//        }

//        public void UpdateObject(SpecialInfo specialInfo)
//        {
//            var sqlString = $@"UPDATE {TableName} SET
//                {nameof(SpecialInfo.SiteId)} = @{nameof(SpecialInfo.SiteId)},  
//                {nameof(SpecialInfo.Title)} = @{nameof(SpecialInfo.Title)}, 
//                {nameof(SpecialInfo.Url)} = @{nameof(SpecialInfo.Url)},
//                {nameof(SpecialInfo.AddDate)} = @{nameof(SpecialInfo.AddDate)}
//            WHERE {nameof(SpecialInfo.Id)} = @{nameof(SpecialInfo.Id)}";

//            IDataParameter[] parameters =
//            {
//                GetParameter(nameof(specialInfo.SiteId), specialInfo.SiteId),
//                GetParameter(nameof(specialInfo.Title), specialInfo.Title),
//                GetParameter(nameof(specialInfo.Url), specialInfo.Url),
//                GetParameter(nameof(specialInfo.AddDate),specialInfo.AddDate),
//                GetParameter(nameof(specialInfo.Id), specialInfo.Id)
//            };

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

//            SpecialManager.RemoveCache(specialInfo.SiteId);
//        }

//        public void DeleteById(int siteId, int specialId)
//        {
//            if (specialId <= 0) return;

//            var sqlString = $"DELETE FROM {TableName} WHERE {nameof(SpecialInfo.Id)} = {specialId}";
//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

//            SpecialManager.RemoveCache(siteId);
//        }

//        public bool IsTitleExists(int siteId, string title)
//        {
//            var exists = false;

//            var sqlString = $@"SELECT {nameof(SpecialInfo.Id)} FROM {TableName} WHERE 
//    {nameof(SpecialInfo.SiteId)} = @{nameof(SpecialInfo.SiteId)} AND {nameof(SpecialInfo.Title)} = @{nameof(SpecialInfo.Title)}";

//            IDataParameter[] parameters =
//            {
//                GetParameter(nameof(SpecialInfo.SiteId), siteId),
//                GetParameter(nameof(SpecialInfo.Title), title)
//            };

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                if (rdr.Read() && !rdr.IsDBNull(0))
//                {
//                    exists = true;
//                }
//                rdr.Close();
//            }

//            return exists;
//        }

//        public bool IsUrlExists(int siteId, string url)
//        {
//            var exists = false;

//            var sqlString = $@"SELECT {nameof(SpecialInfo.Id)} FROM {TableName} WHERE 
//    {nameof(SpecialInfo.SiteId)} = @{nameof(SpecialInfo.SiteId)} AND {nameof(SpecialInfo.Url)} = @{nameof(SpecialInfo.Url)}";

//            IDataParameter[] parameters =
//            {
//                GetParameter(nameof(SpecialInfo.SiteId), siteId),
//                GetParameter(nameof(SpecialInfo.Url), url)
//            };

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                if (rdr.Read() && !rdr.IsDBNull(0))
//                {
//                    exists = true;
//                }
//                rdr.Close();
//            }

//            return exists;
//        }

//        public List<SpecialInfo> GetSpecialInfoList(int siteId)
//        {
//            var list = new List<SpecialInfo>();

//            var sqlString = $@"SELECT {nameof(SpecialInfo.Id)}, 
//                {nameof(SpecialInfo.SiteId)},
//                {nameof(SpecialInfo.Title)}, 
//                {nameof(SpecialInfo.Url)}, 
//                {nameof(SpecialInfo.AddDate)}
//            FROM {TableName} WHERE {nameof(SpecialInfo.SiteId)} = {siteId} ORDER BY {nameof(SpecialInfo.Id)} DESC";

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                while (rdr.Read())
//                {
//                    list.Add(GetSpecialInfo(rdr));
//                }
//                rdr.Close();
//            }

//            return list;
//        }

//        public List<SpecialInfo> GetSpecialInfoList(int siteId, string keyword)
//        {
//            var list = new List<SpecialInfo>();

//            keyword = AttackUtils.FilterSql(keyword);

//            var sqlString = $@"SELECT {nameof(SpecialInfo.Id)}, 
//                {nameof(SpecialInfo.SiteId)},
//                {nameof(SpecialInfo.Title)}, 
//                {nameof(SpecialInfo.Url)}, 
//                {nameof(SpecialInfo.AddDate)}
//            FROM {TableName} WHERE {nameof(SpecialInfo.SiteId)} = {siteId} AND ({nameof(SpecialInfo.Title)} LIKE '%{keyword}%' OR {nameof(SpecialInfo.Url)} LIKE '%{keyword}%')  ORDER BY {nameof(SpecialInfo.Id)} DESC";

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                while (rdr.Read())
//                {
//                    list.Add(GetSpecialInfo(rdr));
//                }
//                rdr.Close();
//            }

//            return list;
//        }

//        public Dictionary<int, SpecialInfo> GetSpecialInfoDictionaryBySiteId(int siteId)
//        {
//            var dictionary = new Dictionary<int, SpecialInfo>();

//            var sqlString = $@"SELECT {nameof(SpecialInfo.Id)}, 
//            {nameof(SpecialInfo.SiteId)},
//            {nameof(SpecialInfo.Title)},
//            {nameof(SpecialInfo.Url)},
//            {nameof(SpecialInfo.AddDate)}
//            FROM {TableName} WHERE {nameof(SpecialInfo.SiteId)} = {siteId}";

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                while (rdr.Read())
//                {
//                    var specialInfo = GetSpecialInfo(rdr);
//                    dictionary.Add(specialInfo.Id, specialInfo);
//                }
//                rdr.Close();
//            }

//            return dictionary;
//        }

//        private static SpecialInfo GetSpecialInfo(IDataRecord rdr)
//        {
//            if (rdr == null) return null;

//            var specialInfo = new SpecialInfo();

//            var i = 0;
//            specialInfo.Id = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
//            i++;
//            specialInfo.SiteId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
//            i++;
//            specialInfo.Title = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
//            i++;
//            specialInfo.Url = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
//            i++;
//            specialInfo.AddDate = rdr.IsDBNull(i) ? DateTime.Now : rdr.GetDateTime(i);

//            return specialInfo;
//        }

//    }
//}
