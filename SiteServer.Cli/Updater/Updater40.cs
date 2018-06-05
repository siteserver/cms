using System;
using System.Collections.Generic;
using SiteServer.Cli.Core;
using SiteServer.CMS.Core;
using SiteServer.Utils;
using SiteServer.Cli.Updater.Model40;
using SiteServer.Cli.Updater.Plugins.GovInteract;
using SiteServer.Cli.Updater.Plugins.GovPublic;

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
            List<TableColumnInfo> newColumns = null;
            Dictionary<string, string> convertDict = null;

            var oldTableNameWithoutPrefix = oldTableName.Substring(oldTableName.IndexOf("_", StringComparison.Ordinal) + 1);

            if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableAdministrator.OldTableName))
            {
                newTableName = TableAdministrator.NewTableName;
                newColumns = TableAdministrator.NewColumns;
                convertDict = TableAdministrator.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableAdministratorsInRoles.OldTableName))
            {
                newTableName = TableAdministratorsInRoles.NewTableName;
                newColumns = TableAdministratorsInRoles.NewColumns;
                convertDict = TableAdministratorsInRoles.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableArea.OldTableName))
            {
                newTableName = TableArea.NewTableName;
                newColumns = TableArea.NewColumns;
                convertDict = TableArea.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableChannel.OldTableName))
            {
                newTableName = TableChannel.NewTableName;
                newColumns = TableChannel.NewColumns;
                convertDict = TableChannel.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableChannelGroup.OldTableName))
            {
                newTableName = TableChannelGroup.NewTableName;
                newColumns = TableChannelGroup.NewColumns;
                convertDict = TableChannelGroup.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableConfig.OldTableName))
            {
                newTableName = TableConfig.NewTableName;
                newColumns = TableConfig.NewColumns;
                convertDict = TableConfig.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableContentCheck.OldTableName))
            {
                newTableName = TableContentCheck.NewTableName;
                newColumns = TableContentCheck.NewColumns;
                convertDict = TableContentCheck.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableContentGroup.OldTableName))
            {
                newTableName = TableContentGroup.NewTableName;
                newColumns = TableContentGroup.NewColumns;
                convertDict = TableContentGroup.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableCount.OldTableName))
            {
                newTableName = TableCount.NewTableName;
                newColumns = TableCount.NewColumns;
                convertDict = TableCount.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableDbCache.OldTableName))
            {
                newTableName = TableDbCache.NewTableName;
                newColumns = TableDbCache.NewColumns;
                convertDict = TableDbCache.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableDepartment.OldTableName))
            {
                newTableName = TableDepartment.NewTableName;
                newColumns = TableDepartment.NewColumns;
                convertDict = TableDepartment.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableErrorLog.OldTableName))
            {
                newTableName = TableErrorLog.NewTableName;
                newColumns = TableErrorLog.NewColumns;
                convertDict = TableErrorLog.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableKeyword.OldTableName))
            {
                newTableName = TableKeyword.NewTableName;
                newColumns = TableKeyword.NewColumns;
                convertDict = TableKeyword.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableLog.OldTableName))
            {
                newTableName = TableLog.NewTableName;
                newColumns = TableLog.NewColumns;
                convertDict = TableLog.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TablePermissionsInRoles.OldTableName))
            {
                newTableName = TablePermissionsInRoles.NewTableName;
                newColumns = TablePermissionsInRoles.NewColumns;
                convertDict = TablePermissionsInRoles.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableRelatedField.OldTableName))
            {
                newTableName = TableRelatedField.NewTableName;
                newColumns = TableRelatedField.NewColumns;
                convertDict = TableRelatedField.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableRelatedFieldItem.OldTableName))
            {
                newTableName = TableRelatedFieldItem.NewTableName;
                newColumns = TableRelatedFieldItem.NewColumns;
                convertDict = TableRelatedFieldItem.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableRole.OldTableName))
            {
                newTableName = TableRole.NewTableName;
                newColumns = TableRole.NewColumns;
                convertDict = TableRole.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableSite.OldTableName))
            {
                newTableName = TableSite.NewTableName;
                newColumns = TableSite.NewColumns;
                convertDict = TableSite.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableSiteLog.OldTableName))
            {
                newTableName = TableSiteLog.NewTableName;
                newColumns = TableSiteLog.NewColumns;
                convertDict = TableSiteLog.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableSitePermissions.OldTableName))
            {
                newTableName = TableSitePermissions.NewTableName;
                newColumns = TableSitePermissions.NewColumns;
                convertDict = TableSitePermissions.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableTable.OldTableName))
            {
                newTableName = TableTable.NewTableName;
                newColumns = TableTable.NewColumns;
                convertDict = TableTable.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableTableMetadata.OldTableName))
            {
                newTableName = TableTableMetadata.NewTableName;
                newColumns = TableTableMetadata.NewColumns;
                convertDict = TableTableMetadata.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableTableStyle.OldTableName))
            {
                newTableName = TableTableStyle.NewTableName;
                newColumns = TableTableStyle.NewColumns;
                convertDict = TableTableStyle.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableTableStyleItem.OldTableName))
            {
                newTableName = TableTableStyleItem.NewTableName;
                newColumns = TableTableStyleItem.NewColumns;
                convertDict = TableTableStyleItem.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableTag.OldTableName))
            {
                newTableName = TableTag.NewTableName;
                newColumns = TableTag.NewColumns;
                convertDict = TableTag.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableTemplate.OldTableName))
            {
                newTableName = TableTemplate.NewTableName;
                newColumns = TableTemplate.NewColumns;
                convertDict = TableTemplate.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableTemplateLog.OldTableName))
            {
                newTableName = TableTemplateLog.NewTableName;
                newColumns = TableTemplateLog.NewColumns;
                convertDict = TableTemplateLog.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableTemplateMatch.OldTableName))
            {
                newTableName = TableTemplateMatch.NewTableName;
                newColumns = TableTemplateMatch.NewColumns;
                convertDict = TableTemplateMatch.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableUser.OldTableName))
            {
                newTableName = TableUser.NewTableName;
                newColumns = TableUser.NewColumns;
                convertDict = TableUser.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableGovInteractChannel.OldTableName))
            {
                newTableName = TableGovInteractChannel.NewTableName;
                newColumns = TableGovInteractChannel.NewColumns;
                convertDict = TableGovInteractChannel.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableGovInteractLog.OldTableName))
            {
                newTableName = TableGovInteractLog.NewTableName;
                newColumns = TableGovInteractLog.NewColumns;
                convertDict = TableGovInteractLog.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableGovInteractPermissions.OldTableName))
            {
                newTableName = TableGovInteractPermissions.NewTableName;
                newColumns = TableGovInteractPermissions.NewColumns;
                convertDict = TableGovInteractPermissions.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableGovInteractRemark.OldTableName))
            {
                newTableName = TableGovInteractRemark.NewTableName;
                newColumns = TableGovInteractRemark.NewColumns;
                convertDict = TableGovInteractRemark.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableGovInteractReply.OldTableName))
            {
                newTableName = TableGovInteractReply.NewTableName;
                newColumns = TableGovInteractReply.NewColumns;
                convertDict = TableGovInteractReply.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableGovInteractType.OldTableName))
            {
                newTableName = TableGovInteractType.NewTableName;
                newColumns = TableGovInteractType.NewColumns;
                convertDict = TableGovInteractType.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableGovPublicCategory.OldTableName))
            {
                newTableName = TableGovPublicCategory.NewTableName;
                newColumns = TableGovPublicCategory.NewColumns;
                convertDict = TableGovPublicCategory.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableGovPublicCategoryClass.OldTableName))
            {
                newTableName = TableGovPublicCategoryClass.NewTableName;
                newColumns = TableGovPublicCategoryClass.NewColumns;
                convertDict = TableGovPublicCategoryClass.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableGovPublicIdentifierRule.OldTableName))
            {
                newTableName = TableGovPublicIdentifierRule.NewTableName;
                newColumns = TableGovPublicIdentifierRule.NewColumns;
                convertDict = TableGovPublicIdentifierRule.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableNameWithoutPrefix, TableGovPublicIdentifierSeq.OldTableName))
            {
                newTableName = TableGovPublicIdentifierSeq.NewTableName;
                newColumns = TableGovPublicIdentifierSeq.NewColumns;
                convertDict = TableGovPublicIdentifierSeq.ConvertDict;
            }
            else if (StringUtils.ContainsIgnoreCase(contentTableNameList, oldTableName))
            {
                newTableName = UpdateUtils.GetContentTableName(oldTableName);
                newColumns = TableContent.GetNewColumns(oldTableInfo.Columns);
                convertDict = TableContent.ConvertDict;
            }

            return GetNewTableInfo(oldTableName, oldTableInfo, newTableName, newColumns, convertDict);
        }
    }
}
