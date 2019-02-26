using System;
using System.Collections.Generic;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;

namespace SiteServer.CMS.Database.Repositories
{
    public class RelatedFieldRepository : GenericRepository<RelatedFieldInfo>
    {
        private static class Attr
        {
            public const string Id = nameof(RelatedFieldInfo.Id);
            public const string Title = nameof(RelatedFieldInfo.Title);
            public const string SiteId = nameof(RelatedFieldInfo.SiteId);
        }

        public int Insert(RelatedFieldInfo relatedFieldInfo)
        {
            //const string sqlString = "INSERT INTO siteserver_RelatedField (Title, SiteId, TotalLevel, Prefixes, Suffixes) VALUES (@Title, @SiteId, @TotalLevel, @Prefixes, @Suffixes)";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamTitle, relatedFieldInfo.Title),
            //    GetParameter(ParamSiteId, relatedFieldInfo.SiteId),
            //    GetParameter(ParamTotalLevel, relatedFieldInfo.TotalLevel),
            //    GetParameter(ParamPrefixes, relatedFieldInfo.Prefixes),
            //    GetParameter(ParamSuffixes, relatedFieldInfo.Suffixes),
            //};

            //return DatabaseApi.ExecuteNonQueryAndReturnId(ConnectionString, TableName, nameof(RelatedFieldInfo.Id), sqlString, parameters);

            return InsertObject(relatedFieldInfo);
        }

        public void Update(RelatedFieldInfo relatedFieldInfo)
        {
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamTitle, relatedFieldInfo.Title),
            //    GetParameter(ParamTotalLevel, relatedFieldInfo.TotalLevel),
            //    GetParameter(ParamPrefixes, relatedFieldInfo.Prefixes),
            //    GetParameter(ParamSuffixes, relatedFieldInfo.Suffixes),
            //    GetParameter(ParamId, relatedFieldInfo.Id)
            //};
            //string SqlUpdate = "UPDATE siteserver_RelatedField SET Title = @Title, TotalLevel = @TotalLevel, Prefixes = @Prefixes, Suffixes = @Suffixes WHERE Id = @Id";
            //DatabaseApi.ExecuteNonQuery(ConnectionString, SqlUpdate, parameters);

            UpdateObject(relatedFieldInfo);
        }
        
        public void Delete(int id)
        {
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamId, id)
            //};
            //string SqlDelete = "DELETE FROM siteserver_RelatedField WHERE Id = @Id";
            //DatabaseApi.ExecuteNonQuery(ConnectionString, SqlDelete, parameters);

            DeleteById(id);
        }

        public RelatedFieldInfo Get(int id)
        {
            return id <= 0 ? null : GetObjectById(id);

            //RelatedFieldInfo relatedFieldInfo = null;

            //var sqlString =
            //    $"SELECT Id, Title, SiteId, TotalLevel, Prefixes, Suffixes FROM siteserver_RelatedField WHERE Id = {id}";

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    if (rdr.Read())
            //    {
            //        var i = 0;
            //        relatedFieldInfo = new RelatedFieldInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i));
            //    }
            //    rdr.Close();
            //}

            //return relatedFieldInfo;
        }

        public RelatedFieldInfo GetRelatedFieldInfo(int siteId, string title)
        {
            //RelatedFieldInfo relatedFieldInfo = null;

            //var sqlString =
            //    $"SELECT Id, Title, SiteId, TotalLevel, Prefixes, Suffixes FROM siteserver_RelatedField WHERE SiteId = {siteId} AND Title = @Title";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamTitle, relatedFieldName)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        var i = 0;
            //        relatedFieldInfo = new RelatedFieldInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i));
            //    }
            //    rdr.Close();
            //}

            //return relatedFieldInfo;
            return GetObject(Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.Title, title));
        }

        public string GetTitle(int id)
        {
            //var relatedFieldName = string.Empty;

            //var sqlString =
            //    $"SELECT Title FROM siteserver_RelatedField WHERE Id = {id}";

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    if (rdr.Read())
            //    {
            //        relatedFieldName = DatabaseApi.GetString(rdr, 0);
            //    }
            //    rdr.Close();
            //}

            //return relatedFieldName;

            return GetValue<string>(Q
                .Select(Attr.Title)
                .Where(Attr.Id, id));
        }

        public IList<RelatedFieldInfo> GetRelatedFieldInfoList(int siteId)
        {
            //var list = new List<RelatedFieldInfo>();
            //var sqlString =
            //    $"SELECT Id, Title, SiteId, TotalLevel, Prefixes, Suffixes FROM siteserver_RelatedField WHERE SiteId = {siteId} ORDER BY Id";

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    while (rdr.Read())
            //    {
            //        var i = 0;
            //        list.Add(new RelatedFieldInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i)));
            //    }
            //    rdr.Close();
            //}

            //return list;

            return GetObjectList(Q
                .Where(Attr.SiteId, siteId)
                .OrderBy(Attr.Id));
        }

        public IList<string> GetTitleList(int siteId)
        {
            //var list = new List<string>();
            //var sqlString =
            //    $"SELECT Title FROM siteserver_RelatedField WHERE SiteId = {siteId} ORDER BY Id";

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    while (rdr.Read())
            //    {
            //        list.Add(DatabaseApi.GetString(rdr, 0));
            //    }
            //    rdr.Close();
            //}

            //return list;

            return GetValueList<string>(Q
                .Select(Attr.Title)
                .Where(Attr.SiteId, siteId)
                .OrderBy(Attr.Id));
        }

        public string GetImportTitle(int siteId, string relatedFieldName)
        {
            string importName;
            if (relatedFieldName.IndexOf("_", StringComparison.Ordinal) != -1)
            {
                var relatedFieldNameCount = 0;
                var lastName = relatedFieldName.Substring(relatedFieldName.LastIndexOf("_", StringComparison.Ordinal) + 1);
                var firstName = relatedFieldName.Substring(0, relatedFieldName.Length - lastName.Length);
                try
                {
                    relatedFieldNameCount = int.Parse(lastName);
                }
                catch
                {
                    // ignored
                }
                relatedFieldNameCount++;
                importName = firstName + relatedFieldNameCount;
            }
            else
            {
                importName = relatedFieldName + "_1";
            }

            var relatedFieldInfo = GetRelatedFieldInfo(siteId, relatedFieldName);
            if (relatedFieldInfo != null)
            {
                importName = GetImportTitle(siteId, importName);
            }

            return importName;
        }
    }
}

//using System;
 //using System.Collections.Generic;
 //using System.Data;
 //using SiteServer.CMS.Database.Core;
 //using SiteServer.CMS.Database.Models;
 //using SiteServer.Plugin;

//namespace SiteServer.CMS.Database.Repositories
//{
//    public class RelatedField : DataProviderBase
//	{
//        public override string TableName => "siteserver_RelatedField";

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(RelatedFieldInfo.Id),
//                DataType = DataType.Integer,
//                IsIdentity = true,
//                IsPrimaryKey = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(RelatedFieldInfo.Title),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(RelatedFieldInfo.SiteId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(RelatedFieldInfo.TotalLevel),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(RelatedFieldInfo.Prefixes),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(RelatedFieldInfo.Suffixes),
//                DataType = DataType.VarChar
//            }
//        };

//        private const string SqlUpdate = "UPDATE siteserver_RelatedField SET Title = @Title, TotalLevel = @TotalLevel, Prefixes = @Prefixes, Suffixes = @Suffixes WHERE Id = @Id";
//        private const string SqlDelete = "DELETE FROM siteserver_RelatedField WHERE Id = @Id";

//        private const string ParamId = "@Id";
//        private const string ParamTitle = "@Title";
//		private const string ParamSiteId = "@SiteId";
//        private const string ParamTotalLevel = "@TotalLevel";
//        private const string ParamPrefixes = "@Prefixes";
//        private const string ParamSuffixes = "@Suffixes";

//		public int InsertObject(RelatedFieldInfo relatedFieldInfo) 
//		{
//            const string sqlString = "INSERT INTO siteserver_RelatedField (Title, SiteId, TotalLevel, Prefixes, Suffixes) VALUES (@Title, @SiteId, @TotalLevel, @Prefixes, @Suffixes)";

//		    IDataParameter[] parameters =
//			{
//				GetParameter(ParamTitle, relatedFieldInfo.Title),
//				GetParameter(ParamSiteId, relatedFieldInfo.SiteId),
//                GetParameter(ParamTotalLevel, relatedFieldInfo.TotalLevel),
//                GetParameter(ParamPrefixes, relatedFieldInfo.Prefixes),
//                GetParameter(ParamSuffixes, relatedFieldInfo.Suffixes),
//			};

//            return DatabaseApi.ExecuteNonQueryAndReturnId(ConnectionString, TableName, nameof(RelatedFieldInfo.Id), sqlString, parameters);
//		}

//        public void UpdateObject(RelatedFieldInfo relatedFieldInfo) 
//		{
//		    IDataParameter[] parameters =
//			{
//				GetParameter(ParamTitle, relatedFieldInfo.Title),
//                GetParameter(ParamTotalLevel, relatedFieldInfo.TotalLevel),
//                GetParameter(ParamPrefixes, relatedFieldInfo.Prefixes),
//                GetParameter(ParamSuffixes, relatedFieldInfo.Suffixes),
//				GetParameter(ParamId, relatedFieldInfo.Id)
//			};

//		    DatabaseApi.ExecuteNonQuery(ConnectionString, SqlUpdate, parameters);
//		}

//		public void DeleteById(int id)
//		{
//		    IDataParameter[] parameters =
//			{
//				GetParameter(ParamId, id)
//			};

//		    DatabaseApi.ExecuteNonQuery(ConnectionString, SqlDelete, parameters);
//		}

//        public RelatedFieldInfo GetRelatedFieldInfo(int id)
//		{
//            if (id <= 0) return null;

//            RelatedFieldInfo relatedFieldInfo = null;

//		    var sqlString =
//		        $"SELECT Id, Title, SiteId, TotalLevel, Prefixes, Suffixes FROM siteserver_RelatedField WHERE Id = {id}";

//		    using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//		    {
//		        if (rdr.Read())
//		        {
//		            var i = 0;
//		            relatedFieldInfo = new RelatedFieldInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i));
//		        }
//		        rdr.Close();
//		    }

//		    return relatedFieldInfo;
//		}

//        public RelatedFieldInfo GetRelatedFieldInfo(int siteId, string relatedFieldName)
//        {
//            RelatedFieldInfo relatedFieldInfo = null;

//            var sqlString =
//                $"SELECT Id, Title, SiteId, TotalLevel, Prefixes, Suffixes FROM siteserver_RelatedField WHERE SiteId = {siteId} AND Title = @Title";

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamTitle, relatedFieldName)			 
//			};

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                if (rdr.Read())
//                {
//                    var i = 0;
//                    relatedFieldInfo = new RelatedFieldInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i));
//                }
//                rdr.Close();
//            }

//            return relatedFieldInfo;
//        }

//        public string GetTitle(int id)
//        {
//            var relatedFieldName = string.Empty;

//            var sqlString =
//                $"SELECT Title FROM siteserver_RelatedField WHERE Id = {id}";

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                if (rdr.Read())
//                {
//                    relatedFieldName = DatabaseApi.GetString(rdr, 0);
//                }
//                rdr.Close();
//            }

//            return relatedFieldName;
//        }

//		public List<RelatedFieldInfo> GetRelatedFieldInfoList(int siteId)
//		{
//			var list = new List<RelatedFieldInfo>();
//            var sqlString =
//                $"SELECT Id, Title, SiteId, TotalLevel, Prefixes, Suffixes FROM siteserver_RelatedField WHERE SiteId = {siteId} ORDER BY Id";

//			using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString)) 
//			{
//				while (rdr.Read())
//				{
//				    var i = 0;
//                    list.Add(new RelatedFieldInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i)));
//				}
//				rdr.Close();
//			}

//			return list;
//		}

//		public List<string> GetTitleList(int siteId)
//		{
//			var list = new List<string>();
//            var sqlString =
//                $"SELECT Title FROM siteserver_RelatedField WHERE SiteId = {siteId} ORDER BY Id";

//			using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString)) 
//			{
//				while (rdr.Read()) 
//				{
//                    list.Add(DatabaseApi.GetString(rdr, 0));
//				}
//				rdr.Close();
//			}

//			return list;
//		}

//        public string GetImportTitle(int siteId, string relatedFieldName)
//        {
//            string importName;
//            if (relatedFieldName.IndexOf("_", StringComparison.Ordinal) != -1)
//            {
//                var relatedFieldNameCount = 0;
//                var lastName = relatedFieldName.Substring(relatedFieldName.LastIndexOf("_", StringComparison.Ordinal) + 1);
//                var firstName = relatedFieldName.Substring(0, relatedFieldName.Length - lastName.Length);
//                try
//                {
//                    relatedFieldNameCount = int.Parse(lastName);
//                }
//                catch
//                {
//                    // ignored
//                }
//                relatedFieldNameCount++;
//                importName = firstName + relatedFieldNameCount;
//            }
//            else
//            {
//                importName = relatedFieldName + "_1";
//            }

//            var relatedFieldInfo = GetRelatedFieldInfo(siteId, relatedFieldName);
//            if (relatedFieldInfo != null)
//            {
//                importName = GetImportTitle(siteId, importName);
//            }

//            return importName;
//        }
//	}
//}