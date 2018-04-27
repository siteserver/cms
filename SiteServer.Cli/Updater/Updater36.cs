using System.Collections.Generic;
using SiteServer.Cli.Core;
using SiteServer.CMS.Core;
using SiteServer.Utils;
using SiteServer.Cli.Updater.Model36;

namespace SiteServer.Cli.Updater
{
    public class Updater36 : UpdaterBase
    {
        public const string Version = "3.6";

        public Updater36(TreeInfo oldTreeInfo, TreeInfo newTreeInfo) : base(oldTreeInfo, newTreeInfo)
        {

        }

        public override KeyValuePair<string, TableInfo> UpdateTableInfo(string oldTableName, TableInfo oldTableInfo, List<string> contentTableNameList)
        {
            string newTableName = null;
            List<TableColumnInfo> newColumns = null;
            Dictionary<string, string> convertDict = null;

            if (StringUtils.ContainsIgnoreCase(contentTableNameList, oldTableName))
            {
                newTableName = oldTableName;
                newColumns = TableContent.GetNewColumns(oldTableInfo.Columns);
                convertDict = TableContent.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableAdministrator.OldTableName))
            {
                newTableName = TableAdministrator.NewTableName;
                newColumns = TableAdministrator.NewColumns;
                convertDict = TableAdministrator.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableAdministratorsInRoles.OldTableName))
            {
                newTableName = TableAdministratorsInRoles.NewTableName;
                newColumns = TableAdministratorsInRoles.NewColumns;
                convertDict = TableAdministratorsInRoles.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableArea.OldTableName))
            {
                newTableName = TableArea.NewTableName;
                newColumns = TableArea.NewColumns;
                convertDict = TableArea.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableChannel.OldTableName))
            {
                newTableName = TableChannel.NewTableName;
                newColumns = TableChannel.NewColumns;
                convertDict = TableChannel.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableChannelGroup.OldTableName))
            {
                newTableName = TableChannelGroup.NewTableName;
                newColumns = TableChannelGroup.NewColumns;
                convertDict = TableChannelGroup.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableConfig.OldTableName))
            {
                newTableName = TableConfig.NewTableName;
                newColumns = TableConfig.NewColumns;
                convertDict = TableConfig.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableContentCheck.OldTableName))
            {
                newTableName = TableContentCheck.NewTableName;
                newColumns = TableContentCheck.NewColumns;
                convertDict = TableContentCheck.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableContentGroup.OldTableName))
            {
                newTableName = TableContentGroup.NewTableName;
                newColumns = TableContentGroup.NewColumns;
                convertDict = TableContentGroup.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableCount.OldTableName))
            {
                newTableName = TableCount.NewTableName;
                newColumns = TableCount.NewColumns;
                convertDict = TableCount.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableDbCache.OldTableName))
            {
                newTableName = TableDbCache.NewTableName;
                newColumns = TableDbCache.NewColumns;
                convertDict = TableDbCache.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableDepartment.OldTableName))
            {
                newTableName = TableDepartment.NewTableName;
                newColumns = TableDepartment.NewColumns;
                convertDict = TableDepartment.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableKeyword.OldTableName))
            {
                newTableName = TableKeyword.NewTableName;
                newColumns = TableKeyword.NewColumns;
                convertDict = TableKeyword.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableLog.OldTableName))
            {
                newTableName = TableLog.NewTableName;
                newColumns = TableLog.NewColumns;
                convertDict = TableLog.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TablePermissionsInRoles.OldTableName))
            {
                newTableName = TablePermissionsInRoles.NewTableName;
                newColumns = TablePermissionsInRoles.NewColumns;
                convertDict = TablePermissionsInRoles.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableRelatedField.OldTableName))
            {
                newTableName = TableRelatedField.NewTableName;
                newColumns = TableRelatedField.NewColumns;
                convertDict = TableRelatedField.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableRelatedFieldItem.OldTableName))
            {
                newTableName = TableRelatedFieldItem.NewTableName;
                newColumns = TableRelatedFieldItem.NewColumns;
                convertDict = TableRelatedFieldItem.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableRole.OldTableName))
            {
                newTableName = TableRole.NewTableName;
                newColumns = TableRole.NewColumns;
                convertDict = TableRole.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableSite.OldTableName))
            {
                newTableName = TableSite.NewTableName;
                newColumns = TableSite.NewColumns;
                convertDict = TableSite.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableSiteLog.OldTableName))
            {
                newTableName = TableSiteLog.NewTableName;
                newColumns = TableSiteLog.NewColumns;
                convertDict = TableSiteLog.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableSitePermissions.OldTableName))
            {
                newTableName = TableSitePermissions.NewTableName;
                newColumns = TableSitePermissions.NewColumns;
                convertDict = TableSitePermissions.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableTable.OldTableName))
            {
                newTableName = TableTable.NewTableName;
                newColumns = TableTable.NewColumns;
                convertDict = TableTable.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableTableMatch.OldTableName))
            {
                newTableName = TableTableMatch.NewTableName;
                newColumns = TableTableMatch.NewColumns;
                convertDict = TableTableMatch.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableTableMetadata.OldTableName))
            {
                newTableName = TableTableMetadata.NewTableName;
                newColumns = TableTableMetadata.NewColumns;
                convertDict = TableTableMetadata.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableTableStyle.OldTableName))
            {
                newTableName = TableTableStyle.NewTableName;
                newColumns = TableTableStyle.NewColumns;
                convertDict = TableTableStyle.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableTableStyleItem.OldTableName))
            {
                newTableName = TableTableStyleItem.NewTableName;
                newColumns = TableTableStyleItem.NewColumns;
                convertDict = TableTableStyleItem.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableTag.OldTableName))
            {
                newTableName = TableTag.NewTableName;
                newColumns = TableTag.NewColumns;
                convertDict = TableTag.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableTemplate.OldTableName))
            {
                newTableName = TableTemplate.NewTableName;
                newColumns = TableTemplate.NewColumns;
                convertDict = TableTemplate.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableTemplateMatch.OldTableName))
            {
                newTableName = TableTemplateMatch.NewTableName;
                newColumns = TableTemplateMatch.NewColumns;
                convertDict = TableTemplateMatch.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, TableUser.OldTableName))
            {
                newTableName = TableUser.NewTableName;
                newColumns = TableUser.NewColumns;
                convertDict = TableUser.ConvertDict;
            }

            return GetNewTableInfo(oldTableName, oldTableInfo, newTableName, newColumns, convertDict);
        }
    }
}
