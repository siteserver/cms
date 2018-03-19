using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalFileEdit : BasePageCms
    {
        protected TextBox TbFileName;
        protected DropDownList DdlIsPureText;
        public DropDownList DdlCharset;
        
        protected PlaceHolder PhPureText;
        protected TextBox TbFileContent;
        protected PlaceHolder PhFileContent;
        protected UEditor UeFileContent;

        protected Literal LtlOpen;
        protected Literal LtlView;

		private string _relatedPath;
        private string _theFileName;
        private bool _isCreate;
        private ECharset _fileCharset;

        public static string GetOpenWindowString(int siteId, string relatedPath, string fileName, bool isCreate)
        {
            var title = isCreate ? "新建文件" : "编辑文件";
            return LayerUtils.GetOpenScript(title, PageUtils.GetCmsUrl(siteId, nameof(ModalFileEdit), new NameValueCollection
            {
                {"RelatedPath", relatedPath},
                {"FileName", fileName},
                {"IsCreate", isCreate.ToString()}
            }), 680, 660);
        }

        public static string GetRedirectUrl(int siteId, string relatedPath, string fileName, bool isCreate)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(ModalFileEdit), new NameValueCollection
            {
                {"RelatedPath", relatedPath},
                {"FileName", fileName},
                {"IsCreate", isCreate.ToString()}
            });
        }

        public static string GetOpenWindowString(int siteId, string fileUrl)
        {
            var relatedPath = "@/";
            var fileName = fileUrl;
            if (!string.IsNullOrEmpty(fileUrl))
            {
                fileUrl = fileUrl.Trim('/');
                var i = fileUrl.LastIndexOf('/');
                if (i != -1)
                {
                    relatedPath = fileUrl.Substring(0, i + 1);
                    fileName = fileUrl.Substring(i + 1, fileUrl.Length - i - 1);
                }
            }
            return GetOpenWindowString(siteId, relatedPath, fileName, false);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "RelatedPath", "FileName", "IsCreate");
            _relatedPath = AuthRequest.GetQueryString("RelatedPath").Trim('/');
            if (!_relatedPath.StartsWith("@"))
            {
                _relatedPath = "@/" + _relatedPath;
            }
            _theFileName = AuthRequest.GetQueryString("FileName");
            _isCreate = AuthRequest.GetQueryBool("IsCreate");
            _fileCharset = ECharset.utf_8;
            if (SiteInfo != null)
            {
                _fileCharset = ECharsetUtils.GetEnumType(SiteInfo.Additional.Charset);
            }

            if (_isCreate == false)
            {
                var filePath = SiteInfo != null
                    ? PathUtility.MapPath(SiteInfo, PathUtils.Combine(_relatedPath, _theFileName))
                    : PathUtils.MapPath(PathUtils.Combine(_relatedPath, _theFileName));

                if (!FileUtils.IsFileExists(filePath))
                {
                    PageUtils.RedirectToErrorPage("此文件不存在！");
                    return;
                }
            }

            if (IsPostBack) return;

            DdlCharset.Items.Add(new ListItem("默认", string.Empty));
            ECharsetUtils.AddListItems(DdlCharset);

            if (_isCreate == false)
            {
                var filePath = SiteInfo != null ? PathUtility.MapPath(SiteInfo, PathUtils.Combine(_relatedPath, _theFileName)) : PathUtils.MapPath(PathUtils.Combine(_relatedPath, _theFileName));
                TbFileName.Text = _theFileName;
                TbFileName.Enabled = false;
                TbFileContent.Text = FileUtils.ReadText(filePath, _fileCharset);
            }

            if (_isCreate) return;

            if (SiteInfo != null)
            {
                LtlOpen.Text =
                    $@"<a class=""btn btn-default m-l-10"" href=""{PageUtility.ParseNavigationUrl(SiteInfo,
                        PageUtils.Combine(_relatedPath, _theFileName), true)}"" target=""_blank"">浏 览</a>";
            }
            else
            {
                LtlOpen.Text =
                    $@"<a class=""btn btn-default m-l-10"" href=""{PageUtils.ParseConfigRootUrl(PageUtils.Combine(_relatedPath, _theFileName))}"" target=""_blank"">浏 览</a>";
            }
            LtlView.Text = $@"<a class=""btn btn-default m-l-10"" href=""{ModalFileView.GetRedirectUrl(SiteId, _relatedPath, _theFileName)}"">查 看</a>";
        }

        protected void DdlIsPureText_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (TranslateUtils.ToBool(DdlIsPureText.SelectedValue))
            {
                PhPureText.Visible = true;
                PhFileContent.Visible = false;
                TbFileContent.Text = UeFileContent.Text;
            }
            else
            {
                PhPureText.Visible = false;
                PhFileContent.Visible = true;
                UeFileContent.Text = TbFileContent.Text;
            }
        }

        protected void Save_OnClick(object sender, EventArgs e)
        {
            Save(true);
        }

        private void Save(bool isClose)
        {
            var isSuccess = false;
            var errorMessage = string.Empty;

            var content = TranslateUtils.ToBool(DdlIsPureText.SelectedValue) ? TbFileContent.Text : UeFileContent.Text;
            if (_isCreate == false)
            {
                var fileExtName = PathUtils.GetExtension(_theFileName);
                if (!PathUtility.IsFileExtenstionAllowed(SiteInfo, fileExtName))
                {
                    FailMessage("此格式不允许创建，请选择有效的文件名");
                    return;
                }

                var filePath = SiteInfo != null
                    ? PathUtility.MapPath(SiteInfo, PathUtils.Combine(_relatedPath, _theFileName))
                    : PathUtils.MapPath(PathUtils.Combine(_relatedPath, _theFileName));

                try
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(DdlCharset.SelectedValue))
                        {
                            _fileCharset = ECharsetUtils.GetEnumType(DdlCharset.SelectedValue);
                        }
                        FileUtils.WriteText(filePath, _fileCharset, content);
                    }
                    catch
                    {
                        FileUtils.RemoveReadOnlyAndHiddenIfExists(filePath);
                        FileUtils.WriteText(filePath, _fileCharset, content);
                    }

                    AuthRequest.AddSiteLog(SiteId, "新建文件", $"文件名:{_theFileName}");

                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }
            }
            else
            {
                var fileExtName = PathUtils.GetExtension(TbFileName.Text);
                if (!PathUtility.IsFileExtenstionAllowed(SiteInfo, fileExtName))
                {
                    FailMessage("此格式不允许创建，请选择有效的文件名");
                    return;
                }

                var filePath = SiteInfo != null
                    ? PathUtility.MapPath(SiteInfo, PathUtils.Combine(_relatedPath, TbFileName.Text))
                    : PathUtils.MapPath(PathUtils.Combine(_relatedPath, TbFileName.Text));

                if (FileUtils.IsFileExists(filePath))
                {
                    errorMessage = "文件名已存在！";
                }
                else
                {
                    try
                    {
                        try
                        {
                            FileUtils.WriteText(filePath, _fileCharset, content);
                        }
                        catch
                        {
                            FileUtils.RemoveReadOnlyAndHiddenIfExists(filePath);
                            FileUtils.WriteText(filePath, _fileCharset, content);
                        }
                        AuthRequest.AddSiteLog(SiteId, "编辑文件", $"文件名:{_theFileName}");
                        isSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        errorMessage = ex.Message;
                    }
                }
            }

            if (isSuccess)
            {
                if (isClose)
                {
                    if (_isCreate)
                    {
                        LayerUtils.Close(Page);
                    }
                    else
                    {
                        LayerUtils.CloseWithoutRefresh(Page);
                    }
                }
                else
                {
                    SuccessMessage("文件保存成功！");
                }
            }
            else
            {
                FailMessage(errorMessage);
            }
        }
	}
}
