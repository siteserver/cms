using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.ImportExport;
using SiteServer.Utils.Enumerations;

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

        private int _channelId;

        public static string GetOpenWindowString(int siteId, int channelId)
        {
            return LayerUtils.GetOpenScript("导入内容",
                PageUtils.GetCmsUrl(siteId, nameof(ModalContentImport), new NameValueCollection
                {
                    {"channelId", channelId.ToString()}
                }), 0, 520);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _channelId = AuthRequest.GetQueryInt("channelId", SiteId);
            if (IsPostBack) return;

            int checkedLevel;
            var isChecked = CheckManager.GetUserCheckLevel(AuthRequest.AdminPermissionsImpl, SiteInfo, SiteId, out checkedLevel);
            CheckManager.LoadContentLevelToEdit(DdlContentLevel, SiteInfo, null, isChecked, checkedLevel);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (HifFile.PostedFile == null || "" == HifFile.PostedFile.FileName) return;

            var isChecked = false;
            var checkedLevel = TranslateUtils.ToIntWithNagetive(DdlContentLevel.SelectedValue);
            if (checkedLevel >= SiteInfo.Additional.CheckContentLevel)
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

                    var importObject = new ImportObject(SiteId, AuthRequest.AdminName);
                    var nodeInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);
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

                    var importObject = new ImportObject(SiteId, AuthRequest.AdminName);
                    importObject.ImportContentsByAccessFile(_channelId, localFilePath, TranslateUtils.ToBool(DdlIsOverride.SelectedValue), TranslateUtils.ToInt(TbImportStart.Text), TranslateUtils.ToInt(TbImportCount.Text), isChecked, checkedLevel);
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

                    var importObject = new ImportObject(SiteId, AuthRequest.AdminName);
                    importObject.ImportContentsByCsvFile(_channelId, localFilePath, TranslateUtils.ToBool(DdlIsOverride.SelectedValue), TranslateUtils.ToInt(TbImportStart.Text), TranslateUtils.ToInt(TbImportCount.Text), isChecked, checkedLevel);
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

                    var importObject = new ImportObject(SiteId, AuthRequest.AdminName);
                    importObject.ImportContentsByTxtZipFile(_channelId, localFilePath, TranslateUtils.ToBool(DdlIsOverride.SelectedValue), TranslateUtils.ToInt(TbImportStart.Text), TranslateUtils.ToInt(TbImportCount.Text), isChecked, checkedLevel);
                }

                AuthRequest.AddSiteLog(SiteId, _channelId, 0, "导入内容", string.Empty);

                LayerUtils.Close(Page);
            }
            catch(Exception ex)
            {
                FailMessage(ex, "导入内容失败！");
            }
        }
	}
}
