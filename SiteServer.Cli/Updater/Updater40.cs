using System;
using System.Collections.Generic;
using SiteServer.Cli.Core;
using SiteServer.CMS.Core;
using SiteServer.Utils;
using SiteServer.Cli.Updater.Model40;
using SiteServer.Cli.Updater.Plugins.GovInteract;
using SiteServer.Cli.Updater.Plugins.GovPublic;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater
{
    public class Updater40 : UpdaterBase
    {
        public const string Version = "4.0";

        public Updater40(TreeInfo oldTreeInfo, TreeInfo newTreeInfo) : base(oldTreeInfo, newTreeInfo)
        {
        }

        public override KeyValuePair<string, TableInfo> UpdateTableInfo(string oldTableName, TableInfo oldTableInfo, List<string> contentTableNameList)
        {
            string newTableName = null;
            List<TableColumn> newColumns = null;
            Dictionary<string, string> convertKeyDict = null;
            Dictionary<string, string> convertValueDict = null;

            var oldTableNameWithoutPrefix = oldTableName.Substring(oldTableName.IndexOf("_", StringComparison.Ordinal) + 1);

            if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableAdministrator.OldTableName))
            {
                newTableName = TableAdministrator.NewTableName;
                newColumns = TableAdministrator.NewColumns;
                convertKeyDict = TableAdministrator.ConvertKeyDict;
                convertValueDict = TableAdministrator.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableAdministratorsInRoles.OldTableName))
            {
                newTableName = TableAdministratorsInRoles.NewTableName;
                newColumns = TableAdministratorsInRoles.NewColumns;
                convertKeyDict = TableAdministratorsInRoles.ConvertKeyDict;
                convertValueDict = TableAdministratorsInRoles.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableArea.OldTableName))
            {
                newTableName = TableArea.NewTableName;
                newColumns = TableArea.NewColumns;
                convertKeyDict = TableArea.ConvertKeyDict;
                convertValueDict = TableArea.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableChannel.OldTableName))
            {
                newTableName = TableChannel.NewTableName;
                newColumns = TableChannel.NewColumns;
                convertKeyDict = TableChannel.ConvertKeyDict;
                convertValueDict = TableChannel.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableChannelGroup.OldTableName))
            {
                newTableName = TableChannelGroup.NewTableName;
                newColumns = TableChannelGroup.NewColumns;
                convertKeyDict = TableChannelGroup.ConvertKeyDict;
                convertValueDict = TableChannelGroup.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableConfig.OldTableName))
            {
                newTableName = TableConfig.NewTableName;
                newColumns = TableConfig.NewColumns;
                convertKeyDict = TableConfig.ConvertKeyDict;
                convertValueDict = TableConfig.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableContentCheck.OldTableName))
            {
                newTableName = TableContentCheck.NewTableName;
                newColumns = TableContentCheck.NewColumns;
                convertKeyDict = TableContentCheck.ConvertKeyDict;
                convertValueDict = TableContentCheck.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableContentGroup.OldTableName))
            {
                newTableName = TableContentGroup.NewTableName;
                newColumns = TableContentGroup.NewColumns;
                convertKeyDict = TableContentGroup.ConvertKeyDict;
                convertValueDict = TableContentGroup.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableCount.OldTableName))
            {
                newTableName = TableCount.NewTableName;
                newColumns = TableCount.NewColumns;
                convertKeyDict = TableCount.ConvertKeyDict;
                convertValueDict = TableCount.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableDbCache.OldTableName))
            {
                newTableName = TableDbCache.NewTableName;
                newColumns = TableDbCache.NewColumns;
                convertKeyDict = TableDbCache.ConvertKeyDict;
                convertValueDict = TableDbCache.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableDepartment.OldTableName))
            {
                newTableName = TableDepartment.NewTableName;
                newColumns = TableDepartment.NewColumns;
                convertKeyDict = TableDepartment.ConvertKeyDict;
                convertValueDict = TableDepartment.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableErrorLog.OldTableName))
            {
                newTableName = TableErrorLog.NewTableName;
                newColumns = TableErrorLog.NewColumns;
                convertKeyDict = TableErrorLog.ConvertKeyDict;
                convertValueDict = TableErrorLog.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableKeyword.OldTableName))
            {
                newTableName = TableKeyword.NewTableName;
                newColumns = TableKeyword.NewColumns;
                convertKeyDict = TableKeyword.ConvertKeyDict;
                convertValueDict = TableKeyword.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableLog.OldTableName))
            {
                newTableName = TableLog.NewTableName;
                newColumns = TableLog.NewColumns;
                convertKeyDict = TableLog.ConvertKeyDict;
                convertValueDict = TableLog.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TablePermissionsInRoles.OldTableName))
            {
                newTableName = TablePermissionsInRoles.NewTableName;
                newColumns = TablePermissionsInRoles.NewColumns;
                convertKeyDict = TablePermissionsInRoles.ConvertKeyDict;
                convertValueDict = TablePermissionsInRoles.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableRelatedField.OldTableName))
            {
                newTableName = TableRelatedField.NewTableName;
                newColumns = TableRelatedField.NewColumns;
                convertKeyDict = TableRelatedField.ConvertKeyDict;
                convertValueDict = TableRelatedField.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableRelatedFieldItem.OldTableName))
            {
                newTableName = TableRelatedFieldItem.NewTableName;
                newColumns = TableRelatedFieldItem.NewColumns;
                convertKeyDict = TableRelatedFieldItem.ConvertKeyDict;
                convertValueDict = TableRelatedFieldItem.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableRole.OldTableName))
            {
                newTableName = TableRole.NewTableName;
                newColumns = TableRole.NewColumns;
                convertKeyDict = TableRole.ConvertKeyDict;
                convertValueDict = TableRole.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableSite.OldTableName))
            {
                newTableName = TableSite.NewTableName;
                newColumns = TableSite.NewColumns;
                convertKeyDict = TableSite.ConvertKeyDict;
                convertValueDict = TableSite.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableSiteLog.OldTableName))
            {
                newTableName = TableSiteLog.NewTableName;
                newColumns = TableSiteLog.NewColumns;
                convertKeyDict = TableSiteLog.ConvertKeyDict;
                convertValueDict = TableSiteLog.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableSitePermissions.OldTableName))
            {
                newTableName = TableSitePermissions.NewTableName;
                newColumns = TableSitePermissions.NewColumns;
                convertKeyDict = TableSitePermissions.ConvertKeyDict;
                convertValueDict = TableSitePermissions.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableTable.OldTableName))
            {
                newTableName = TableTable.NewTableName;
                newColumns = TableTable.NewColumns;
                convertKeyDict = TableTable.ConvertKeyDict;
                convertValueDict = TableTable.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableTableMetadata.OldTableName))
            {
                newTableName = TableTableMetadata.NewTableName;
                newColumns = TableTableMetadata.NewColumns;
                convertKeyDict = TableTableMetadata.ConvertKeyDict;
                convertValueDict = TableTableMetadata.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableTableStyle.OldTableName))
            {
                newTableName = TableTableStyle.NewTableName;
                newColumns = TableTableStyle.NewColumns;
                convertKeyDict = TableTableStyle.ConvertKeyDict;
                convertValueDict = TableTableStyle.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableTableStyleItem.OldTableName))
            {
                newTableName = TableTableStyleItem.NewTableName;
                newColumns = TableTableStyleItem.NewColumns;
                convertKeyDict = TableTableStyleItem.ConvertKeyDict;
                convertValueDict = TableTableStyleItem.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableTag.OldTableName))
            {
                newTableName = TableTag.NewTableName;
                newColumns = TableTag.NewColumns;
                convertKeyDict = TableTag.ConvertKeyDict;
                convertValueDict = TableTag.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableTemplate.OldTableName))
            {
                newTableName = TableTemplate.NewTableName;
                newColumns = TableTemplate.NewColumns;
                convertKeyDict = TableTemplate.ConvertKeyDict;
                convertValueDict = TableTemplate.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableTemplateLog.OldTableName))
            {
                newTableName = TableTemplateLog.NewTableName;
                newColumns = TableTemplateLog.NewColumns;
                convertKeyDict = TableTemplateLog.ConvertKeyDict;
                convertValueDict = TableTemplateLog.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableTemplateMatch.OldTableName))
            {
                newTableName = TableTemplateMatch.NewTableName;
                newColumns = TableTemplateMatch.NewColumns;
                convertKeyDict = TableTemplateMatch.ConvertKeyDict;
                convertValueDict = TableTemplateMatch.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableUser.OldTableName))
            {
                newTableName = TableUser.NewTableName;
                newColumns = TableUser.NewColumns;
                convertKeyDict = TableUser.ConvertKeyDict;
                convertValueDict = TableUser.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableGovInteractChannel.OldTableName))
            {
                newTableName = TableGovInteractChannel.NewTableName;
                newColumns = TableGovInteractChannel.NewColumns;
                convertKeyDict = TableGovInteractChannel.ConvertKeyDict;
                convertValueDict = TableGovInteractChannel.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableGovInteractLog.OldTableName))
            {
                newTableName = TableGovInteractLog.NewTableName;
                newColumns = TableGovInteractLog.NewColumns;
                convertKeyDict = TableGovInteractLog.ConvertKeyDict;
                convertValueDict = TableGovInteractLog.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableGovInteractPermissions.OldTableName))
            {
                newTableName = TableGovInteractPermissions.NewTableName;
                newColumns = TableGovInteractPermissions.NewColumns;
                convertKeyDict = TableGovInteractPermissions.ConvertKeyDict;
                convertValueDict = TableGovInteractPermissions.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableGovInteractRemark.OldTableName))
            {
                newTableName = TableGovInteractRemark.NewTableName;
                newColumns = TableGovInteractRemark.NewColumns;
                convertKeyDict = TableGovInteractRemark.ConvertKeyDict;
                convertValueDict = TableGovInteractRemark.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableGovInteractReply.OldTableName))
            {
                newTableName = TableGovInteractReply.NewTableName;
                newColumns = TableGovInteractReply.NewColumns;
                convertKeyDict = TableGovInteractReply.ConvertKeyDict;
                convertValueDict = TableGovInteractReply.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableGovInteractType.OldTableName))
            {
                newTableName = TableGovInteractType.NewTableName;
                newColumns = TableGovInteractType.NewColumns;
                convertKeyDict = TableGovInteractType.ConvertKeyDict;
                convertValueDict = TableGovInteractType.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableGovPublicCategory.OldTableName))
            {
                newTableName = TableGovPublicCategory.NewTableName;
                newColumns = TableGovPublicCategory.NewColumns;
                convertKeyDict = TableGovPublicCategory.ConvertKeyDict;
                convertValueDict = TableGovPublicCategory.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableGovPublicCategoryClass.OldTableName))
            {
                newTableName = TableGovPublicCategoryClass.NewTableName;
                newColumns = TableGovPublicCategoryClass.NewColumns;
                convertKeyDict = TableGovPublicCategoryClass.ConvertKeyDict;
                convertValueDict = TableGovPublicCategoryClass.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableGovPublicIdentifierRule.OldTableName))
            {
                newTableName = TableGovPublicIdentifierRule.NewTableName;
                newColumns = TableGovPublicIdentifierRule.NewColumns;
                convertKeyDict = TableGovPublicIdentifierRule.ConvertKeyDict;
                convertValueDict = TableGovPublicIdentifierRule.ConvertValueDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableGovPublicIdentifierSeq.OldTableName))
            {
                newTableName = TableGovPublicIdentifierSeq.NewTableName;
                newColumns = TableGovPublicIdentifierSeq.NewColumns;
                convertKeyDict = TableGovPublicIdentifierSeq.ConvertKeyDict;
                convertValueDict = TableGovPublicIdentifierSeq.ConvertValueDict;
            }
            else if (StringUtils.ContainsIgnoreCase(contentTableNameList, oldTableName))
            {
                newTableName = UpdateUtils.GetContentTableName(oldTableName);
                newColumns = TableContent.GetNewColumns(oldTableInfo.Columns);
                convertKeyDict = TableContent.ConvertKeyDict;
                convertValueDict = TableContent.ConvertValueDict;
            }

            return GetNewTableInfo(oldTableName, oldTableInfo, newTableName, newColumns, convertKeyDict, convertValueDict);
        }
    }
}
