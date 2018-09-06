using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SiteServer.Cli.Core;
using SiteServer.Cli.Updater.Model4;
using SiteServer.Utils;
using SiteServer.Cli.Updater.Plugins.GovInteract;
using SiteServer.Cli.Updater.Plugins.GovPublic;
using SiteServer.Cli.Updater.Plugins.Jobs;

namespace SiteServer.Cli.Updater
{
    public class Updater4 : UpdaterBase
    {
        public const string Version = "4";

        public Updater4(TreeInfo oldTreeInfo, TreeInfo newTreeInfo) : base(oldTreeInfo, newTreeInfo)
        {
            
        }

        public override async Task<Tuple<string, TableInfo>> UpdateTableInfoAsync(string oldTableName, TableInfo oldTableInfo, List<string> tableNameListForContent, List<string> tableNameListForGovPublic, List<string> tableNameListForGovInteract, List<string> tableNameListForJob)
        {
            ConvertInfo converter = null;

            if (StringUtils.EqualsIgnoreCase(TableAdministrator.OldTableName, oldTableName))
            {
                converter = TableAdministrator.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableAdministratorsInRoles.OldTableName, oldTableName))
            {
                converter = TableAdministratorsInRoles.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableArea.OldTableName, oldTableName))
            {
                converter = TableArea.Converter;
            }
            else if (StringUtils.ContainsIgnoreCase(TableChannel.OldTableNames, oldTableName))
            {
                converter = TableChannel.Converter;
            }
            else if (StringUtils.ContainsIgnoreCase(TableChannelGroup.OldTableNames, oldTableName))
            {
                converter = TableChannelGroup.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableConfig.OldTableName, oldTableName))
            {
                converter = TableConfig.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableContentCheck.OldTableName, oldTableName))
            {
                converter = TableContentCheck.Converter;
            }
            else if (StringUtils.ContainsIgnoreCase(TableContentGroup.OldTableNames, oldTableName))
            {
                converter = TableContentGroup.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableDbCache.OldTableName, oldTableName))
            {
                converter = TableDbCache.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableDepartment.OldTableName, oldTableName))
            {
                converter = TableDepartment.Converter;
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
                converter = TableLog.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TablePermissionsInRoles.OldTableName, oldTableName))
            {
                converter = TablePermissionsInRoles.Converter;
            }
            else if (StringUtils.ContainsIgnoreCase(TableRelatedField.OldTableNames, oldTableName))
            {
                converter = TableRelatedField.Converter;
            }
            else if (StringUtils.ContainsIgnoreCase(TableRelatedFieldItem.OldTableNames, oldTableName))
            {
                converter = TableRelatedFieldItem.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableRole.OldTableName, oldTableName))
            {
                converter = TableRole.Converter;
            }
            else if (StringUtils.ContainsIgnoreCase(TableSite.OldTableNames, oldTableName))
            {
                converter = TableSite.Converter;
            }
            else if (StringUtils.ContainsIgnoreCase(TableSiteLog.OldTableNames, oldTableName))
            {
                converter = TableSiteLog.Converter;
            }
            else if (StringUtils.ContainsIgnoreCase(TableSitePermissions.OldTableNames, oldTableName))
            {
                converter = TableSitePermissions.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableTable.OldTableName, oldTableName))
            {
                converter = TableTable.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableTableMetadata.OldTableName, oldTableName))
            {
                converter = TableTableMetadata.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableTableStyle.OldTableName, oldTableName))
            {
                converter = TableTableStyle.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableTableStyleItem.OldTableName, oldTableName))
            {
                converter = TableTableStyleItem.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableTag.OldTableName, oldTableName))
            {
                converter = TableTag.Converter;
            }
            else if (StringUtils.ContainsIgnoreCase(TableTemplate.OldTableNames, oldTableName))
            {
                converter = TableTemplate.Converter;
            }
            else if (StringUtils.ContainsIgnoreCase(TableTemplateLog.OldTableNames, oldTableName))
            {
                converter = TableTemplateLog.Converter;
            }
            else if (StringUtils.ContainsIgnoreCase(TableTemplateMatch.OldTableNames, oldTableName))
            {
                converter = TableTemplateMatch.Converter;
            }
            else if (StringUtils.EqualsIgnoreCase(TableUser.OldTableName, oldTableName))
            {
                converter = TableUser.Converter;
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
            else if (StringUtils.ContainsIgnoreCase(tableNameListForContent, oldTableName))
            {
                converter = TableContent.GetConverter(oldTableName, oldTableInfo.Columns);
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
