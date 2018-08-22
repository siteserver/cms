using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalTemplateAssetsConfig : BasePageCms
    {
        public TextBox TbDirectoryPath;

        private string _type;
        private string _assetsDir;

        public static string GetOpenWindowString(int siteId, string type)
        {
            var name = string.Empty;
            if (type == PageTemplateAssets.TypeInclude)
            {
                name = PageTemplateAssets.NameInclude;
            }
            else if (type == PageTemplateAssets.TypeJs)
            {
                name = PageTemplateAssets.NameJs;
            }
            else if (type == PageTemplateAssets.TypeCss)
            {
                name = PageTemplateAssets.NameCss;
            }

            return LayerUtils.GetOpenScript($"{name}文件夹设置",
                PageUtils.GetCmsUrl(siteId, nameof(ModalTemplateAssetsConfig), new NameValueCollection
                {
                    {"type", type}
                }), 500, 400);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "type");
            _type = AuthRequest.GetQueryString("type");

            _type = AuthRequest.GetQueryString("type");
            if (_type == PageTemplateAssets.TypeInclude)
            {
                _assetsDir = SiteInfo.Additional.TemplatesAssetsIncludeDir.Trim('/');
            }
            else if (_type == PageTemplateAssets.TypeJs)
            {
                _assetsDir = SiteInfo.Additional.TemplatesAssetsJsDir.Trim('/');
            }
            else if (_type == PageTemplateAssets.TypeCss)
            {
                _assetsDir = SiteInfo.Additional.TemplatesAssetsCssDir.Trim('/');
            }

            if (string.IsNullOrEmpty(_assetsDir)) return;

            if (IsPostBack) return;

            TbDirectoryPath.Text = _assetsDir;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isSuccess = false;

            try
            {
                var assetsDir = TbDirectoryPath.Text.Trim('/');
                if (_type == PageTemplateAssets.TypeInclude)
                {
                    SiteInfo.Additional.TemplatesAssetsIncludeDir = assetsDir;
                }
                else if (_type == PageTemplateAssets.TypeJs)
                {
                    SiteInfo.Additional.TemplatesAssetsJsDir = assetsDir;
                }
                else if (_type == PageTemplateAssets.TypeCss)
                {
                    SiteInfo.Additional.TemplatesAssetsCssDir = assetsDir;
                }

                DataProvider.SiteDao.Update(SiteInfo);

                AuthRequest.AddSiteLog(SiteId, "模板文件夹设置");

                isSuccess = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }

            if (isSuccess)
            {
                LayerUtils.CloseAndRedirect(Page, PageTemplateAssets.GetRedirectUrl(SiteId, _type));
            }
        }
    }
}
