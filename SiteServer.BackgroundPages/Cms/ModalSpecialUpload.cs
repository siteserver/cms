using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalSpecialUpload : BasePageCms
    {
        public Literal LtlTitle;
        public HtmlInputFile HifUpload;

        private SpecialInfo _specialInfo;

        public static string GetOpenWindowString(int siteId, int specialId)
        {
            return LayerUtils.GetOpenScript("上传压缩包", PageUtils.GetCmsUrl(siteId, nameof(ModalSpecialUpload), new NameValueCollection
            {
                {"specialId", specialId.ToString()}
            }), 500, 320);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            var specialId = AuthRequest.GetQueryInt("specialId");
            _specialInfo = SpecialManager.GetSpecialInfo(SiteId, specialId);

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.WebSitePermissions.Template);

            LtlTitle.Text = _specialInfo.Title;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            if (HifUpload.PostedFile == null || "" == HifUpload.PostedFile.FileName)
            {
                FailMessage("上传压缩包失败，请选择ZIP文件上传！");
                return;
            }

            var filePath = HifUpload.PostedFile.FileName;
            if (!StringUtils.EqualsIgnoreCase(Path.GetExtension(filePath), ".zip"))
            {
                FailMessage("上传压缩包失败，必须上传ZIP文件！");
                return;
            }

            var directoryPath = SpecialManager.GetSpecialDirectoryPath(SiteInfo, _specialInfo.Url);
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);

            var zipFilePath = SpecialManager.GetSpecialZipFilePath(directoryPath);

            HifUpload.PostedFile.SaveAs(zipFilePath);
            var srcDirectoryPath = SpecialManager.GetSpecialSrcDirectoryPath(directoryPath);
            ZipUtils.ExtractZip(zipFilePath, srcDirectoryPath);

            DirectoryUtils.Copy(srcDirectoryPath, directoryPath, true);

            LayerUtils.Close(Page);
        }
    }
}