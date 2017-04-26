using System;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Images;
using BaiRong.Core.IO.FileManagement;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageFileManagement : BasePageCms
    {
		public Literal ltlCurrentDirectory;
        public Literal ltlFileSystems;
		public ImageButton DeleteButton;
		public HyperLink UploadLink;
		public DropDownList ListType;

        private string _directoryPath;
        private string _relatedPath;

        public static string GetRedirectUrl(int publishmentSystemId, string relatedPath)
        {
            return PageUtils.GetCmsUrl(nameof(PageFileManagement), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"relatedPath", relatedPath}
            });
        }

        public static string GetRedirectUrlWithType(int publishmentSystemId, string relatedPath, string listTypeStr)
		{
            return PageUtils.GetCmsUrl(nameof(PageFileManagement), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"relatedPath", relatedPath},
                {"ListType", listTypeStr}
            });
		}

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _relatedPath = Body.GetQueryString("relatedPath");

            PageUtils.CheckRequestParameter("PublishmentSystemID", "relatedPath");

            if (ProductPermissionsManager.Current.PublishmentSystemIdList.Contains(PublishmentSystemId))
            {
                _directoryPath = PathUtility.MapPath(PublishmentSystemInfo, _relatedPath);

                if (!DirectoryUtils.IsDirectoryExists(_directoryPath))
                {
                    PageUtils.RedirectToErrorPage("文件夹不存在！");
                    return;
                }

                if (!IsPostBack)
                {
                    //BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, "站点文件管理", AppManager.Cms.Permission.WebSite.FileManagement);

                    ListType.Items.Add(new ListItem("显示缩略图", "Image"));
                    ListType.Items.Add(new ListItem("显示详细信息", "List"));
                    if (Body.IsQueryExists("ListType"))
                    {
                        ControlUtils.SelectListItems(ListType, Body.GetQueryString("ListType"));
                    }

                    ltlCurrentDirectory.Text = PageUtils.Combine(PublishmentSystemInfo.PublishmentSystemDir, _relatedPath);

                    FillFileSystems(false);

                    DeleteButton.Attributes.Add("onclick", "return confirm(\"此操作将删除所选文件夹及文件，确定吗？\");");

                    var showPopWinString = ModalUploadFile.GetOpenWindowStringToList(PublishmentSystemId, EUploadType.File, _relatedPath);
                    UploadLink.Attributes.Add("onclick", showPopWinString);
                }
            }
		}

		public void LinkButton_Command(object sender, CommandEventArgs e)
		{
			if (e.CommandName.Equals("NavigationBar"))
			{
				if (e.CommandArgument.Equals("Delete"))
				{
					var directoryNameCollection = Request["DirectoryNameCollection"];
					if (!string.IsNullOrEmpty(directoryNameCollection))
					{
						var directoryNameArrayList = TranslateUtils.StringCollectionToStringList(directoryNameCollection);
						if (directoryNameArrayList != null && directoryNameArrayList.Count > 0)
						{
							foreach (string directoryName in directoryNameArrayList)
							{
								var path = PathUtils.Combine(_directoryPath, directoryName);
								DirectoryUtils.DeleteDirectoryIfExists(path);
							}
						}
					}
					var fileNameCollection = Request["FileNameCollection"];
					if (!string.IsNullOrEmpty(fileNameCollection))
					{
						var fileNameArrayList = TranslateUtils.StringCollectionToStringList(fileNameCollection);
						if (fileNameArrayList != null && fileNameArrayList.Count > 0)
						{
							FileUtils.DeleteFilesIfExists(_directoryPath, fileNameArrayList);
						}
					}
				}
			}

			PageUtils.Redirect(GetRedirectUrl(PublishmentSystemId, _relatedPath));
		}

		public void ListType_SelectedIndexChanged(object sender, EventArgs e)
		{
			var navigationUrl = GetRedirectUrlWithType(PublishmentSystemId, _relatedPath, ListType.SelectedValue);
			PageUtils.Redirect(navigationUrl);
		}


		#region Helper
		private void FillFileSystems(bool isReload)
		{
			var cookieName = "SiteServer.BackgroundPages.Cms.BackgroundFileManagement";
			var isSetCookie = Body.IsQueryExists("ListType");
			if (!isSetCookie)
			{
				var cookieExists = false;
				if (CookieUtils.IsExists(cookieName))
				{
					var cookieValue = CookieUtils.GetCookie(cookieName);
					foreach (ListItem item in ListType.Items)
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
					CookieUtils.SetCookie(cookieName, ListType.SelectedValue, DateTime.MaxValue);
				}
			}
			else
			{
                CookieUtils.SetCookie(cookieName, Body.GetQueryString("ListType"), DateTime.MaxValue);
			}
			if (ListType.SelectedValue == "List")
			{
				FillFileSystemsToList(isReload);
			}
			else if (ListType.SelectedValue == "Image")
			{
				FillFileSystemsToImage(isReload);
			}
		}

		private void FillFileSystemsToImage(bool isReload)
		{
			var builder = new StringBuilder();
            builder.Append("<table class=\"table table-noborder table-hover\">");

            var directoryUrl = PageUtility.GetPublishmentSystemUrl(PublishmentSystemInfo, _relatedPath);

            var backgroundImageUrl = SiteServerAssets.GetIconUrl("filesystem/management/background.gif");
			var directoryImageUrl = SiteServerAssets.GetFileSystemIconUrl(EFileSystemType.Directory, true);

			var fileSystemInfoExtendCollection = FileManager.GetFileSystemInfoExtendCollection(_directoryPath, isReload);

			var mod = 0;
			foreach (FileSystemInfoExtend subDirectoryInfo in fileSystemInfoExtendCollection.Folders)
			{
                if (string.IsNullOrEmpty(_relatedPath))
                {
                    if (StringUtils.EqualsIgnoreCase(subDirectoryInfo.Name, "api"))
                    {
                        continue;
                    }
                }
				if (mod % 5 == 0)
				{
					builder.Append("<tr>");
				}
                var linkUrl = GetRedirectUrl(PublishmentSystemId, PageUtils.Combine(_relatedPath, subDirectoryInfo.Name));

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
				    .MaxLengthText(subDirectoryInfo.Name, 7)}</a> <input type=""checkbox"" name=""DirectoryNameCollection"" value=""{subDirectoryInfo
				    .Name}"" /></td>
			</tr>
		</table>
	</td>
");

				if (mod % 5 == 4)
				{
					builder.Append("</tr>");
				}
				mod++;
			}

            foreach (FileSystemInfoExtend fileInfo in fileSystemInfoExtendCollection.Files)
            {
                if (mod % 5 == 0)
                {
                    builder.Append("<tr>");
                }
                var fileSystemType = EFileSystemTypeUtils.GetEnumType(fileInfo.Type);
                var showPopWinString = ModalFileView.GetOpenWindowString(PublishmentSystemId, _relatedPath, fileInfo.Name);
                var linkUrl = PageUtils.Combine(directoryUrl, fileInfo.Name);
                var fileImageUrl = string.Empty;
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
                    fileImageUrl = SiteServerAssets.GetFileSystemIconUrl(fileSystemType, true);
                }

                builder.Append($@"
<td>
		<table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"">
			<tr>
				<td style=""height:100px; width:100px; text-align:center; vertical-align:middle;"">
					<table cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"">
						<tr>
							<td background=""{backgroundImageUrl}"" style=""background-repeat:no-repeat; background-position:center;height:96px; width:96px; text-align:center; vertical-align:middle;"" align=""center""><a href=""javascript:;"" onclick=""{showPopWinString}"" target=""_blank""><img src=""{fileImageUrl}"" {imageStyleAttributes} border=0 /></a></td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td style=""height:20px; width:100%; text-align:center; vertical-align:middle;""><a href=""{linkUrl}"" target=""_blank"">{StringUtils
                    .MaxLengthText(fileInfo.Name, 7)}</a> <input type=""checkbox"" name=""FileNameCollection"" value=""{fileInfo
                    .Name}"" /></td>
			</tr>
		</table>
	</td>
");

                if (mod % 5 == 4)
                {
                    builder.Append("</tr>");
                }
                mod++;
            }

			builder.Append("</table>");
			ltlFileSystems.Text = builder.ToString();
		}

		private void FillFileSystemsToList(bool isReload)
		{
			var builder = new StringBuilder();

            builder.Append("<table class=\"table table-noborder table-hover\"><tr class=\"info thead\"><td>名称</td><td width=\"80\">大小</td><td width=\"120\">类型</td><td width=\"120\">修改日期</td><td width=\"40\"><input type=\"checkbox\" onclick=\"_checkFormAll(this.checked)\" /></td></tr>");
            var directoryUrl = PageUtility.GetPublishmentSystemUrl(PublishmentSystemInfo, _relatedPath);

			var fileSystemInfoExtendCollection = FileManager.GetFileSystemInfoExtendCollection(_directoryPath, isReload);

			foreach (FileSystemInfoExtend subDirectoryInfo in fileSystemInfoExtendCollection.Folders)
			{
                if (string.IsNullOrEmpty(_relatedPath))
                {
                    if (StringUtils.EqualsIgnoreCase(subDirectoryInfo.Name, "api"))
                    {
                        continue;
                    }
                }
				string fileNameString =
				    $"<img src={SiteServerAssets.GetFileSystemIconUrl(EFileSystemType.Directory, false)} border=0 /> {subDirectoryInfo.Name}";
				var fileSystemTypeString = "文件夹";
				var fileModifyDateTime = subDirectoryInfo.LastWriteTime;
                var linkUrl = GetRedirectUrl(PublishmentSystemId, PageUtils.Combine(_relatedPath, subDirectoryInfo.Name));
                string trHtml =
                    $"<tr><td><nobr><a href=\"{linkUrl}\">{fileNameString}</a></nobr></td><td class=\"center\">&nbsp;</td><td class=\"center\">{fileSystemTypeString}</td><td class=\"center\">{DateUtils.GetDateAndTimeString(fileModifyDateTime, EDateFormatType.Day, ETimeFormatType.ShortTime)}</td><td class=\"center\"><input type=\"checkbox\" name=\"DirectoryNameCollection\" value=\"{subDirectoryInfo.Name}\" /></td></tr>";
				builder.Append(trHtml);
			}

            foreach (FileSystemInfoExtend fileInfo in fileSystemInfoExtendCollection.Files)
            {
                var fileExt = fileInfo.Type;
                var fileSystemType = EFileSystemTypeUtils.GetEnumType(fileExt);
                string fileNameString =
                    $"<img src={SiteServerAssets.GetFileSystemIconUrl(fileSystemType, false)} border=0 /> {fileInfo.Name}";
                var fileSystemTypeString = (fileSystemType == EFileSystemType.Unknown) ?
                    $"{fileExt.TrimStart('.').ToUpper()} 文件"
                    : EFileSystemTypeUtils.GetText(fileSystemType);
                var fileKBSize = TranslateUtils.GetKbSize(fileInfo.Size);
                var fileModifyDateTime = fileInfo.LastWriteTime;
                var linkUrl = PageUtils.Combine(directoryUrl, fileInfo.Name);
                string trHtml =
                    $"<tr><td><nobr><a href=\"{linkUrl}\" target=\"_blank\">{fileNameString}</a></nobr></td><td class=\"center\">{fileKBSize} KB</td><td class=\"center\">{fileSystemTypeString}</td><td class=\"center\">{DateUtils.GetDateAndTimeString(fileModifyDateTime, EDateFormatType.Day, ETimeFormatType.ShortTime)}</td><td class=\"center\"><input type=\"checkbox\" name=\"FileNameCollection\" value=\"{fileInfo.Name}\" /></td></tr>";
                builder.Append(trHtml);
            }

			builder.Append("</table>");
			ltlFileSystems.Text = builder.ToString();
		}

		#endregion
	}
}
