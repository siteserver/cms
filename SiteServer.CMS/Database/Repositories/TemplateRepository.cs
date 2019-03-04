using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SiteServer.CMS.Apis;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Database.Repositories
{
    public class TemplateRepository : GenericRepository<TemplateInfo>
    {
        private static class Attr
        {
            public const string Id = nameof(TemplateInfo.Id);
            public const string SiteId = nameof(TemplateInfo.SiteId);
            public const string TemplateName = nameof(TemplateInfo.TemplateName);
            public const string TemplateType = "TemplateType";
            public const string RelatedFileName = nameof(TemplateInfo.RelatedFileName);
            public const string IsDefault = "IsDefault";
        }

        public int Insert(TemplateInfo templateInfo, string templateContent, string administratorName)
        {
            if (templateInfo.Default)
            {
                SetAllTemplateDefaultToFalse(templateInfo.SiteId, templateInfo.Type);
            }

            //const string sqlInsertTemplate = "INSERT INTO siteserver_Template (SiteId, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault) VALUES (@SiteId, @TemplateName, @TemplateType, @RelatedFileName, @CreatedFileFullName, @CreatedFileExtName, @Charset, @IsDefault)";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamSiteId, templateInfo.SiteId),
            //    GetParameter(ParamTemplateName, templateInfo.TemplateName),
            //    GetParameter(ParamTemplateType, templateInfo.TemplateType.Value),
            //    GetParameter(ParamRelatedFileName, templateInfo.RelatedFileName),
            //    GetParameter(ParamCreatedFileFullName, templateInfo.CreatedFileFullName),
            //    GetParameter(ParamCreatedFileExtName, templateInfo.CreatedFileExtName),
            //    GetParameter(ParamCharset, ECharsetUtils.GetValueById(templateInfo.Charset)),
            //    GetParameter(ParamIsDefault, templateInfo.IsDefault.ToString())
            //};

            //var id = DatabaseApi.ExecuteNonQueryAndReturnId(ConnectionString, TableName, nameof(TemplateInfo.Id), sqlInsertTemplate, parameters);

            var id = InsertObject(templateInfo);

            var siteInfo = SiteManager.GetSiteInfo(templateInfo.SiteId);
            TemplateManager.WriteContentToTemplateFile(siteInfo, templateInfo, templateContent, administratorName);

            TemplateManager.RemoveCache(templateInfo.SiteId);

            return id;
        }

        public void Update(SiteInfo siteInfo, TemplateInfo templateInfo, string templateContent, string administratorName)
        {
            if (templateInfo.Default)
            {
                SetAllTemplateDefaultToFalse(siteInfo.Id, templateInfo.Type);
            }

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamTemplateName, templateInfo.TemplateName),
            //    GetParameter(ParamTemplateType, templateInfo.TemplateType.Value),
            //    GetParameter(ParamRelatedFileName, templateInfo.RelatedFileName),
            //    GetParameter(ParamCreatedFileFullName, templateInfo.CreatedFileFullName),
            //    GetParameter(ParamCreatedFileExtName, templateInfo.CreatedFileExtName),
            //    GetParameter(ParamCharset, ECharsetUtils.GetValueById(templateInfo.Charset)),
            //    GetParameter(ParamIsDefault, templateInfo.IsDefault.ToString()),
            //    GetParameter(ParamId, templateInfo.Id)
            //};
            //string SqlUpdateTemplate = "UPDATE siteserver_Template SET TemplateName = @TemplateName, TemplateType = @TemplateType, RelatedFileName = @RelatedFileName, CreatedFileFullName = @CreatedFileFullName, CreatedFileExtName = @CreatedFileExtName, Charset = @Charset, IsDefault = @IsDefault WHERE  Id = @Id";
            //DatabaseApi.ExecuteNonQuery(ConnectionString, SqlUpdateTemplate, parameters);

            UpdateObject(templateInfo);

            TemplateManager.WriteContentToTemplateFile(siteInfo, templateInfo, templateContent, administratorName);

            TemplateManager.RemoveCache(templateInfo.SiteId);
        }

        private void SetAllTemplateDefaultToFalse(int siteId, TemplateType templateType)
        {
            //var sqlString = "UPDATE siteserver_Template SET IsDefault = @IsDefault WHERE SiteId = @SiteId AND TemplateType = @TemplateType";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamIsDefault, false.ToString()),
            //    GetParameter(ParamSiteId, siteId),
            //    GetParameter(ParamTemplateType, templateType.Value)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            UpdateAll(Q
                .Set(Attr.IsDefault, false.ToString())
                .Where(Attr.SiteId, siteId)
                .Where(Attr.TemplateType, templateType.Value)
            );
        }

        public void SetDefault(int siteId, int id)
        {
            var info = TemplateManager.GetTemplateInfo(siteId, id);
            SetAllTemplateDefaultToFalse(info.SiteId, info.Type);

            //const string sqlString = "UPDATE siteserver_Template SET IsDefault = @IsDefault WHERE Id = @Id";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamIsDefault, true.ToString()),
            //    GetParameter(ParamId, id)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            UpdateAll(Q
                .Set(Attr.IsDefault, true.ToString())
                .Where(Attr.Id, id)
            );

            TemplateManager.RemoveCache(siteId);
        }

        public void Delete(int siteId, int id)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var templateInfo = TemplateManager.GetTemplateInfo(siteId, id);
            var filePath = TemplateManager.GetTemplateFilePath(siteInfo, templateInfo);

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamId, id)
            //};
            //string SqlDeleteTemplate = "DELETE FROM siteserver_Template WHERE Id = @Id";
            //DatabaseApi.ExecuteNonQuery(ConnectionString, SqlDeleteTemplate, parameters);

            DeleteById(id);

            FileUtils.DeleteFileIfExists(filePath);

            TemplateManager.RemoveCache(siteId);
        }

        public string GetImportTemplateName(int siteId, string templateName)
        {
            string importTemplateName;
            if (templateName.IndexOf("_", StringComparison.Ordinal) != -1)
            {
                var templateNameCount = 0;
                var lastTemplateName = templateName.Substring(templateName.LastIndexOf("_", StringComparison.Ordinal) + 1);
                var firstTemplateName = templateName.Substring(0, templateName.Length - lastTemplateName.Length);
                try
                {
                    templateNameCount = int.Parse(lastTemplateName);
                }
                catch
                {
                    // ignored
                }
                templateNameCount++;
                importTemplateName = firstTemplateName + templateNameCount;
            }
            else
            {
                importTemplateName = templateName + "_1";
            }

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamSiteId, siteId),
            //    GetParameter(ParamTemplateName, importTemplateName)
            //};
            //string SqlSelectTemplateByTemplateName = "SELECT Id, SiteId, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault FROM siteserver_Template WHERE SiteId = @SiteId AND TemplateType = @TemplateType AND TemplateName = @TemplateName";
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectTemplateByTemplateName, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        importTemplateName = GetImportTemplateName(siteId, importTemplateName);
            //    }
            //    rdr.Close();
            //}

            var isExists = Exists(Q.Where(Attr.SiteId, siteId).Where(Attr.TemplateName, importTemplateName));
            if (isExists)
            {
                importTemplateName = GetImportTemplateName(siteId, importTemplateName);
            }

            return importTemplateName;
        }

        public Dictionary<TemplateType, int> GetCountDictionary(int siteId)
        {
            var dictionary = new Dictionary<TemplateType, int>();

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamSiteId, siteId)
            //};
            //string SqlSelectTemplateCount = "SELECT TemplateType, COUNT(*) FROM siteserver_Template WHERE SiteId = @SiteId GROUP BY TemplateType";
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectTemplateCount, parameters))
            //{
            //    while (rdr.Read())
            //    {
            //        var templateType = TemplateTypeUtils.GetEnumType(DatabaseApi.GetString(rdr, 0));
            //        var count = DatabaseApi.GetInt(rdr, 1);

            //        dictionary.Add(templateType, count);
            //    }
            //    rdr.Close();
            //}

            var dataList = GetValueList<(string TemplateType, int Count)>(Q
                .Select(Attr.TemplateType)
                .SelectRaw("COUNT(*) as Count")
                .Where(Attr.SiteId, siteId)
                .GroupBy(Attr.TemplateType));

            foreach (var data in dataList)
            {
                var templateType = TemplateTypeUtils.GetEnumType(data.TemplateType);
                var count = data.Count;

                if (dictionary.ContainsKey(templateType))
                {
                    dictionary[templateType] += count;
                }
                else
                {
                    dictionary.Add(templateType, count);
                }
            }

            return dictionary;
        }

        public IDataReader GetDataSourceByType(int siteId, TemplateType type)
        {
            IDataParameter[] parameters =
            {
                DatabaseApi.Instance.GetParameter("@SiteId", siteId),
                DatabaseApi.Instance.GetParameter("@TemplateType", type.Value)
            };
            var sqlString = "SELECT Id, SiteId, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault FROM siteserver_Template WHERE SiteId = @SiteId AND TemplateType = @TemplateType ORDER BY RelatedFileName";
            return DatabaseApi.Instance.ExecuteReader(WebConfigUtils.ConnectionString, sqlString, parameters);
        }

        public IDataReader GetDataSource(int siteId, string searchText, string templateTypeString)
        {
            if (string.IsNullOrEmpty(searchText) && string.IsNullOrEmpty(templateTypeString))
            {
                IDataParameter[] parameters =
                {
                    DatabaseApi.Instance.GetParameter("@SiteId", siteId)
                };
                string SqlSelectAllTemplateBySiteId = "SELECT Id, SiteId, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault FROM siteserver_Template WHERE SiteId = @SiteId ORDER BY TemplateType, RelatedFileName";
                var enumerable = DatabaseApi.Instance.ExecuteReader(WebConfigUtils.ConnectionString, SqlSelectAllTemplateBySiteId, parameters);
                return enumerable;
            }
            if (!string.IsNullOrEmpty(searchText))
            {
                var whereString = (string.IsNullOrEmpty(templateTypeString)) ? string.Empty :
                    $"AND TemplateType = '{templateTypeString}' ";
                searchText = AttackUtils.FilterSql(searchText);
                whereString +=
                    $"AND (TemplateName LIKE '%{searchText}%' OR RelatedFileName LIKE '%{searchText}%' OR CreatedFileFullName LIKE '%{searchText}%' OR CreatedFileExtName LIKE '%{searchText}%')";
                var sqlString =
                    $"SELECT Id, SiteId, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault FROM siteserver_Template WHERE SiteId = {siteId} {whereString} ORDER BY TemplateType, RelatedFileName";

                var enumerable = DatabaseApi.Instance.ExecuteReader(WebConfigUtils.ConnectionString, sqlString);
                return enumerable;
            }

            return GetDataSourceByType(siteId, TemplateTypeUtils.GetEnumType(templateTypeString));
        }

        public IList<TemplateInfo> GetTemplateInfoListByType(int siteId, TemplateType type)
        {
            //var list = new List<TemplateInfo>();

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamSiteId, siteId),
            //    GetParameter(ParamTemplateType, type.Value)
            //};
            //string SqlSelectAllTemplateByType = "SELECT Id, SiteId, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault FROM siteserver_Template WHERE SiteId = @SiteId AND TemplateType = @TemplateType ORDER BY RelatedFileName";
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectAllTemplateByType, parameters))
            //{
            //    while (rdr.Read())
            //    {
            //        var i = 0;
            //        var info = new TemplateInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), TemplateTypeUtils.GetEnumType(DatabaseApi.GetString(rdr, i++)), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), ECharsetUtils.GetEnumType(DatabaseApi.GetString(rdr, i++)), TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i)));
            //        list.Add(info);
            //    }
            //    rdr.Close();
            //}
            //return list;

            return GetObjectList(Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.TemplateType, type.Value)
                .OrderBy(Attr.RelatedFileName));
        }

        public IList<TemplateInfo> GetTemplateInfoListOfFile(int siteId)
        {
            //var list = new List<TemplateInfo>();

            //string sqlString =
            //    $"SELECT Id, SiteId, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault FROM siteserver_Template WHERE SiteId = {siteId} AND TemplateType = '{TemplateType.FileTemplate.Value}' ORDER BY RelatedFileName";

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    while (rdr.Read())
            //    {
            //        var i = 0;
            //        var info = new TemplateInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), TemplateTypeUtils.GetEnumType(DatabaseApi.GetString(rdr, i++)), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), ECharsetUtils.GetEnumType(DatabaseApi.GetString(rdr, i++)), TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i)));
            //        list.Add(info);
            //    }
            //    rdr.Close();
            //}
            //return list;

            return GetObjectList(Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.TemplateType, TemplateType.FileTemplate.Value)
                .OrderBy(Attr.RelatedFileName));
        }

        public IList<TemplateInfo> GetTemplateInfoListBySiteId(int siteId)
        {
            //var list = new List<TemplateInfo>();

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamSiteId, siteId)
            //};
            //string SqlSelectAllTemplateBySiteId = "SELECT Id, SiteId, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault FROM siteserver_Template WHERE SiteId = @SiteId ORDER BY TemplateType, RelatedFileName";
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectAllTemplateBySiteId, parameters))
            //{
            //    while (rdr.Read())
            //    {
            //        var i = 0;
            //        var info = new TemplateInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), TemplateTypeUtils.GetEnumType(DatabaseApi.GetString(rdr, i++)), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), ECharsetUtils.GetEnumType(DatabaseApi.GetString(rdr, i++)), TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i)));
            //        list.Add(info);
            //    }
            //    rdr.Close();
            //}
            //return list;

            return GetObjectList(Q
                .Where(Attr.SiteId, siteId)
                .OrderBy(Attr.TemplateType, Attr.RelatedFileName));
        }

        public IList<string> GetTemplateNameList(int siteId, TemplateType templateType)
        {
            //var list = new List<string>();

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamSiteId, siteId),
            //    GetParameter(ParamTemplateType, templateType.Value)
            //};
            //string SqlSelectTemplateNames = "SELECT TemplateName FROM siteserver_Template WHERE SiteId = @SiteId AND TemplateType = @TemplateType";
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectTemplateNames, parameters))
            //{
            //    while (rdr.Read())
            //    {
            //        list.Add(DatabaseApi.GetString(rdr, 0));
            //    }
            //    rdr.Close();
            //}

            //return list;

            return GetValueList<string>(Q
                .Select(Attr.TemplateName)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.TemplateType, templateType.Value));
        }

        public IList<string> GetLowerRelatedFileNameList(int siteId, TemplateType templateType)
        {
            //var list = new List<string>();

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamSiteId, siteId),
            //    GetParameter(ParamTemplateType, templateType.Value)
            //};
            //string SqlSelectRelatedFileNameByTemplateType = "SELECT RelatedFileName FROM siteserver_Template WHERE SiteId = @SiteId AND TemplateType = @TemplateType";
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectRelatedFileNameByTemplateType, parameters))
            //{
            //    while (rdr.Read())
            //    {
            //        list.Add(DatabaseApi.GetString(rdr, 0).ToLower());
            //    }
            //    rdr.Close();
            //}

            //return list;

            return GetValueList<string>(Q
                .Select(Attr.RelatedFileName)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.TemplateType, templateType.Value));
        }

        public void CreateDefaultTemplateInfo(int siteId, string administratorName)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);

            var templateInfoList = new List<TemplateInfo>();

            var templateInfo = new TemplateInfo
            {
                SiteId = siteInfo.Id,
                TemplateName = "系统首页模板",
                Type = TemplateType.IndexPageTemplate,
                RelatedFileName = "T_系统首页模板.html",
                CreatedFileFullName = "@/index.html",
                CreatedFileExtName = ".html",
                Default = true
            };
            templateInfoList.Add(templateInfo);

            templateInfo = new TemplateInfo
            {
                SiteId = siteInfo.Id,
                TemplateName = "系统栏目模板",
                Type = TemplateType.ChannelTemplate,
                RelatedFileName = "T_系统栏目模板.html",
                CreatedFileFullName = "index.html",
                CreatedFileExtName = ".html",
                Default = true
            };
            templateInfoList.Add(templateInfo);

            templateInfo = new TemplateInfo
            {
                SiteId = siteInfo.Id,
                TemplateName = "系统内容模板",
                Type = TemplateType.ContentTemplate,
                RelatedFileName = "T_系统内容模板.html",
                CreatedFileFullName = "index.html",
                CreatedFileExtName = ".html",
                Default = true
            };
            templateInfoList.Add(templateInfo);

            foreach (var theTemplateInfo in templateInfoList)
            {
                Insert(theTemplateInfo, theTemplateInfo.Content, administratorName);
            }
        }

        public Dictionary<int, TemplateInfo> GetTemplateInfoDictionaryBySiteId(int siteId)
        {
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamSiteId, siteId)
            //};
            //string SqlSelectAllTemplateBySiteId = "SELECT Id, SiteId, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault FROM siteserver_Template WHERE SiteId = @SiteId ORDER BY TemplateType, RelatedFileName";
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectAllTemplateBySiteId, parameters))
            //{
            //    while (rdr.Read())
            //    {
            //        var i = 0;
            //        var info = new TemplateInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), TemplateTypeUtils.GetEnumType(DatabaseApi.GetString(rdr, i++)), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), ECharsetUtils.GetEnumType(DatabaseApi.GetString(rdr, i++)), TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i)));
            //        dictionary.Add(info.Id, info);
            //    }
            //    rdr.Close();
            //}

            var templateInfoList = GetObjectList(Q
                .Where(Attr.SiteId, siteId)
                .OrderBy(Attr.TemplateType, Attr.RelatedFileName));

            return templateInfoList.ToDictionary(templateInfo => templateInfo.Id);
        }
    }
}


//using System;
//using System.Collections.Generic;
//using System.Data;
//using SiteServer.CMS.Core;
//using SiteServer.CMS.Database.Caches;
//using SiteServer.CMS.Database.Core;
//using SiteServer.CMS.Database.Models;
//using SiteServer.Plugin;
//using SiteServer.Utils;
//using SiteServer.Utils.Enumerations;

//namespace SiteServer.CMS.Database.Repositories
//{
//    public class Template : DataProviderBase
//    {
//        public override string TableName => "siteserver_Template";

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(TemplateInfo.Id),
//                DataType = DataType.Integer,
//                IsIdentity = true,
//                IsPrimaryKey = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TemplateInfo.SiteId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TemplateInfo.TemplateName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TemplateInfo.TemplateType),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TemplateInfo.RelatedFileName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TemplateInfo.CreatedFileFullName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TemplateInfo.CreatedFileExtName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TemplateInfo.Charset),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TemplateInfo.IsDefault),
//                DataType = DataType.VarChar
//            }
//        };

//        private const string SqlSelectTemplateByTemplateName = "SELECT Id, SiteId, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault FROM siteserver_Template WHERE SiteId = @SiteId AND TemplateType = @TemplateType AND TemplateName = @TemplateName";

//        private const string SqlSelectAllTemplateByType = "SELECT Id, SiteId, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault FROM siteserver_Template WHERE SiteId = @SiteId AND TemplateType = @TemplateType ORDER BY RelatedFileName";

//        private const string SqlSelectAllTemplateBySiteId = "SELECT Id, SiteId, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault FROM siteserver_Template WHERE SiteId = @SiteId ORDER BY TemplateType, RelatedFileName";

//        private const string SqlSelectTemplateNames = "SELECT TemplateName FROM siteserver_Template WHERE SiteId = @SiteId AND TemplateType = @TemplateType";

//        private const string SqlSelectTemplateCount = "SELECT TemplateType, COUNT(*) FROM siteserver_Template WHERE SiteId = @SiteId GROUP BY TemplateType";

//        private const string SqlSelectRelatedFileNameByTemplateType = "SELECT RelatedFileName FROM siteserver_Template WHERE SiteId = @SiteId AND TemplateType = @TemplateType";

//        private const string SqlUpdateTemplate = "UPDATE siteserver_Template SET TemplateName = @TemplateName, TemplateType = @TemplateType, RelatedFileName = @RelatedFileName, CreatedFileFullName = @CreatedFileFullName, CreatedFileExtName = @CreatedFileExtName, Charset = @Charset, IsDefault = @IsDefault WHERE  Id = @Id";

//        private const string SqlDeleteTemplate = "DELETE FROM siteserver_Template WHERE Id = @Id";

//        private const string ParamId = "@Id";
//        private const string ParamSiteId = "@SiteId";
//        private const string ParamTemplateName = "@TemplateName";
//        private const string ParamTemplateType = "@TemplateType";
//        private const string ParamRelatedFileName = "@RelatedFileName";
//        private const string ParamCreatedFileFullName = "@CreatedFileFullName";
//        private const string ParamCreatedFileExtName = "@CreatedFileExtName";
//        private const string ParamCharset = "@Charset";
//        private const string ParamIsDefault = "@IsDefault";

//        public int InsertObject(TemplateInfo templateInfo, string templateContent, string administratorName)
//        {
//            if (templateInfo.IsDefault)
//            {
//                SetAllTemplateDefaultToFalse(templateInfo.SiteId, templateInfo.TemplateType);
//            }

//            const string sqlInsertTemplate = "INSERT INTO siteserver_Template (SiteId, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault) VALUES (@SiteId, @TemplateName, @TemplateType, @RelatedFileName, @CreatedFileFullName, @CreatedFileExtName, @Charset, @IsDefault)";

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamSiteId, templateInfo.SiteId),
//				GetParameter(ParamTemplateName, templateInfo.TemplateName),
//				GetParameter(ParamTemplateType, templateInfo.TemplateType.Value),
//				GetParameter(ParamRelatedFileName, templateInfo.RelatedFileName),
//				GetParameter(ParamCreatedFileFullName, templateInfo.CreatedFileFullName),
//				GetParameter(ParamCreatedFileExtName, templateInfo.CreatedFileExtName),
//                GetParameter(ParamCharset, ECharsetUtils.GetValueById(templateInfo.Charset)),
//				GetParameter(ParamIsDefault, templateInfo.IsDefault.ToString())
//			};

//            var id = DatabaseApi.ExecuteNonQueryAndReturnId(ConnectionString, TableName, nameof(TemplateInfo.Id), sqlInsertTemplate, parameters);

//            var siteInfo = SiteManager.GetSiteInfo(templateInfo.SiteId);
//            TemplateManager.WriteContentToTemplateFile(siteInfo, templateInfo, templateContent, administratorName);

//            TemplateManager.RemoveCache(templateInfo.SiteId);

//            return id;
//        }

//        public void UpdateObject(SiteInfo siteInfo, TemplateInfo templateInfo, string templateContent, string administratorName)
//        {
//            if (templateInfo.IsDefault)
//            {
//                SetAllTemplateDefaultToFalse(siteInfo.Id, templateInfo.TemplateType);
//            }

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamTemplateName, templateInfo.TemplateName),
//				GetParameter(ParamTemplateType, templateInfo.TemplateType.Value),
//				GetParameter(ParamRelatedFileName, templateInfo.RelatedFileName),
//				GetParameter(ParamCreatedFileFullName, templateInfo.CreatedFileFullName),
//				GetParameter(ParamCreatedFileExtName, templateInfo.CreatedFileExtName),
//				GetParameter(ParamCharset, ECharsetUtils.GetValueById(templateInfo.Charset)),
//				GetParameter(ParamIsDefault, templateInfo.IsDefault.ToString()),
//				GetParameter(ParamId, templateInfo.Id)
//			};

//            DatabaseApi.ExecuteNonQuery(ConnectionString, SqlUpdateTemplate, parameters);

//            TemplateManager.WriteContentToTemplateFile(siteInfo, templateInfo, templateContent, administratorName);

//            TemplateManager.RemoveCache(templateInfo.SiteId);
//        }

//        private void SetAllTemplateDefaultToFalse(int siteId, TemplateType templateType)
//        {
//            var sqlString = "UPDATE siteserver_Template SET IsDefault = @IsDefault WHERE SiteId = @SiteId AND TemplateType = @TemplateType";

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamIsDefault, false.ToString()),
//				GetParameter(ParamSiteId, siteId),
//				GetParameter(ParamTemplateType, templateType.Value)
//			};

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
//        }

//        public void SetDefault(int siteId, int id)
//        {
//            var info = TemplateManager.GetTemplateInfo(siteId, id);
//            SetAllTemplateDefaultToFalse(info.SiteId, info.TemplateType);

//            const string sqlString = "UPDATE siteserver_Template SET IsDefault = @IsDefault WHERE Id = @Id";

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamIsDefault, true.ToString()),
//				GetParameter(ParamId, id)
//			};

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

//            TemplateManager.RemoveCache(siteId);
//        }

//        public void DeleteById(int siteId, int id)
//        {
//            var siteInfo = SiteManager.GetSiteInfo(siteId);
//            var templateInfo = TemplateManager.GetTemplateInfo(siteId, id);
//            var filePath = TemplateManager.GetTemplateFilePath(siteInfo, templateInfo);

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamId, id)
//			};

//            DatabaseApi.ExecuteNonQuery(ConnectionString, SqlDeleteTemplate, parameters);
//            FileUtils.DeleteFileIfExists(filePath);

//            TemplateManager.RemoveCache(siteId);
//        }

//        public string GetImportTemplateName(int siteId, string templateName)
//        {
//            string importTemplateName;
//            if (templateName.IndexOf("_", StringComparison.Ordinal) != -1)
//            {
//                var templateNameCount = 0;
//                var lastTemplateName = templateName.Substring(templateName.LastIndexOf("_", StringComparison.Ordinal) + 1);
//                var firstTemplateName = templateName.Substring(0, templateName.Length - lastTemplateName.Length);
//                try
//                {
//                    templateNameCount = int.Parse(lastTemplateName);
//                }
//                catch
//                {
//                    // ignored
//                }
//                templateNameCount++;
//                importTemplateName = firstTemplateName + templateNameCount;
//            }
//            else
//            {
//                importTemplateName = templateName + "_1";
//            }

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamSiteId, siteId),
//				GetParameter(ParamTemplateName, importTemplateName)
//			};

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectTemplateByTemplateName, parameters))
//            {
//                if (rdr.Read())
//                {
//                    importTemplateName = GetImportTemplateName(siteId, importTemplateName);
//                }
//                rdr.Close();
//            }

//            return importTemplateName;
//        }

//        public Dictionary<TemplateType, int> GetCountDictionary(int siteId)
//        {
//            var dictionary = new Dictionary<TemplateType, int>();

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamSiteId, siteId)
//			};

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectTemplateCount, parameters))
//            {
//                while (rdr.Read())
//                {
//                    var templateType = TemplateTypeUtils.GetEnumType(DatabaseApi.GetString(rdr, 0));
//                    var count = DatabaseApi.GetInt(rdr, 1);

//                    dictionary.Add(templateType, count);
//                }
//                rdr.Close();
//            }

//            return dictionary;
//        }

//        public IDataReader GetDataSourceByType(int siteId, TemplateType type)
//        {
//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamSiteId, siteId),
//				GetParameter(ParamTemplateType, type.Value)
//			};

//            var enumerable = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectAllTemplateByType, parameters);
//            return enumerable;
//        }

//        public IDataReader GetDataSource(int siteId, string searchText, string templateTypeString)
//        {
//            if (string.IsNullOrEmpty(searchText) && string.IsNullOrEmpty(templateTypeString))
//            {
//                IDataParameter[] parameters =
//				{
//					GetParameter(ParamSiteId, siteId)
//				};

//                var enumerable = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectAllTemplateBySiteId, parameters);
//                return enumerable;
//            }
//            if (!string.IsNullOrEmpty(searchText))
//            {
//                var whereString = (string.IsNullOrEmpty(templateTypeString)) ? string.Empty :
//                    $"AND TemplateType = '{templateTypeString}' ";
//                searchText = AttackUtils.FilterSql(searchText);
//                whereString +=
//                    $"AND (TemplateName LIKE '%{searchText}%' OR RelatedFileName LIKE '%{searchText}%' OR CreatedFileFullName LIKE '%{searchText}%' OR CreatedFileExtName LIKE '%{searchText}%')";
//                var sqlString =
//                    $"SELECT Id, SiteId, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault FROM siteserver_Template WHERE SiteId = {siteId} {whereString} ORDER BY TemplateType, RelatedFileName";

//                var enumerable = DatabaseApi.ExecuteReader(ConnectionString, sqlString);
//                return enumerable;
//            }

//            return GetDataSourceByType(siteId, TemplateTypeUtils.GetEnumType(templateTypeString));
//        }

//        public List<TemplateInfo> GetTemplateInfoListByType(int siteId, TemplateType type)
//        {
//            var list = new List<TemplateInfo>();

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamSiteId, siteId),
//				GetParameter(ParamTemplateType, type.Value)
//			};

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectAllTemplateByType, parameters))
//            {
//                while (rdr.Read())
//                {
//                    var i = 0;
//                    var info = new TemplateInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), TemplateTypeUtils.GetEnumType(DatabaseApi.GetString(rdr, i++)), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), ECharsetUtils.GetEnumType(DatabaseApi.GetString(rdr, i++)), TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i)));
//                    list.Add(info);
//                }
//                rdr.Close();
//            }
//            return list;
//        }

//        public List<TemplateInfo> GetTemplateInfoListOfFile(int siteId)
//        {
//            var list = new List<TemplateInfo>();

//            string sqlString =
//                $"SELECT Id, SiteId, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault FROM siteserver_Template WHERE SiteId = {siteId} AND TemplateType = '{TemplateType.FileTemplate.Value}' ORDER BY RelatedFileName";

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                while (rdr.Read())
//                {
//                    var i = 0;
//                    var info = new TemplateInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), TemplateTypeUtils.GetEnumType(DatabaseApi.GetString(rdr, i++)), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), ECharsetUtils.GetEnumType(DatabaseApi.GetString(rdr, i++)), TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i)));
//                    list.Add(info);
//                }
//                rdr.Close();
//            }
//            return list;
//        }

//        public List<TemplateInfo> GetTemplateInfoListBySiteId(int siteId)
//        {
//            var list = new List<TemplateInfo>();

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamSiteId, siteId)
//			};

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectAllTemplateBySiteId, parameters))
//            {
//                while (rdr.Read())
//                {
//                    var i = 0;
//                    var info = new TemplateInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), TemplateTypeUtils.GetEnumType(DatabaseApi.GetString(rdr, i++)), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), ECharsetUtils.GetEnumType(DatabaseApi.GetString(rdr, i++)), TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i)));
//                    list.Add(info);
//                }
//                rdr.Close();
//            }
//            return list;
//        }

//        public List<string> GetTemplateNameList(int siteId, TemplateType templateType)
//        {
//            var list = new List<string>();

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamSiteId, siteId),
//				GetParameter(ParamTemplateType, templateType.Value)
//			};

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectTemplateNames, parameters))
//            {
//                while (rdr.Read())
//                {
//                    list.Add(DatabaseApi.GetString(rdr, 0));
//                }
//                rdr.Close();
//            }

//            return list;
//        }

//        public List<string> GetLowerRelatedFileNameList(int siteId, TemplateType templateType)
//        {
//            var list = new List<string>();

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamSiteId, siteId),
//				GetParameter(ParamTemplateType, templateType.Value)
//			};

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectRelatedFileNameByTemplateType, parameters))
//            {
//                while (rdr.Read())
//                {
//                    list.Add(DatabaseApi.GetString(rdr, 0).ToLower());
//                }
//                rdr.Close();
//            }

//            return list;
//        }

//        public void CreateDefaultTemplateInfo(int siteId, string administratorName)
//        {
//            var siteInfo = SiteManager.GetSiteInfo(siteId);

//            var templateInfoList = new List<TemplateInfo>();
//            var charset = ECharsetUtils.GetEnumType(siteInfo.Charset);

//            var templateInfo = new TemplateInfo(0, siteInfo.Id, "系统首页模板", TemplateType.IndexPageTemplate, "T_系统首页模板.html", "@/index.html", ".html", charset, true);
//            templateInfoList.Add(templateInfo);

//            templateInfo = new TemplateInfo(0, siteInfo.Id, "系统栏目模板", TemplateType.ChannelTemplate, "T_系统栏目模板.html", "index.html", ".html", charset, true);
//            templateInfoList.Add(templateInfo);

//            templateInfo = new TemplateInfo(0, siteInfo.Id, "系统内容模板", TemplateType.ContentTemplate, "T_系统内容模板.html", "index.html", ".html", charset, true);
//            templateInfoList.Add(templateInfo);

//            foreach (var theTemplateInfo in templateInfoList)
//            {
//                InsertObject(theTemplateInfo, theTemplateInfo.Content, administratorName);
//            }
//        }

//        public Dictionary<int, TemplateInfo> GetTemplateInfoDictionaryBySiteId(int siteId)
//        {
//            var dictionary = new Dictionary<int, TemplateInfo>();

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamSiteId, siteId)
//			};

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectAllTemplateBySiteId, parameters))
//            {
//                while (rdr.Read())
//                {
//                    var i = 0;
//                    var info = new TemplateInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), TemplateTypeUtils.GetEnumType(DatabaseApi.GetString(rdr, i++)), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), ECharsetUtils.GetEnumType(DatabaseApi.GetString(rdr, i++)), TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i)));
//                    dictionary.Add(info.Id, info);
//                }
//                rdr.Close();
//            }

//            return dictionary;
//        }
//    }
//}
