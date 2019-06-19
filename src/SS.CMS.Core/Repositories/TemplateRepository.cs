using System;
using System.Collections.Generic;
using System.Linq;
using SS.CMS.Data;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services.ICacheManager;
using SS.CMS.Services.ISettingsManager;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class TemplateRepository : ITemplateRepository
    {
        private static readonly string CacheKey = StringUtils.GetCacheKey(nameof(TemplateRepository));
        private readonly Repository<TemplateInfo> _repository;
        private readonly ISettingsManager _settingsManager;
        private readonly ICacheManager _cacheManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly ITemplateLogRepository _templateLogRepository;

        public TemplateRepository(ISettingsManager settingsManager, ICacheManager cacheManager, ISiteRepository siteRepository, IChannelRepository channelRepository, ITemplateLogRepository templateLogRepository)
        {
            _repository = new Repository<TemplateInfo>(new Db(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
            _settingsManager = settingsManager;
            _cacheManager = cacheManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _templateLogRepository = templateLogRepository;
        }

        public IDb Db => _repository.Db;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

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
            if (templateInfo.IsDefault)
            {
                SetAllTemplateDefaultToFalse(templateInfo.SiteId, templateInfo.Type);
            }

            var id = _repository.Insert(templateInfo);

            var siteInfo = _siteRepository.GetSiteInfo(templateInfo.SiteId);
            WriteContentToTemplateFile(siteInfo, templateInfo, templateContent, administratorName);

            RemoveCache(templateInfo.SiteId);

            return id;
        }

        public void Update(SiteInfo siteInfo, TemplateInfo templateInfo, string templateContent, string administratorName)
        {
            if (templateInfo.IsDefault)
            {
                SetAllTemplateDefaultToFalse(siteInfo.Id, templateInfo.Type);
            }

            _repository.Update(templateInfo);

            WriteContentToTemplateFile(siteInfo, templateInfo, templateContent, administratorName);

            RemoveCache(templateInfo.SiteId);
        }

        private void SetAllTemplateDefaultToFalse(int siteId, TemplateType templateType)
        {
            _repository.Update(Q
                .Set(Attr.IsDefault, false.ToString())
                .Where(Attr.SiteId, siteId)
                .Where(Attr.TemplateType, templateType.Value)
            );
        }

        public void SetDefault(int siteId, int id)
        {
            var info = GetTemplateInfo(siteId, id);
            SetAllTemplateDefaultToFalse(info.SiteId, info.Type);

            _repository.Update(Q
                .Set(Attr.IsDefault, true.ToString())
                .Where(Attr.Id, id)
            );

            RemoveCache(siteId);
        }

        public void Delete(int siteId, int id)
        {
            var siteInfo = _siteRepository.GetSiteInfo(siteId);
            var templateInfo = GetTemplateInfo(siteId, id);
            var filePath = GetTemplateFilePath(siteInfo, templateInfo);

            _repository.Delete(id);

            FileUtils.DeleteFileIfExists(filePath);

            RemoveCache(siteId);
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

            var isExists = _repository.Exists(Q.Where(Attr.SiteId, siteId).Where(Attr.TemplateName, importTemplateName));
            if (isExists)
            {
                importTemplateName = GetImportTemplateName(siteId, importTemplateName);
            }

            return importTemplateName;
        }

        public Dictionary<TemplateType, int> GetCountDictionary(int siteId)
        {
            var dictionary = new Dictionary<TemplateType, int>();

            var dataList = _repository.GetAll<(string TemplateType, int Count)>(Q
                .Select(Attr.TemplateType)
                .SelectRaw("COUNT(*) as Count")
                .Where(Attr.SiteId, siteId)
                .GroupBy(Attr.TemplateType));

            foreach (var data in dataList)
            {
                var templateType = TemplateType.Parse(data.TemplateType);
                var count = data.Count;

                if (dictionary.ContainsKey(templateType))
                {
                    dictionary[templateType] += count;
                }
                else
                {
                    dictionary[templateType] = count;
                }
            }

            return dictionary;
        }

        // public IDataReader GetDataSourceByType(int siteId, TemplateType type)
        // {
        //     IDataParameter[] parameters =
        //     {
        //         DataProvider.DatabaseApi.GetParameter("@SiteId", siteId),
        //         DataProvider.DatabaseApi.GetParameter("@TemplateType", type.Value)
        //     };
        //     var sqlString = "SELECT Id, SiteId, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault FROM siteserver_Template WHERE SiteId = @SiteId AND TemplateType = @TemplateType ORDER BY RelatedFileName";
        //     return DataProvider.DatabaseApi.ExecuteReader(WebConfigUtils.ConnectionString, sqlString, parameters);
        // }

        // public IDataReader GetDataSource(int siteId, string searchText, string templateTypeString)
        // {
        //     if (string.IsNullOrEmpty(searchText) && string.IsNullOrEmpty(templateTypeString))
        //     {
        //         IDataParameter[] parameters =
        //         {
        //             DataProvider.DatabaseApi.GetParameter("@SiteId", siteId)
        //         };
        //         string SqlSelectAllTemplateBySiteId = "SELECT Id, SiteId, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault FROM siteserver_Template WHERE SiteId = @SiteId ORDER BY TemplateType, RelatedFileName";
        //         var enumerable = DataProvider.DatabaseApi.ExecuteReader(WebConfigUtils.ConnectionString, SqlSelectAllTemplateBySiteId, parameters);
        //         return enumerable;
        //     }
        //     if (!string.IsNullOrEmpty(searchText))
        //     {
        //         var whereString = (string.IsNullOrEmpty(templateTypeString)) ? string.Empty :
        //             $"AND TemplateType = '{templateTypeString}' ";
        //         searchText = AttackUtils.FilterSql(searchText);
        //         whereString +=
        //             $"AND (TemplateName LIKE '%{searchText}%' OR RelatedFileName LIKE '%{searchText}%' OR CreatedFileFullName LIKE '%{searchText}%' OR CreatedFileExtName LIKE '%{searchText}%')";
        //         var sqlString =
        //             $"SELECT Id, SiteId, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault FROM siteserver_Template WHERE SiteId = {siteId} {whereString} ORDER BY TemplateType, RelatedFileName";

        //         var enumerable = DataProvider.DatabaseApi.ExecuteReader(WebConfigUtils.ConnectionString, sqlString);
        //         return enumerable;
        //     }

        //     return GetDataSourceByType(siteId, TemplateTypeUtils.GetEnumType(templateTypeString));
        // }

        public IList<TemplateInfo> GetTemplateInfoListByType(int siteId, TemplateType type)
        {
            return _repository.GetAll(Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.TemplateType, type.Value)
                .OrderBy(Attr.RelatedFileName)).ToList();
        }

        public IList<TemplateInfo> GetTemplateInfoListOfFile(int siteId)
        {
            return _repository.GetAll(Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.TemplateType, TemplateType.FileTemplate.Value)
                .OrderBy(Attr.RelatedFileName)).ToList();
        }

        public IList<TemplateInfo> GetTemplateInfoListBySiteId(int siteId)
        {
            return _repository.GetAll(Q
                .Where(Attr.SiteId, siteId)
                .OrderBy(Attr.TemplateType, Attr.RelatedFileName)).ToList();
        }

        public IList<string> GetTemplateNameList(int siteId, TemplateType templateType)
        {
            return _repository.GetAll<string>(Q
                .Select(Attr.TemplateName)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.TemplateType, templateType.Value)).ToList();
        }

        public IList<string> GetLowerRelatedFileNameList(int siteId, TemplateType templateType)
        {
            return _repository.GetAll<string>(Q
                .Select(Attr.RelatedFileName)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.TemplateType, templateType.Value)).ToList();
        }

        public void CreateDefaultTemplateInfo(int siteId, string administratorName)
        {
            var siteInfo = _siteRepository.GetSiteInfo(siteId);

            var templateInfoList = new List<TemplateInfo>();

            var templateInfo = new TemplateInfo
            {
                SiteId = siteInfo.Id,
                TemplateName = "系统首页模板",
                Type = TemplateType.IndexPageTemplate,
                RelatedFileName = "T_系统首页模板.html",
                CreatedFileFullName = "@/index.html",
                CreatedFileExtName = ".html",
                IsDefault = true
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
                IsDefault = true
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
                IsDefault = true
            };
            templateInfoList.Add(templateInfo);

            foreach (var theTemplateInfo in templateInfoList)
            {
                Insert(theTemplateInfo, theTemplateInfo.Content, administratorName);
            }
        }

        private Dictionary<int, TemplateInfo> GetTemplateInfoDictionaryBySiteIdToCache(int siteId)
        {
            var templateInfoList = _repository.GetAll(Q
                .Where(Attr.SiteId, siteId)
                .OrderBy(Attr.TemplateType, Attr.RelatedFileName));

            return templateInfoList.ToDictionary(templateInfo => templateInfo.Id);
        }
    }
}
