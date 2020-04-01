using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalFileChangeName : BasePageCms
    {
        protected Literal LtlFileName;
        protected TextBox TbFileName;

		private string _rootPath;
		private string _directoryPath;

        public static string GetOpenWindowString(int siteId, string rootPath, string fileName)
        {
            return LayerUtils.GetOpenScript("修改文件名", PageUtils.GetCmsUrl(siteId, nameof(ModalFileChangeName), new NameValueCollection
            {
                {"RootPath", rootPath},
                {"FileName", fileName}
            }), 400, 250);
        }

        public static string GetOpenWindowString(int siteId, string rootPath, string fileName, string hiddenClientId)
        {
            return LayerUtils.GetOpenScript("修改文件名", PageUtils.GetCmsUrl(siteId, nameof(ModalFileChangeName), new NameValueCollection
            {
                {"RootPath", rootPath},
                {"FileName", fileName},
                {"HiddenClientID", hiddenClientId}
            }), 400, 250);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "RootPath");

            _rootPath = AuthRequest.GetQueryString("RootPath").TrimEnd('/');
            _directoryPath = PathUtility.MapPath(SiteInfo, _rootPath);

			if (!Page.IsPostBack)
			{
                LtlFileName.Text = AuthRequest.GetQueryString("FileName");
			}
		}

        private string RedirectUrl()
        {
            return ModalFileView.GetRedirectUrl(SiteId, AuthRequest.GetQueryString("rootPath"),
                AuthRequest.GetQueryString("FileName"), TbFileName.Text, AuthRequest.GetQueryString("HiddenClientID"));
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!DirectoryUtils.IsDirectoryNameCompliant(TbFileName.Text))
            {
                FailMessage("文件名称不符合要求");
                return;
            }

            var path = PathUtils.Combine(_directoryPath, TbFileName.Text);
            if (FileUtils.IsFileExists(path))
            {
                FailMessage("文件已经存在");
                return;
            }
            var pathSource = PathUtils.Combine(_directoryPath, LtlFileName.Text);
            FileUtils.MoveFile(pathSource, path, true);
            FileUtils.DeleteFileIfExists(pathSource);

            AuthRequest.AddSiteLog(SiteId, "修改文件名", $"文件名:{TbFileName.Text}");
            //JsUtils.SubModal.CloseModalPageWithoutRefresh(Page);
            LayerUtils.CloseAndRedirect(Page, RedirectUrl());
        }
	}
}
