using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.ImportExport;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalContentImport : BasePageCms
    {
        public DropDownList DdlImportType;
		public HtmlInputFile HifFile;
        public DropDownList DdlIsOverride;
        public TextBox TbImportStart;
        public TextBox TbImportCount;
        public DropDownList DdlContentLevel;

        private int _nodeId;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId)
        {
            return LayerUtils.GetOpenScript("导入内容",
                PageUtils.GetCmsUrl(nameof(ModalContentImport), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"NodeID", nodeId.ToString()}
                }), 0, 520);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _nodeId = Body.GetQueryInt("NodeID", PublishmentSystemId);
            if (IsPostBack) return;

            int checkedLevel;
            var isChecked = CheckManager.GetUserCheckLevel(Body.AdminName, PublishmentSystemInfo, PublishmentSystemId, out checkedLevel);
            CheckManager.LoadContentLevelToEdit(DdlContentLevel, PublishmentSystemInfo, _nodeId, null, isChecked, checkedLevel);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			if (HifFile.PostedFile != null && "" != HifFile.PostedFile.FileName)
			{
                var isChecked = false;
                var checkedLevel = TranslateUtils.ToIntWithNagetive(DdlContentLevel.SelectedValue);
                if (checkedLevel >= PublishmentSystemInfo.CheckContentLevel)
                {
                    isChecked = true;
                }

				try
				{
                    if (StringUtils.EqualsIgnoreCase(DdlImportType.SelectedValue, ModalExportMessage.ExportTypeContentZip))
                    {
                        var filePath = HifFile.PostedFile.FileName;
                        if (!EFileSystemTypeUtils.Equals(EFileSystemType.Zip, PathUtils.GetExtension(filePath)))
                        {
                            FailMessage("必须上传后缀为“.zip”的压缩文件");
                            return;
                        }

                        var localFilePath = PathUtils.GetTemporaryFilesPath(PathUtils.GetFileName(filePath));

                        HifFile.PostedFile.SaveAs(localFilePath);

                        var importObject = new ImportObject(PublishmentSystemId);
                        var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _nodeId);
                        importObject.ImportContentsByZipFile(nodeInfo, localFilePath, TranslateUtils.ToBool(DdlIsOverride.SelectedValue), TranslateUtils.ToInt(TbImportStart.Text), TranslateUtils.ToInt(TbImportCount.Text), isChecked, checkedLevel);
                    }
                    else if (StringUtils.EqualsIgnoreCase(DdlImportType.SelectedValue, ModalExportMessage.ExportTypeContentAccess))
                    {
                        var filePath = HifFile.PostedFile.FileName;
                        if (!StringUtils.EqualsIgnoreCase(PathUtils.GetExtension(filePath), ".mdb"))
                        {
                            FailMessage("必须上传后缀为“.mdb”的Access文件");
                            return;
                        }

                        var localFilePath = PathUtils.GetTemporaryFilesPath(PathUtils.GetFileName(filePath));

                        HifFile.PostedFile.SaveAs(localFilePath);

                        var importObject = new ImportObject(PublishmentSystemId);
                        importObject.ImportContentsByAccessFile(_nodeId, localFilePath, TranslateUtils.ToBool(DdlIsOverride.SelectedValue), TranslateUtils.ToInt(TbImportStart.Text), TranslateUtils.ToInt(TbImportCount.Text), isChecked, checkedLevel);
                    }
                    else if (StringUtils.EqualsIgnoreCase(DdlImportType.SelectedValue, ModalExportMessage.ExportTypeContentExcel))
                    {
                        var filePath = HifFile.PostedFile.FileName;
                        if (!StringUtils.EqualsIgnoreCase(PathUtils.GetExtension(filePath), ".csv"))
                        {
                            FailMessage("必须上传后缀为“.csv”的Excel文件");
                            return;
                        }

                        var localFilePath = PathUtils.GetTemporaryFilesPath(PathUtils.GetFileName(filePath));

                        HifFile.PostedFile.SaveAs(localFilePath);

                        var importObject = new ImportObject(PublishmentSystemId);
                        importObject.ImportContentsByCsvFile(_nodeId, localFilePath, TranslateUtils.ToBool(DdlIsOverride.SelectedValue), TranslateUtils.ToInt(TbImportStart.Text), TranslateUtils.ToInt(TbImportCount.Text), isChecked, checkedLevel);
                    }
                    else if (StringUtils.EqualsIgnoreCase(DdlImportType.SelectedValue, ModalExportMessage.ExportTypeContentTxtZip))
                    {
                        var filePath = HifFile.PostedFile.FileName;
                        if (!EFileSystemTypeUtils.Equals(EFileSystemType.Zip, PathUtils.GetExtension(filePath)))
                        {
                            FailMessage("必须上传后缀为“.zip”的压缩文件");
                            return;
                        }

                        var localFilePath = PathUtils.GetTemporaryFilesPath(PathUtils.GetFileName(filePath));

                        HifFile.PostedFile.SaveAs(localFilePath);

                        var importObject = new ImportObject(PublishmentSystemId);
                        importObject.ImportContentsByTxtZipFile(_nodeId, localFilePath, TranslateUtils.ToBool(DdlIsOverride.SelectedValue), TranslateUtils.ToInt(TbImportStart.Text), TranslateUtils.ToInt(TbImportCount.Text), isChecked, checkedLevel);
                    }

                    Body.AddSiteLog(PublishmentSystemId, _nodeId, 0, "导入内容", string.Empty);

                    LayerUtils.Close(Page);
				}
				catch(Exception ex)
				{
					FailMessage(ex, "导入内容失败！");
				}
			}
		}
	}
}
