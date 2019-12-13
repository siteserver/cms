using System;
using System.Web.UI.WebControls;
using SiteServer.Abstractions;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Repositories;

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

            VerifySitePermissions(Constants.SitePermissions.ConfigCreateRule);

            EBooleanUtils.AddListItems(DdlIsCreateContentIfContentChanged, "生成", "不生成");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateContentIfContentChanged, Site.IsCreateContentIfContentChanged.ToString());

            EBooleanUtils.AddListItems(DdlIsCreateChannelIfChannelChanged, "生成", "不生成");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateChannelIfChannelChanged, Site.IsCreateChannelIfChannelChanged.ToString());

            EBooleanUtils.AddListItems(DdlIsCreateShowPageInfo, "显示", "不显示");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateShowPageInfo, Site.IsCreateShowPageInfo.ToString());

            EBooleanUtils.AddListItems(DdlIsCreateIe8Compatible, "强制兼容", "不设置");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateIe8Compatible, Site.IsCreateIe8Compatible.ToString());

            EBooleanUtils.AddListItems(DdlIsCreateBrowserNoCache, "强制清除缓存", "不设置");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateBrowserNoCache, Site.IsCreateBrowserNoCache.ToString());

            EBooleanUtils.AddListItems(DdlIsCreateJsIgnoreError, "包含JS容错代码", "不设置");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateJsIgnoreError, Site.IsCreateJsIgnoreError.ToString());

            EBooleanUtils.AddListItems(DdlIsCreateWithJQuery, "是", "否");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateWithJQuery, Site.IsCreateWithJQuery.ToString());

            EBooleanUtils.AddListItems(DdlIsCreateDoubleClick, "启用双击生成", "不启用");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateDoubleClick, Site.IsCreateDoubleClick.ToString());

            TbCreateStaticMaxPage.Text = Site.CreateStaticMaxPage.ToString();

            EBooleanUtils.AddListItems(DdlIsCreateUseDefaultFileName, "启用", "不启用");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateUseDefaultFileName, Site.IsCreateUseDefaultFileName.ToString());
            PhIsCreateUseDefaultFileName.Visible = Site.IsCreateUseDefaultFileName;
            TbCreateDefaultFileName.Text = Site.CreateDefaultFileName;

            EBooleanUtils.AddListItems(DdlIsCreateStaticContentByAddDate, "启用", "不启用");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateStaticContentByAddDate, Site.IsCreateStaticContentByAddDate.ToString());
            PhIsCreateStaticContentByAddDate.Visible = Site.IsCreateStaticContentByAddDate;
            if (Site.CreateStaticContentAddDate != DateTime.MinValue)
            {
                TbCreateStaticContentAddDate.DateTime = Site.CreateStaticContentAddDate;
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

		    Site.IsCreateContentIfContentChanged = TranslateUtils.ToBool(DdlIsCreateContentIfContentChanged.SelectedValue);
		    Site.IsCreateChannelIfChannelChanged = TranslateUtils.ToBool(DdlIsCreateChannelIfChannelChanged.SelectedValue);

		    Site.IsCreateShowPageInfo = TranslateUtils.ToBool(DdlIsCreateShowPageInfo.SelectedValue);
		    Site.IsCreateIe8Compatible = TranslateUtils.ToBool(DdlIsCreateIe8Compatible.SelectedValue);
		    Site.IsCreateBrowserNoCache = TranslateUtils.ToBool(DdlIsCreateBrowserNoCache.SelectedValue);
		    Site.IsCreateJsIgnoreError = TranslateUtils.ToBool(DdlIsCreateJsIgnoreError.SelectedValue);
		    Site.IsCreateWithJQuery = TranslateUtils.ToBool(DdlIsCreateWithJQuery.SelectedValue);

		    Site.IsCreateDoubleClick = TranslateUtils.ToBool(DdlIsCreateDoubleClick.SelectedValue);
		    Site.CreateStaticMaxPage = TranslateUtils.ToInt(TbCreateStaticMaxPage.Text);

		    Site.IsCreateUseDefaultFileName = TranslateUtils.ToBool(DdlIsCreateUseDefaultFileName.SelectedValue);
		    if (Site.IsCreateUseDefaultFileName)
		    {
		        Site.CreateDefaultFileName = TbCreateDefaultFileName.Text;
		    }

            Site.IsCreateStaticContentByAddDate = TranslateUtils.ToBool(DdlIsCreateStaticContentByAddDate.SelectedValue);
		    if (Site.IsCreateStaticContentByAddDate)
		    {
		        Site.CreateStaticContentAddDate = TbCreateStaticContentAddDate.DateTime;
		    } 

            DataProvider.SiteRepository.UpdateAsync(Site).GetAwaiter().GetResult();

            AuthRequest.AddSiteLogAsync(SiteId, "修改页面生成设置").GetAwaiter().GetResult();

            SuccessMessage("页面生成设置修改成功！");
        }
	}
}
