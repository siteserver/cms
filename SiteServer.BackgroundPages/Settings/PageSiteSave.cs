using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Model;
using SiteServer.Utils.Enumerations;
using SiteServer.Utils.IO;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageSiteSave : BasePageCms
    {
        public PlaceHolder PhWelcome;
        public TextBox TbSiteTemplateName;
        public TextBox TbSiteTemplateDir;
        public TextBox TbWebSiteUrl;
        public TextBox TbDescription;

        public PlaceHolder PhSaveFiles;
        public RadioButtonList RblIsSaveAllFiles;
        public PlaceHolder PhDirectoriesAndFiles;
        public CheckBoxList CblDirectoriesAndFiles;

        public PlaceHolder PhSaveSiteContents;
        public RadioButtonList RblIsSaveContents;
        public RadioButtonList RblIsSaveAllChannels;

        public PlaceHolder PhChannels;
        public Literal LtlChannelTree;

        public PlaceHolder PhSaveSiteStyles;

        public PlaceHolder PhUploadImageFile;
        public HtmlInputFile HifSamplePicFile;

        public PlaceHolder PhDone;

        public Button BtnWelcomeNext;
        public Button BtnSaveFilesNext;
        public Button BtnSaveSiteContentsNext;
        public Button BtnSaveSiteStylesNext;
        public Button BtnUploadImageFileNext;

        private ExportObject _exportObject;

        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetSettingsUrl(nameof(PageSiteSave), new NameValueCollection
            {
                {"siteId", siteId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            _exportObject = new ExportObject(SiteId, AuthRequest.AdminName);

            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Site);

            if (SiteInfo.IsRoot)
            {
                TbSiteTemplateDir.Text = "T_" + SiteInfo.SiteName;
            }
            else
            {
                TbSiteTemplateDir.Text = "T_" + SiteInfo.SiteDir.Replace("\\", "_");
            }
            TbSiteTemplateName.Text = SiteInfo.SiteName;

            EBooleanUtils.AddListItems(RblIsSaveAllFiles, "全部文件", "指定文件");
            ControlUtils.SelectSingleItemIgnoreCase(RblIsSaveAllFiles, true.ToString());

            var siteDirList = DataProvider.SiteDao.GetLowerSiteDirListThatNotIsRoot();
            var fileSystems = FileManager.GetFileSystemInfoExtendCollection(PathUtility.GetSitePath(SiteInfo), true);
            foreach (FileSystemInfoExtend fileSystem in fileSystems)
            {
                if (!fileSystem.IsDirectory) continue;

                var isSiteDirectory = false;
                if (SiteInfo.IsRoot)
                {
                    foreach (var siteDir in siteDirList)
                    {
                        if (StringUtils.EqualsIgnoreCase(siteDir, fileSystem.Name))
                        {
                            isSiteDirectory = true;
                        }
                    }
                }
                if (!isSiteDirectory && !DirectoryUtils.IsSystemDirectory(fileSystem.Name))
                {
                    CblDirectoriesAndFiles.Items.Add(new ListItem(fileSystem.Name, fileSystem.Name.ToLower()));
                }
            }
            foreach (FileSystemInfoExtend fileSystem in fileSystems)
            {
                if (fileSystem.IsDirectory || StringUtils.EqualsIgnoreCase(fileSystem.Name, "web.config")) continue;
                if (!PathUtility.IsSystemFile(fileSystem.Name))
                {
                    CblDirectoriesAndFiles.Items.Add(new ListItem(fileSystem.Name, fileSystem.Name.ToLower()));
                }
            }

            EBooleanUtils.AddListItems(RblIsSaveContents, "保存内容数据", "不保存内容数据");
            ControlUtils.SelectSingleItemIgnoreCase(RblIsSaveContents, true.ToString());

            EBooleanUtils.AddListItems(RblIsSaveAllChannels, "全部栏目", "指定栏目");
            ControlUtils.SelectSingleItemIgnoreCase(RblIsSaveAllChannels, true.ToString());

            LtlChannelTree.Text = GetChannelTreeHtml();
        }

        private string GetChannelTreeHtml()
        {
            var htmlBuilder = new StringBuilder();

            var treeDirectoryUrl = SiteServerAssets.GetIconUrl("tree");

            htmlBuilder.Append("<span id='ChannelSelectControl'>");
            var channelIdList = ChannelManager.GetChannelIdList(SiteId);
            var isLastNodeArray = new bool[channelIdList.Count];
            foreach (var channelId in channelIdList)
            {
                var nodeInfo = ChannelManager.GetChannelInfo(SiteId, channelId);
                htmlBuilder.Append(GetTitle(nodeInfo, treeDirectoryUrl, isLastNodeArray));
                htmlBuilder.Append("<br/>");
            }
            htmlBuilder.Append("</span>");
            return htmlBuilder.ToString();
        }

        private string GetTitle(ChannelInfo channelInfo, string treeDirectoryUrl, IList<bool> isLastNodeArray)
        {
            var itemBuilder = new StringBuilder();
            if (channelInfo.Id == SiteId)
            {
                channelInfo.IsLastNode = true;
            }
            if (channelInfo.IsLastNode == false)
            {
                isLastNodeArray[channelInfo.ParentsCount] = false;
            }
            else
            {
                isLastNodeArray[channelInfo.ParentsCount] = true;
            }
            for (var i = 0; i < channelInfo.ParentsCount; i++)
            {
                itemBuilder.Append($"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/tree_empty.gif\"/>");
            }
            if (channelInfo.IsLastNode)
            {
                itemBuilder.Append(channelInfo.ChildrenCount > 0
                    ? $"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/minus.png\"/>"
                    : $"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/tree_empty.gif\"/>");
            }
            else
            {
                itemBuilder.Append(channelInfo.ChildrenCount > 0
                    ? $"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/minus.png\"/>"
                    : $"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/tree_empty.gif\"/>");
            }

            var onlyAdminId = AuthRequest.AdminPermissionsImpl.GetOnlyAdminId(SiteId, channelInfo.Id);
            var count = ContentManager.GetCount(SiteInfo, channelInfo, onlyAdminId);

            itemBuilder.Append($@"
<span class=""checkbox checkbox-primary"" style=""padding-left: 0px;"">
    <input type=""checkbox"" id=""ChannelIdCollection_{channelInfo.Id}"" name=""ChannelIdCollection"" value=""{channelInfo.Id}""/>
    <label for=""ChannelIdCollection_{channelInfo.Id}""> {channelInfo.ChannelName} &nbsp;<span style=""font-size:8pt;font-family:arial"" class=""gray"">({count})</span></label>
</span>
");

            return itemBuilder.ToString();
        }

        public void RblIsSaveAllFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhDirectoriesAndFiles.Visible = !TranslateUtils.ToBool(RblIsSaveAllFiles.SelectedValue);
        }

        public void RblIsSaveAllChannels_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhChannels.Visible = !TranslateUtils.ToBool(RblIsSaveAllChannels.SelectedValue);
        }

        private bool UploadImageFile(out string errorMessage, out string samplePicPath)
        {
            if (HifSamplePicFile.PostedFile != null && "" != HifSamplePicFile.PostedFile.FileName)
            {
                var filePath = HifSamplePicFile.PostedFile.FileName;
                var fileExtName = filePath.ToLower().Substring(filePath.LastIndexOf(".", StringComparison.Ordinal));
                if (fileExtName == ".jpg" || fileExtName == ".jpeg" || fileExtName == ".gif" || fileExtName == ".bmp" || fileExtName == ".png" || fileExtName == ".tif")
                {
                    try
                    {
                        var siteTemplatePath = PathUtility.GetSiteTemplatesPath(TbSiteTemplateDir.Text);
                        var localFilePath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, TbSiteTemplateDir.Text + fileExtName);

                        HifSamplePicFile.PostedFile.SaveAs(localFilePath);
                        samplePicPath = TbSiteTemplateDir.Text + fileExtName;
                        errorMessage = "";
                        return true;
                    }
                    catch (Exception ex)
                    {
                        errorMessage = ex.Message;
                        samplePicPath = "";
                        return false;
                    }
                }
                errorMessage = "网站样图不是有效的图片文件";
                samplePicPath = "";
                return false;
            }
            errorMessage = "";
            samplePicPath = "";
            return true;
        }

        private bool SaveFiles(out string errorMessage)
        {
            try
            {
                var siteTemplatePath = PathUtility.GetSiteTemplatesPath(TbSiteTemplateDir.Text);
                var isSaveAll = TranslateUtils.ToBool(RblIsSaveAllFiles.SelectedValue);
                var lowerFileSystemArrayList = ControlUtils.GetSelectedListControlValueArrayList(CblDirectoriesAndFiles);
                _exportObject.ExportFilesToSite(siteTemplatePath, isSaveAll, lowerFileSystemArrayList, true);
                errorMessage = "";
                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }
        }

        private bool SaveSiteContents(out string errorMessage)
        {
            try
            {
                var siteTemplatePath = PathUtility.GetSiteTemplatesPath(TbSiteTemplateDir.Text);
                var siteContentDirectoryPath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.SiteContent);

                var isSaveContents = TranslateUtils.ToBool(RblIsSaveContents.SelectedValue);
                var isSaveAllChannels = TranslateUtils.ToBool(RblIsSaveAllChannels.SelectedValue);

                var channelIdList = TranslateUtils.StringCollectionToIntList(Request.Form["ChannelIdCollection"]);
                _exportObject.ExportSiteContent(siteContentDirectoryPath, isSaveContents, isSaveAllChannels, channelIdList);

                errorMessage = "";
                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }
        }

        private bool SaveSiteStyles(out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                SiteTemplateManager.ExportSiteToSiteTemplate(SiteInfo, TbSiteTemplateDir.Text, AuthRequest.AdminName);

                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }
        }

        public void BtnWelcomeNext_Click(object sender, EventArgs e)
        {
            BtnWelcomeNext.Visible = BtnSaveFilesNext.Visible = BtnSaveSiteContentsNext.Visible = BtnSaveSiteStylesNext.Visible = BtnUploadImageFileNext.Visible = PhWelcome.Visible = PhSaveFiles.Visible = PhSaveSiteContents.Visible = PhSaveSiteStyles.Visible = PhUploadImageFile.Visible = PhDone.Visible = false;

            if (SiteTemplateManager.Instance.IsSiteTemplateDirectoryExists(TbSiteTemplateDir.Text))
            {
                BtnWelcomeNext.Visible = PhWelcome.Visible = true;
                FailMessage("站点模板文件夹已存在，请更换站点模板文件夹！");
            }
            else
            {
                BtnSaveFilesNext.Visible = PhSaveFiles.Visible = true;
            }
        }

        public void BtnSaveFilesNext_Click(object sender, EventArgs e)
        {
            BtnWelcomeNext.Visible = BtnSaveFilesNext.Visible = BtnSaveSiteContentsNext.Visible = BtnSaveSiteStylesNext.Visible = BtnUploadImageFileNext.Visible = PhWelcome.Visible = PhSaveFiles.Visible = PhSaveSiteContents.Visible = PhSaveSiteStyles.Visible = PhUploadImageFile.Visible = PhDone.Visible = false;

            string errorMessage;
            if (SaveFiles(out errorMessage))
            {
                BtnSaveSiteContentsNext.Visible = PhSaveSiteContents.Visible = true;
                AuthRequest.AddAdminLog("保存站点模板", $"站点:{SiteInfo.SiteName}");
            }
            else
            {
                BtnSaveFilesNext.Visible = PhSaveFiles.Visible = true;
                FailMessage(errorMessage);
            }
        }

        public void BtnSaveSiteContentsNext_Click(object sender, EventArgs e)
        {
            BtnWelcomeNext.Visible = BtnSaveFilesNext.Visible = BtnSaveSiteContentsNext.Visible = BtnSaveSiteStylesNext.Visible = BtnUploadImageFileNext.Visible = PhWelcome.Visible = PhSaveFiles.Visible = PhSaveSiteContents.Visible = PhSaveSiteStyles.Visible = PhUploadImageFile.Visible = PhDone.Visible = false;

            string errorMessage;
            if (SaveSiteContents(out errorMessage))
            {
                BtnSaveSiteStylesNext.Visible = PhSaveSiteStyles.Visible = true;
            }
            else
            {
                BtnSaveSiteContentsNext.Visible = PhSaveSiteContents.Visible = true;
                FailMessage(errorMessage);
            }
        }

        public void BtnSaveSiteStylesNext_Click(object sender, EventArgs e)
        {
            BtnWelcomeNext.Visible = BtnSaveFilesNext.Visible = BtnSaveSiteContentsNext.Visible = BtnSaveSiteStylesNext.Visible = BtnUploadImageFileNext.Visible = PhWelcome.Visible = PhSaveFiles.Visible = PhSaveSiteContents.Visible = PhSaveSiteStyles.Visible = PhUploadImageFile.Visible = PhDone.Visible = false;

            string errorMessage;
            if (SaveSiteStyles(out errorMessage))
            {
                BtnUploadImageFileNext.Visible = PhUploadImageFile.Visible = true;
            }
            else
            {
                BtnSaveSiteStylesNext.Visible = PhSaveSiteStyles.Visible = true;
                FailMessage(errorMessage);
            }
        }

        public void BtnUploadImageFileNext_Click(object sender, EventArgs e)
        {
            BtnWelcomeNext.Visible = BtnSaveFilesNext.Visible = BtnSaveSiteContentsNext.Visible = BtnSaveSiteStylesNext.Visible = BtnUploadImageFileNext.Visible = PhWelcome.Visible = PhSaveFiles.Visible = PhSaveSiteContents.Visible = PhSaveSiteStyles.Visible = PhUploadImageFile.Visible = PhDone.Visible = false;

            string errorMessage;
            string samplePicPath;
            if (UploadImageFile(out errorMessage, out samplePicPath))
            {
                var siteTemplateInfo = new SiteTemplateInfo
                {
                    SiteTemplateName = TbSiteTemplateName.Text,
                    PicFileName = samplePicPath,
                    WebSiteUrl = TbWebSiteUrl.Text,
                    Description = TbDescription.Text
                };

                var siteTemplatePath = PathUtility.GetSiteTemplatesPath(TbSiteTemplateDir.Text);
                var xmlPath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath,
                    DirectoryUtils.SiteTemplates.FileMetadata);
                Serializer.SaveAsXML(siteTemplateInfo, xmlPath);

                PhDone.Visible = true;
            }
            else
            {
                BtnUploadImageFileNext.Visible = PhUploadImageFile.Visible = true;
                FailMessage(errorMessage);
            }
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageSite.GetRedirectUrl());
        }
    }
}
