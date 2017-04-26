using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalFileEdit : BasePageCms
    {
        protected TextBox FileName;
        protected RadioButtonList IsPureText;
        public DropDownList Charset;
        
        protected PlaceHolder PlaceHolder_PureText;
        protected TextBox FileContentTextBox;
        protected PlaceHolder PlaceHolder_TextEditor;
        protected UEditor FileContent;

        protected Literal ltlOpen;
        protected Literal ltlView;

		private string _relatedPath;
        private string _theFileName;
        private bool _isCreate;
        private ECharset _fileCharset;

        public static string GetOpenWindowString(int publishmentSystemId, string relatedPath, string fileName, bool isCreate)
        {
            var title = isCreate ? "新建文件" : "编辑文件";
            return PageUtils.GetOpenLayerString(title, PageUtils.GetCmsUrl(nameof(ModalFileEdit), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"RelatedPath", relatedPath},
                {"FileName", fileName},
                {"IsCreate", isCreate.ToString()}
            }), 680, 660);
        }

        public static string GetRedirectUrl(int publishmentSystemId, string relatedPath, string fileName, bool isCreate)
        {
            return PageUtils.GetCmsUrl(nameof(ModalFileEdit), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"RelatedPath", relatedPath},
                {"FileName", fileName},
                {"IsCreate", isCreate.ToString()}
            });
        }

        public static string GetOpenWindowString(int publishmentSystemId, string fileUrl)
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
            return GetOpenWindowString(publishmentSystemId, relatedPath, fileName, false);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "RelatedPath", "FileName", "IsCreate");
            _relatedPath = Body.GetQueryString("RelatedPath").Trim('/');
            if (!_relatedPath.StartsWith("@"))
            {
                _relatedPath = "@/" + _relatedPath;
            }
            _theFileName = Body.GetQueryString("FileName");
            _isCreate = Body.GetQueryBool("IsCreate");
            _fileCharset = ECharset.utf_8;
            if (PublishmentSystemInfo != null)
            {
                _fileCharset = ECharsetUtils.GetEnumType(PublishmentSystemInfo.Additional.Charset);
            }

            if (_isCreate == false)
            {
                string filePath;
                if (PublishmentSystemInfo != null)
                {
                    filePath = PathUtility.MapPath(PublishmentSystemInfo, PathUtils.Combine(_relatedPath, _theFileName));
                }
                else
                {
                    filePath = PathUtils.MapPath(PathUtils.Combine(_relatedPath, _theFileName));
                }
                if (!FileUtils.IsFileExists(filePath))
                {
                    PageUtils.RedirectToErrorPage("此文件不存在！");
                    return;
                }
            }

			if (!IsPostBack)
			{
                Charset.Items.Add(new ListItem("默认", string.Empty));
                ECharsetUtils.AddListItems(Charset);
                
                if (_isCreate == false)
                {
                    var filePath = string.Empty;
                    if (PublishmentSystemInfo != null)
                    {
                        filePath = PathUtility.MapPath(PublishmentSystemInfo, PathUtils.Combine(_relatedPath, _theFileName));
                    }
                    else
                    {
                        filePath = PathUtils.MapPath(PathUtils.Combine(_relatedPath, _theFileName));
                    }
                    FileName.Text = _theFileName;
                    FileName.Enabled = false;
                    FileContentTextBox.Text = FileUtils.ReadText(filePath, _fileCharset);
                }

                if (!_isCreate)
                {
                    if (PublishmentSystemInfo != null)
                    {
                        ltlOpen.Text =
                            $@"<a href=""{PageUtility.ParseNavigationUrl(PublishmentSystemInfo,
                                PageUtils.Combine(_relatedPath, _theFileName))}"" target=""_blank"">浏 览</a>&nbsp;&nbsp;";
                    }
                    else
                    {
                        ltlOpen.Text =
                            $@"<a href=""{PageUtils.ParseConfigRootUrl(PageUtils.Combine(_relatedPath, _theFileName))}"" target=""_blank"">浏 览</a>&nbsp;&nbsp;";
                    }
                    ltlView.Text = $@"<a href=""{ModalFileView.GetRedirectUrl(PublishmentSystemId, _relatedPath, _theFileName)}"">属 性</a>";
                }
			}
		}

        protected void IsPureText_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (TranslateUtils.ToBool(IsPureText.SelectedValue))
            {
                PlaceHolder_PureText.Visible = true;
                PlaceHolder_TextEditor.Visible = false;
                FileContentTextBox.Text = FileContent.Text;
            }
            else
            {
                PlaceHolder_PureText.Visible = false;
                PlaceHolder_TextEditor.Visible = true;
                FileContent.Text = FileContentTextBox.Text;
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

            var content = TranslateUtils.ToBool(IsPureText.SelectedValue) ? FileContentTextBox.Text : FileContent.Text;
            if (_isCreate == false)
            {
                string filePath;

                var fileExtName = PathUtils.GetExtension(_theFileName);
                if (!PathUtility.IsFileExtenstionAllowed(PublishmentSystemInfo, fileExtName))
                {
                    FailMessage("此格式不允许创建，请选择有效的文件名");
                    return;
                }

                if (PublishmentSystemInfo != null)
                {
                    filePath = PathUtility.MapPath(PublishmentSystemInfo, PathUtils.Combine(_relatedPath, _theFileName));
                }
                else
                {
                    filePath = PathUtils.MapPath(PathUtils.Combine(_relatedPath, _theFileName));
                }
                try
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(Charset.SelectedValue))
                        {
                            _fileCharset = ECharsetUtils.GetEnumType(Charset.SelectedValue);
                        }
                        FileUtils.WriteText(filePath, _fileCharset, content);
                    }
                    catch
                    {
                        FileUtils.RemoveReadOnlyAndHiddenIfExists(filePath);
                        FileUtils.WriteText(filePath, _fileCharset, content);
                    }

                    Body.AddSiteLog(PublishmentSystemId, "新建文件", $"文件名:{_theFileName}");

                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }
            }
            else
            {
                string filePath;

                var fileExtName = PathUtils.GetExtension(FileName.Text);
                if (!PathUtility.IsFileExtenstionAllowed(PublishmentSystemInfo, fileExtName))
                {
                    FailMessage("此格式不允许创建，请选择有效的文件名");
                    return;
                }

                if (PublishmentSystemInfo != null)
                {
                    filePath = PathUtility.MapPath(PublishmentSystemInfo, PathUtils.Combine(_relatedPath, FileName.Text));
                }
                else
                {
                    filePath = PathUtils.MapPath(PathUtils.Combine(_relatedPath, FileName.Text));
                }
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
                        Body.AddSiteLog(PublishmentSystemId, "编辑文件", $"文件名:{_theFileName}");
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
                        PageUtils.CloseModalPage(Page);
                    }
                    else
                    {
                        PageUtils.CloseModalPageWithoutRefresh(Page);
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
