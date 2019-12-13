using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Repositories;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTemplateAssetsAdd : BasePageCms
    {
        public Literal LtlPageTitle;
        public TextBox TbRelatedFileName;
        public TextBox TbContent;
        public PlaceHolder PhCodeMirror;
        public PlaceHolder PhCodeMirrorInclude;
        public PlaceHolder PhCodeMirrorJs;
        public PlaceHolder PhCodeMirrorCss;
        public Button BtnEditorType;

        private string _type;
        private string _name;
        private string _ext;
        private string _assetsDir;
        private string _directoryPath;
        private string _fileName;

        public static string GetRedirectUrlToAdd(int siteId, string type)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageTemplateAssetsAdd), new NameValueCollection
            {
                {"type", type}
            });
        }

        public static string GetRedirectUrlToEdit(int siteId, string type, string fileName)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageTemplateAssetsAdd), new NameValueCollection
            {
                {"type", type},
                {"fileName", fileName}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "type");
            _type = AuthRequest.GetQueryString("type");

            var permissions = string.Empty;
            if (_type == PageTemplateAssets.TypeInclude)
            {
                permissions = Constants.SitePermissions.TemplatesIncludes;
                _name = PageTemplateAssets.NameInclude;
                _ext = PageTemplateAssets.ExtInclude;
                _assetsDir = Site.TemplatesAssetsIncludeDir.Trim('/');
                PhCodeMirrorInclude.Visible = true;
            }
            else if (_type == PageTemplateAssets.TypeJs)
            {
                permissions = Constants.SitePermissions.TemplatesJs;
                _name = PageTemplateAssets.NameJs;
                _ext = PageTemplateAssets.ExtJs;
                _assetsDir = Site.TemplatesAssetsJsDir.Trim('/');
                PhCodeMirrorJs.Visible = true;
            }
            else if (_type == PageTemplateAssets.TypeCss)
            {
                permissions = Constants.SitePermissions.TemplatesCss;
                _name = PageTemplateAssets.NameCss;
                _ext = PageTemplateAssets.ExtCss;
                _assetsDir = Site.TemplatesAssetsCssDir.Trim('/');
                PhCodeMirrorCss.Visible = true;
            }

            if (string.IsNullOrEmpty(_assetsDir)) return;

            _directoryPath = PathUtility.MapPath(Site, "@/" + _assetsDir);

            if (AuthRequest.IsQueryExists("fileName"))
            {
                _fileName = AuthRequest.GetQueryString("fileName");
                _fileName = PathUtils.RemoveParentPath(_fileName);
            }

            if (IsPostBack) return;

            VerifySitePermissions(permissions);

            LtlPageTitle.Text = string.IsNullOrEmpty(_fileName) ? $"添加{_name}" : $"编辑{_name}";

            var isCodeMirror = Site.ConfigTemplateIsCodeMirror;
            BtnEditorType.Text = isCodeMirror ? "采用纯文本编辑模式" : "采用代码编辑模式";
            PhCodeMirror.Visible = isCodeMirror;

            if (_fileName != null)
            {
                if (!StringUtils.EqualsIgnoreCase(PathUtils.GetExtension(_fileName), _ext))
                {
                    PageUtils.RedirectToErrorPage("对不起，此文件格式无法编辑！");
                }
                else
                {
                    TbRelatedFileName.Text = _fileName;
                    var fileCharset = FileUtils.GetFileCharset(PathUtils.Combine(_directoryPath, _fileName));
                    
                    TbContent.Text = FileUtils.ReadText(PathUtils.Combine(_directoryPath, _fileName), fileCharset);
                }
            }
        }

        public void EditorType_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var isCodeMirror = Site.ConfigTemplateIsCodeMirror;
            isCodeMirror = !isCodeMirror;
            Site.ConfigTemplateIsCodeMirror = isCodeMirror;
            DataProvider.SiteRepository.UpdateAsync(Site).GetAwaiter().GetResult();

            BtnEditorType.Text = isCodeMirror ? "采用纯文本编辑模式" : "采用代码编辑模式";
            PhCodeMirror.Visible = isCodeMirror;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var relatedFileName = TbRelatedFileName.Text;
            if (!StringUtils.EndsWithIgnoreCase(relatedFileName, _ext))
            {
                relatedFileName += _ext;
            }

            if (_fileName != null)
            {
                var isChanged = false;
                if (!StringUtils.EqualsIgnoreCase(_fileName, relatedFileName))//文件名改变
                {
                    var fileNames = DirectoryUtils.GetFileNames(_directoryPath);
                    foreach (var theFileName in fileNames)
                    {
                        if (StringUtils.EqualsIgnoreCase(theFileName, relatedFileName))
                        {
                            FailMessage($"{_name}修改失败，文件已存在！");
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
                
                FileUtils.WriteText(PathUtils.Combine(_directoryPath, relatedFileName), TbContent.Text);
                if (!string.IsNullOrEmpty(previousFileName))
                {
                    FileUtils.DeleteFileIfExists(PathUtils.Combine(_directoryPath, previousFileName));
                }
                AuthRequest.AddSiteLogAsync(SiteId, $"修改{_name}", $"{_name}:{relatedFileName}").GetAwaiter().GetResult();
                SuccessMessage($"{_name}修改成功！");
                AddWaitAndRedirectScript(PageTemplateAssets.GetRedirectUrl(SiteId, _type));
            }
            else
            {
                var fileNames = DirectoryUtils.GetFileNames(_directoryPath);
                foreach (var theFileName in fileNames)
                {
                    if (StringUtils.EqualsIgnoreCase(theFileName, relatedFileName))
                    {
                        FailMessage($"{_name}添加失败，文件已存在！");
                        return;
                    }
                }

                FileUtils.WriteText(PathUtils.Combine(_directoryPath, relatedFileName), TbContent.Text);
                AuthRequest.AddSiteLogAsync(SiteId, $"添加{_name}", $"{_name}:{relatedFileName}").GetAwaiter().GetResult();
                SuccessMessage($"{_name}添加成功！");
                AddWaitAndRedirectScript(PageTemplateAssets.GetRedirectUrl(SiteId, _type));
            }
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageTemplateAssets.GetRedirectUrl(SiteId, _type));
        }
    }
}
