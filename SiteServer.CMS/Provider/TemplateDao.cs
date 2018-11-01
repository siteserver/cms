using System;
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
    public class TemplateDao : DataProviderBase
    {
        public override string TableName => "siteserver_Template";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(TemplateInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(TemplateInfo.SiteId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(TemplateInfo.TemplateName),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(TemplateInfo.TemplateType),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(TemplateInfo.RelatedFileName),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(TemplateInfo.CreatedFileFullName),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(TemplateInfo.CreatedFileExtName),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(TemplateInfo.Charset),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(TemplateInfo.IsDefault),
                DataType = DataType.VarChar,
                DataLength = 18
            }
        };

        private const string SqlSelectTemplateByTemplateName = "SELECT Id, SiteId, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault FROM siteserver_Template WHERE SiteId = @SiteId AND TemplateType = @TemplateType AND TemplateName = @TemplateName";

        private const string SqlSelectAllTemplateByType = "SELECT Id, SiteId, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault FROM siteserver_Template WHERE SiteId = @SiteId AND TemplateType = @TemplateType ORDER BY RelatedFileName";

        private const string SqlSelectAllIdByType = "SELECT Id FROM siteserver_Template WHERE SiteId = @SiteId AND TemplateType = @TemplateType ORDER BY RelatedFileName";

        private const string SqlSelectAllTemplateBySiteId = "SELECT Id, SiteId, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault FROM siteserver_Template WHERE SiteId = @SiteId ORDER BY TemplateType, RelatedFileName";

        private const string SqlSelectTemplateNames = "SELECT TemplateName FROM siteserver_Template WHERE SiteId = @SiteId AND TemplateType = @TemplateType";

        private const string SqlSelectTemplateCount = "SELECT TemplateType, COUNT(*) FROM siteserver_Template WHERE SiteId = @SiteId GROUP BY TemplateType";

        private const string SqlSelectRelatedFileNameByTemplateType = "SELECT RelatedFileName FROM siteserver_Template WHERE SiteId = @SiteId AND TemplateType = @TemplateType";

        private const string SqlUpdateTemplate = "UPDATE siteserver_Template SET TemplateName = @TemplateName, TemplateType = @TemplateType, RelatedFileName = @RelatedFileName, CreatedFileFullName = @CreatedFileFullName, CreatedFileExtName = @CreatedFileExtName, Charset = @Charset, IsDefault = @IsDefault WHERE  Id = @Id";

        private const string SqlDeleteTemplate = "DELETE FROM siteserver_Template WHERE  Id = @Id";

        //by 20151106 sofuny
        private const string SqlSelectTemplateByUrlType = "SELECT * FROM siteserver_Template WHERE SiteId = @SiteId AND TemplateType = @TemplateType and CreatedFileFullName=@CreatedFileFullName ";
        private const string SqlSelectTemplateById = "SELECT * FROM siteserver_Template WHERE SiteId = @SiteId AND TemplateType = @TemplateType and Id = @Id ";

        private const string ParmId = "@Id";
        private const string ParmSiteId = "@SiteId";
        private const string ParmTemplateName = "@TemplateName";
        private const string ParmTemplateType = "@TemplateType";
        private const string ParmRelatedFileName = "@RelatedFileName";
        private const string ParmCreatedFileFullName = "@CreatedFileFullName";
        private const string ParmCreatedFileExtName = "@CreatedFileExtName";
        private const string ParmCharset = "@Charset";
        private const string ParmIsDefault = "@IsDefault";

        public int Insert(TemplateInfo templateInfo, string templateContent, string administratorName)
        {
            if (templateInfo.IsDefault)
            {
                SetAllTemplateDefaultToFalse(templateInfo.SiteId, templateInfo.TemplateType);
            }

            var sqlInsertTemplate = "INSERT INTO siteserver_Template (SiteId, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault) VALUES (@SiteId, @TemplateName, @TemplateType, @RelatedFileName, @CreatedFileFullName, @CreatedFileExtName, @Charset, @IsDefault)";
            
            var insertParms = new IDataParameter[]
			{
				GetParameter(ParmSiteId, DataType.Integer, templateInfo.SiteId),
				GetParameter(ParmTemplateName, DataType.VarChar, 50, templateInfo.TemplateName),
				GetParameter(ParmTemplateType, DataType.VarChar, 50, templateInfo.TemplateType.Value),
				GetParameter(ParmRelatedFileName, DataType.VarChar, 50, templateInfo.RelatedFileName),
				GetParameter(ParmCreatedFileFullName, DataType.VarChar, 50, templateInfo.CreatedFileFullName),
				GetParameter(ParmCreatedFileExtName, DataType.VarChar, 50, templateInfo.CreatedFileExtName),
                GetParameter(ParmCharset, DataType.VarChar, 50, ECharsetUtils.GetValue(templateInfo.Charset)),
				GetParameter(ParmIsDefault, DataType.VarChar, 18, templateInfo.IsDefault.ToString())
			};

            var id = ExecuteNonQueryAndReturnId(TableName, nameof(TemplateInfo.Id), sqlInsertTemplate, insertParms);

            var siteInfo = SiteManager.GetSiteInfo(templateInfo.SiteId);
            TemplateManager.WriteContentToTemplateFile(siteInfo, templateInfo, templateContent, administratorName);

            TemplateManager.RemoveCache(templateInfo.SiteId);

            return id;
        }

        public void Update(SiteInfo siteInfo, TemplateInfo templateInfo, string templateContent, string administratorName)
        {
            if (templateInfo.IsDefault)
            {
                SetAllTemplateDefaultToFalse(siteInfo.Id, templateInfo.TemplateType);
            }

            var updateParms = new IDataParameter[]
			{
				GetParameter(ParmTemplateName, DataType.VarChar, 50, templateInfo.TemplateName),
				GetParameter(ParmTemplateType, DataType.VarChar, 50, templateInfo.TemplateType.Value),
				GetParameter(ParmRelatedFileName, DataType.VarChar, 50, templateInfo.RelatedFileName),
				GetParameter(ParmCreatedFileFullName, DataType.VarChar, 50, templateInfo.CreatedFileFullName),
				GetParameter(ParmCreatedFileExtName, DataType.VarChar, 50, templateInfo.CreatedFileExtName),
				GetParameter(ParmCharset, DataType.VarChar, 50, ECharsetUtils.GetValue(templateInfo.Charset)),
				GetParameter(ParmIsDefault, DataType.VarChar, 18, templateInfo.IsDefault.ToString()),
				GetParameter(ParmId, DataType.Integer, templateInfo.Id)
			};

            ExecuteNonQuery(SqlUpdateTemplate, updateParms);

            TemplateManager.WriteContentToTemplateFile(siteInfo, templateInfo, templateContent, administratorName);

            TemplateManager.RemoveCache(templateInfo.SiteId);
        }

        private void SetAllTemplateDefaultToFalse(int siteId, TemplateType templateType)
        {
            var sqlString = "UPDATE siteserver_Template SET IsDefault = @IsDefault WHERE SiteId = @SiteId AND TemplateType = @TemplateType";

            var updateParms = new IDataParameter[]
			{
				GetParameter(ParmIsDefault, DataType.VarChar, 18, false.ToString()),
				GetParameter(ParmSiteId, DataType.Integer, siteId),
				GetParameter(ParmTemplateType, DataType.VarChar, 50, templateType.Value)
			};

            ExecuteNonQuery(sqlString, updateParms);

        }

        public void SetDefault(int siteId, int id)
        {
            var info = TemplateManager.GetTemplateInfo(siteId, id);
            SetAllTemplateDefaultToFalse(info.SiteId, info.TemplateType);

            var sqlString = "UPDATE siteserver_Template SET IsDefault = @IsDefault WHERE Id = @Id";

            var updateParms = new IDataParameter[]
			{
				GetParameter(ParmIsDefault, DataType.VarChar, 18, true.ToString()),
				GetParameter(ParmId, DataType.Integer, id)
			};

            ExecuteNonQuery(sqlString, updateParms);

            TemplateManager.RemoveCache(siteId);
        }

        public void Delete(int siteId, int id)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var templateInfo = TemplateManager.GetTemplateInfo(siteId, id);
            var filePath = TemplateManager.GetTemplateFilePath(siteInfo, templateInfo);

            var parms = new IDataParameter[]
			{
				GetParameter(ParmId, DataType.Integer, id)
			};

            ExecuteNonQuery(SqlDeleteTemplate, parms);
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

            var parms = new IDataParameter[]
			{
				GetParameter(ParmSiteId, DataType.Integer, siteId),
				GetParameter(ParmTemplateName, DataType.VarChar, 50, importTemplateName)
			};

            using (var rdr = ExecuteReader(SqlSelectTemplateByTemplateName, parms))
            {
                if (rdr.Read())
                {
                    importTemplateName = GetImportTemplateName(siteId, importTemplateName);
                }
                rdr.Close();
            }

            return importTemplateName;
        }

        public Dictionary<TemplateType, int> GetCountDictionary(int siteId)
        {
            var dictionary = new Dictionary<TemplateType, int>();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmSiteId, DataType.Integer, siteId)
			};

            using (var rdr = ExecuteReader(SqlSelectTemplateCount, parms))
            {
                while (rdr.Read())
                {
                    var templateType = TemplateTypeUtils.GetEnumType(GetString(rdr, 0));
                    var count = GetInt(rdr, 1);

                    dictionary.Add(templateType, count);
                }
                rdr.Close();
            }

            return dictionary;
        }

        public IDataReader GetDataSourceByType(int siteId, TemplateType type)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmSiteId, DataType.Integer, siteId),
				GetParameter(ParmTemplateType, DataType.VarChar, 50, type.Value)
			};

            var enumerable = ExecuteReader(SqlSelectAllTemplateByType, parms);
            return enumerable;
        }

        public IDataReader GetDataSource(int siteId, string searchText, string templateTypeString)
        {
            if (string.IsNullOrEmpty(searchText) && string.IsNullOrEmpty(templateTypeString))
            {
                var parms = new IDataParameter[]
				{
					GetParameter(ParmSiteId, DataType.Integer, siteId)
				};

                var enumerable = ExecuteReader(SqlSelectAllTemplateBySiteId, parms);
                return enumerable;
            }
            if (!string.IsNullOrEmpty(searchText))
            {
                var whereString = (string.IsNullOrEmpty(templateTypeString)) ? string.Empty :
                    $"AND TemplateType = '{templateTypeString}' ";
                searchText = AttackUtils.FilterSql(searchText);
                whereString +=
                    $"AND (TemplateName LIKE '%{searchText}%' OR RelatedFileName LIKE '%{searchText}%' OR CreatedFileFullName LIKE '%{searchText}%' OR CreatedFileExtName LIKE '%{searchText}%')";
                string sqlString =
                    $"SELECT Id, SiteId, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault FROM siteserver_Template WHERE SiteId = {siteId} {whereString} ORDER BY TemplateType, RelatedFileName";

                var enumerable = ExecuteReader(sqlString);
                return enumerable;
            }

            return GetDataSourceByType(siteId, TemplateTypeUtils.GetEnumType(templateTypeString));
        }

        public List<int> GetIdListByType(int siteId, TemplateType type)
        {
            var list = new List<int>();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmSiteId, DataType.Integer, siteId),
				GetParameter(ParmTemplateType, DataType.VarChar, 50, type.Value)
			};

            using (var rdr = ExecuteReader(SqlSelectAllIdByType, parms))
            {
                while (rdr.Read())
                {
                    var id = GetInt(rdr, 0);
                    list.Add(id);
                }
                rdr.Close();
            }
            return list;
        }

        public List<TemplateInfo> GetTemplateInfoListByType(int siteId, TemplateType type)
        {
            var list = new List<TemplateInfo>();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmSiteId, DataType.Integer, siteId),
				GetParameter(ParmTemplateType, DataType.VarChar, 50, type.Value)
			};

            using (var rdr = ExecuteReader(SqlSelectAllTemplateByType, parms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var info = new TemplateInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), TemplateTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), ECharsetUtils.GetEnumType(GetString(rdr, i++)), GetBool(rdr, i));
                    list.Add(info);
                }
                rdr.Close();
            }
            return list;
        }

        public List<TemplateInfo> GetTemplateInfoListOfFile(int siteId)
        {
            var list = new List<TemplateInfo>();

            string sqlString =
                $"SELECT Id, SiteId, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault FROM siteserver_Template WHERE SiteId = {siteId} AND TemplateType = '{TemplateType.FileTemplate.Value}' ORDER BY RelatedFileName";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var info = new TemplateInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), TemplateTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), ECharsetUtils.GetEnumType(GetString(rdr, i++)), GetBool(rdr, i));
                    list.Add(info);
                }
                rdr.Close();
            }
            return list;
        }

        public List<TemplateInfo> GetTemplateInfoListBySiteId(int siteId)
        {
            var list = new List<TemplateInfo>();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmSiteId, DataType.Integer, siteId)
			};

            using (var rdr = ExecuteReader(SqlSelectAllTemplateBySiteId, parms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var info = new TemplateInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), TemplateTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), ECharsetUtils.GetEnumType(GetString(rdr, i++)), GetBool(rdr, i));
                    list.Add(info);
                }
                rdr.Close();
            }
            return list;
        }

        public List<string> GetTemplateNameList(int siteId, TemplateType templateType)
        {
            var list = new List<string>();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmSiteId, DataType.Integer, siteId),
				GetParameter(ParmTemplateType, DataType.VarChar, 50, templateType.Value)
			};

            using (var rdr = ExecuteReader(SqlSelectTemplateNames, parms))
            {
                while (rdr.Read())
                {
                    list.Add(GetString(rdr, 0));
                }
                rdr.Close();
            }

            return list;
        }

        public List<string> GetLowerRelatedFileNameList(int siteId, TemplateType templateType)
        {
            var list = new List<string>();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmSiteId, DataType.Integer, siteId),
				GetParameter(ParmTemplateType, DataType.VarChar, 50, templateType.Value)
			};

            using (var rdr = ExecuteReader(SqlSelectRelatedFileNameByTemplateType, parms))
            {
                while (rdr.Read())
                {
                    list.Add(GetString(rdr, 0).ToLower());
                }
                rdr.Close();
            }

            return list;
        }

        public void CreateDefaultTemplateInfo(int siteId, string administratorName)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);

            var templateInfoList = new List<TemplateInfo>();
            var charset = ECharsetUtils.GetEnumType(siteInfo.Additional.Charset);

            var templateInfo = new TemplateInfo(0, siteInfo.Id, "系统首页模板", TemplateType.IndexPageTemplate, "T_系统首页模板.html", "@/index.html", ".html", charset, true);
            templateInfoList.Add(templateInfo);

            templateInfo = new TemplateInfo(0, siteInfo.Id, "系统栏目模板", TemplateType.ChannelTemplate, "T_系统栏目模板.html", "index.html", ".html", charset, true);
            templateInfoList.Add(templateInfo);

            templateInfo = new TemplateInfo(0, siteInfo.Id, "系统内容模板", TemplateType.ContentTemplate, "T_系统内容模板.html", "index.html", ".html", charset, true);
            templateInfoList.Add(templateInfo);

            foreach (var theTemplateInfo in templateInfoList)
            {
                Insert(theTemplateInfo, theTemplateInfo.Content, administratorName);
            }
        }

        public Dictionary<int, TemplateInfo> GetTemplateInfoDictionaryBySiteId(int siteId)
        {
            var dictionary = new Dictionary<int, TemplateInfo>();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmSiteId, DataType.Integer, siteId)
			};

            using (var rdr = ExecuteReader(SqlSelectAllTemplateBySiteId, parms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var info = new TemplateInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), TemplateTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), ECharsetUtils.GetEnumType(GetString(rdr, i++)), GetBool(rdr, i));
                    dictionary.Add(info.Id, info);
                }
                rdr.Close();
            }

            return dictionary;
        }


        public TemplateInfo GetTemplateByUrlType(int siteId, TemplateType type, string createdFileFullName)
        {
            TemplateInfo info = null;
            var parms = new IDataParameter[]
			{
				GetParameter(ParmSiteId, DataType.Integer, siteId),
				GetParameter(ParmTemplateType, DataType.VarChar, 50, type.Value),
				GetParameter(ParmCreatedFileFullName, DataType.VarChar, 50, createdFileFullName)
			};

            using (var rdr = ExecuteReader(SqlSelectTemplateByUrlType, parms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    info = new TemplateInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), TemplateTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), ECharsetUtils.GetEnumType(GetString(rdr, i++)), GetBool(rdr, i));
                }
                rdr.Close();
            }
            return info;
        }

        public TemplateInfo GetTemplateById(int siteId, TemplateType type, string tId)
        {
            TemplateInfo info = null;
            var parms = new IDataParameter[]
			{
				GetParameter(ParmSiteId, DataType.Integer, siteId),
				GetParameter(ParmTemplateType, DataType.VarChar, 50, type.Value),
				GetParameter(ParmId, DataType.Integer, tId)
			};

            using (var rdr = ExecuteReader(SqlSelectTemplateById, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    info = new TemplateInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), TemplateTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), ECharsetUtils.GetEnumType(GetString(rdr, i++)), GetBool(rdr, i));
                }
                rdr.Close();
            }
            return info;
        }

    }
}
