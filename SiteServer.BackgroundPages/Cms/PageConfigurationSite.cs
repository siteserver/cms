using System;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
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
            ControlUtils.SelectSingleItem(DdlCharset, SiteInfo.Additional.Charset);

            TbPageSize.Text = SiteInfo.Additional.PageSize.ToString();

            EBooleanUtils.AddListItems(DdlIsCreateDoubleClick, "启用双击生成", "不启用");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateDoubleClick, SiteInfo.Additional.IsCreateDoubleClick.ToString());
        }

        public override void Submit_OnClick(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid) return;

            if (SiteInfo.Additional.Charset != DdlCharset.SelectedValue)
		    {
		        SiteInfo.Additional.Charset = DdlCharset.SelectedValue;
		    }

		    SiteInfo.Additional.PageSize = TranslateUtils.ToInt(TbPageSize.Text, SiteInfo.Additional.PageSize);
		    SiteInfo.Additional.IsCreateDoubleClick = TranslateUtils.ToBool(DdlIsCreateDoubleClick.SelectedValue);

            //修改所有模板编码
            var templateInfoList = DataProvider.TemplateDao.GetTemplateInfoListBySiteId(SiteId);
            var charset = ECharsetUtils.GetEnumType(SiteInfo.Additional.Charset);
            foreach (var templateInfo in templateInfoList)
            {
                if (templateInfo.Charset == charset) continue;

                var templateContent = TemplateManager.GetTemplateContent(SiteInfo, templateInfo);
                templateInfo.Charset = charset;
                DataProvider.TemplateDao.Update(SiteInfo, templateInfo, templateContent, AuthRequest.AdminName);
            }

            DataProvider.SiteDao.Update(SiteInfo);

            AuthRequest.AddSiteLog(SiteId, "修改站点设置");

            SuccessMessage("站点设置修改成功！");
        }
	}
}
