using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using SiteServer.Cli.Core;
using SiteServer.Cli.Updater.Model364;
using SiteServer.CMS.Model;
using SiteServer.Utils;
using TableInfo = SiteServer.Cli.Core.TableInfo;

namespace SiteServer.Cli.Updater
{
    public class Updater364 : UpdaterBase
    {
        public const string Version = "3.6.4";

        public override KeyValuePair<string, TableInfo> UpdateTableInfo(string oldTableName, TableInfo oldTableInfo)
        {
            string newTableName = null;
            List<TableColumnInfo> newColumns = null;
            Dictionary<string, string> convertDict = null;

            if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongAdministrator"))
            {
                newTableName = BairongAdministrator.NewTableName;
                newColumns = BairongAdministrator.NewColumns;
                convertDict = BairongAdministrator.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongAdministratorsInRoles"))
            {
                newTableName = BairongAdministratorsInRoles.NewTableName;
                newColumns = BairongAdministratorsInRoles.NewColumns;
                convertDict = BairongAdministratorsInRoles.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongArea"))
            {
                newTableName = BairongArea.NewTableName;
                newColumns = BairongArea.NewColumns;
                convertDict = BairongArea.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongCache"))
            {
                newTableName = BairongCache.NewTableName;
                newColumns = BairongCache.NewColumns;
                convertDict = BairongCache.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongCloudStorage"))
            {
                newTableName = BairongCloudStorage.NewTableName;
                newColumns = BairongCloudStorage.NewColumns;
                convertDict = BairongCloudStorage.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongConfig"))
            {
                newTableName = BairongConfig.NewTableName;
                newColumns = BairongConfig.NewColumns;
                convertDict = BairongConfig.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongContentCheck"))
            {
                newTableName = BairongContentCheck.NewTableName;
                newColumns = BairongContentCheck.NewColumns;
                convertDict = BairongContentCheck.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongContentModel"))
            {
                newTableName = BairongContentModel.NewTableName;
                newColumns = BairongContentModel.NewColumns;
                convertDict = BairongContentModel.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongCount"))
            {
                newTableName = BairongCount.NewTableName;
                newColumns = BairongCount.NewColumns;
                convertDict = BairongCount.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongDepartment"))
            {
                newTableName = BairongDepartment.NewTableName;
                newColumns = BairongDepartment.NewColumns;
                convertDict = BairongDepartment.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongDigg"))
            {
                newTableName = BairongDigg.NewTableName;
                newColumns = BairongDigg.NewColumns;
                convertDict = BairongDigg.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongFTPStorage"))
            {
                newTableName = BairongFtpStorage.NewTableName;
                newColumns = BairongFtpStorage.NewColumns;
                convertDict = BairongFtpStorage.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongIP2City"))
            {
                newTableName = BairongIp2City.NewTableName;
                newColumns = BairongIp2City.NewColumns;
                convertDict = BairongIp2City.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongLocalStorage"))
            {
                newTableName = BairongLocalStorage.NewTableName;
                newColumns = BairongLocalStorage.NewColumns;
                convertDict = BairongLocalStorage.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongLog"))
            {
                newTableName = BairongLog.NewTableName;
                newColumns = BairongLog.NewColumns;
                convertDict = BairongLog.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongModule"))
            {
                newTableName = BairongModule.NewTableName;
                newColumns = BairongModule.NewColumns;
                convertDict = BairongModule.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongPermissionsInRoles"))
            {
                newTableName = BairongPermissionsInRoles.NewTableName;
                newColumns = BairongPermissionsInRoles.NewColumns;
                convertDict = BairongPermissionsInRoles.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongRoles"))
            {
                newTableName = BairongRoles.NewTableName;
                newColumns = BairongRoles.NewColumns;
                convertDict = BairongRoles.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongSMSMessages"))
            {
                newTableName = BairongSmsMessages.NewTableName;
                newColumns = BairongSmsMessages.NewColumns;
                convertDict = BairongSmsMessages.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongSSOApp"))
            {
                newTableName = BairongSsoApp.NewTableName;
                newColumns = BairongSsoApp.NewColumns;
                convertDict = BairongSsoApp.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongStorage"))
            {
                newTableName = BairongStorage.NewTableName;
                newColumns = BairongStorage.NewColumns;
                convertDict = BairongStorage.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongTableCollection"))
            {
                newTableName = BairongTableCollection.NewTableName;
                newColumns = BairongTableCollection.NewColumns;
                convertDict = BairongTableCollection.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongTableMatch"))
            {
                newTableName = BairongTableMatch.NewTableName;
                newColumns = BairongTableMatch.NewColumns;
                convertDict = BairongTableMatch.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongTableMetadata"))
            {
                newTableName = BairongTableMetadata.NewTableName;
                newColumns = BairongTableMetadata.NewColumns;
                convertDict = BairongTableMetadata.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongTableStyle"))
            {
                newTableName = BairongTableStyle.NewTableName;
                newColumns = BairongTableStyle.NewColumns;
                convertDict = BairongTableStyle.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongTableStyleItem"))
            {
                newTableName = BairongTableStyleItem.NewTableName;
                newColumns = BairongTableStyleItem.NewColumns;
                convertDict = BairongTableStyleItem.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongTags"))
            {
                newTableName = BairongTags.NewTableName;
                newColumns = BairongTags.NewColumns;
                convertDict = BairongTags.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongTask"))
            {
                newTableName = BairongTask.NewTableName;
                newColumns = BairongTask.NewColumns;
                convertDict = BairongTask.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongTaskLog"))
            {
                newTableName = BairongTaskLog.NewTableName;
                newColumns = BairongTaskLog.NewColumns;
                convertDict = BairongTaskLog.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongUserBinding"))
            {
                newTableName = BairongUserBinding.NewTableName;
                newColumns = BairongUserBinding.NewColumns;
                convertDict = BairongUserBinding.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongUserConfig"))
            {
                newTableName = BairongUserConfig.NewTableName;
                newColumns = BairongUserConfig.NewColumns;
                convertDict = BairongUserConfig.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongUserContact"))
            {
                newTableName = BairongUserContact.NewTableName;
                newColumns = BairongUserContact.NewColumns;
                convertDict = BairongUserContact.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongUserCreditsLog"))
            {
                newTableName = BairongUserCreditsLog.NewTableName;
                newColumns = BairongUserCreditsLog.NewColumns;
                convertDict = BairongUserCreditsLog.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongUserDownload"))
            {
                newTableName = BairongUserDownload.NewTableName;
                newColumns = BairongUserDownload.NewColumns;
                convertDict = BairongUserDownload.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongUserMessage"))
            {
                newTableName = BairongUserMessage.NewTableName;
                newColumns = BairongUserMessage.NewColumns;
                convertDict = BairongUserMessage.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongUsers"))
            {
                newTableName = BairongUsers.NewTableName;
                newColumns = BairongUsers.NewColumns;
                convertDict = BairongUsers.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "BairongUserType"))
            {
                newTableName = BairongUserType.NewTableName;
                newColumns = BairongUserType.NewColumns;
                convertDict = BairongUserType.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "CmsContent"))
            {
                newTableName = CmsContent.NewTableName;
                newColumns = CmsContent.NewColumns;
                convertDict = CmsContent.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "CmsContentCustom"))
            {
                newTableName = CmsContentCustom.NewTableName;
                newColumns = CmsContentCustom.NewColumns;
                convertDict = CmsContentCustom.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "CmsContentJob"))
            {
                newTableName = CmsContentJob.NewTableName;
                newColumns = CmsContentJob.NewColumns;
                convertDict = CmsContentJob.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "CmsContentVote"))
            {
                newTableName = CmsContentVote.NewTableName;
                newColumns = CmsContentVote.NewColumns;
                convertDict = CmsContentVote.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverAd"))
            {
                newTableName = SiteserverAd.NewTableName;
                newColumns = SiteserverAd.NewColumns;
                convertDict = SiteserverAd.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverAdvertisement"))
            {
                newTableName = SiteserverAdvertisement.NewTableName;
                newColumns = SiteserverAdvertisement.NewColumns;
                convertDict = SiteserverAdvertisement.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverComment"))
            {
                newTableName = SiteserverComment.NewTableName;
                newColumns = SiteserverComment.NewColumns;
                convertDict = SiteserverComment.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverCommentContent"))
            {
                newTableName = SiteserverCommentContent.NewTableName;
                newColumns = SiteserverCommentContent.NewColumns;
                convertDict = SiteserverCommentContent.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverConfiguration"))
            {
                newTableName = SiteserverConfiguration.NewTableName;
                newColumns = SiteserverConfiguration.NewColumns;
                convertDict = SiteserverConfiguration.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverContentGroup"))
            {
                newTableName = SiteserverContentGroup.NewTableName;
                newColumns = SiteserverContentGroup.NewColumns;
                convertDict = SiteserverContentGroup.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverGatherDatabaseRule"))
            {
                newTableName = SiteserverGatherDatabaseRule.NewTableName;
                newColumns = SiteserverGatherDatabaseRule.NewColumns;
                convertDict = SiteserverGatherDatabaseRule.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverGatherFileRule"))
            {
                newTableName = SiteserverGatherFileRule.NewTableName;
                newColumns = SiteserverGatherFileRule.NewColumns;
                convertDict = SiteserverGatherFileRule.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverGatherRule"))
            {
                newTableName = SiteserverGatherRule.NewTableName;
                newColumns = SiteserverGatherRule.NewColumns;
                convertDict = SiteserverGatherRule.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverGovInteractChannel"))
            {
                newTableName = SiteserverGovInteractChannel.NewTableName;
                newColumns = SiteserverGovInteractChannel.NewColumns;
                convertDict = SiteserverGovInteractChannel.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverGovInteractLog"))
            {
                newTableName = SiteserverGovInteractLog.NewTableName;
                newColumns = SiteserverGovInteractLog.NewColumns;
                convertDict = SiteserverGovInteractLog.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverGovInteractPermissions"))
            {
                newTableName = SiteserverGovInteractPermissions.NewTableName;
                newColumns = SiteserverGovInteractPermissions.NewColumns;
                convertDict = SiteserverGovInteractPermissions.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverGovInteractRemark"))
            {
                newTableName = SiteserverGovInteractRemark.NewTableName;
                newColumns = SiteserverGovInteractRemark.NewColumns;
                convertDict = SiteserverGovInteractRemark.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverGovInteractReply"))
            {
                newTableName = SiteserverGovInteractReply.NewTableName;
                newColumns = SiteserverGovInteractReply.NewColumns;
                convertDict = SiteserverGovInteractReply.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverGovInteractType"))
            {
                newTableName = SiteserverGovInteractType.NewTableName;
                newColumns = SiteserverGovInteractType.NewColumns;
                convertDict = SiteserverGovInteractType.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverGovPublicApply"))
            {
                newTableName = SiteserverGovPublicApply.NewTableName;
                newColumns = SiteserverGovPublicApply.NewColumns;
                convertDict = SiteserverGovPublicApply.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverGovPublicApplyLog"))
            {
                newTableName = SiteserverGovPublicApplyLog.NewTableName;
                newColumns = SiteserverGovPublicApplyLog.NewColumns;
                convertDict = SiteserverGovPublicApplyLog.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverGovPublicApplyRemark"))
            {
                newTableName = SiteserverGovPublicApplyRemark.NewTableName;
                newColumns = SiteserverGovPublicApplyRemark.NewColumns;
                convertDict = SiteserverGovPublicApplyRemark.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverGovPublicApplyReply"))
            {
                newTableName = SiteserverGovPublicApplyReply.NewTableName;
                newColumns = SiteserverGovPublicApplyReply.NewColumns;
                convertDict = SiteserverGovPublicApplyReply.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverGovPublicCategory"))
            {
                newTableName = SiteserverGovPublicCategory.NewTableName;
                newColumns = SiteserverGovPublicCategory.NewColumns;
                convertDict = SiteserverGovPublicCategory.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverGovPublicCategoryClass"))
            {
                newTableName = SiteserverGovPublicCategoryClass.NewTableName;
                newColumns = SiteserverGovPublicCategoryClass.NewColumns;
                convertDict = SiteserverGovPublicCategoryClass.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverGovPublicChannel"))
            {
                newTableName = SiteserverGovPublicChannel.NewTableName;
                newColumns = SiteserverGovPublicChannel.NewColumns;
                convertDict = SiteserverGovPublicChannel.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverGovPublicIdentifierRule"))
            {
                newTableName = SiteserverGovPublicIdentifierRule.NewTableName;
                newColumns = SiteserverGovPublicIdentifierRule.NewColumns;
                convertDict = SiteserverGovPublicIdentifierRule.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverGovPublicIdentifierSeq"))
            {
                newTableName = SiteserverGovPublicIdentifierSeq.NewTableName;
                newColumns = SiteserverGovPublicIdentifierSeq.NewColumns;
                convertDict = SiteserverGovPublicIdentifierSeq.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverInnerLink"))
            {
                newTableName = SiteserverInnerLink.NewTableName;
                newColumns = SiteserverInnerLink.NewColumns;
                convertDict = SiteserverInnerLink.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverInput"))
            {
                newTableName = SiteserverInput.NewTableName;
                newColumns = SiteserverInput.NewColumns;
                convertDict = SiteserverInput.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverInputContent"))
            {
                newTableName = SiteserverInputContent.NewTableName;
                newColumns = SiteserverInputContent.NewColumns;
                convertDict = SiteserverInputContent.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverKeyword"))
            {
                newTableName = SiteserverKeyword.NewTableName;
                newColumns = SiteserverKeyword.NewColumns;
                convertDict = SiteserverKeyword.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverLog"))
            {
                newTableName = SiteserverLog.NewTableName;
                newColumns = SiteserverLog.NewColumns;
                convertDict = SiteserverLog.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverMailSendLog"))
            {
                newTableName = SiteserverMailSendLog.NewTableName;
                newColumns = SiteserverMailSendLog.NewColumns;
                convertDict = SiteserverMailSendLog.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverMailSubscribe"))
            {
                newTableName = SiteserverMailSubscribe.NewTableName;
                newColumns = SiteserverMailSubscribe.NewColumns;
                convertDict = SiteserverMailSubscribe.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverMenuDisplay"))
            {
                newTableName = SiteserverMenuDisplay.NewTableName;
                newColumns = SiteserverMenuDisplay.NewColumns;
                convertDict = SiteserverMenuDisplay.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverNode"))
            {
                newTableName = SiteserverNode.NewTableName;
                newColumns = SiteserverNode.NewColumns;
                convertDict = SiteserverNode.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverNodeGroup"))
            {
                newTableName = SiteserverNodeGroup.NewTableName;
                newColumns = SiteserverNodeGroup.NewColumns;
                convertDict = SiteserverNodeGroup.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverPhotoContent"))
            {
                newTableName = SiteserverPhotoContent.NewTableName;
                newColumns = SiteserverPhotoContent.NewColumns;
                convertDict = SiteserverPhotoContent.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverPublishmentSystem"))
            {
                newTableName = SiteserverPublishmentSystem.NewTableName;
                newColumns = SiteserverPublishmentSystem.NewColumns;
                convertDict = SiteserverPublishmentSystem.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverRelatedField"))
            {
                newTableName = SiteserverRelatedField.NewTableName;
                newColumns = SiteserverRelatedField.NewColumns;
                convertDict = SiteserverRelatedField.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverRelatedFieldItem"))
            {
                newTableName = SiteserverRelatedFieldItem.NewTableName;
                newColumns = SiteserverRelatedFieldItem.NewColumns;
                convertDict = SiteserverRelatedFieldItem.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverResumeContent"))
            {
                newTableName = SiteserverResumeContent.NewTableName;
                newColumns = SiteserverResumeContent.NewColumns;
                convertDict = SiteserverResumeContent.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverSeoMeta"))
            {
                newTableName = SiteserverSeoMeta.NewTableName;
                newColumns = SiteserverSeoMeta.NewColumns;
                convertDict = SiteserverSeoMeta.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverSeoMetasInNodes"))
            {
                newTableName = SiteserverSeoMetasInNodes.NewTableName;
                newColumns = SiteserverSeoMetasInNodes.NewColumns;
                convertDict = SiteserverSeoMetasInNodes.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverSigninLog"))
            {
                newTableName = SiteserverSigninLog.NewTableName;
                newColumns = SiteserverSigninLog.NewColumns;
                convertDict = SiteserverSigninLog.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverSigninSetting"))
            {
                newTableName = SiteserverSigninSetting.NewTableName;
                newColumns = SiteserverSigninSetting.NewColumns;
                convertDict = SiteserverSigninSetting.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverSigninUserContentID"))
            {
                newTableName = SiteserverSigninUserContentId.NewTableName;
                newColumns = SiteserverSigninUserContentId.NewColumns;
                convertDict = SiteserverSigninUserContentId.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverStar"))
            {
                newTableName = SiteserverStar.NewTableName;
                newColumns = SiteserverStar.NewColumns;
                convertDict = SiteserverStar.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverStarSetting"))
            {
                newTableName = SiteserverStarSetting.NewTableName;
                newColumns = SiteserverStarSetting.NewColumns;
                convertDict = SiteserverStarSetting.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverStlTag"))
            {
                newTableName = SiteserverStlTag.NewTableName;
                newColumns = SiteserverStlTag.NewColumns;
                convertDict = SiteserverStlTag.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverSystemPermissions"))
            {
                newTableName = SiteserverSystemPermissions.NewTableName;
                newColumns = SiteserverSystemPermissions.NewColumns;
                convertDict = SiteserverSystemPermissions.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverTagStyle"))
            {
                newTableName = SiteserverTagStyle.NewTableName;
                newColumns = SiteserverTagStyle.NewColumns;
                convertDict = SiteserverTagStyle.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverTemplate"))
            {
                newTableName = SiteserverTemplate.NewTableName;
                newColumns = SiteserverTemplate.NewColumns;
                convertDict = SiteserverTemplate.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverTemplateMatch"))
            {
                newTableName = SiteserverTemplateMatch.NewTableName;
                newColumns = SiteserverTemplateMatch.NewColumns;
                convertDict = SiteserverTemplateMatch.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverTemplateRule"))
            {
                newTableName = SiteserverTemplateRule.NewTableName;
                newColumns = SiteserverTemplateRule.NewColumns;
                convertDict = SiteserverTemplateRule.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverTouGaoSetting"))
            {
                newTableName = SiteserverTouGaoSetting.NewTableName;
                newColumns = SiteserverTouGaoSetting.NewColumns;
                convertDict = SiteserverTouGaoSetting.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverTracking"))
            {
                newTableName = SiteserverTracking.NewTableName;
                newColumns = SiteserverTracking.NewColumns;
                convertDict = SiteserverTracking.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverUserGroup"))
            {
                newTableName = SiteserverUserGroup.NewTableName;
                newColumns = SiteserverUserGroup.NewColumns;
                convertDict = SiteserverUserGroup.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverUsers"))
            {
                newTableName = SiteserverUsers.NewTableName;
                newColumns = SiteserverUsers.NewColumns;
                convertDict = SiteserverUsers.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverVoteOperation"))
            {
                newTableName = SiteserverVoteOperation.NewTableName;
                newColumns = SiteserverVoteOperation.NewColumns;
                convertDict = SiteserverVoteOperation.ConvertDict;
            }
            else if (StringUtils.EqualsIgnoreCase(oldTableName, "SiteserverVoteOption"))
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
                var oldFilePath = CliUtils.GetTableContentFilePath(OldFolderName, oldTableName, fileName);
                var newFilePath = CliUtils.GetTableContentFilePath(NewFolderName, newTableName, fileName);

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

        public Updater364(string oldFolderName, string newFolderName) : base(oldFolderName, newFolderName)
        {

        }
    }
}
