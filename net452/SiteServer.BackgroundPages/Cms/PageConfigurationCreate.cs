using System;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Utils;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Fx;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageConfigurationCreate : BasePageCms
    {
        public DropDownList DdlIsCreateContentIfContentChanged;
        public DropDownList DdlIsCreateChannelIfChannelChanged;
        public DropDownList DdlIsCreateShowPageInfo;
        public DropDownList DdlIsCreateIe8Compatible;
        public DropDownList DdlIsCreateBrowserNoCache;
        public DropDownList DdlIsCreateJsIgnoreError;
        public DropDownList DdlIsCreateWithJQuery;
        public DropDownList DdlIsCreateDoubleClick;
        public TextBox TbCreateStaticMaxPage;
        public DropDownList DdlIsCreateUseDefaultFileName;
        public PlaceHolder PhIsCreateUseDefaultFileName;
        public TextBox TbCreateDefaultFileName;
        public DropDownList DdlIsCreateStaticContentByAddDate;
        public PlaceHolder PhIsCreateStaticContentByAddDate;
        public DateTimeTextBox TbCreateStaticContentAddDate;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            WebPageUtils.CheckRequestParameter("siteId");

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.WebSitePermissions.Create);

            FxUtils.AddListItems(DdlIsCreateContentIfContentChanged, "生成", "不生成");
            SystemWebUtils.SelectSingleItemIgnoreCase(DdlIsCreateContentIfContentChanged, SiteInfo.IsCreateContentIfContentChanged.ToString());

            FxUtils.AddListItems(DdlIsCreateChannelIfChannelChanged, "生成", "不生成");
            SystemWebUtils.SelectSingleItemIgnoreCase(DdlIsCreateChannelIfChannelChanged, SiteInfo.IsCreateChannelIfChannelChanged.ToString());

            FxUtils.AddListItems(DdlIsCreateShowPageInfo, "显示", "不显示");
            SystemWebUtils.SelectSingleItemIgnoreCase(DdlIsCreateShowPageInfo, SiteInfo.IsCreateShowPageInfo.ToString());

            FxUtils.AddListItems(DdlIsCreateIe8Compatible, "强制兼容", "不设置");
            SystemWebUtils.SelectSingleItemIgnoreCase(DdlIsCreateIe8Compatible, SiteInfo.IsCreateIe8Compatible.ToString());

            FxUtils.AddListItems(DdlIsCreateBrowserNoCache, "强制清除缓存", "不设置");
            SystemWebUtils.SelectSingleItemIgnoreCase(DdlIsCreateBrowserNoCache, SiteInfo.IsCreateBrowserNoCache.ToString());

            FxUtils.AddListItems(DdlIsCreateJsIgnoreError, "包含JS容错代码", "不设置");
            SystemWebUtils.SelectSingleItemIgnoreCase(DdlIsCreateJsIgnoreError, SiteInfo.IsCreateJsIgnoreError.ToString());

            FxUtils.AddListItems(DdlIsCreateWithJQuery, "是", "否");
            SystemWebUtils.SelectSingleItemIgnoreCase(DdlIsCreateWithJQuery, SiteInfo.IsCreateWithJQuery.ToString());

            FxUtils.AddListItems(DdlIsCreateDoubleClick, "启用双击生成", "不启用");
            SystemWebUtils.SelectSingleItemIgnoreCase(DdlIsCreateDoubleClick, SiteInfo.IsCreateDoubleClick.ToString());

            TbCreateStaticMaxPage.Text = SiteInfo.CreateStaticMaxPage.ToString();

            FxUtils.AddListItems(DdlIsCreateUseDefaultFileName, "启用", "不启用");
            SystemWebUtils.SelectSingleItemIgnoreCase(DdlIsCreateUseDefaultFileName, SiteInfo.IsCreateUseDefaultFileName.ToString());
            PhIsCreateUseDefaultFileName.Visible = SiteInfo.IsCreateUseDefaultFileName;
            TbCreateDefaultFileName.Text = SiteInfo.CreateDefaultFileName;

            FxUtils.AddListItems(DdlIsCreateStaticContentByAddDate, "启用", "不启用");
            SystemWebUtils.SelectSingleItemIgnoreCase(DdlIsCreateStaticContentByAddDate, SiteInfo.IsCreateStaticContentByAddDate.ToString());
            PhIsCreateStaticContentByAddDate.Visible = SiteInfo.IsCreateStaticContentByAddDate;
            if (SiteInfo.CreateStaticContentAddDate.HasValue)
            {
                TbCreateStaticContentAddDate.DateTime = SiteInfo.CreateStaticContentAddDate.Value;
            }
        }

        public void DdlIsCreateUseDefaultFileName_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhIsCreateUseDefaultFileName.Visible = TranslateUtils.ToBool(DdlIsCreateUseDefaultFileName.SelectedValue);
        }

        public void DdlIsCreateStaticContentByAddDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhIsCreateStaticContentByAddDate.Visible = TranslateUtils.ToBool(DdlIsCreateStaticContentByAddDate.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid) return;

		    SiteInfo.IsCreateContentIfContentChanged = TranslateUtils.ToBool(DdlIsCreateContentIfContentChanged.SelectedValue);
		    SiteInfo.IsCreateChannelIfChannelChanged = TranslateUtils.ToBool(DdlIsCreateChannelIfChannelChanged.SelectedValue);

		    SiteInfo.IsCreateShowPageInfo = TranslateUtils.ToBool(DdlIsCreateShowPageInfo.SelectedValue);
		    SiteInfo.IsCreateIe8Compatible = TranslateUtils.ToBool(DdlIsCreateIe8Compatible.SelectedValue);
		    SiteInfo.IsCreateBrowserNoCache = TranslateUtils.ToBool(DdlIsCreateBrowserNoCache.SelectedValue);
		    SiteInfo.IsCreateJsIgnoreError = TranslateUtils.ToBool(DdlIsCreateJsIgnoreError.SelectedValue);
		    SiteInfo.IsCreateWithJQuery = TranslateUtils.ToBool(DdlIsCreateWithJQuery.SelectedValue);

		    SiteInfo.IsCreateDoubleClick = TranslateUtils.ToBool(DdlIsCreateDoubleClick.SelectedValue);
		    SiteInfo.CreateStaticMaxPage = TranslateUtils.ToInt(TbCreateStaticMaxPage.Text);

		    SiteInfo.IsCreateUseDefaultFileName = TranslateUtils.ToBool(DdlIsCreateUseDefaultFileName.SelectedValue);
		    if (SiteInfo.IsCreateUseDefaultFileName)
		    {
		        SiteInfo.CreateDefaultFileName = TbCreateDefaultFileName.Text;
		    }

            SiteInfo.IsCreateStaticContentByAddDate = TranslateUtils.ToBool(DdlIsCreateStaticContentByAddDate.SelectedValue);
		    if (SiteInfo.IsCreateStaticContentByAddDate)
		    {
		        SiteInfo.CreateStaticContentAddDate = TbCreateStaticContentAddDate.DateTime;
		    } 

            DataProvider.Site.Update(SiteInfo);

            AuthRequest.AddSiteLog(SiteId, "修改页面生成设置");

            SuccessMessage("页面生成设置修改成功！");
        }
	}
}
