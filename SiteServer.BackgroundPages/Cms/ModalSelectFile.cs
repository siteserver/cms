using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.Utils.Images;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils.Enumerations;
using SiteServer.Utils.IO;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalSelectFile : BasePageCms
    {
		public Literal LtlCurrentDirectory;
        public Literal LtlFileSystems;
		public Button BtnUpload;
		public DropDownList DdlListType;

		private string _currentRootPath;
        private string _rootPath;
        private string _directoryPath;
        private string _hiddenClientId;

        //限制限制文件夹路径最高路径
        private const string TopPath = "@/upload/files";

		private string GetRedirectUrl(string path)
		{
            //here, limit top path
            if (!DirectoryUtils.IsInDirectory(TopPath, path))
                path = TopPath;
            return PageUtils.GetCmsUrl(SiteId, nameof(ModalSelectFile), new NameValueCollection
            {
                {"RootPath", _rootPath},
                {"CurrentRootPath", path},
                {"HiddenClientID", _hiddenClientId}
            });
		}

		private string GetRedirectUrlWithType(string path, string listTypeStr)
		{
            //here, limit top path
            if (!DirectoryUtils.IsInDirectory(TopPath, path))
                path = TopPath;
            return PageUtils.GetCmsUrl(SiteId, nameof(ModalSelectFile), new NameValueCollection
            {
                {"RootPath", _rootPath},
                {"CurrentRootPath", path},
                {"ListType", listTypeStr},
                {"HiddenClientID", _hiddenClientId}
            });
		}

        public static string GetOpenWindowString(int siteId, string hiddenClientId)
        {
            var currentRootPath = string.Empty;
            return GetOpenWindowString(siteId, hiddenClientId, currentRootPath);
        }

        public static string GetOpenWindowString(int siteId, string hiddenClientId, string currentRootPath)
        {
            return LayerUtils.GetOpenScript("选择文件",
                PageUtils.GetCmsUrl(siteId, nameof(ModalSelectFile), new NameValueCollection
                {
                    {"RootPath", "@"},
                    {"CurrentRootPath", currentRootPath},
                    {"HiddenClientID", hiddenClientId}
                }));
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "RootPath", "CurrentRootPath", "HiddenClientID");
			
			_rootPath = AuthRequest.GetQueryString("RootPath").TrimEnd('/');
			_currentRootPath = AuthRequest.GetQueryString("CurrentRootPath");
            _hiddenClientId = AuthRequest.GetQueryString("HiddenClientID");

            if (string.IsNullOrEmpty(_currentRootPath))
            {
                _currentRootPath = SiteInfo.Additional.ConfigSelectFileCurrentUrl.TrimEnd('/');
            }
            else
            {
                SiteInfo.Additional.ConfigSelectFileCurrentUrl = _currentRootPath;
                DataProvider.SiteDao.Update(SiteInfo);
            }
            _currentRootPath = _currentRootPath.TrimEnd('/');

			_directoryPath = PathUtility.MapPath(SiteInfo, _currentRootPath);
            DirectoryUtils.CreateDirectoryIfNotExists(_directoryPath);
			if (!DirectoryUtils.IsDirectoryExists(_directoryPath))
			{
                PageUtils.RedirectToErrorPage("文件夹不存在！");
                return;
			}

            if (Page.IsPostBack) return;

            BtnUpload.Attributes.Add("onclick", ModalUploadFile.GetOpenWindowStringToList(SiteId, EUploadType.File, _currentRootPath));

            DdlListType.Items.Add(new ListItem("显示缩略图", "Image"));
            DdlListType.Items.Add(new ListItem("显示详细信息", "List"));
            if (AuthRequest.IsQueryExists("ListType"))
            {
                ControlUtils.SelectSingleItem(DdlListType, AuthRequest.GetQueryString("ListType"));
            }

            var previousUrls = Session["PreviousUrls"] as ArrayList ?? new ArrayList();
            var currentUrl = GetRedirectUrl(_currentRootPath);
            if (previousUrls.Count > 0)
            {
                var url = previousUrls[previousUrls.Count - 1] as string;
                if (!string.Equals(url, currentUrl))
                {
                    previousUrls.Add(currentUrl);
                    Session["PreviousUrls"] = previousUrls;
                }
            }
            else
            {
                previousUrls.Add(currentUrl);
                Session["PreviousUrls"] = previousUrls;
            }

            var navigationBuilder = new StringBuilder();
            var directoryNames = _currentRootPath.Split('/');
            var linkCurrentRootPath = _rootPath;
            foreach (var directoryName in directoryNames)
            {
                if (!string.IsNullOrEmpty(directoryName))
                {
                    if (directoryName.Equals("~"))
                    {
                        navigationBuilder.Append($"<a href='{GetRedirectUrl(_rootPath)}'>根目录</a>");
                    }
                    else if (directoryName.Equals("@"))
                    {
                        navigationBuilder.Append(
                            $"<a href='{GetRedirectUrl(_rootPath)}'>{SiteManager.GetSiteInfo(SiteId).SiteDir}</a>");
                    }
                    else
                    {
                        linkCurrentRootPath += "/" + directoryName;
                        navigationBuilder.Append($"<a href='{GetRedirectUrl(linkCurrentRootPath)}'>{directoryName}</a>");
                    }
                    navigationBuilder.Append("\\");
                }
            }
            LtlCurrentDirectory.Text = navigationBuilder.ToString();

            FillFileSystems(false);
        }

		public void LinkButton_Command(object sender, CommandEventArgs e)
		{
			var navigationUrl = string.Empty;
			if (e.CommandName.Equals("NavigationBar"))
			{
				if (e.CommandArgument.Equals("Back"))
				{
					var previousUrls = Session["PreviousUrls"] as ArrayList;
					if (previousUrls != null && previousUrls.Count > 1)
					{
						previousUrls.RemoveAt(previousUrls.Count - 1);
						Session["PreviousUrls"] = previousUrls;

						navigationUrl = previousUrls[previousUrls.Count - 1] as string;
					}
				}
				else if (e.CommandArgument.Equals("Up"))
				{
					if (_currentRootPath.StartsWith(_rootPath) && _currentRootPath.Length > _rootPath.Length)
					{
						var index = _currentRootPath.LastIndexOf("/", StringComparison.Ordinal);
						if (index != -1)
						{
							_currentRootPath = _currentRootPath.Substring(0, index);
							navigationUrl = GetRedirectUrl(_currentRootPath);
						}
					}
				}
			}

			if (string.IsNullOrEmpty(navigationUrl))
			{
				navigationUrl = GetRedirectUrl(_currentRootPath);
			}
			PageUtils.Redirect(navigationUrl);
		}

		public void ddlListType_SelectedIndexChanged(object sender, EventArgs e)
		{
			var navigationUrl = GetRedirectUrlWithType(_currentRootPath, DdlListType.SelectedValue);
			PageUtils.Redirect(navigationUrl);
		}

		#region Helper
		private void FillFileSystems(bool isReload)
		{
			const string cookieName = "SiteServer.BackgroundPages.Cms.Modal.SelectAttachment";
			var isSetCookie = AuthRequest.IsQueryExists("ListType");
			if (!isSetCookie)
			{
				var cookieExists = false;
				if (CookieUtils.IsExists(cookieName))
				{
					var cookieValue = CookieUtils.GetCookie(cookieName);
					foreach (ListItem item in DdlListType.Items)
					{
						if (string.Equals(item.Value, cookieValue))
						{
							cookieExists = true;
							item.Selected = true;
						}
					}
				}
				if (!cookieExists)
				{
					CookieUtils.SetCookie(cookieName, DdlListType.SelectedValue, DateTime.MaxValue);
				}
			}
			else
			{
                CookieUtils.SetCookie(cookieName, AuthRequest.GetQueryString("ListType"), DateTime.MaxValue);
			}
			if (DdlListType.SelectedValue == "List")
			{
				FillFileSystemsToList(isReload);
			}
			else if (DdlListType.SelectedValue == "Image")
			{
				FillFileSystemsToImage(isReload);
			}
		}

        public static string GetFileSystemIconUrl(SiteInfo siteInfo, FileSystemInfoExtend fileInfo, bool isLargeIcon)
        {
            EFileSystemType fileSystemType;
            if (PathUtility.IsVideoExtenstionAllowed(siteInfo, fileInfo.Type))
            {
                fileSystemType = EFileSystemType.Video;
            }
            else if (PathUtility.IsImageExtenstionAllowed(siteInfo, fileInfo.Type))
            {
                fileSystemType = EFileSystemType.Image;
            }
            else
            {
                fileSystemType = EFileSystemTypeUtils.GetEnumType(fileInfo.Type);
            }
            return SiteServerAssets.GetFileSystemIconUrl(fileSystemType, isLargeIcon);
        }

        private void FillFileSystemsToImage(bool isReload)
		{
			var builder = new StringBuilder();
			builder.Append(@"<table class=""table table-noborder table-hover"">");
			
			var directoryUrl = PageUtility.GetSiteUrlByPhysicalPath(SiteInfo, _directoryPath, true);
            var backgroundImageUrl = SiteServerAssets.GetIconUrl("filesystem/management/background.gif");
			var directoryImageUrl = SiteServerAssets.GetFileSystemIconUrl(EFileSystemType.Directory, true);

			var fileSystemInfoExtendCollection = FileManager.GetFileSystemInfoExtendCollection(_directoryPath, isReload);

			var mod = 0;
			foreach (FileSystemInfoExtend subDirectoryInfo in fileSystemInfoExtendCollection.Folders)
			{
				if (mod % 4 == 0)
				{
					builder.Append("<tr>");
				}
				var linkUrl = GetRedirectUrl(PageUtils.Combine(_currentRootPath, subDirectoryInfo.Name));

				builder.Append($@"
<td>
		<table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"">
			<tr>
				<td style=""height:100px; width:100px; text-align:center; vertical-align:middle;"">
					<table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"">
						<tr>
							<td background=""{backgroundImageUrl}"" style=""background-repeat:no-repeat; background-position:center;height:96px; width:96px; text-align:center; vertical-align:middle;"" align=""center""><a href=""{linkUrl}""><img src=""{directoryImageUrl}"" border=0 /></a></td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td style=""height:20px; width:100%; text-align:center; vertical-align:middle;""><a href=""{linkUrl}"">{StringUtils
				    .MaxLengthText(subDirectoryInfo.Name, 8)}</a></td>
			</tr>
		</table>
	</td>
");

				if (mod % 4 == 3)
				{
					builder.Append("</tr>");
				}
				mod++;
			}

			foreach (FileSystemInfoExtend fileInfo in fileSystemInfoExtendCollection.Files)
			{
				if (mod % 4 == 0)
				{
					builder.Append("<tr>");
				}
				var fileSystemType = EFileSystemTypeUtils.GetEnumType(fileInfo.Type);
				var linkUrl = PageUtils.Combine(directoryUrl, fileInfo.Name);
				string fileImageUrl;
                var imageStyleAttributes = string.Empty;
                if (EFileSystemTypeUtils.IsImage(fileInfo.Type))
				{
					var imagePath = PathUtils.Combine(_directoryPath, fileInfo.Name);
					try
					{
						var image = ImageUtils.GetImage(imagePath);
						if (image.Height > image.Width)
						{
							if (image.Height > 94)
							{
                                imageStyleAttributes = @"style=""height:94px;""";
							}
						}
						else
						{
							if (image.Width > 94)
							{
                                imageStyleAttributes = @"style=""width:94px;""";
							}
						}
						fileImageUrl = PageUtils.Combine(directoryUrl, fileInfo.Name);
						image.Dispose();
					}
					catch
					{
						fileImageUrl = SiteServerAssets.GetFileSystemIconUrl(fileSystemType, true);
					}
				}
				else
				{
					fileImageUrl = GetFileSystemIconUrl(SiteInfo, fileInfo, true);
				}

                var attachmentUrl = PageUtility.GetVirtualUrl(SiteInfo, linkUrl);
                //string fileViewUrl = Modal.FileView.GetOpenWindowString(base.SiteId, attachmentUrl);
                var fileViewUrl = ModalFileView.GetOpenWindowStringHidden(SiteId, attachmentUrl,_hiddenClientId);

				builder.Append($@"
<td>
		<table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"">
			<tr>
				<td style=""height:100px; width:100px; text-align:center; vertical-align:middle;"">
					<table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"">
						<tr>
							<td background=""{backgroundImageUrl}"" style=""background-repeat:no-repeat; background-position:center;height:96px; width:96px; text-align:center; vertical-align:middle;"" align=""center""><a href=""javascript:;"" onClick=""window.parent.SelectAttachment('{_hiddenClientId}', '{attachmentUrl
				    .Replace("'", "\\'")}', '{fileViewUrl.Replace("'", "\\'")}');{LayerUtils.CloseScript}"" title=""{fileInfo.Name}""><img src=""{fileImageUrl}"" {imageStyleAttributes} border=0 /></a></td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td style=""height:20px; width:100%; text-align:center; vertical-align:middle;""><a href=""{linkUrl}"" title=""点击此项浏览此附件"" target=""_blank"">{StringUtils
				    .MaxLengthText(fileInfo.Name, 8)}</a></td>
			</tr>
		</table>
	</td>
");

				if (mod % 4 == 3)
				{
					builder.Append("</tr>");
				}
				mod++;
			}

			builder.Append("</table>");
			LtlFileSystems.Text = builder.ToString();
		}

		private void FillFileSystemsToList(bool isReload)
		{
			var builder = new StringBuilder();
			builder.Append(@"<table class=""table table-bordered table-hover""><tr class=""info thead""><td>名称</td><td width=""80"">大小</td><td width=""120"">类型</td><td width=""120"">修改日期</td></tr>");
			var directoryUrl = PageUtility.GetSiteUrlByPhysicalPath(SiteInfo, _directoryPath, true);

			var fileSystemInfoExtendCollection = FileManager.GetFileSystemInfoExtendCollection(_directoryPath, isReload);

			foreach (FileSystemInfoExtend subDirectoryInfo in fileSystemInfoExtendCollection.Folders)
			{
				string fileNameString =
				    $"<img src={SiteServerAssets.GetFileSystemIconUrl(EFileSystemType.Directory, false)} border=0 /> {subDirectoryInfo.Name}";
				var fileSystemTypeString = "文件夹";
				var fileModifyDateTime = subDirectoryInfo.LastWriteTime;
				var linkUrl = GetRedirectUrl(PageUtils.Combine(_currentRootPath, subDirectoryInfo.Name));
				string trHtml =
				    $"<tr><td><nobr><a href=\"{linkUrl}\">{fileNameString}</a></nobr></td><td align=\"right\">&nbsp;</td><td align=\"center\">{fileSystemTypeString}</td><td align=\"center\">{DateUtils.GetDateString(fileModifyDateTime, EDateFormatType.Day)}</td></tr>";
				builder.Append(trHtml);
			}

			foreach (FileSystemInfoExtend fileInfo in fileSystemInfoExtendCollection.Files)
			{
				string fileNameString =
				    $"<img src={GetFileSystemIconUrl(SiteInfo, fileInfo, false)} border=0 /> {fileInfo.Name}";
                var fileSystemType = EFileSystemTypeUtils.GetEnumType(fileInfo.Type);
                var fileSystemTypeString = (fileSystemType == EFileSystemType.Unknown) ?
                    $"{fileInfo.Type.TrimStart('.').ToUpper()} 文件"
                    : EFileSystemTypeUtils.GetText(fileSystemType);
				var fileKbSize = fileInfo.Size / 1024;
				if (fileKbSize == 0)
				{
					fileKbSize = 1;
				}
				var fileModifyDateTime = fileInfo.LastWriteTime;
				var linkUrl = PageUtils.Combine(directoryUrl, fileInfo.Name);
				var attachmentUrl = linkUrl.Replace(SiteInfo.Additional.WebUrl, "@");
                //string fileViewUrl = Modal.FileView.GetOpenWindowString(base.SiteId, attachmentUrl);
                var fileViewUrl = ModalFileView.GetOpenWindowStringHidden(SiteId, attachmentUrl,_hiddenClientId);
                string trHtml =
                    $"<tr><td><a href=\"javascript:;\" onClick=\"window.parent.SelectAttachment('{_hiddenClientId}', '{attachmentUrl.Replace("'", "\\'")}', '{fileViewUrl.Replace("'", "\\'")}');{LayerUtils.CloseScript}\" title=\"点击此项选择此附件\">{fileNameString}</a></td><td align=\"right\">{fileKbSize} KB</td><td align=\"center\">{fileSystemTypeString}</td><td align=\"center\">{DateUtils.GetDateString(fileModifyDateTime, EDateFormatType.Day)}</td></tr>";
				builder.Append(trHtml);
			}

			builder.Append("</table>");
			LtlFileSystems.Text = builder.ToString();
		}

		#endregion
	}
}
