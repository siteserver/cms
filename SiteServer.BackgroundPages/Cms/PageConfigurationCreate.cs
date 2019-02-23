using System;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Database.Caches;
using SiteServer.CMS.Database.Core;
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

            PageUtils.CheckRequestParameter("siteId");

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.WebSitePermissions.Create);

            EBooleanUtils.AddListItems(DdlIsCreateContentIfContentChanged, "生成", "不生成");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateContentIfContentChanged, SiteInfo.Extend.IsCreateContentIfContentChanged.ToString());

            EBooleanUtils.AddListItems(DdlIsCreateChannelIfChannelChanged, "生成", "不生成");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateChannelIfChannelChanged, SiteInfo.Extend.IsCreateChannelIfChannelChanged.ToString());

            EBooleanUtils.AddListItems(DdlIsCreateShowPageInfo, "显示", "不显示");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateShowPageInfo, SiteInfo.Extend.IsCreateShowPageInfo.ToString());

            EBooleanUtils.AddListItems(DdlIsCreateIe8Compatible, "强制兼容", "不设置");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateIe8Compatible, SiteInfo.Extend.IsCreateIe8Compatible.ToString());

            EBooleanUtils.AddListItems(DdlIsCreateBrowserNoCache, "强制清除缓存", "不设置");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateBrowserNoCache, SiteInfo.Extend.IsCreateBrowserNoCache.ToString());

            EBooleanUtils.AddListItems(DdlIsCreateJsIgnoreError, "包含JS容错代码", "不设置");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateJsIgnoreError, SiteInfo.Extend.IsCreateJsIgnoreError.ToString());

            EBooleanUtils.AddListItems(DdlIsCreateWithJQuery, "是", "否");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateWithJQuery, SiteInfo.Extend.IsCreateWithJQuery.ToString());

            EBooleanUtils.AddListItems(DdlIsCreateDoubleClick, "启用双击生成", "不启用");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateDoubleClick, SiteInfo.Extend.IsCreateDoubleClick.ToString());

            TbCreateStaticMaxPage.Text = SiteInfo.Extend.CreateStaticMaxPage.ToString();

            EBooleanUtils.AddListItems(DdlIsCreateUseDefaultFileName, "启用", "不启用");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateUseDefaultFileName, SiteInfo.Extend.IsCreateUseDefaultFileName.ToString());
            PhIsCreateUseDefaultFileName.Visible = SiteInfo.Extend.IsCreateUseDefaultFileName;
            TbCreateDefaultFileName.Text = SiteInfo.Extend.CreateDefaultFileName;

            EBooleanUtils.AddListItems(DdlIsCreateStaticContentByAddDate, "启用", "不启用");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateStaticContentByAddDate, SiteInfo.Extend.IsCreateStaticContentByAddDate.ToString());
            PhIsCreateStaticContentByAddDate.Visible = SiteInfo.Extend.IsCreateStaticContentByAddDate;
            if (SiteInfo.Extend.CreateStaticContentAddDate != DateTime.MinValue)
            {
                TbCreateStaticContentAddDate.DateTime = SiteInfo.Extend.CreateStaticContentAddDate;
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

		    SiteInfo.Extend.IsCreateContentIfContentChanged = TranslateUtils.ToBool(DdlIsCreateContentIfContentChanged.SelectedValue);
		    SiteInfo.Extend.IsCreateChannelIfChannelChanged = TranslateUtils.ToBool(DdlIsCreateChannelIfChannelChanged.SelectedValue);

		    SiteInfo.Extend.IsCreateShowPageInfo = TranslateUtils.ToBool(DdlIsCreateShowPageInfo.SelectedValue);
		    SiteInfo.Extend.IsCreateIe8Compatible = TranslateUtils.ToBool(DdlIsCreateIe8Compatible.SelectedValue);
		    SiteInfo.Extend.IsCreateBrowserNoCache = TranslateUtils.ToBool(DdlIsCreateBrowserNoCache.SelectedValue);
		    SiteInfo.Extend.IsCreateJsIgnoreError = TranslateUtils.ToBool(DdlIsCreateJsIgnoreError.SelectedValue);
		    SiteInfo.Extend.IsCreateWithJQuery = TranslateUtils.ToBool(DdlIsCreateWithJQuery.SelectedValue);

		    SiteInfo.Extend.IsCreateDoubleClick = TranslateUtils.ToBool(DdlIsCreateDoubleClick.SelectedValue);
		    SiteInfo.Extend.CreateStaticMaxPage = TranslateUtils.ToInt(TbCreateStaticMaxPage.Text);

		    SiteInfo.Extend.IsCreateUseDefaultFileName = TranslateUtils.ToBool(DdlIsCreateUseDefaultFileName.SelectedValue);
		    if (SiteInfo.Extend.IsCreateUseDefaultFileName)
		    {
		        SiteInfo.Extend.CreateDefaultFileName = TbCreateDefaultFileName.Text;
		    }

            SiteInfo.Extend.IsCreateStaticContentByAddDate = TranslateUtils.ToBool(DdlIsCreateStaticContentByAddDate.SelectedValue);
		    if (SiteInfo.Extend.IsCreateStaticContentByAddDate)
		    {
		        SiteInfo.Extend.CreateStaticContentAddDate = TbCreateStaticContentAddDate.DateTime;
		    } 

            DataProvider.Site.Update(SiteInfo);

            AuthRequest.AddSiteLog(SiteId, "修改页面生成设置");

            SuccessMessage("页面生成设置修改成功！");
        }
	}
}
