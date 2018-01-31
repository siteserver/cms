using System;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;
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
        public DropDownList DdlIsCreateSearchDuplicate;
        public DropDownList DdlIsCreateWithJQuery;
        public DropDownList DdlIsCreateDoubleClick;
        public TextBox TbCreateStaticMaxPage;
        public DropDownList DdlIsCreateStaticContentByAddDate;
        public PlaceHolder PhIsCreateStaticContentByAddDate;
        public DateTimeTextBox TbCreateStaticContentAddDate;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.Permissions.WebSite.Create);

            EBooleanUtils.AddListItems(DdlIsCreateContentIfContentChanged, "生成", "不生成");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateContentIfContentChanged, SiteInfo.Additional.IsCreateContentIfContentChanged.ToString());

            EBooleanUtils.AddListItems(DdlIsCreateChannelIfChannelChanged, "生成", "不生成");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateChannelIfChannelChanged, SiteInfo.Additional.IsCreateChannelIfChannelChanged.ToString());

            EBooleanUtils.AddListItems(DdlIsCreateShowPageInfo, "显示", "不显示");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateShowPageInfo, SiteInfo.Additional.IsCreateShowPageInfo.ToString());

            EBooleanUtils.AddListItems(DdlIsCreateIe8Compatible, "强制兼容", "不设置");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateIe8Compatible, SiteInfo.Additional.IsCreateIe8Compatible.ToString());

            EBooleanUtils.AddListItems(DdlIsCreateBrowserNoCache, "强制清除缓存", "不设置");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateBrowserNoCache, SiteInfo.Additional.IsCreateBrowserNoCache.ToString());

            EBooleanUtils.AddListItems(DdlIsCreateJsIgnoreError, "包含JS容错代码", "不设置");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateJsIgnoreError, SiteInfo.Additional.IsCreateJsIgnoreError.ToString());

            EBooleanUtils.AddListItems(DdlIsCreateSearchDuplicate, "包含", "不包含");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateSearchDuplicate, SiteInfo.Additional.IsCreateSearchDuplicate.ToString());

            EBooleanUtils.AddListItems(DdlIsCreateWithJQuery, "是", "否");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateWithJQuery, SiteInfo.Additional.IsCreateWithJQuery.ToString());

            EBooleanUtils.AddListItems(DdlIsCreateDoubleClick, "启用双击生成", "不启用");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateDoubleClick, SiteInfo.Additional.IsCreateDoubleClick.ToString());

            TbCreateStaticMaxPage.Text = SiteInfo.Additional.CreateStaticMaxPage.ToString();

            EBooleanUtils.AddListItems(DdlIsCreateStaticContentByAddDate, "启用", "不启用");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateStaticContentByAddDate, SiteInfo.Additional.IsCreateStaticContentByAddDate.ToString());
            PhIsCreateStaticContentByAddDate.Visible = SiteInfo.Additional.IsCreateStaticContentByAddDate;
            if (SiteInfo.Additional.CreateStaticContentAddDate != DateTime.MinValue)
            {
                TbCreateStaticContentAddDate.DateTime = SiteInfo.Additional.CreateStaticContentAddDate;
            }
        }

        public void DdlIsCreateStaticContentByAddDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhIsCreateStaticContentByAddDate.Visible = TranslateUtils.ToBool(DdlIsCreateStaticContentByAddDate.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid) return;

		    SiteInfo.Additional.IsCreateContentIfContentChanged = TranslateUtils.ToBool(DdlIsCreateContentIfContentChanged.SelectedValue);
		    SiteInfo.Additional.IsCreateChannelIfChannelChanged = TranslateUtils.ToBool(DdlIsCreateChannelIfChannelChanged.SelectedValue);

		    SiteInfo.Additional.IsCreateShowPageInfo = TranslateUtils.ToBool(DdlIsCreateShowPageInfo.SelectedValue);
		    SiteInfo.Additional.IsCreateIe8Compatible = TranslateUtils.ToBool(DdlIsCreateIe8Compatible.SelectedValue);
		    SiteInfo.Additional.IsCreateBrowserNoCache = TranslateUtils.ToBool(DdlIsCreateBrowserNoCache.SelectedValue);
		    SiteInfo.Additional.IsCreateJsIgnoreError = TranslateUtils.ToBool(DdlIsCreateJsIgnoreError.SelectedValue);
		    SiteInfo.Additional.IsCreateSearchDuplicate = TranslateUtils.ToBool(DdlIsCreateSearchDuplicate.SelectedValue);
		    SiteInfo.Additional.IsCreateWithJQuery = TranslateUtils.ToBool(DdlIsCreateWithJQuery.SelectedValue);

		    SiteInfo.Additional.IsCreateDoubleClick = TranslateUtils.ToBool(DdlIsCreateDoubleClick.SelectedValue);
		    SiteInfo.Additional.CreateStaticMaxPage = TranslateUtils.ToInt(TbCreateStaticMaxPage.Text);

		    SiteInfo.Additional.IsCreateStaticContentByAddDate = TranslateUtils.ToBool(DdlIsCreateStaticContentByAddDate.SelectedValue);
		    if (SiteInfo.Additional.IsCreateStaticContentByAddDate)
		    {
		        SiteInfo.Additional.CreateStaticContentAddDate = TbCreateStaticContentAddDate.DateTime;
		    } 

            DataProvider.SiteDao.Update(SiteInfo);

            Body.AddSiteLog(SiteId, "修改页面生成设置");

            SuccessMessage("页面生成设置修改成功！");
        }
	}
}
