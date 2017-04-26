using System;
using System.Collections.Specialized;
using System.IO;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalFileView : BasePageCms
    {
        public Literal ltlFileName;
        public Literal ltlFileType;
        public Literal ltlFilePath;
        public Literal ltlFileSize;
        public Literal ltlCreationTime;
        public Literal ltlLastWriteTime;
        public Literal ltlLastAccessTime;

        public Literal ltlOpen;
        public Literal ltlEdit;
        public Literal ltlChangeName;

		private string _relatedPath;
        private string _fileName;
        private string _filePath;
        private string _updateName;
        private string _hiddenClientId;

        public static string GetRedirectUrl(int publishmentSystemId, string relatedPath, string fileName)
        {
            return PageUtils.GetCmsUrl(nameof(ModalFileView), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"RelatedPath", relatedPath},
                {"FileName", fileName}
            });
        }

        public static string GetRedirectUrl(int publishmentSystemId, string relatedPath, string fileName, string updateName, string hiddenClientId)
        {
            return PageUtils.GetCmsUrl(nameof(ModalFileView), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"RelatedPath", relatedPath},
                {"FileName", fileName},
                {"UpdateName", updateName},
                {"random", DateTime.Now.Millisecond.ToString()},
                {"HiddenClientID", hiddenClientId}
            });
        }

        public static string GetOpenWindowString(int publishmentSystemId, string relatedPath, string fileName)
        {
            return PageUtils.GetOpenLayerString("查看文件属性", PageUtils.GetCmsUrl(nameof(ModalFileView), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"RelatedPath", relatedPath},
                {"FileName", fileName}
            }), 680, 660);
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
            return GetOpenWindowString(publishmentSystemId, relatedPath, fileName);
        }

        public static string GetOpenWindowStringHidden(int publishmentSystemId, string fileUrl, string hiddenClientId)
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
            return GetOpenWindowString(publishmentSystemId,hiddenClientId, relatedPath, fileName);
        }

        public static string GetOpenWindowString(int publishmentSystemId,string hiddenClientId, string relatedPath, string fileName)
        {
            return PageUtils.GetOpenLayerString("查看文件属性", PageUtils.GetCmsUrl(nameof(ModalFileView), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"HiddenClientID", hiddenClientId},
                {"RelatedPath", relatedPath},
                {"FileName", fileName}
            }), 680, 660);
        }

        public static string GetOpenWindowStringWithTextBoxValue(int publishmentSystemId, string textBoxId)
        {
            return PageUtils.GetOpenLayerStringWithTextBoxValue("查看文件属性", PageUtils.GetCmsUrl(nameof(ModalFileView), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"TextBoxID", textBoxId}
            }), textBoxId, 680, 660);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            if (Body.IsQueryExists("TextBoxID"))
            {
                var textBoxId = Body.GetQueryString("TextBoxID");
                var virtualUrl = Body.GetQueryString(textBoxId);
                _filePath = PathUtility.MapPath(PublishmentSystemInfo, virtualUrl);
                _relatedPath = PageUtils.RemoveFileNameFromUrl(virtualUrl);
                _fileName = PathUtils.GetFileName(_filePath);
            }
            else
            {
                _relatedPath = Body.GetQueryString("RelatedPath").Trim('/');
                _hiddenClientId = Body.GetQueryString("HiddenClientID");
                if (!_relatedPath.StartsWith("~") && !_relatedPath.StartsWith("@"))
                {
                    _relatedPath = "@/" + _relatedPath;
                }
                _fileName = Body.GetQueryString("FileName");
                _updateName = Body.GetQueryString("UpdateName");
                if (!string.IsNullOrEmpty(_updateName))
                {
                    _fileName = _updateName;
                }
                _filePath = PathUtility.MapPath(PublishmentSystemInfo, PathUtils.Combine(_relatedPath, _fileName));
            }

            if (!FileUtils.IsFileExists(_filePath))
            {
                PageUtils.RedirectToErrorPage("此文件不存在！");
                return;
            }

			if (!IsPostBack)
			{
                var fileInfo = new FileInfo(_filePath);
                var fileType = EFileSystemTypeUtils.GetEnumType(fileInfo.Extension);
                if (Body.IsQueryExists("UpdateName"))
                {
                    ltlFileName.Text = Body.GetQueryString("UpdateName");
                }
                else
                {
                    ltlFileName.Text = fileInfo.Name;
                }
                ltlFileType.Text = EFileSystemTypeUtils.GetText(fileType);
                ltlFilePath.Text = Path.GetDirectoryName(_filePath);
                ltlFileSize.Text = TranslateUtils.GetKbSize(fileInfo.Length) + " KB";
                ltlCreationTime.Text = fileInfo.CreationTime.ToString("yyyy-MM-dd hh:mm:ss");
                ltlLastWriteTime.Text = fileInfo.LastWriteTime.ToString("yyyy-MM-dd hh:mm:ss");
                ltlLastAccessTime.Text = fileInfo.LastAccessTime.ToString("yyyy-MM-dd hh:mm:ss");

                ltlOpen.Text =
                    $@"<a href=""{PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, _filePath)}"" target=""_blank"">浏 览</a>&nbsp;&nbsp;";
			    if (EFileSystemTypeUtils.IsTextEditable(fileType))
			    {
                    ltlEdit.Text = $@"<a href=""{ModalFileEdit.GetRedirectUrl(PublishmentSystemId, _relatedPath, _fileName, false)}"">修 改</a>&nbsp;&nbsp;";
			    }
			    if (!string.IsNullOrEmpty(_hiddenClientId))
                {
                    ltlChangeName.Text =
                        $@"<a href=""javascript:;"" onclick=""{ModalFileChangeName.GetOpenWindowString(
                            PublishmentSystemId, _relatedPath, fileInfo.Name, _hiddenClientId)}"">改 名</a>";
                }
			}
		}
	}
}
