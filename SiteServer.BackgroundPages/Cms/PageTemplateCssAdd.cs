using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTemplateCssAdd : BasePageCms
    {
        public Literal LtlPageTitle;
        public TextBox TbRelatedFileName;
		public DropDownList DdlCharset;
		public TextBox TbContent;
        public PlaceHolder PhCodeMirror;
        public Button BtnEditorType;

        private string _fileName;
        private string _directoryPath;

        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageTemplateCssAdd), null);
        }

        public static string GetRedirectUrl(int siteId, string fileName)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageTemplateCssAdd), new NameValueCollection
            {
                {"FileName", fileName}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            if (Body.IsQueryExists("FileName"))
            {
                _fileName = Body.GetQueryString("FileName");
                _fileName = PathUtils.RemoveParentPath(_fileName);
            }
            _directoryPath = PathUtility.MapPath(SiteInfo, "@/css");

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.Permissions.WebSite.Template);

            LtlPageTitle.Text = string.IsNullOrEmpty(_fileName) ? "添加样式文件" : "编辑样式文件";

            var isCodeMirror = SiteInfo.Additional.ConfigTemplateIsCodeMirror;
            BtnEditorType.Text = isCodeMirror ? "采用纯文本编辑模式" : "采用代码编辑模式";
            PhCodeMirror.Visible = isCodeMirror;

            ECharsetUtils.AddListItems(DdlCharset);

            if (_fileName != null)
            {
                if (!EFileSystemTypeUtils.IsCss(PathUtils.GetExtension(_fileName)))
                {
                    PageUtils.RedirectToErrorPage("对不起，此文件无法编辑！");
                }
                else
                {
                    TbRelatedFileName.Text = PathUtils.RemoveExtension(_fileName);
                    var fileCharset = FileUtils.GetFileCharset(PathUtils.Combine(_directoryPath, _fileName));
                    ControlUtils.SelectSingleItemIgnoreCase(DdlCharset, ECharsetUtils.GetValue(fileCharset));
                    TbContent.Text = FileUtils.ReadText(PathUtils.Combine(_directoryPath, _fileName), fileCharset);
                }
            }
            else
            {
                ControlUtils.SelectSingleItemIgnoreCase(DdlCharset, SiteInfo.Additional.Charset);
            }
        }

        public void EditorType_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var isCodeMirror = SiteInfo.Additional.ConfigTemplateIsCodeMirror;
            isCodeMirror = !isCodeMirror;
            SiteInfo.Additional.ConfigTemplateIsCodeMirror = isCodeMirror;
            DataProvider.SiteDao.Update(SiteInfo);

            BtnEditorType.Text = isCodeMirror ? "采用纯文本编辑模式" : "采用代码编辑模式";
            PhCodeMirror.Visible = isCodeMirror;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            if (_fileName != null)
            {
                var isChanged = false;
                if (PathUtils.RemoveExtension(_fileName) != PathUtils.RemoveExtension(TbRelatedFileName.Text))//文件名改变
                {
                    var fileNames = DirectoryUtils.GetFileNames(_directoryPath);
                    foreach (var theFileName in fileNames)
                    {
                        var fileNameWithoutExtension = PathUtils.RemoveExtension(theFileName);
                        if (fileNameWithoutExtension == TbRelatedFileName.Text.ToLower())
                        {
                            FailMessage("样式文件修改失败，样式文件已存在！");
                            return;
                        }
                    }

                    isChanged = true;
                }

                var previousFileName = string.Empty;
                if (isChanged)
                {
                    previousFileName = _fileName;
                }

                var currentFileName = TbRelatedFileName.Text + ".css";
                var charset = ECharsetUtils.GetEnumType(DdlCharset.SelectedValue);
                FileUtils.WriteText(PathUtils.Combine(_directoryPath, currentFileName), charset, TbContent.Text);
                if (!string.IsNullOrEmpty(previousFileName))
                {
                    FileUtils.DeleteFileIfExists(PathUtils.Combine(_directoryPath, previousFileName));
                }
                Body.AddSiteLog(SiteId, "修改样式文件", $"样式文件:{currentFileName}");
                SuccessMessage("样式文件修改成功！");
                AddWaitAndRedirectScript(PageTemplateCss.GetRedirectUrl(SiteId));
            }
            else
            {
                var currentFileName = TbRelatedFileName.Text + ".css";

                var fileNames = DirectoryUtils.GetFileNames(_directoryPath);
                foreach (var theFileName in fileNames)
                {
                    if (StringUtils.EqualsIgnoreCase(theFileName, currentFileName))
                    {
                        FailMessage("样式文件添加失败，样式文件文件已存在！");
                        return;
                    }
                }

                var charset = ECharsetUtils.GetEnumType(DdlCharset.SelectedValue);
                FileUtils.WriteText(PathUtils.Combine(_directoryPath, currentFileName), charset, TbContent.Text);
                Body.AddSiteLog(SiteId, "添加样式文件", $"样式文件:{currentFileName}");
                SuccessMessage("样式文件添加成功！");
                AddWaitAndRedirectScript(PageTemplateCss.GetRedirectUrl(SiteId));
            }
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageTemplateCss.GetRedirectUrl(SiteId));
        }
    }
}
