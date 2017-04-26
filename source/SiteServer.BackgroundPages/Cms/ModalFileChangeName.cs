using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalFileChangeName : BasePageCms
    {
        protected Literal ltlFileName;
        protected TextBox FileName;
		protected RegularExpressionValidator FileNameValidator;

		private string _rootPath;
		private string _directoryPath;

        public static string GetOpenWindowString(int publishmentSystemId, string rootPath, string fileName)
        {
            return PageUtils.GetOpenWindowString("修改文件名", PageUtils.GetCmsUrl(nameof(ModalFileChangeName), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"RootPath", rootPath},
                {"FileName", fileName}
            }), 400, 250);
        }

        public static string GetOpenWindowString(int publishmentSystemId, string rootPath, string fileName, string hiddenClientId)
        {
            return PageUtils.GetOpenWindowString("修改文件名", PageUtils.GetCmsUrl(nameof(ModalFileChangeName), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"RootPath", rootPath},
                {"FileName", fileName},
                {"HiddenClientID", hiddenClientId}
            }), 400, 250);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "RootPath");

            _rootPath = Body.GetQueryString("RootPath").TrimEnd('/');
            _directoryPath = PathUtility.MapPath(PublishmentSystemInfo, _rootPath);

			if (!Page.IsPostBack)
			{
                ltlFileName.Text = Body.GetQueryString("FileName");
			}
		}

        private string RedirectURL()
        {
            return ModalFileView.GetRedirectUrl(PublishmentSystemId, Body.GetQueryString("rootPath"),
                Body.GetQueryString("FileName"), FileName.Text, Body.GetQueryString("HiddenClientID"));
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			var isChange = false;

            if (!DirectoryUtils.IsDirectoryNameCompliant(FileName.Text))
            {
                FileNameValidator.IsValid = false;
                FileNameValidator.ErrorMessage = "文件名称不符合要求";
                return;
            }

            var path = PathUtils.Combine(_directoryPath, FileName.Text);
            if (FileUtils.IsFileExists(path))
            {
                FileNameValidator.IsValid = false;
                FileNameValidator.ErrorMessage = "文件已经存在";
            }
            else
            {
                var pathSource = PathUtils.Combine(_directoryPath, ltlFileName.Text);
                FileUtils.MoveFile(pathSource, path, true);
                FileUtils.DeleteFileIfExists(pathSource);
                isChange = true;
            }

            if (isChange)
			{
                Body.AddSiteLog(PublishmentSystemId, "修改文件名", $"文件名:{FileName.Text}");
                //JsUtils.SubModal.CloseModalPageWithoutRefresh(Page);
                PageUtils.CloseModalPageAndRedirect(Page, RedirectURL());
			}
		}
	}
}
