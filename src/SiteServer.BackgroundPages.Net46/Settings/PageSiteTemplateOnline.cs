using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageSiteTemplateOnline : BasePageCms
    {
        protected override bool IsSinglePage => true;

        public Repeater RptContents;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Page.IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Site);

            InfoMessage($@"本页面只显示部分免费模板，更多站点模板请访问官网：<a href=""{OnlineTemplateManager.UrlHome}"" target=""_blank"">{OnlineTemplateManager.UrlHome}</a>");

            List<Dictionary<string, string>> list;
            if (OnlineTemplateManager.TryGetOnlineTemplates(out list))
            {
                RptContents.DataSource = list;
                RptContents.ItemDataBound += RptContents_ItemDataBound;
                RptContents.DataBind();
            }
            else
            {
                FailMessage($"在线模板获取失败：页面地址{OnlineTemplateManager.UrlHome}无法访问！");
            }
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item) return;

            var dict = (Dictionary<string, string>)e.Item.DataItem;
            var title = dict["title"];
            var description = dict["description"];
            var author = dict["author"];
            var source = dict["source"];
            var lastEditDate = dict["lastEditDate"];

            var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");
            var ltlDescription = (Literal)e.Item.FindControl("ltlDescription"); 
            var ltlAuthor = (Literal)e.Item.FindControl("ltlAuthor");
            var ltlLastEditDate = (Literal)e.Item.FindControl("ltlLastEditDate");
            var ltlPreviewUrl = (Literal)e.Item.FindControl("ltlPreviewUrl");
            var ltlCreateUrl = (Literal)e.Item.FindControl("ltlCreateUrl");

            var templateUrl = OnlineTemplateManager.GetTemplateUrl(title);

            ltlTitle.Text = $@"<a href=""{templateUrl}"" target=""_blank"">{title}</a>";
            
            ltlDescription.Text = description;
            ltlAuthor.Text = author;
            if (!string.IsNullOrEmpty(source) && PageUtils.IsProtocolUrl(source))
            {
                ltlAuthor.Text = $@"<a href=""{source}"" target=""_blank"">{ltlAuthor.Text}</a>";
            }
            ltlLastEditDate.Text = lastEditDate;

            ltlPreviewUrl.Text = $@"<a href=""{templateUrl}"" target=""_blank"">模板详情</a>";

            var urlAdd = PageSiteAdd.GetRedirectUrl(string.Empty, title);
            ltlCreateUrl.Text = $@"<a href=""{urlAdd}"">创建站点</a>";

            //if (_directoryNameLowerList.Contains($"T_{title}".ToLower().Trim()))
            //{
            //    ltlDownloadUrl.Text = "已下载";
            //}
            //else
            //{
            //    var downloadUrl = OnlineTemplateManager.GetDownloadUrl(title);

            //    ltlCreateUrl.Text =
            //        $@"<a href=""javascript:;"" onclick=""{Cms.ModalProgressBar.GetOpenWindowStringWithSiteTemplateDownload($"{downloadUrl}")}"">下载并导入</a>";
            //}
        }
	}
}
