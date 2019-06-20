using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SS.CMS.Cli.Core;
using SS.CMS.Cli.Updater;
using SS.CMS.Cli.Updater.Tables;
using SS.CMS.Cli.Updater.Tables.GovInteract;
using SS.CMS.Cli.Updater.Tables.GovPublic;
using SS.CMS.Cli.Updater.Tables.Jobs;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Utils;

namespace SS.CMS.Cli.Services
{
    public class UpdaterManager
    {
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IAreaRepository _areaRepository;
        private readonly IChannelGroupRepository _channelGroupRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IConfigRepository _configRepository;
        private readonly IContentCheckRepository _contentCheckRepository;
        private readonly IContentGroupRepository _contentGroupRepository;
        private readonly IDbCacheRepository _dbCacheRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly ILogRepository _logRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IPluginConfigRepository _pluginConfigRepository;
        private readonly IPluginRepository _pluginRepository;
        private readonly IRelatedFieldItemRepository _relatedFieldItemRepository;
        private readonly IRelatedFieldRepository _relatedFieldRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ISiteLogRepository _siteLogRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly ISpecialRepository _specialRepository;
        private readonly ITableStyleItemRepository _tableStyleItemRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ITemplateLogRepository _templateLogRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IUserLogRepository _userLogRepository;
        private readonly IUserMenuRepository _userMenuRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;

        public UpdaterManager(
            IAccessTokenRepository accessTokenRepository,
            IAreaRepository areaRepository,
            IChannelGroupRepository channelGroupRepository,
            IChannelRepository channelRepository,
            IConfigRepository configRepository,
            IContentCheckRepository contentCheckRepository,
            IContentGroupRepository contentGroupRepository,
            IDbCacheRepository dbCacheRepository,
            IDepartmentRepository departmentRepository,
            IErrorLogRepository errorLogRepository,
            ILogRepository logRepository,
            IPermissionRepository permissionRepository,
            IPluginConfigRepository pluginConfigRepository,
            IPluginRepository pluginRepository,
            IRelatedFieldItemRepository relatedFieldItemRepository,
            IRelatedFieldRepository relatedFieldRepository,
            IRoleRepository roleRepository,
            ISiteLogRepository siteLogRepository,
            ISiteRepository siteRepository,
            ISpecialRepository specialRepository,
            ITableStyleItemRepository tableStyleItemRepository,
            ITableStyleRepository tableStyleRepository,
            ITagRepository tagRepository,
            ITemplateLogRepository templateLogRepository,
            ITemplateRepository templateRepository,
            IUserGroupRepository userGroupRepository,
            IUserLogRepository userLogRepository,
            IUserMenuRepository userMenuRepository,
            IUserRepository userRepository,
            IUserRoleRepository userRoleRepository
        )
        {
            _accessTokenRepository = accessTokenRepository;
            _areaRepository = areaRepository;
            _channelGroupRepository = channelGroupRepository;
            _channelRepository = channelRepository;
            _configRepository = configRepository;
            _contentCheckRepository = contentCheckRepository;
            _contentGroupRepository = contentGroupRepository;
            _dbCacheRepository = dbCacheRepository;
            _departmentRepository = departmentRepository;
            _errorLogRepository = errorLogRepository;
            _logRepository = logRepository;
            _permissionRepository = permissionRepository;
            _pluginConfigRepository = pluginConfigRepository;
            _pluginRepository = pluginRepository;
            _relatedFieldItemRepository = relatedFieldItemRepository;
            _relatedFieldRepository = relatedFieldRepository;
            _roleRepository = roleRepository;
            _siteLogRepository = siteLogRepository;
            _siteRepository = siteRepository;
            _specialRepository = specialRepository;
            _tableStyleItemRepository = tableStyleItemRepository;
            _tableStyleRepository = tableStyleRepository;
            _tagRepository = tagRepository;
            _templateLogRepository = templateLogRepository;
            _templateRepository = templateRepository;
            _userGroupRepository = userGroupRepository;
            _userLogRepository = userLogRepository;
            _userMenuRepository = userMenuRepository;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
        }

        public void LoadTree(TreeInfo oldTreeInfo, TreeInfo newTreeInfo)
        {
            OldTreeInfo = oldTreeInfo;
            NewTreeInfo = newTreeInfo;
        }

        protected TreeInfo OldTreeInfo { get; set; }

        protected TreeInfo NewTreeInfo { get; set; }

        public async Task<Tuple<string, TableInfo>> GetNewTableInfoAsync(string oldTableName, TableInfo oldTableInfo, ConvertInfo converter)
        {
            if (converter == null)
            {
                converter = new ConvertInfo();
            }

            if (converter.IsAbandon)
            {
                await CliUtils.PrintRowAsync(oldTableName, "Abandon", "--");
                return null;
            }

            if (string.IsNullOrEmpty(converter.NewTableName))
            {
                converter.NewTableName = oldTableName;
            }
            if (converter.NewColumns == null || converter.NewColumns.Count == 0)
            {
                converter.NewColumns = oldTableInfo.Columns;
            }

            var newTableInfo = new TableInfo
            {
                Columns = converter.NewColumns,
                TotalCount = oldTableInfo.TotalCount,
                RowFiles = oldTableInfo.RowFiles
            };

            await CliUtils.PrintRowAsync(oldTableName, converter.NewTableName, oldTableInfo.TotalCount.ToString("#,0"));

            if (oldTableInfo.RowFiles.Count > 0)
            {
                var i = 0;
                using (var progress = new ProgressBar())
                {
                    foreach (var fileName in oldTableInfo.RowFiles)
                    {
                        progress.Report((double)i++ / oldTableInfo.RowFiles.Count);

                        var oldFilePath = OldTreeInfo.GetTableContentFilePath(oldTableName, fileName);
                        var newFilePath = NewTreeInfo.GetTableContentFilePath(converter.NewTableName, fileName);

                        if (converter.ConvertKeyDict != null)
                        {
                            var oldRows =
                                TranslateUtils.JsonDeserialize<List<JObject>>(await FileUtils.ReadTextAsync(oldFilePath, Encoding.UTF8));

                            var newRows = UpdateUtils.UpdateRows(oldRows, converter.ConvertKeyDict, converter.ConvertValueDict, converter.Process);

                            await FileUtils.WriteTextAsync(newFilePath, Encoding.UTF8, TranslateUtils.JsonSerialize(newRows));
                        }
                        else
                        {
                            FileUtils.CopyFile(oldFilePath, newFilePath);
                        }
                    }
                }
            }

            return new Tuple<string, TableInfo>(converter.NewTableName, newTableInfo);
        }

        public async Task UpdateSplitContentsTableInfoAsync(Dictionary<int, TableInfo> splitSiteTableDict, List<int> siteIdList, string oldTableName, TableInfo oldTableInfo, ConvertInfo converter)
        {
            if (converter == null)
            {
                converter = new ConvertInfo();
            }

            if (converter.IsAbandon)
            {
                await CliUtils.PrintRowAsync(oldTableName, "Abandon", "--");
                return;
            }

            if (converter.NewColumns == null || converter.NewColumns.Count == 0)
            {
                converter.NewColumns = oldTableInfo.Columns;
            }

            await CliUtils.PrintRowAsync(oldTableName, "#split-content#", oldTableInfo.TotalCount.ToString("#,0"));

            if (oldTableInfo.RowFiles.Count > 0)
            {
                var i = 0;
                using (var progress = new ProgressBar())
                {
                    foreach (var fileName in oldTableInfo.RowFiles)
                    {
                        progress.Report((double)i++ / oldTableInfo.RowFiles.Count);

                        var newRows = new List<Dictionary<string, object>>();

                        var oldFilePath = OldTreeInfo.GetTableContentFilePath(oldTableName, fileName);

                        var oldRows =
                            TranslateUtils.JsonDeserialize<List<JObject>>(await FileUtils.ReadTextAsync(oldFilePath, Encoding.UTF8));

                        newRows.AddRange(UpdateUtils.UpdateRows(oldRows, converter.ConvertKeyDict, converter.ConvertValueDict, converter.Process));

                        var siteIdWithRows = new Dictionary<int, List<Dictionary<string, object>>>();
                        foreach (var siteId in siteIdList)
                        {
                            siteIdWithRows.Add(siteId, new List<Dictionary<string, object>>());
                        }

                        foreach (var newRow in newRows)
                        {
                            if (newRow.ContainsKey(nameof(ContentInfo.SiteId)))
                            {
                                var siteId = Convert.ToInt32(newRow[nameof(ContentInfo.SiteId)]);
                                if (siteIdList.Contains(siteId))
                                {
                                    var rows = siteIdWithRows[siteId];
                                    rows.Add(newRow);
                                }
                            }
                        }

                        foreach (var siteId in siteIdList)
                        {
                            var siteRows = siteIdWithRows[siteId];
                            var siteTableName = StringUtils.GetContentTableName(siteId);
                            var siteTableInfo = splitSiteTableDict[siteId];
                            siteTableInfo.TotalCount += siteRows.Count;

                            foreach (var tableColumn in converter.NewColumns)
                            {
                                if (!siteTableInfo.Columns.Any(t => StringUtils.EqualsIgnoreCase(t.AttributeName, tableColumn.AttributeName)))
                                {
                                    siteTableInfo.Columns.Add(tableColumn);
                                }
                            }

                            if (siteRows.Count > 0)
                            {
                                var siteTableFileName = $"{siteTableInfo.RowFiles.Count + 1}.json";
                                siteTableInfo.RowFiles.Add(siteTableFileName);
                                var filePath = NewTreeInfo.GetTableContentFilePath(siteTableName, siteTableFileName);
                                await FileUtils.WriteTextAsync(filePath, Encoding.UTF8, TranslateUtils.JsonSerialize(siteRows));
                            }
                        }
                    }
                }
            }
        }

        public async Task<Tuple<string, TableInfo>> UpdateTableInfoAsync(string oldTableName, TableInfo oldTableInfo, List<string> tableNameListForGovPublic, List<string> tableNameListForGovInteract, List<string> tableNameListForJob)
        {
            ConvertInfo converter = null;

            if (StringUtils.ContainsIgnoreCase(TableAdministrator.OldTableNames, oldTableName))
            {
                converter = TableAdministrator.GetConverter(_userRepository);
            }
            else if (StringUtils.ContainsIgnoreCase(TableAdministratorsInRoles.OldTableNames, oldTableName))
            {
                converter = TableAdministratorsInRoles.GetConverter(_userRoleRepository);
            }
            else if (StringUtils.EqualsIgnoreCase(TableArea.OldTableName, oldTableName))
            {
                converter = TableArea.GetConverter(_areaRepository);
            }
            else if (StringUtils.ContainsIgnoreCase(TableChannel.OldTableNames, oldTableName))
            {
                converter = TableChannel.GetConverter(_channelRepository);
            }
            else if (StringUtils.ContainsIgnoreCase(TableChannelGroup.OldTableNames, oldTableName))
            {
                converter = TableChannelGroup.GetConverter(_channelGroupRepository);
            }
            else if (StringUtils.EqualsIgnoreCase(TableConfig.OldTableName, oldTableName))
            {
                converter = TableConfig.GetConverter(_configRepository);
            }
            else if (StringUtils.EqualsIgnoreCase(TableContentCheck.OldTableName, oldTableName))
            {
                converter = TableContentCheck.GetConverter(_contentCheckRepository);
            }
            else if (StringUtils.ContainsIgnoreCase(TableContentGroup.OldTableNames, oldTableName))
            {
                converter = TableContentGroup.GetConverter(_contentGroupRepository);
            }
            else if (StringUtils.EqualsIgnoreCase(TableDbCache.OldTableName, oldTableName))
            {
                converter = TableDbCache.GetConverter(_dbCacheRepository);
            }
            else if (StringUtils.EqualsIgnoreCase(TableDepartment.OldTableName, oldTableName))
            {
                converter = TableDepartment.GetConverter(_departmentRepository);
            }
            else if (StringUtils.EqualsIgnoreCase(TableErrorLog.OldTableName, oldTableName))
            {
                converter = TableErrorLog.Converter;
            }
            else if (StringUtils.ContainsIgnoreCase(TableKeyword.OldTableNames, oldTableName))
            {
                converter = TableKeyword.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableLog.OldTableName, oldTableName))
            {
                converter = TableLog.GetConverter(_logRepository);
            }
            else if (StringUtils.ContainsIgnoreCase(TablePermissionsInRoles.OldTableNames, oldTableName))
            {
                converter = TablePermissionsInRoles.GetConverter(_permissionRepository);
            }
            else if (StringUtils.ContainsIgnoreCase(TableRelatedField.OldTableNames, oldTableName))
            {
                converter = TableRelatedField.GetConverter(_relatedFieldRepository);
            }
            else if (StringUtils.ContainsIgnoreCase(TableRelatedFieldItem.OldTableNames, oldTableName))
            {
                converter = TableRelatedFieldItem.GetConverter(_relatedFieldItemRepository);
            }
            else if (StringUtils.EqualsIgnoreCase(TableRole.OldTableName, oldTableName))
            {
                converter = TableRole.GetConverter(_roleRepository);
            }
            else if (StringUtils.ContainsIgnoreCase(TableSite.OldTableNames, oldTableName))
            {
                converter = TableSite.GetConverter(_siteRepository);
            }
            else if (StringUtils.ContainsIgnoreCase(TableSiteLog.OldTableNames, oldTableName))
            {
                converter = TableSiteLog.GetConverter(_siteLogRepository);
            }
            else if (StringUtils.ContainsIgnoreCase(TableSitePermissions.OldTableNames, oldTableName))
            {
                converter = TableSitePermissions.GetConverter(_permissionRepository);
            }
            else if (StringUtils.EqualsIgnoreCase(TableTableStyle.OldTableName, oldTableName))
            {
                converter = TableTableStyle.GetConverter(_tableStyleRepository, _siteRepository, _channelRepository);
            }
            else if (StringUtils.EqualsIgnoreCase(TableTableStyleItem.OldTableName, oldTableName))
            {
                converter = TableTableStyleItem.GetConverter(_tableStyleItemRepository);
            }
            else if (StringUtils.EqualsIgnoreCase(TableTag.OldTableName, oldTableName))
            {
                converter = TableTag.GetConverter(_tagRepository);
            }
            else if (StringUtils.ContainsIgnoreCase(TableTemplate.OldTableNames, oldTableName))
            {
                converter = TableTemplate.GetConverter(_templateRepository);
            }
            else if (StringUtils.ContainsIgnoreCase(TableTemplateLog.OldTableNames, oldTableName))
            {
                converter = TableTemplateLog.GetConverter(_templateLogRepository);
            }
            else if (StringUtils.ContainsIgnoreCase(TableTemplateMatch.OldTableNames, oldTableName))
            {
                converter = TableTemplateMatch.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableUser.OldTableName, oldTableName))
            {
                converter = TableUser.GetConverter(_userRepository);
            }
            else if (StringUtils.EqualsIgnoreCase(TableUserLog.OldTableName, oldTableName))
            {
                converter = TableUserLog.GetConverter(_userLogRepository);
            }
            else if (StringUtils.EqualsIgnoreCase(TableGovInteractChannel.OldTableName, oldTableName))
            {
                converter = TableGovInteractChannel.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableGovInteractLog.OldTableName, oldTableName))
            {
                converter = TableGovInteractLog.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableGovInteractPermissions.OldTableName, oldTableName))
            {
                converter = TableGovInteractPermissions.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableGovInteractRemark.OldTableName, oldTableName))
            {
                converter = TableGovInteractRemark.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableGovInteractReply.OldTableName, oldTableName))
            {
                converter = TableGovInteractReply.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableGovInteractType.OldTableName, oldTableName))
            {
                converter = TableGovInteractType.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableGovPublicCategory.OldTableName, oldTableName))
            {
                converter = TableGovPublicCategory.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableGovPublicCategoryClass.OldTableName, oldTableName))
            {
                converter = TableGovPublicCategoryClass.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableGovPublicIdentifierRule.OldTableName, oldTableName))
            {
                converter = TableGovPublicIdentifierRule.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableGovPublicIdentifierSeq.OldTableName, oldTableName))
            {
                converter = TableGovPublicIdentifierSeq.Converter;
            }
            else if (StringUtils.ContainsIgnoreCase(tableNameListForGovPublic, oldTableName))
            {
                converter = TableGovPublicContent.GetConverter(oldTableInfo.Columns);
            }
            else if (StringUtils.ContainsIgnoreCase(tableNameListForGovInteract, oldTableName))
            {
                converter = TableGovInteractContent.GetConverter(oldTableInfo.Columns);
            }
            else if (StringUtils.ContainsIgnoreCase(tableNameListForJob, oldTableName))
            {
                converter = TableJobsContent.GetConverter(oldTableInfo.Columns);
            }

            return await GetNewTableInfoAsync(oldTableName, oldTableInfo, converter);
        }
    }
}
