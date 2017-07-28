using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.ImportExport;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalTableStyleImport : BasePageCms
    {
		public HtmlInputFile myFile;

        private string _tableName;
        private ETableStyle _tableStyle;
        private int _relatedIdentity;

        public static string GetOpenWindowString(string tableName, ETableStyle tableStyle, int publishmentSystemId,
            int relatedIdentity)
        {
            return PageUtils.GetOpenWindowString("导入表样式",
                PageUtils.GetCmsUrl(nameof(ModalTableStyleImport), new NameValueCollection
                {
                    {"TableName", tableName},
                    {"TableStyle", ETableStyleUtils.GetValue(tableStyle)},
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"RelatedIdentity", relatedIdentity.ToString()}
                }), 560, 200);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _tableName = Body.GetQueryString("TableName");
            _tableStyle = ETableStyleUtils.GetEnumType(Body.GetQueryString("TableStyle"));
            _relatedIdentity = int.Parse(Body.GetQueryString("RelatedIdentity"));
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			if (myFile.PostedFile != null && "" != myFile.PostedFile.FileName)
			{
				var filePath = myFile.PostedFile.FileName;
                if (!EFileSystemTypeUtils.IsCompressionFile(PathUtils.GetExtension(filePath)))
				{
                    FailMessage("必须上传压缩文件");
					return;
				}

				try
				{
                    var localFilePath = PathUtils.GetTemporaryFilesPath(PathUtils.GetFileName(filePath));

					myFile.PostedFile.SaveAs(localFilePath);

                    ImportObject.ImportTableStyleByZipFile(_tableStyle, _tableName, _relatedIdentity, localFilePath);

                    Body.AddSiteLog(PublishmentSystemId, "导入表单显示样式", $"类型:{ETableStyleUtils.GetText(_tableStyle)}");

					PageUtils.CloseModalPage(Page);
				}
				catch(Exception ex)
				{
					FailMessage(ex, "导入表样式失败！");
				}
			}
		}
	}
}
