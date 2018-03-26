using System;
using System.Collections.Specialized;
using System.IO;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalFileView : BasePageCms
    {
        public Literal LtlFileName;
        public Literal LtlFileType;
        public Literal LtlFilePath;
        public Literal LtlFileSize;
        public Literal LtlCreationTime;
        public Literal LtlLastWriteTime;
        public Literal LtlLastAccessTime;
        public Literal LtlOpen;
        public Literal LtlEdit;
        public Literal LtlChangeName;

		private string _relatedPath;
        private string _fileName;
        private string _filePath;
        private string _updateName;
        private string _hiddenClientId;

        public static string GetRedirectUrl(int siteId, string relatedPath, string fileName)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(ModalFileView), new NameValueCollection
            {
                {"RelatedPath", relatedPath},
                {"FileName", fileName}
            });
        }

        public static string GetRedirectUrl(int siteId, string relatedPath, string fileName, string updateName, string hiddenClientId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(ModalFileView), new NameValueCollection
            {
                {"RelatedPath", relatedPath},
                {"FileName", fileName},
                {"UpdateName", updateName},
                {"random", DateTime.Now.Millisecond.ToString()},
                {"HiddenClientID", hiddenClientId}
            });
        }

        public static string GetOpenWindowString(int siteId, string relatedPath, string fileName)
        {
            return LayerUtils.GetOpenScript("查看文件属性", PageUtils.GetCmsUrl(siteId, nameof(ModalFileView), new NameValueCollection
            {
                {"RelatedPath", relatedPath},
                {"FileName", fileName}
            }), 680, 660);
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
            return GetOpenWindowString(siteId, relatedPath, fileName);
        }

        public static string GetOpenWindowStringHidden(int siteId, string fileUrl, string hiddenClientId)
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
            return GetOpenWindowString(siteId,hiddenClientId, relatedPath, fileName);
        }

        public static string GetOpenWindowString(int siteId,string hiddenClientId, string relatedPath, string fileName)
        {
            return LayerUtils.GetOpenScript("查看文件属性", PageUtils.GetCmsUrl(siteId, nameof(ModalFileView), new NameValueCollection
            {
                {"HiddenClientID", hiddenClientId},
                {"RelatedPath", relatedPath},
                {"FileName", fileName}
            }), 680, 660);
        }

        public static string GetOpenWindowStringWithTextBoxValue(int siteId, string textBoxId)
        {
            return LayerUtils.GetOpenScriptWithTextBoxValue("查看文件属性", PageUtils.GetCmsUrl(siteId, nameof(ModalFileView), new NameValueCollection
            {
                {"TextBoxID", textBoxId}
            }), textBoxId, 680, 660);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");
            if (AuthRequest.IsQueryExists("TextBoxID"))
            {
                var textBoxId = AuthRequest.GetQueryString("TextBoxID");
                var virtualUrl = AuthRequest.GetQueryString(textBoxId);
                _filePath = PathUtility.MapPath(SiteInfo, virtualUrl);
                _relatedPath = PageUtils.RemoveFileNameFromUrl(virtualUrl);
                _fileName = PathUtils.GetFileName(_filePath);
            }
            else
            {
                _relatedPath = AuthRequest.GetQueryString("RelatedPath").Trim('/');
                _hiddenClientId = AuthRequest.GetQueryString("HiddenClientID");
                if (!_relatedPath.StartsWith("~") && !_relatedPath.StartsWith("@"))
                {
                    _relatedPath = "@/" + _relatedPath;
                }
                _fileName = AuthRequest.GetQueryString("FileName");
                _updateName = AuthRequest.GetQueryString("UpdateName");
                if (!string.IsNullOrEmpty(_updateName))
                {
                    _fileName = _updateName;
                }
                _filePath = PathUtility.MapPath(SiteInfo, PathUtils.Combine(_relatedPath, _fileName));
            }

            if (!FileUtils.IsFileExists(_filePath))
            {
                PageUtils.RedirectToErrorPage("此文件不存在！");
                return;
            }

            if (IsPostBack) return;

            var fileInfo = new FileInfo(_filePath);
            var fileType = EFileSystemTypeUtils.GetEnumType(fileInfo.Extension);
            LtlFileName.Text = AuthRequest.IsQueryExists("UpdateName") ? AuthRequest.GetQueryString("UpdateName") : fileInfo.Name;
            LtlFileType.Text = EFileSystemTypeUtils.GetText(fileType);
            LtlFilePath.Text = Path.GetDirectoryName(_filePath);
            LtlFileSize.Text = TranslateUtils.GetKbSize(fileInfo.Length) + " KB";
            LtlCreationTime.Text = fileInfo.CreationTime.ToString("yyyy-MM-dd hh:mm:ss");
            LtlLastWriteTime.Text = fileInfo.LastWriteTime.ToString("yyyy-MM-dd hh:mm:ss");
            LtlLastAccessTime.Text = fileInfo.LastAccessTime.ToString("yyyy-MM-dd hh:mm:ss");

            LtlOpen.Text =
                $@"<a class=""btn btn-default m-l-5"" href=""{PageUtility.GetSiteUrlByPhysicalPath(SiteInfo, _filePath, true)}"" target=""_blank"">浏 览</a>";
            if (EFileSystemTypeUtils.IsTextEditable(fileType))
            {
                LtlEdit.Text = $@"<a class=""btn btn-default m-l-5"" href=""{ModalFileEdit.GetRedirectUrl(SiteId, _relatedPath, _fileName, false)}"">修 改</a>";
            }
            if (!string.IsNullOrEmpty(_hiddenClientId))
            {
                LtlChangeName.Text =
                    $@"<a class=""btn btn-default m-l-5"" href=""javascript:;"" onclick=""{ModalFileChangeName.GetOpenWindowString(
                        SiteId, _relatedPath, fileInfo.Name, _hiddenClientId)}"">改 名</a>";
            }
        }
	}
}
