using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTemplateCss : BasePageCms
    {
		public DataGrid dgContents;

        public string PublishmentSystemUrl => PublishmentSystemInfo.PublishmentSystemUrl;

	    private string _directoryPath;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetCmsUrl(nameof(PageTemplateCss), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            _directoryPath = PathUtility.MapPath(PublishmentSystemInfo, "@/css");

			if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdTemplate, "样式文件管理", AppManager.Cms.Permission.WebSite.Template);

				if (Body.IsQueryExists("Delete"))
				{
                    var fileName = Body.GetQueryString("FileName");

					try
					{
                        FileUtils.DeleteFileIfExists(PathUtils.Combine(_directoryPath, fileName));
                        Body.AddSiteLog(PublishmentSystemId, "删除样式文件", $"样式文件:{fileName}");
						SuccessDeleteMessage();
					}
					catch(Exception ex)
					{
                        FailDeleteMessage(ex);
					}
				}
			}
			BindGrid();
		}

		public void BindGrid()
		{
			try
			{
				DirectoryUtils.CreateDirectoryIfNotExists(_directoryPath);
                var fileNames = DirectoryUtils.GetFileNames(_directoryPath);
                var fileNameArrayList = new ArrayList();
                foreach (var fileName in fileNames)
                {
                    if (EFileSystemTypeUtils.IsTextEditable(EFileSystemTypeUtils.GetEnumType(PathUtils.GetExtension(fileName))))
                    {
                        if (!fileName.Contains("_parsed"))
                        {
                            fileNameArrayList.Add(fileName);
                        }
                    }
                }
                dgContents.DataSource = fileNameArrayList;
                dgContents.DataBind();
			}
			catch(Exception ex)
			{
                PageUtils.RedirectToErrorPage(ex.Message);
			}
		}

        public string GetCharset(string fileName)
        {
            var charset = FileUtils.GetFileCharset(PathUtils.Combine(_directoryPath, fileName));
            return ECharsetUtils.GetText(charset);
        }

	}
}
