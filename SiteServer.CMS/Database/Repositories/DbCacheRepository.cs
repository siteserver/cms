using System;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;

namespace SiteServer.CMS.Database.Repositories
{
    public class DbCacheRepository : GenericRepository<DbCacheInfo>
    {
        private static class Attr
        {
            public const string CacheKey = nameof(DbCacheInfo.CacheKey);
            public const string CacheValue = nameof(DbCacheInfo.CacheValue);
            public const string AddDate = nameof(DbCacheInfo.AddDate);
        }

        public void RemoveAndInsert(string cacheKey, string cacheValue)
        {
            if (string.IsNullOrEmpty(cacheKey)) return;

            DeleteExcess90Days();

            DeleteAll(Q
                .Where(Attr.CacheKey, cacheKey));

            InsertObject(new DbCacheInfo
            {
                CacheKey = cacheKey,
                CacheValue = cacheValue,
                AddDate = DateTime.Now
            });

            //using (var conn = GetConnection())
            //{
            //    conn.Open();
            //    using (var trans = conn.BeginTransaction())
            //    {
            //        try
            //        {
            //            IDataParameter[] parameters =
            //            {
            //                GetParameter(ParamCacheKey, cacheKey)
            //            };

            //            DatabaseApi.ExecuteNonQuery(trans, SqlDelete, parameters);

            //            parameters = new[]
            //            {
            //                GetParameter(ParamCacheKey, cacheKey),
            //                GetParameter(ParamCacheValue, cacheValue),
            //                GetParameter(ParamAddDate,DateTime.Now)
            //            };

            //            DatabaseApi.ExecuteNonQuery(trans, SqlInsert, parameters);

            //            trans.Commit();
            //        }
            //        catch
            //        {
            //            trans.Rollback();
            //            throw;
            //        }
            //    }
            //}
        }

        public void Clear()
        {
            DeleteAll();
            //DatabaseApi.ExecuteNonQuery(ConnectionString, SqlDeleteAll);
        }

        public bool IsExists(string cacheKey)
        {
            return Exists(Q.Where(Attr.CacheKey, cacheKey));
            //var retVal = false;

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamCacheKey, cacheKey)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectValue, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        retVal = true;
            //    }
            //    rdr.Close();
            //}
            //return retVal;
        }

        public string GetValue(string cacheKey)
        {
            //var retVal = string.Empty;

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamCacheKey, cacheKey)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectValue, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        retVal = DatabaseApi.GetString(rdr, 0);
            //    }
            //    rdr.Close();
            //}
            //return retVal;
            return base.GetValue<string>(Q
                .Select(Attr.CacheValue)
                .Where(Attr.CacheKey, cacheKey));
        }

        public string GetValueAndRemove(string cacheKey)
        {
            var retVal = base.GetValue<string>(Q
                .Select(Attr.CacheValue)
                .Where(Attr.CacheKey, cacheKey));

            DeleteAll(Q
                .Where(Attr.CacheKey, cacheKey));

            return retVal;
        }

        public int GetCount()
        {
            //var count = 0;
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectCount))
            //{
            //    if (rdr.Read())
            //    {
            //        count = DatabaseApi.GetInt(rdr, 0);
            //    }
            //    rdr.Close();
            //}
            //return count;

            return Count();
        }

        public void DeleteExcess90Days()
        {
            //DatabaseApi.ExecuteNonQuery(ConnectionString, "DELETE FROM siteserver_DbCache WHERE " + SqlUtils.GetDateDiffGreatThanDays("AddDate", 90.ToString()));

            DeleteAll(Q
                .Where(Attr.AddDate, "<", DateTime.Now.AddDays(-90)));
        }
    }
}


//using System;
//using System.Collections.Generic;
//using System.Data;
//using SiteServer.CMS.Database.Core;
//using SiteServer.CMS.Database.Models;
//using SiteServer.Plugin;
//using SiteServer.Utils;

//namespace SiteServer.CMS.Database.Repositories
//{
//    public class DbCache : DataProviderBase
//    {
//        public override string TableName => "siteserver_DbCache";

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(DbCacheInfo.Id),
//                DataType = DataType.Integer,
//                IsIdentity = true,
//                IsPrimaryKey = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(DbCacheInfo.CacheKey),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(DbCacheInfo.CacheValue),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(DbCacheInfo.AddDate),
//                DataType = DataType.DateTime
//            }
//        };

//        private const string SqlSelectValue = "SELECT CacheValue FROM siteserver_DbCache WHERE CacheKey = @CacheKey";

//        private const string SqlSelectCount = "SELECT COUNT(*) FROM siteserver_DbCache";

//        private const string SqlInsert = "INSERT INTO siteserver_DbCache (CacheKey, CacheValue, AddDate) VALUES (@CacheKey, @CacheValue, @AddDate)";

//        private const string SqlDelete = "DELETE FROM siteserver_DbCache WHERE CacheKey = @CacheKey";

//        private const string SqlDeleteAll = "DELETE FROM siteserver_DbCache";

//        private const string ParamCacheKey = "@CacheKey";
//        private const string ParamCacheValue = "@CacheValue";
//        private const string ParamAddDate = "@AddDate";

//        public void RemoveAndInsert(string cacheKey, string cacheValue)
//        {
//            if (string.IsNullOrEmpty(cacheKey)) return;

//            DeleteExcess90Days();

//            using (var conn = GetConnection())
//            {
//                conn.Open();
//                using (var trans = conn.BeginTransaction())
//                {
//                    try
//                    {
//                        IDataParameter[] parameters =
//                        {
//                            GetParameter(ParamCacheKey, cacheKey)
//                        };

//                        DatabaseApi.ExecuteNonQuery(trans, SqlDelete, parameters);

//                        parameters = new []
//                        {
//                            GetParameter(ParamCacheKey, cacheKey),
//                            GetParameter(ParamCacheValue, cacheValue),
//                            GetParameter(ParamAddDate,DateTime.Now)
//                        };

//                        DatabaseApi.ExecuteNonQuery(trans, SqlInsert, parameters);

//                        trans.Commit();
//                    }
//                    catch
//                    {
//                        trans.Rollback();
//                        throw;
//                    }
//                }
//            }
//        }

//        public void Clear()
//        {
//            DatabaseApi.ExecuteNonQuery(ConnectionString, SqlDeleteAll);
//        }

//        public bool IsExists(string cacheKey)
//        {
//            var retVal = false;

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamCacheKey, cacheKey)
//			};

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectValue, parameters))
//            {
//                if (rdr.Read())
//                {
//                    retVal = true;
//                }
//                rdr.Close();
//            }
//            return retVal;
//        }

//        public string GetValueById(string cacheKey)
//        {
//            var retVal = string.Empty;

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamCacheKey, cacheKey)
//			};

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectValue, parameters))
//            {
//                if (rdr.Read())
//                {
//                    retVal = DatabaseApi.GetString(rdr, 0);
//                }
//                rdr.Close();
//            }
//            return retVal;
//        }

//        public string GetValueAndRemove(string cacheKey)
//        {
//            var retVal = string.Empty;

//            using (var conn = GetConnection())
//            {
//                conn.Open();
//                using (var trans = conn.BeginTransaction())
//                {
//                    try
//                    {
//                        IDataParameter[] parameters =
//                        {
//                            GetParameter(ParamCacheKey, cacheKey)
//                        };

//                        using (var rdr = DatabaseApi.ExecuteReader(trans, SqlSelectValue, parameters))
//                        {
//                            if (rdr.Read())
//                            {
//                                retVal = DatabaseApi.GetString(rdr, 0);
//                            }
//                            rdr.Close();
//                        }

//                        parameters = new[]
//                        {
//                            GetParameter(ParamCacheKey, cacheKey)
//                        };

//                        DatabaseApi.ExecuteNonQuery(trans, SqlDelete, parameters);

//                        trans.Commit();
//                    }
//                    catch
//                    {
//                        trans.Rollback();
//                        throw;
//                    }
//                }
//            }

//            return retVal;
//        }

//        public int GetCount()
//        {
//            var count = 0;
//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectCount))
//            {
//                if (rdr.Read())
//                {
//                    count = DatabaseApi.GetInt(rdr, 0);
//                }
//                rdr.Close();
//            }
//            return count;
//        }

//        public void DeleteExcess90Days()
//        {
//            DatabaseApi.ExecuteNonQuery(ConnectionString, "DELETE FROM siteserver_DbCache WHERE " + SqlUtils.GetDateDiffGreatThanDays("AddDate", 90.ToString()));
//        }
//    }
//}
