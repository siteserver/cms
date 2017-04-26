using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.IO.FileManagement;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Sys
{
    public class PageSiteTemplateSave : BasePageCms
    {
        public enum WizardPanel
        {
            Welcome,
            SaveFiles,
            SaveSiteContents,
            SaveSiteStyles,
            UploadImageFile,
            Done,
            OperatingError,
        }

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
        public PlaceHolder PhOperatingError;
        public Literal LtlErrorMessage;
        public Button BtnPrevious;
        public Button BtnNext;

        private ExportObject _exportObject;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            _exportObject = new ExportObject(PublishmentSystemId);

            if (IsPostBack) return;

            BreadCrumbSys(AppManager.Sys.LeftMenu.Site, "保存站点模板", AppManager.Sys.Permission.SysSite);

            if (PublishmentSystemInfo.IsHeadquarters)
            {
                TbSiteTemplateDir.Text = "T_" + PublishmentSystemInfo.PublishmentSystemName;
            }
            else
            {
                TbSiteTemplateDir.Text = "T_" + PublishmentSystemInfo.PublishmentSystemDir.Replace("\\", "_");
            }
            TbSiteTemplateName.Text = PublishmentSystemInfo.PublishmentSystemName;

            EBooleanUtils.AddListItems(RblIsSaveAllFiles, "全部文件", "指定文件");
            ControlUtils.SelectListItemsIgnoreCase(RblIsSaveAllFiles, true.ToString());

            var publishmentSystemDirList = DataProvider.PublishmentSystemDao.GetLowerPublishmentSystemDirListThatNotIsHeadquarters();
            var fileSystems = FileManager.GetFileSystemInfoExtendCollection(PathUtility.GetPublishmentSystemPath(PublishmentSystemInfo), true);
            foreach (FileSystemInfoExtend fileSystem in fileSystems)
            {
                if (!fileSystem.IsDirectory) continue;

                var isPublishmentSystemDirectory = false;
                if (PublishmentSystemInfo.IsHeadquarters)
                {
                    foreach (var publishmentSystemDir in publishmentSystemDirList)
                    {
                        if (StringUtils.EqualsIgnoreCase(publishmentSystemDir, fileSystem.Name))
                        {
                            isPublishmentSystemDirectory = true;
                        }
                    }
                }
                if (!isPublishmentSystemDirectory && !DirectoryUtils.IsSystemDirectory(fileSystem.Name))
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
            ControlUtils.SelectListItemsIgnoreCase(RblIsSaveContents, true.ToString());

            EBooleanUtils.AddListItems(RblIsSaveAllChannels, "全部栏目", "指定栏目");
            ControlUtils.SelectListItemsIgnoreCase(RblIsSaveAllChannels, true.ToString());

            LtlChannelTree.Text = GetChannelTreeHtml();

            SetActivePlaceHolder(WizardPanel.Welcome, PhWelcome);
        }

        private string GetChannelTreeHtml()
        {
            var htmlBuilder = new StringBuilder();

            var treeDirectoryUrl = SiteServerAssets.GetIconUrl("tree");

            htmlBuilder.Append("<span id='ChannelSelectControl'>");
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(PublishmentSystemId);
            var isLastNodeArray = new bool[nodeIdList.Count];
            foreach (var nodeId in nodeIdList)
            {
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);
                htmlBuilder.Append(GetChannelTreeTitle(nodeInfo, treeDirectoryUrl, isLastNodeArray));
                htmlBuilder.Append("<br/>");
            }
            htmlBuilder.Append("</span>");
            return htmlBuilder.ToString();
        }

        private string GetChannelTreeTitle(NodeInfo nodeInfo, string treeDirectoryUrl, bool[] isLastNodeArray)
        {
            var itemBuilder = new StringBuilder();
            if (nodeInfo.NodeId == PublishmentSystemId)
            {
                nodeInfo.IsLastNode = true;
            }
            if (nodeInfo.IsLastNode == false)
            {
                isLastNodeArray[nodeInfo.ParentsCount] = false;
            }
            else
            {
                isLastNodeArray[nodeInfo.ParentsCount] = true;
            }
            for (var i = 0; i < nodeInfo.ParentsCount; i++)
            {
                itemBuilder.Append(isLastNodeArray[i]
                    ? $"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/tree_empty.gif\"/>"
                    : $"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/tree_line.gif\"/>");
            }
            if (nodeInfo.IsLastNode)
            {
                itemBuilder.Append(nodeInfo.ChildrenCount > 0
                    ? $"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/tree_plusbottom.gif\"/>"
                    : $"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/tree_minusbottom.gif\"/>");
            }
            else
            {
                itemBuilder.Append(nodeInfo.ChildrenCount > 0
                    ? $"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/tree_plusmiddle.gif\"/>"
                    : $"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/tree_minusmiddle.gif\"/>");
            }

            itemBuilder.Append(
                $@"<label class=""checkbox inline""><input type=""checkbox"" name=""NodeIDCollection"" value=""{nodeInfo
                    .NodeId}""/> {nodeInfo.NodeName} {$"&nbsp;<span style=\"font-size:8pt;font-family:arial\" class=\"gray\">({nodeInfo.ContentNum})</span>"}</label>");

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

        private WizardPanel CurrentWizardPanel
        {
            get
            {
                if (ViewState["WizardPanel"] != null)
                    return (WizardPanel)ViewState["WizardPanel"];

                return WizardPanel.Welcome;
            }
            set
            {
                ViewState["WizardPanel"] = value;
            }
        }

        private void SetActivePlaceHolder(WizardPanel panel, Control controlToShow)
        {
            var currentPanel = FindControl("ph" + CurrentWizardPanel);
            if (currentPanel != null)
                currentPanel.Visible = false;

            if (panel == WizardPanel.Welcome)
            {
                BtnPrevious.CssClass = "btn disabled";
                BtnPrevious.Enabled = false;
                BtnNext.CssClass = "btn btn-primary";
                BtnNext.Enabled = true;
            }
            else if (panel == WizardPanel.Done)
            {
                BtnPrevious.CssClass = "btn disabled";
                BtnPrevious.Enabled = false;
                BtnNext.CssClass = "btn btn-primary disabled";
                BtnNext.Enabled = false;
            }
            else if (panel == WizardPanel.OperatingError)
            {
                BtnPrevious.CssClass = "btn disabled";
                BtnPrevious.Enabled = false;
                BtnNext.CssClass = "btn btn-primary disabled";
                BtnNext.Enabled = false;
            }
            else
            {
                BtnPrevious.CssClass = "btn";
                BtnPrevious.Enabled = true;
                BtnNext.CssClass = "btn btn-primary";
                BtnNext.Enabled = true;
            }

            controlToShow.Visible = true;
            CurrentWizardPanel = panel;
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

                var nodeIdList = TranslateUtils.StringCollectionToIntList(Request.Form["NodeIDCollection"]);
                _exportObject.ExportSiteContent(siteContentDirectoryPath, isSaveContents, isSaveAllChannels, nodeIdList);

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
                SiteTemplateManager.ExportPublishmentSystemToSiteTemplate(PublishmentSystemInfo, TbSiteTemplateDir.Text);

                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }
        }

        public void BtnNext_Click(object sender, EventArgs e)
        {
            string errorMessage;
            if (CurrentWizardPanel == WizardPanel.Welcome)
            {
                if (SiteTemplateManager.Instance.IsSiteTemplateDirectoryExists(TbSiteTemplateDir.Text))
                {
                    LtlErrorMessage.Text = "站点模板保存失败，站点模板已存在！";
                    SetActivePlaceHolder(WizardPanel.OperatingError, PhOperatingError);
                }
                else
                {
                    SetActivePlaceHolder(WizardPanel.SaveFiles, PhSaveFiles);
                }
            }
            else if (CurrentWizardPanel == WizardPanel.SaveFiles)
            {
                if (SaveFiles(out errorMessage))
                {
                    Body.AddAdminLog("保存站点模板", $"站点:{PublishmentSystemInfo.PublishmentSystemName}");

                    SetActivePlaceHolder(WizardPanel.SaveSiteContents, PhSaveSiteContents);
                }
                else
                {
                    LtlErrorMessage.Text = errorMessage;
                    SetActivePlaceHolder(WizardPanel.OperatingError, PhOperatingError);
                }
            }
            else if (CurrentWizardPanel == WizardPanel.SaveSiteContents)
            {
                if (SaveSiteContents(out errorMessage))
                {
                    SetActivePlaceHolder(WizardPanel.SaveSiteStyles, PhSaveSiteStyles);
                }
                else
                {
                    LtlErrorMessage.Text = errorMessage;
                    SetActivePlaceHolder(WizardPanel.OperatingError, PhOperatingError);
                }
            }
            else if (CurrentWizardPanel == WizardPanel.SaveSiteStyles)
            {
                if (SaveSiteStyles(out errorMessage))
                {
                    SetActivePlaceHolder(WizardPanel.UploadImageFile, PhUploadImageFile);
                }
                else
                {
                    LtlErrorMessage.Text = errorMessage;
                    SetActivePlaceHolder(WizardPanel.OperatingError, PhOperatingError);
                }
            }
            else if (CurrentWizardPanel == WizardPanel.UploadImageFile)
            {
                string samplePicPath;
                if (UploadImageFile(out errorMessage, out samplePicPath))
                {
                    try
                    {
                        var siteTemplateInfo = new SiteTemplateInfo
                        {
                            SiteTemplateName = TbSiteTemplateName.Text,
                            PublishmentSystemType =
                                EPublishmentSystemTypeUtils.GetValue(PublishmentSystemInfo.PublishmentSystemType),
                            PicFileName = samplePicPath,
                            WebSiteUrl = TbWebSiteUrl.Text,
                            Description = TbDescription.Text
                        };

                        var siteTemplatePath = PathUtility.GetSiteTemplatesPath(TbSiteTemplateDir.Text);
                        var xmlPath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath,
                            DirectoryUtils.SiteTemplates.FileMetadata);
                        Serializer.SaveAsXML(siteTemplateInfo, xmlPath);

                        SetActivePlaceHolder(WizardPanel.Done, PhDone);
                    }
                    catch (Exception ex)
                    {
                        LtlErrorMessage.Text = ex.Message;
                        SetActivePlaceHolder(WizardPanel.OperatingError, PhOperatingError);
                    }
                }
                else
                {
                    LtlErrorMessage.Text = errorMessage;
                    SetActivePlaceHolder(WizardPanel.OperatingError, PhOperatingError);
                }
            }
        }

        public void BtnPrevious_Click(object sender, EventArgs e)
        {
            if (CurrentWizardPanel == WizardPanel.Welcome)
            {
            }
            else if (CurrentWizardPanel == WizardPanel.SaveFiles)
            {
                SetActivePlaceHolder(WizardPanel.Welcome, PhWelcome);
            }
            else if (CurrentWizardPanel == WizardPanel.SaveSiteContents)
            {
                SetActivePlaceHolder(WizardPanel.SaveFiles, PhSaveFiles);
            }
            else if (CurrentWizardPanel == WizardPanel.SaveSiteStyles)
            {
                SetActivePlaceHolder(WizardPanel.SaveSiteContents, PhSaveSiteContents);
            }
            else if (CurrentWizardPanel == WizardPanel.UploadImageFile)
            {
                SetActivePlaceHolder(WizardPanel.SaveSiteStyles, PhSaveSiteStyles);
            }
        }

    }
}
