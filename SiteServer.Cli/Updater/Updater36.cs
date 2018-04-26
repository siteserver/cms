using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using SiteServer.Cli.Core;
using SiteServer.Cli.Updater.Model36;
using SiteServer.CMS.Model;
using SiteServer.Utils;
using TableInfo = SiteServer.Cli.Core.TableInfo;

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
                newColumns = SiteServerContent.GetNewColumns(oldTableInfo.Columns);
                convertDict = SiteServerContent.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_Administrator"))
            {
                newTableName = BairongAdministrator.NewTableName;
                newColumns = BairongAdministrator.NewColumns;
                convertDict = BairongAdministrator.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_AdministratorsInRoles"))
            {
                newTableName = BairongAdministratorsInRoles.NewTableName;
                newColumns = BairongAdministratorsInRoles.NewColumns;
                convertDict = BairongAdministratorsInRoles.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_Area"))
            {
                newTableName = BairongArea.NewTableName;
                newColumns = BairongArea.NewColumns;
                convertDict = BairongArea.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_Cache"))
            {
                newTableName = BairongCache.NewTableName;
                newColumns = BairongCache.NewColumns;
                convertDict = BairongCache.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_CloudStorage"))
            {
                newTableName = BairongCloudStorage.NewTableName;
                newColumns = BairongCloudStorage.NewColumns;
                convertDict = BairongCloudStorage.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_Config"))
            {
                newTableName = BairongConfig.NewTableName;
                newColumns = BairongConfig.NewColumns;
                convertDict = BairongConfig.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_ContentCheck"))
            {
                newTableName = BairongContentCheck.NewTableName;
                newColumns = BairongContentCheck.NewColumns;
                convertDict = BairongContentCheck.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_ContentModel"))
            {
                newTableName = BairongContentModel.NewTableName;
                newColumns = BairongContentModel.NewColumns;
                convertDict = BairongContentModel.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_Count"))
            {
                newTableName = BairongCount.NewTableName;
                newColumns = BairongCount.NewColumns;
                convertDict = BairongCount.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_Department"))
            {
                newTableName = BairongDepartment.NewTableName;
                newColumns = BairongDepartment.NewColumns;
                convertDict = BairongDepartment.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_Digg"))
            {
                newTableName = BairongDigg.NewTableName;
                newColumns = BairongDigg.NewColumns;
                convertDict = BairongDigg.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_FTPStorage"))
            {
                newTableName = BairongFtpStorage.NewTableName;
                newColumns = BairongFtpStorage.NewColumns;
                convertDict = BairongFtpStorage.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_IP2City"))
            {
                newTableName = BairongIp2City.NewTableName;
                newColumns = BairongIp2City.NewColumns;
                convertDict = BairongIp2City.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_LocalStorage"))
            {
                newTableName = BairongLocalStorage.NewTableName;
                newColumns = BairongLocalStorage.NewColumns;
                convertDict = BairongLocalStorage.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_Log"))
            {
                newTableName = BairongLog.NewTableName;
                newColumns = BairongLog.NewColumns;
                convertDict = BairongLog.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_Module"))
            {
                newTableName = BairongModule.NewTableName;
                newColumns = BairongModule.NewColumns;
                convertDict = BairongModule.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_PermissionsInRoles"))
            {
                newTableName = BairongPermissionsInRoles.NewTableName;
                newColumns = BairongPermissionsInRoles.NewColumns;
                convertDict = BairongPermissionsInRoles.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_Roles"))
            {
                newTableName = BairongRoles.NewTableName;
                newColumns = BairongRoles.NewColumns;
                convertDict = BairongRoles.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_SMSMessages"))
            {
                newTableName = BairongSmsMessages.NewTableName;
                newColumns = BairongSmsMessages.NewColumns;
                convertDict = BairongSmsMessages.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_SSOApp"))
            {
                newTableName = BairongSsoApp.NewTableName;
                newColumns = BairongSsoApp.NewColumns;
                convertDict = BairongSsoApp.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_Storage"))
            {
                newTableName = BairongStorage.NewTableName;
                newColumns = BairongStorage.NewColumns;
                convertDict = BairongStorage.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_TableCollection"))
            {
                newTableName = BairongTableCollection.NewTableName;
                newColumns = BairongTableCollection.NewColumns;
                convertDict = BairongTableCollection.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_TableMatch"))
            {
                newTableName = BairongTableMatch.NewTableName;
                newColumns = BairongTableMatch.NewColumns;
                convertDict = BairongTableMatch.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_TableMetadata"))
            {
                newTableName = BairongTableMetadata.NewTableName;
                newColumns = BairongTableMetadata.NewColumns;
                convertDict = BairongTableMetadata.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_TableStyle"))
            {
                newTableName = BairongTableStyle.NewTableName;
                newColumns = BairongTableStyle.NewColumns;
                convertDict = BairongTableStyle.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_TableStyleItem"))
            {
                newTableName = BairongTableStyleItem.NewTableName;
                newColumns = BairongTableStyleItem.NewColumns;
                convertDict = BairongTableStyleItem.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_Tags"))
            {
                newTableName = BairongTags.NewTableName;
                newColumns = BairongTags.NewColumns;
                convertDict = BairongTags.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_Task"))
            {
                newTableName = BairongTask.NewTableName;
                newColumns = BairongTask.NewColumns;
                convertDict = BairongTask.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_TaskLog"))
            {
                newTableName = BairongTaskLog.NewTableName;
                newColumns = BairongTaskLog.NewColumns;
                convertDict = BairongTaskLog.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_UserBinding"))
            {
                newTableName = BairongUserBinding.NewTableName;
                newColumns = BairongUserBinding.NewColumns;
                convertDict = BairongUserBinding.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_UserConfig"))
            {
                newTableName = BairongUserConfig.NewTableName;
                newColumns = BairongUserConfig.NewColumns;
                convertDict = BairongUserConfig.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_UserContact"))
            {
                newTableName = BairongUserContact.NewTableName;
                newColumns = BairongUserContact.NewColumns;
                convertDict = BairongUserContact.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_UserCreditsLog"))
            {
                newTableName = BairongUserCreditsLog.NewTableName;
                newColumns = BairongUserCreditsLog.NewColumns;
                convertDict = BairongUserCreditsLog.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_UserDownload"))
            {
                newTableName = BairongUserDownload.NewTableName;
                newColumns = BairongUserDownload.NewColumns;
                convertDict = BairongUserDownload.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_UserMessage"))
            {
                newTableName = BairongUserMessage.NewTableName;
                newColumns = BairongUserMessage.NewColumns;
                convertDict = BairongUserMessage.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_Users"))
            {
                newTableName = BairongUsers.NewTableName;
                newColumns = BairongUsers.NewColumns;
                convertDict = BairongUsers.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "bairong_UserType"))
            {
                newTableName = BairongUserType.NewTableName;
                newColumns = BairongUserType.NewColumns;
                convertDict = BairongUserType.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_Ad"))
            {
                newTableName = SiteserverAd.NewTableName;
                newColumns = SiteserverAd.NewColumns;
                convertDict = SiteserverAd.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_Advertisement"))
            {
                newTableName = SiteserverAdvertisement.NewTableName;
                newColumns = SiteserverAdvertisement.NewColumns;
                convertDict = SiteserverAdvertisement.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_Comment"))
            {
                newTableName = SiteserverComment.NewTableName;
                newColumns = SiteserverComment.NewColumns;
                convertDict = SiteserverComment.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_CommentContent"))
            {
                newTableName = SiteserverCommentContent.NewTableName;
                newColumns = SiteserverCommentContent.NewColumns;
                convertDict = SiteserverCommentContent.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_Configuration"))
            {
                newTableName = SiteserverConfiguration.NewTableName;
                newColumns = SiteserverConfiguration.NewColumns;
                convertDict = SiteserverConfiguration.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_ContentGroup"))
            {
                newTableName = SiteserverContentGroup.NewTableName;
                newColumns = SiteserverContentGroup.NewColumns;
                convertDict = SiteserverContentGroup.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_GatherDatabaseRule"))
            {
                newTableName = SiteserverGatherDatabaseRule.NewTableName;
                newColumns = SiteserverGatherDatabaseRule.NewColumns;
                convertDict = SiteserverGatherDatabaseRule.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_GatherFileRule"))
            {
                newTableName = SiteserverGatherFileRule.NewTableName;
                newColumns = SiteserverGatherFileRule.NewColumns;
                convertDict = SiteserverGatherFileRule.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_GatherRule"))
            {
                newTableName = SiteserverGatherRule.NewTableName;
                newColumns = SiteserverGatherRule.NewColumns;
                convertDict = SiteserverGatherRule.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_GovInteractChannel"))
            {
                newTableName = SiteserverGovInteractChannel.NewTableName;
                newColumns = SiteserverGovInteractChannel.NewColumns;
                convertDict = SiteserverGovInteractChannel.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_GovInteractLog"))
            {
                newTableName = SiteserverGovInteractLog.NewTableName;
                newColumns = SiteserverGovInteractLog.NewColumns;
                convertDict = SiteserverGovInteractLog.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_GovInteractPermissions"))
            {
                newTableName = SiteserverGovInteractPermissions.NewTableName;
                newColumns = SiteserverGovInteractPermissions.NewColumns;
                convertDict = SiteserverGovInteractPermissions.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_GovInteractRemark"))
            {
                newTableName = SiteserverGovInteractRemark.NewTableName;
                newColumns = SiteserverGovInteractRemark.NewColumns;
                convertDict = SiteserverGovInteractRemark.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_GovInteractReply"))
            {
                newTableName = SiteserverGovInteractReply.NewTableName;
                newColumns = SiteserverGovInteractReply.NewColumns;
                convertDict = SiteserverGovInteractReply.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_GovInteractType"))
            {
                newTableName = SiteserverGovInteractType.NewTableName;
                newColumns = SiteserverGovInteractType.NewColumns;
                convertDict = SiteserverGovInteractType.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_GovPublicApply"))
            {
                newTableName = SiteserverGovPublicApply.NewTableName;
                newColumns = SiteserverGovPublicApply.NewColumns;
                convertDict = SiteserverGovPublicApply.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_GovPublicApplyLog"))
            {
                newTableName = SiteserverGovPublicApplyLog.NewTableName;
                newColumns = SiteserverGovPublicApplyLog.NewColumns;
                convertDict = SiteserverGovPublicApplyLog.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_GovPublicApplyRemark"))
            {
                newTableName = SiteserverGovPublicApplyRemark.NewTableName;
                newColumns = SiteserverGovPublicApplyRemark.NewColumns;
                convertDict = SiteserverGovPublicApplyRemark.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_GovPublicApplyReply"))
            {
                newTableName = SiteserverGovPublicApplyReply.NewTableName;
                newColumns = SiteserverGovPublicApplyReply.NewColumns;
                convertDict = SiteserverGovPublicApplyReply.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_GovPublicCategory"))
            {
                newTableName = SiteserverGovPublicCategory.NewTableName;
                newColumns = SiteserverGovPublicCategory.NewColumns;
                convertDict = SiteserverGovPublicCategory.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_GovPublicCategoryClass"))
            {
                newTableName = SiteserverGovPublicCategoryClass.NewTableName;
                newColumns = SiteserverGovPublicCategoryClass.NewColumns;
                convertDict = SiteserverGovPublicCategoryClass.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_GovPublicChannel"))
            {
                newTableName = SiteserverGovPublicChannel.NewTableName;
                newColumns = SiteserverGovPublicChannel.NewColumns;
                convertDict = SiteserverGovPublicChannel.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_GovPublicIdentifierRule"))
            {
                newTableName = SiteserverGovPublicIdentifierRule.NewTableName;
                newColumns = SiteserverGovPublicIdentifierRule.NewColumns;
                convertDict = SiteserverGovPublicIdentifierRule.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_GovPublicIdentifierSeq"))
            {
                newTableName = SiteserverGovPublicIdentifierSeq.NewTableName;
                newColumns = SiteserverGovPublicIdentifierSeq.NewColumns;
                convertDict = SiteserverGovPublicIdentifierSeq.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_InnerLink"))
            {
                newTableName = SiteserverInnerLink.NewTableName;
                newColumns = SiteserverInnerLink.NewColumns;
                convertDict = SiteserverInnerLink.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_Input"))
            {
                newTableName = SiteserverInput.NewTableName;
                newColumns = SiteserverInput.NewColumns;
                convertDict = SiteserverInput.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_InputContent"))
            {
                newTableName = SiteserverInputContent.NewTableName;
                newColumns = SiteserverInputContent.NewColumns;
                convertDict = SiteserverInputContent.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_Keyword"))
            {
                newTableName = SiteserverKeyword.NewTableName;
                newColumns = SiteserverKeyword.NewColumns;
                convertDict = SiteserverKeyword.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_Log"))
            {
                newTableName = SiteserverLog.NewTableName;
                newColumns = SiteserverLog.NewColumns;
                convertDict = SiteserverLog.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_MailSendLog"))
            {
                newTableName = SiteserverMailSendLog.NewTableName;
                newColumns = SiteserverMailSendLog.NewColumns;
                convertDict = SiteserverMailSendLog.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_MailSubscribe"))
            {
                newTableName = SiteserverMailSubscribe.NewTableName;
                newColumns = SiteserverMailSubscribe.NewColumns;
                convertDict = SiteserverMailSubscribe.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_MenuDisplay"))
            {
                newTableName = SiteserverMenuDisplay.NewTableName;
                newColumns = SiteserverMenuDisplay.NewColumns;
                convertDict = SiteserverMenuDisplay.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_Node"))
            {
                newTableName = SiteserverNode.NewTableName;
                newColumns = SiteserverNode.NewColumns;
                convertDict = SiteserverNode.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_NodeGroup"))
            {
                newTableName = SiteserverNodeGroup.NewTableName;
                newColumns = SiteserverNodeGroup.NewColumns;
                convertDict = SiteserverNodeGroup.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_PhotoContent"))
            {
                newTableName = SiteserverPhotoContent.NewTableName;
                newColumns = SiteserverPhotoContent.NewColumns;
                convertDict = SiteserverPhotoContent.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_PublishmentSystem"))
            {
                newTableName = SiteserverPublishmentSystem.NewTableName;
                newColumns = SiteserverPublishmentSystem.NewColumns;
                convertDict = SiteserverPublishmentSystem.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_RelatedField"))
            {
                newTableName = SiteserverRelatedField.NewTableName;
                newColumns = SiteserverRelatedField.NewColumns;
                convertDict = SiteserverRelatedField.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_RelatedFieldItem"))
            {
                newTableName = SiteserverRelatedFieldItem.NewTableName;
                newColumns = SiteserverRelatedFieldItem.NewColumns;
                convertDict = SiteserverRelatedFieldItem.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_ResumeContent"))
            {
                newTableName = SiteserverResumeContent.NewTableName;
                newColumns = SiteserverResumeContent.NewColumns;
                convertDict = SiteserverResumeContent.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_SeoMeta"))
            {
                newTableName = SiteserverSeoMeta.NewTableName;
                newColumns = SiteserverSeoMeta.NewColumns;
                convertDict = SiteserverSeoMeta.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_SeoMetasInNodes"))
            {
                newTableName = SiteserverSeoMetasInNodes.NewTableName;
                newColumns = SiteserverSeoMetasInNodes.NewColumns;
                convertDict = SiteserverSeoMetasInNodes.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_SigninLog"))
            {
                newTableName = SiteserverSigninLog.NewTableName;
                newColumns = SiteserverSigninLog.NewColumns;
                convertDict = SiteserverSigninLog.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_SigninSetting"))
            {
                newTableName = SiteserverSigninSetting.NewTableName;
                newColumns = SiteserverSigninSetting.NewColumns;
                convertDict = SiteserverSigninSetting.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_SigninUserContentID"))
            {
                newTableName = SiteserverSigninUserContentId.NewTableName;
                newColumns = SiteserverSigninUserContentId.NewColumns;
                convertDict = SiteserverSigninUserContentId.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_Star"))
            {
                newTableName = SiteserverStar.NewTableName;
                newColumns = SiteserverStar.NewColumns;
                convertDict = SiteserverStar.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_StarSetting"))
            {
                newTableName = SiteserverStarSetting.NewTableName;
                newColumns = SiteserverStarSetting.NewColumns;
                convertDict = SiteserverStarSetting.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_StlTag"))
            {
                newTableName = SiteserverStlTag.NewTableName;
                newColumns = SiteserverStlTag.NewColumns;
                convertDict = SiteserverStlTag.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_SystemPermissions"))
            {
                newTableName = SiteserverSystemPermissions.NewTableName;
                newColumns = SiteserverSystemPermissions.NewColumns;
                convertDict = SiteserverSystemPermissions.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_TagStyle"))
            {
                newTableName = SiteserverTagStyle.NewTableName;
                newColumns = SiteserverTagStyle.NewColumns;
                convertDict = SiteserverTagStyle.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_Template"))
            {
                newTableName = SiteserverTemplate.NewTableName;
                newColumns = SiteserverTemplate.NewColumns;
                convertDict = SiteserverTemplate.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_TemplateMatch"))
            {
                newTableName = SiteserverTemplateMatch.NewTableName;
                newColumns = SiteserverTemplateMatch.NewColumns;
                convertDict = SiteserverTemplateMatch.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_TemplateRule"))
            {
                newTableName = SiteserverTemplateRule.NewTableName;
                newColumns = SiteserverTemplateRule.NewColumns;
                convertDict = SiteserverTemplateRule.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_TouGaoSetting"))
            {
                newTableName = SiteserverTouGaoSetting.NewTableName;
                newColumns = SiteserverTouGaoSetting.NewColumns;
                convertDict = SiteserverTouGaoSetting.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_Tracking"))
            {
                newTableName = SiteserverTracking.NewTableName;
                newColumns = SiteserverTracking.NewColumns;
                convertDict = SiteserverTracking.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_UserGroup"))
            {
                newTableName = SiteserverUserGroup.NewTableName;
                newColumns = SiteserverUserGroup.NewColumns;
                convertDict = SiteserverUserGroup.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_Users"))
            {
                newTableName = SiteserverUsers.NewTableName;
                newColumns = SiteserverUsers.NewColumns;
                convertDict = SiteserverUsers.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_VoteOperation"))
            {
                newTableName = SiteserverVoteOperation.NewTableName;
                newColumns = SiteserverVoteOperation.NewColumns;
                convertDict = SiteserverVoteOperation.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "siteserver_VoteOption"))
            {
                newTableName = SiteserverVoteOption.NewTableName;
                newColumns = SiteserverVoteOption.NewColumns;
                convertDict = SiteserverVoteOption.ConvertDict;
            }

            if (string.IsNullOrEmpty(newTableName))
            {
                newTableName = oldTableName;
            }
            if (newColumns == null || newColumns.Count == 0)
            {
                newColumns = oldTableInfo.Columns;
            }

            var newTableInfo = new TableInfo
            {
                Columns = newColumns,
                TotalCount = oldTableInfo.TotalCount,
                RowFiles = oldTableInfo.RowFiles
            };

            foreach (var fileName in oldTableInfo.RowFiles)
            {
                var oldFilePath = OldTreeInfo.GetTableContentFilePath(oldTableName, fileName);
                var newFilePath = NewTreeInfo.GetTableContentFilePath(newTableName, fileName);

                if (convertDict != null)
                {
                    var oldRows =
                        TranslateUtils.JsonDeserialize<List<JObject>>(FileUtils.ReadText(oldFilePath, Encoding.UTF8));

                    var newRows = UpdateUtils.UpdateRows(oldRows, convertDict);

                    FileUtils.WriteText(newFilePath, Encoding.UTF8, TranslateUtils.JsonSerialize(newRows));
                }
                else
                {
                    FileUtils.CopyFile(oldFilePath, newFilePath);
                }
            }

            return new KeyValuePair<string, TableInfo>(newTableName, newTableInfo);
        }
    }
}
