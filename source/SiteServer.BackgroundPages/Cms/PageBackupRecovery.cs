using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.IO;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageBackupRecovery : BasePageCms
    {
		public DropDownList BackupType;
        public PlaceHolder PlaceHolder_Delete;
        public RadioButtonList IsDeleteChannels;
        public RadioButtonList IsDeleteTemplates;
        public RadioButtonList IsDeleteFiles;
        public RadioButtonList IsOverride;
        public RadioButtonList IsRecoveryByUpload;

        public PlaceHolder PlaceHolderByUpload;
        public HtmlInputFile myFile;

		public Button RecoveryButton;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdBackup, "数据恢复", AppManager.Cms.Permission.WebSite.Backup);

                EBackupTypeUtils.AddListItems(BackupType);
                ControlUtils.SelectListItems(BackupType, EBackupTypeUtils.GetValue(EBackupType.Templates));

                EBooleanUtils.AddListItems(IsRecoveryByUpload, "从上传文件中恢复", "从服务器备份文件中恢复");
                ControlUtils.SelectListItems(IsRecoveryByUpload, true.ToString());

                Options_SelectedIndexChanged(null, EventArgs.Empty);
			}
		}

        public void Options_SelectedIndexChanged(object sender, EventArgs e)
        {
            var backupType = EBackupTypeUtils.GetEnumType(BackupType.SelectedValue);
            if (backupType == EBackupType.Site)
            {
                PlaceHolder_Delete.Visible = true;
            }
            else
            {
                PlaceHolder_Delete.Visible = false;
            }

            PlaceHolderByUpload.Visible = TranslateUtils.ToBool(IsRecoveryByUpload.SelectedValue);
        }


		public void RecoveryButton_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                if (TranslateUtils.ToBool(IsRecoveryByUpload.SelectedValue))
                {
                    if (myFile.PostedFile != null && "" != myFile.PostedFile.FileName)
                    {
                        var filePath = myFile.PostedFile.FileName;
                        if (EBackupTypeUtils.Equals(EBackupType.Templates, BackupType.SelectedValue))
                        {
                            if (EFileSystemTypeUtils.GetEnumType(PathUtils.GetExtension(filePath)) != EFileSystemType.Xml)
                            {
                                FailMessage("必须上传Xml文件");
                                return;
                            }
                        }
                        else
                        {
                            if (!EFileSystemTypeUtils.IsCompressionFile(PathUtils.GetExtension(filePath)))
                            {
                                FailMessage("必须上传压缩文件");
                                return;
                            }
                        }

                        try
                        {
                            var localFilePath = PathUtils.GetTemporaryFilesPath(PathUtils.GetFileName(filePath));

                            myFile.PostedFile.SaveAs(localFilePath);

                            var importObject = new ImportObject(PublishmentSystemId);

                            if (EBackupTypeUtils.Equals(EBackupType.Templates, BackupType.SelectedValue))
                            {
                                importObject.ImportTemplates(localFilePath, TranslateUtils.ToBool(IsOverride.SelectedValue), Body.AdministratorName);
                                SuccessMessage("恢复模板成功!");
                            }
                            else if (EBackupTypeUtils.Equals(EBackupType.ChannelsAndContents, BackupType.SelectedValue))
                            {
                                importObject.ImportChannelsAndContentsByZipFile(0, localFilePath, TranslateUtils.ToBool(IsOverride.SelectedValue));
                                SuccessMessage("恢复栏目及内容成功!");
                            }
                            else if (EBackupTypeUtils.Equals(EBackupType.Files, BackupType.SelectedValue))
                            {
                                var filesDirectoryPath = PathUtils.GetTemporaryFilesPath(EBackupTypeUtils.GetValue(EBackupType.Files));
                                DirectoryUtils.DeleteDirectoryIfExists(filesDirectoryPath);
                                DirectoryUtils.CreateDirectoryIfNotExists(filesDirectoryPath);

                                ZipUtils.UnpackFiles(localFilePath, filesDirectoryPath);

                                importObject.ImportFiles(filesDirectoryPath, TranslateUtils.ToBool(IsOverride.SelectedValue));
                                SuccessMessage("恢复文件成功!");
                            }
                            else if (EBackupTypeUtils.Equals(EBackupType.Site, BackupType.SelectedValue))
                            {
                                var userKeyPrefix = StringUtils.Guid();
                                PageUtils.Redirect(PageProgressBar.GetRecoveryUrl(PublishmentSystemId, IsDeleteChannels.SelectedValue, IsDeleteTemplates.SelectedValue, IsDeleteFiles.SelectedValue, true, localFilePath, IsOverride.SelectedValue, IsOverride.SelectedValue, userKeyPrefix));
                            }
                        }
                        catch (Exception ex)
                        {
                            FailMessage(ex, "数据恢复失败！");
                        }
                    }
                }
			}
		}
	}
}