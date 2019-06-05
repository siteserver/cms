using System.Collections.Generic;
using System.Linq;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Data;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public class SpecialDao : IDatabaseDao
    {
        private readonly Repository<SpecialInfo> _repository;
        public SpecialDao()
        {
            _repository = new Repository<SpecialInfo>(AppSettings.DatabaseType, AppSettings.ConnectionString);
        }

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(SpecialInfo.Id);
            public const string SiteId = nameof(SpecialInfo.SiteId);
            public const string Title = nameof(SpecialInfo.Title);
            public const string Url = nameof(SpecialInfo.Url);
        }

        public int Insert(SpecialInfo specialInfo)
        {
            specialInfo.Id = _repository.Insert(specialInfo);

            SpecialManager.RemoveCache(specialInfo.SiteId);

            return specialInfo.Id;
        }

        public bool Update(SpecialInfo specialInfo)
        {
            var updated = _repository.Update(specialInfo);

            SpecialManager.RemoveCache(specialInfo.SiteId);

            return updated;
        }

        public void Delete(int siteId, int specialId)
        {
            if (specialId <= 0) return;

            _repository.Delete(specialId);

            SpecialManager.RemoveCache(siteId);
        }

        public bool IsTitleExists(int siteId, string title)
        {
            return _repository.Exists(Q.Where(Attr.SiteId, siteId).Where(Attr.Title, title));
        }

        public bool IsUrlExists(int siteId, string url)
        {
            return _repository.Exists(Q.Where(Attr.SiteId, siteId).Where(Attr.Url, url));
        }

        public IList<SpecialInfo> GetSpecialInfoList(int siteId)
        {
            return _repository.GetAll(Q.Where(Attr.SiteId, siteId).OrderByDesc(Attr.Id));
        }

        public IList<SpecialInfo> GetSpecialInfoList(int siteId, string keyword)
        {
            return _repository.GetAll(Q
                .Where(Attr.SiteId, siteId)
                .OrWhereContains(Attr.Title, keyword)
                .OrWhereContains(Attr.Url, keyword)
                .OrderByDesc(Attr.Id));
        }

        public Dictionary<int, SpecialInfo> GetSpecialInfoDictionaryBySiteId(int siteId)
        {
            var specialInfoList = GetSpecialInfoList(siteId);

            return specialInfoList.ToDictionary(specialInfo => specialInfo.Id);
        }
    }
}


// using System;
// using System.Collections.Generic;
// using System.Data;
// using Datory;
// using SiteServer.CMS.Core;
// using SiteServer.CMS.DataCache;
// using SiteServer.CMS.Model;
// using SiteServer.Utils;

// namespace SiteServer.CMS.Provider
// {
//     public class SpecialDao
//     {
//         public override string TableName => "siteserver_Special";

//         public override List<TableColumn> TableColumns => new List<TableColumn>
//         {
//             new TableColumn
//             {
//                 AttributeName = nameof(SpecialInfo.Id),
//                 DataType = DataType.Integer,
//                 IsIdentity = true,
//                 IsPrimaryKey = true
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(SpecialInfo.SiteId),
//                 DataType = DataType.Integer
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(SpecialInfo.Title),
//                 DataType = DataType.VarChar,
//                 DataLength = 200
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(SpecialInfo.Url),
//                 DataType = DataType.VarChar,
//                 DataLength = 200
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(SpecialInfo.AddDate),
//                 DataType = DataType.DateTime
//             }
//         };

//         public void Insert(SpecialInfo specialInfo)
//         {
//             var sqlString = $@"INSERT INTO {TableName}
//            ({nameof(SpecialInfo.SiteId)}, 
//             {nameof(SpecialInfo.Title)}, 
//             {nameof(SpecialInfo.Url)},
//             {nameof(SpecialInfo.AddDate)})
//      VALUES
//            (@{nameof(SpecialInfo.SiteId)}, 
//             @{nameof(SpecialInfo.Title)}, 
//             @{nameof(SpecialInfo.Url)}, 
//             @{nameof(SpecialInfo.AddDate)})";

//             IDataParameter[] parameters = 
//             {
//                 GetParameter(nameof(specialInfo.SiteId), DataType.Integer, specialInfo.SiteId),
//                 GetParameter(nameof(specialInfo.Title), DataType.VarChar, 200, specialInfo.Title),
//                 GetParameter(nameof(specialInfo.Url), DataType.VarChar, 200, specialInfo.Url),
//                 GetParameter(nameof(specialInfo.AddDate), DataType.DateTime, specialInfo.AddDate)
//             };

//             ExecuteNonQuery(sqlString, parameters);

//             SpecialManager.RemoveCache(specialInfo.SiteId);
//         }

//         public void Update(SpecialInfo specialInfo)
//         {
//             var sqlString = $@"UPDATE {TableName} SET
//                 {nameof(SpecialInfo.SiteId)} = @{nameof(SpecialInfo.SiteId)},  
//                 {nameof(SpecialInfo.Title)} = @{nameof(SpecialInfo.Title)}, 
//                 {nameof(SpecialInfo.Url)} = @{nameof(SpecialInfo.Url)},
//                 {nameof(SpecialInfo.AddDate)} = @{nameof(SpecialInfo.AddDate)}
//             WHERE {nameof(SpecialInfo.Id)} = @{nameof(SpecialInfo.Id)}";

//             IDataParameter[] parameters =
//             {
//                 GetParameter(nameof(specialInfo.SiteId), DataType.Integer, specialInfo.SiteId),
//                 GetParameter(nameof(specialInfo.Title), DataType.VarChar, 200, specialInfo.Title),
//                 GetParameter(nameof(specialInfo.Url), DataType.VarChar, 200, specialInfo.Url),
//                 GetParameter(nameof(specialInfo.AddDate), DataType.DateTime, specialInfo.AddDate),
//                 GetParameter(nameof(specialInfo.Id), DataType.Integer, specialInfo.Id)
//             };

//             ExecuteNonQuery(sqlString, parameters);

//             SpecialManager.RemoveCache(specialInfo.SiteId);
//         }

//         public void Delete(int siteId, int specialId)
//         {
//             if (specialId <= 0) return;

//             var sqlString = $"DELETE FROM {TableName} WHERE {nameof(SpecialInfo.Id)} = {specialId}";
//             ExecuteNonQuery(sqlString);

//             SpecialManager.RemoveCache(siteId);
//         }

//         public bool IsTitleExists(int siteId, string title)
//         {
//             var exists = false;

//             var sqlString = $@"SELECT {nameof(SpecialInfo.Id)} FROM {TableName} WHERE 
//     {nameof(SpecialInfo.SiteId)} = @{nameof(SpecialInfo.SiteId)} AND {nameof(SpecialInfo.Title)} = @{nameof(SpecialInfo.Title)}";

//             IDataParameter[] parameters =
//             {
//                 GetParameter(nameof(SpecialInfo.SiteId), DataType.Integer, siteId),
//                 GetParameter(nameof(SpecialInfo.Title), DataType.VarChar, 200, title)
//             };

//             using (var rdr = ExecuteReader(sqlString, parameters))
//             {
//                 if (rdr.Read() && !rdr.IsDBNull(0))
//                 {
//                     exists = true;
//                 }
//                 rdr.Close();
//             }

//             return exists;
//         }

//         public bool IsUrlExists(int siteId, string url)
//         {
//             var exists = false;

//             var sqlString = $@"SELECT {nameof(SpecialInfo.Id)} FROM {TableName} WHERE 
//     {nameof(SpecialInfo.SiteId)} = @{nameof(SpecialInfo.SiteId)} AND {nameof(SpecialInfo.Url)} = @{nameof(SpecialInfo.Url)}";

//             IDataParameter[] parameters =
//             {
//                 GetParameter(nameof(SpecialInfo.SiteId), DataType.Integer, siteId),
//                 GetParameter(nameof(SpecialInfo.Url), DataType.VarChar, 200, url)
//             };

//             using (var rdr = ExecuteReader(sqlString, parameters))
//             {
//                 if (rdr.Read() && !rdr.IsDBNull(0))
//                 {
//                     exists = true;
//                 }
//                 rdr.Close();
//             }

//             return exists;
//         }

//         public List<SpecialInfo> GetSpecialInfoList(int siteId)
//         {
//             var list = new List<SpecialInfo>();

//             var sqlString = $@"SELECT {nameof(SpecialInfo.Id)}, 
//                 {nameof(SpecialInfo.SiteId)},
//                 {nameof(SpecialInfo.Title)}, 
//                 {nameof(SpecialInfo.Url)}, 
//                 {nameof(SpecialInfo.AddDate)}
//             FROM {TableName} WHERE {nameof(SpecialInfo.SiteId)} = {siteId} ORDER BY {nameof(SpecialInfo.Id)} DESC";

//             using (var rdr = ExecuteReader(sqlString))
//             {
//                 while (rdr.Read())
//                 {
//                     list.Add(GetSpecialInfo(rdr));
//                 }
//                 rdr.Close();
//             }

//             return list;
//         }

//         public List<SpecialInfo> GetSpecialInfoList(int siteId, string keyword)
//         {
//             var list = new List<SpecialInfo>();

//             keyword = AttackUtils.FilterSql(keyword);

//             var sqlString = $@"SELECT {nameof(SpecialInfo.Id)}, 
//                 {nameof(SpecialInfo.SiteId)},
//                 {nameof(SpecialInfo.Title)}, 
//                 {nameof(SpecialInfo.Url)}, 
//                 {nameof(SpecialInfo.AddDate)}
//             FROM {TableName} WHERE {nameof(SpecialInfo.SiteId)} = {siteId} AND ({nameof(SpecialInfo.Title)} LIKE '%{keyword}%' OR {nameof(SpecialInfo.Url)} LIKE '%{keyword}%')  ORDER BY {nameof(SpecialInfo.Id)} DESC";

//             using (var rdr = ExecuteReader(sqlString))
//             {
//                 while (rdr.Read())
//                 {
//                     list.Add(GetSpecialInfo(rdr));
//                 }
//                 rdr.Close();
//             }

//             return list;
//         }

//         public Dictionary<int, SpecialInfo> GetSpecialInfoDictionaryBySiteId(int siteId)
//         {
//             var dictionary = new Dictionary<int, SpecialInfo>();

//             var sqlString = $@"SELECT {nameof(SpecialInfo.Id)}, 
//             {nameof(SpecialInfo.SiteId)},
//             {nameof(SpecialInfo.Title)},
//             {nameof(SpecialInfo.Url)},
//             {nameof(SpecialInfo.AddDate)}
//             FROM {TableName} WHERE {nameof(SpecialInfo.SiteId)} = {siteId}";

//             using (var rdr = ExecuteReader(sqlString))
//             {
//                 while (rdr.Read())
//                 {
//                     var specialInfo = GetSpecialInfo(rdr);
//                     dictionary.Add(specialInfo.Id, specialInfo);
//                 }
//                 rdr.Close();
//             }

//             return dictionary;
//         }

//         private static SpecialInfo GetSpecialInfo(IDataRecord rdr)
//         {
//             if (rdr == null) return null;

//             var specialInfo = new SpecialInfo();

//             var i = 0;
//             specialInfo.Id = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
//             i++;
//             specialInfo.SiteId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
//             i++;
//             specialInfo.Title = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
//             i++;
//             specialInfo.Url = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
//             i++;
//             specialInfo.AddDate = rdr.IsDBNull(i) ? DateTime.Now : rdr.GetDateTime(i);

//             return specialInfo;
//         }

//     }
// }
