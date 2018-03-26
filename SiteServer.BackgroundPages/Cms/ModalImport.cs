using System;
using System.Collections.Specialized;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.ImportExport;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalImport : BasePageCms
    {
		public HtmlInputFile HifMyFile;
		public DropDownList DdlIsOverride;

        private string _type;

        public const int Width = 560;
        public const int Height = 260;
        public const string TypeRelatedField = "RELATED_FIELD";

        public static string GetOpenWindowString(int siteId, string type)
        {
            var title = string.Empty;
            if (StringUtils.EqualsIgnoreCase(type, TypeRelatedField))
            {
                title = "导入联动字段";
            }
            return LayerUtils.GetOpenScript(title, PageUtils.GetCmsUrl(siteId, nameof(ModalImport), new NameValueCollection
            {
                {"Type", type}
            }), Width, Height);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");
            _type = AuthRequest.GetQueryString("Type");

            if (!IsPostBack)
			{
			
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (StringUtils.EqualsIgnoreCase(_type, TypeRelatedField))
            {
                if (HifMyFile.PostedFile != null && "" != HifMyFile.PostedFile.FileName)
                {
                    var filePath = HifMyFile.PostedFile.FileName;
                    if (EFileSystemTypeUtils.GetEnumType(Path.GetExtension(filePath)) != EFileSystemType.Zip)
                    {
                        FailMessage("必须上传ZIP文件");
                        return;
                    }

                    try
                    {
                        var localFilePath = PathUtils.GetTemporaryFilesPath(Path.GetFileName(filePath));

                        HifMyFile.PostedFile.SaveAs(localFilePath);

                        var importObject = new ImportObject(SiteId, AuthRequest.AdminName);
                        importObject.ImportRelatedFieldByZipFile(localFilePath, TranslateUtils.ToBool(DdlIsOverride.SelectedValue));

                        AuthRequest.AddSiteLog(SiteId, "导入联动字段");

                        LayerUtils.Close(Page);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "导入联动字段失败！");
                    }
                }
            }
		}
	}
}
