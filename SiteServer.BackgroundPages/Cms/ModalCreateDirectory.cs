using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalCreateDirectory : BasePageCms
    {
		protected TextBox DirectoryName;
		protected RegularExpressionValidator DirectoryNameValidator;

		private string _currentRootPath;
		private string _directoryPath;

        public static string GetOpenWindowString(int publishmentSystemId, string currentRootPath)
        {
            return PageUtils.GetOpenWindowString("创建文件夹", PageUtils.GetCmsUrl(nameof(ModalCreateDirectory), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"CurrentRootPath", currentRootPath}
            }), 400, 250);
        }
	
		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "CurrentRootPath");

			_currentRootPath = Body.GetQueryString("CurrentRootPath").TrimEnd('/');
			_directoryPath = PathUtility.MapPath(PublishmentSystemInfo, _currentRootPath);
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			var isCreated = false;

            if (!DirectoryUtils.IsDirectoryNameCompliant(DirectoryName.Text))
            {
                DirectoryNameValidator.IsValid = false;
                DirectoryNameValidator.ErrorMessage = "文件夹名称不符合要求";
                return;
            }

            var path = PathUtils.Combine(_directoryPath, DirectoryName.Text);
            if (DirectoryUtils.IsDirectoryExists(path))
            {
                DirectoryNameValidator.IsValid = false;
                DirectoryNameValidator.ErrorMessage = "文件夹已经存在";
            }
            else
            {
                DirectoryUtils.CreateDirectoryIfNotExists(path);
                isCreated = true;
            }

			if (isCreated)
			{
                Body.AddSiteLog(PublishmentSystemId, "新建文件夹", $"文件夹:{DirectoryName.Text}");
				PageUtils.CloseModalPage(Page);
			}
		}
	}
}
