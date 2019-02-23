using System;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Database.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationSite : BasePageCms
    {
        public DropDownList DdlCharset;
        public TextBox TbPageSize;
        public DropDownList DdlIsCreateDoubleClick;

        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageConfigurationSite), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.WebSitePermissions.Configration);

            ECharsetUtils.AddListItems(DdlCharset);
            ControlUtils.SelectSingleItem(DdlCharset, SiteInfo.Extend.Charset);

            TbPageSize.Text = SiteInfo.Extend.PageSize.ToString();

            EBooleanUtils.AddListItems(DdlIsCreateDoubleClick, "启用双击生成", "不启用");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateDoubleClick, SiteInfo.Extend.IsCreateDoubleClick.ToString());
        }

        public override void Submit_OnClick(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid) return;

            if (SiteInfo.Extend.Charset != DdlCharset.SelectedValue)
		    {
		        SiteInfo.Extend.Charset = DdlCharset.SelectedValue;
		    }

		    SiteInfo.Extend.PageSize = TranslateUtils.ToInt(TbPageSize.Text, SiteInfo.Extend.PageSize);
		    SiteInfo.Extend.IsCreateDoubleClick = TranslateUtils.ToBool(DdlIsCreateDoubleClick.SelectedValue);

            //修改所有模板编码
            var templateInfoList = DataProvider.Template.GetTemplateInfoListBySiteId(SiteId);
            var charset = ECharsetUtils.GetEnumType(SiteInfo.Extend.Charset);
            foreach (var templateInfo in templateInfoList)
            {
                if (templateInfo.FileCharset == charset) continue;

                var templateContent = TemplateManager.GetTemplateContent(SiteInfo, templateInfo);
                templateInfo.FileCharset = charset;
                DataProvider.Template.Update(SiteInfo, templateInfo, templateContent, AuthRequest.AdminName);
            }

            DataProvider.Site.Update(SiteInfo);

            AuthRequest.AddSiteLog(SiteId, "修改站点设置");

            SuccessMessage("站点设置修改成功！");
        }
	}
}
