using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.User;
using SiteServer.CMS.ImportExport;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalContentImport : BasePageCms
    {
        public RadioButtonList ImportType;
		public HtmlInputFile myFile;
        public RadioButtonList IsOverride;
        public TextBox ImportStart;
        public TextBox ImportCount;
        public RadioButtonList ContentLevel;

        private int _nodeId;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetOpenWindowString("导入内容",
                PageUtils.GetCmsUrl(nameof(ModalContentImport), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"NodeID", nodeId.ToString()}
                }), 650, 500);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _nodeId = Body.GetQueryInt("NodeID", PublishmentSystemId);
			if (!IsPostBack)
			{
                int checkedLevel;
                var isChecked = CheckManager.GetUserCheckLevel(Body.AdministratorName, PublishmentSystemInfo, PublishmentSystemId, out checkedLevel);
                LevelManager.LoadContentLevelToEdit(ContentLevel, PublishmentSystemInfo, _nodeId, null, isChecked, checkedLevel);
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			if (myFile.PostedFile != null && "" != myFile.PostedFile.FileName)
			{
                var isChecked = false;
                var checkedLevel = TranslateUtils.ToIntWithNagetive(ContentLevel.SelectedValue);
                if (checkedLevel >= PublishmentSystemInfo.CheckContentLevel)
                {
                    isChecked = true;
                }

				try
				{
                    if (StringUtils.EqualsIgnoreCase(ImportType.SelectedValue, ModalExportMessage.ExportTypeContentZip))
                    {
                        var filePath = myFile.PostedFile.FileName;
                        if (!EFileSystemTypeUtils.Equals(EFileSystemType.Zip, PathUtils.GetExtension(filePath)))
                        {
                            FailMessage("必须上传后缀为“.zip”的压缩文件");
                            return;
                        }

                        var localFilePath = PathUtils.GetTemporaryFilesPath(PathUtils.GetFileName(filePath));

                        myFile.PostedFile.SaveAs(localFilePath);

                        var importObject = new ImportObject(PublishmentSystemId);
                        var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _nodeId);
                        importObject.ImportContentsByZipFile(nodeInfo, localFilePath, TranslateUtils.ToBool(IsOverride.SelectedValue), TranslateUtils.ToInt(ImportStart.Text), TranslateUtils.ToInt(ImportCount.Text), isChecked, checkedLevel);
                    }
                    else if (StringUtils.EqualsIgnoreCase(ImportType.SelectedValue, ModalExportMessage.ExportTypeContentAccess))
                    {
                        var filePath = myFile.PostedFile.FileName;
                        if (!StringUtils.EqualsIgnoreCase(PathUtils.GetExtension(filePath), ".mdb"))
                        {
                            FailMessage("必须上传后缀为“.mdb”的Access文件");
                            return;
                        }

                        var localFilePath = PathUtils.GetTemporaryFilesPath(PathUtils.GetFileName(filePath));

                        myFile.PostedFile.SaveAs(localFilePath);

                        var importObject = new ImportObject(PublishmentSystemId);
                        importObject.ImportContentsByAccessFile(_nodeId, localFilePath, TranslateUtils.ToBool(IsOverride.SelectedValue), TranslateUtils.ToInt(ImportStart.Text), TranslateUtils.ToInt(ImportCount.Text), isChecked, checkedLevel);
                    }
                    else if (StringUtils.EqualsIgnoreCase(ImportType.SelectedValue, ModalExportMessage.ExportTypeContentExcel))
                    {
                        var filePath = myFile.PostedFile.FileName;
                        if (!StringUtils.EqualsIgnoreCase(PathUtils.GetExtension(filePath), ".csv"))
                        {
                            FailMessage("必须上传后缀为“.csv”的Excel文件");
                            return;
                        }

                        var localFilePath = PathUtils.GetTemporaryFilesPath(PathUtils.GetFileName(filePath));

                        myFile.PostedFile.SaveAs(localFilePath);

                        var importObject = new ImportObject(PublishmentSystemId);
                        importObject.ImportContentsByCsvFile(_nodeId, localFilePath, TranslateUtils.ToBool(IsOverride.SelectedValue), TranslateUtils.ToInt(ImportStart.Text), TranslateUtils.ToInt(ImportCount.Text), isChecked, checkedLevel);
                    }
                    else if (StringUtils.EqualsIgnoreCase(ImportType.SelectedValue, ModalExportMessage.ExportTypeContentTxtZip))
                    {
                        var filePath = myFile.PostedFile.FileName;
                        if (!EFileSystemTypeUtils.Equals(EFileSystemType.Zip, PathUtils.GetExtension(filePath)))
                        {
                            FailMessage("必须上传后缀为“.zip”的压缩文件");
                            return;
                        }

                        var localFilePath = PathUtils.GetTemporaryFilesPath(PathUtils.GetFileName(filePath));

                        myFile.PostedFile.SaveAs(localFilePath);

                        var importObject = new ImportObject(PublishmentSystemId);
                        importObject.ImportContentsByTxtZipFile(_nodeId, localFilePath, TranslateUtils.ToBool(IsOverride.SelectedValue), TranslateUtils.ToInt(ImportStart.Text), TranslateUtils.ToInt(ImportCount.Text), isChecked, checkedLevel);
                    }

                    Body.AddSiteLog(PublishmentSystemId, _nodeId, 0, "导入内容", string.Empty);

					PageUtils.CloseModalPage(Page);
				}
				catch(Exception ex)
				{
					FailMessage(ex, "导入内容失败！");
				}
			}
		}
	}
}
