using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.ImportExport;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalTableStyleImport : BasePageCms
    {
		public HtmlInputFile HifMyFile;

        private string _tableName;
        private int _relatedIdentity;

        public static string GetOpenWindowString(string tableName, int siteId, int relatedIdentity)
        {
            return LayerUtils.GetOpenScript("导入表样式",
                PageUtils.GetCmsUrl(siteId, nameof(ModalTableStyleImport), new NameValueCollection
                {
                    {"TableName", tableName},
                    {"RelatedIdentity", relatedIdentity.ToString()}
                }), 760, 200);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _tableName = AuthRequest.GetQueryString("TableName");
            _relatedIdentity = int.Parse(AuthRequest.GetQueryString("RelatedIdentity"));
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			if (HifMyFile.PostedFile != null && "" != HifMyFile.PostedFile.FileName)
			{
				var filePath = HifMyFile.PostedFile.FileName;
                if (!EFileSystemTypeUtils.IsZip(PathUtils.GetExtension(filePath)))
				{
                    FailMessage("必须上传Zip压缩文件");
					return;
				}

				try
				{
                    var localFilePath = PathUtils.GetTemporaryFilesPath(PathUtils.GetFileName(filePath));

                    HifMyFile.PostedFile.SaveAs(localFilePath);

                    ImportObject.ImportTableStyleByZipFileAsync(_tableName, _relatedIdentity, localFilePath).GetAwaiter().GetResult();

                    AuthRequest.AddSiteLogAsync(SiteId, "导入表单显示样式").GetAwaiter().GetResult();

                    LayerUtils.Close(Page);
				}
				catch(Exception ex)
				{
					FailMessage(ex, "导入表样式失败！");
				}
			}
		}
	}
}
