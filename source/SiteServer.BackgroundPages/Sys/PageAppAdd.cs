using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using System.Collections.Generic;

namespace SiteServer.BackgroundPages.Sys
{
    public class PageAppAdd : BasePageCms
    {
        protected override bool IsSinglePage => true;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSysUrl(nameof(PageAppAdd), null);
        }

        public Repeater RptContents;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                //if (AppManager.IsWcm())
                //{
                //    PageUtils.RedirectToLoadingPage(PagePublishmentSystemAdd.GetRedirectUrl(EPublishmentSystemType.Wcm));
                //}
                //else if (AppManager.IsWeiXin())
                //{
                //    PageUtils.RedirectToLoadingPage(PagePublishmentSystemAdd.GetRedirectUrl(EPublishmentSystemType.WeiXin));
                //}
                //else
                //{
                //    PageUtils.RedirectToLoadingPage(PagePublishmentSystemAdd.GetRedirectUrl(EPublishmentSystemType.Cms));
                //}

                BreadCrumbSys(AppManager.Sys.LeftMenu.Site, "创建新应用", AppManager.Sys.Permission.SysSite);

                var list = new List<EPublishmentSystemType>
                {
                    EPublishmentSystemType.Cms
                };
                if (AppManager.IsWcm())
                {
                    list.Add(EPublishmentSystemType.Wcm);
                }
                if (AppManager.IsWeiXin())
                {
                    list.Add(EPublishmentSystemType.WeiXin);
                }

                RptContents.DataSource = list;
                RptContents.ItemDataBound += rptContents_ItemDataBound;
                RptContents.DataBind();
            }
        }

        private static void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var publishmentSystemType = (EPublishmentSystemType)e.Item.DataItem;

            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");

            string redirectUrl = PagePublishmentSystemAdd.GetRedirectUrl(publishmentSystemType);
            string iconHtml = EPublishmentSystemTypeUtils.GetIconHtml(publishmentSystemType, "icon-5");
            string description = EPublishmentSystemTypeUtils.GetText(publishmentSystemType);

            string appName = EPublishmentSystemTypeUtils.GetText(publishmentSystemType);

            ltlHtml.Text = $@"
  <span class=""icon-span"">
    <a href=""{redirectUrl}"">
      {iconHtml}
      <h5>
        创建{appName}应用
        <br>
        <small>{description}</small>
      </h5>
    </a>
  </span>
";
        }
    }
}
