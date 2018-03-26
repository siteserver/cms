using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Utils.Enumerations;
using SiteServer.Utils.IO;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalSelectVideo : BasePageCms
    {
		public Literal LtlCurrentDirectory;
        public Literal LtlFileSystems;
		public Button BtnUpload;

		private string _currentRootPath;
        private string _textBoxClientId;
        private string _rootPath;
        private string _directoryPath;
        //限制限制文件夹路径最高路径
        private const string TopPath = "@/upload/videos";

		private string GetRedirectUrl(string path)
		{
            //here, limit top path
            if (!DirectoryUtils.IsInDirectory(TopPath, path))
                path = TopPath;
            return PageUtils.GetCmsUrl(SiteId, nameof(ModalSelectVideo), new NameValueCollection
            {
                {"RootPath", _rootPath},
                {"CurrentRootPath", path},
                {"TextBoxClientID", _textBoxClientId}
            });
		}

        public string SiteUrl => SiteInfo.Additional.WebUrl;

	    public string RootUrl => PageUtils.ApplicationPath;

	    public static string GetOpenWindowString(SiteInfo siteInfo, string textBoxClientId)
	    {
	        return LayerUtils.GetOpenScript("选择视频",
	            PageUtils.GetCmsUrl(siteInfo.Id, nameof(ModalSelectVideo), new NameValueCollection
	            {
	                {"RootPath", "@"},
	                {"CurrentRootPath", string.Empty},
	                {"TextBoxClientID", textBoxClientId}
	            }));
	    }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "RootPath", "CurrentRootPath", "TextBoxClientID");

			_rootPath = AuthRequest.GetQueryString("RootPath").TrimEnd('/');
            _currentRootPath = AuthRequest.GetQueryString("CurrentRootPath");
            _textBoxClientId = AuthRequest.GetQueryString("TextBoxClientID");

            if (string.IsNullOrEmpty(_currentRootPath))
            {
                _currentRootPath = SiteInfo.Additional.ConfigSelectVideoCurrentUrl.TrimEnd('/');
            }
            else
            {
                SiteInfo.Additional.ConfigSelectVideoCurrentUrl = _currentRootPath;
                DataProvider.SiteDao.Update(SiteInfo);
            }
            _currentRootPath = _currentRootPath.TrimEnd('/');

			_directoryPath = PathUtility.MapPath(SiteInfo, _currentRootPath);
            DirectoryUtils.CreateDirectoryIfNotExists(_directoryPath);

            if (Page.IsPostBack) return;

            BtnUpload.Attributes.Add("onclick", ModalUploadVideo.GetOpenWindowStringToList(SiteId, _currentRootPath));

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
                            $"<a href='{GetRedirectUrl(_rootPath)}'>{SiteInfo.SiteDir}</a>");
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

            FillFileSystemsToImage(false);
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


		#region Helper

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
				    .MaxLengthText(subDirectoryInfo.Name, 7)}</a></td>
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
                if (!PathUtility.IsVideoExtenstionAllowed(SiteInfo, fileInfo.Type))
				{
					continue;
				}
				if (mod % 4 == 0)
				{
					builder.Append("<tr>");
				}
				
				var linkUrl = PageUtils.Combine(directoryUrl, fileInfo.Name);
			    var imageAttributes = string.Empty;

                var fileImageUrl = SiteServerAssets.GetFileSystemIconUrl(EFileSystemType.Video, true);

                var textBoxUrl = PageUtility.GetVirtualUrl(SiteInfo, linkUrl);

				builder.Append($@"
<td onmouseover=""this.className='tdbg-dark';"" onmouseout=""this.className='';"">
		<table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"">
			<tr>
				<td style=""height:100px; width:100px; text-align:center; vertical-align:middle;"">
					<table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"">
						<tr>
							<td background=""{backgroundImageUrl}"" style=""background-repeat:no-repeat; background-position:center;height:96px; width:96px; text-align:center; vertical-align:middle;"" align=""center""><a href=""javascript:;"" onClick=""selectVideo('{textBoxUrl}', '{linkUrl}');"" title=""点击选择视频""><img src=""{fileImageUrl}"" {imageAttributes} border=0 /></a></td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td style=""height:20px; width:100%; text-align:center; vertical-align:middle;""><a href=""{linkUrl}"" title=""点击浏览视频"" target=""_blank"">{StringUtils
				    .MaxLengthText(fileInfo.Name, 7)}</a></td>
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

		#endregion
	}
}
