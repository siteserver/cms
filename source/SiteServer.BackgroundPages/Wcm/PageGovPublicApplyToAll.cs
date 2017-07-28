using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Wcm
{
    public class PageGovPublicApplyToAll : BasePageGovPublicApplyTo
    {
        public DropDownList ddlTaxis;
        public DropDownList ddlState;
        public TextBox tbKeyword;

        public void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovPublic, AppManager.Wcm.LeftMenu.GovPublic.IdGovPublicApply, "所有申请", AppManager.Wcm.Permission.WebSite.GovPublicApply);

                EBooleanUtils.AddListItems(ddlTaxis, "倒序", "正序");
                if (Body.IsQueryExists("isTaxisDESC"))
                {
                    var isTaxisDesc = TranslateUtils.ToBool(Request.QueryString["isTaxisDESC"]);
                    ControlUtils.SelectListItemsIgnoreCase(ddlTaxis, isTaxisDesc.ToString());
                }
                var listItem = new ListItem("全部", string.Empty);
                ddlState.Items.Add(listItem);
                EGovPublicApplyStateUtils.AddListItems(ddlState);
                if (Body.IsQueryExists("state"))
                {
                    ControlUtils.SelectListItemsIgnoreCase(ddlState, Request.QueryString["state"]);
                }
                tbKeyword.Text = Request.QueryString["keyword"];
            }
        }

        public void Search_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageUrl);
        }

        protected override string GetSelectString()
        {
            if (Body.IsQueryExists("state") || Body.IsQueryExists("keyword"))
            {
                return DataProvider.GovPublicApplyDao.GetSelectString(PublishmentSystemId, Request.QueryString["state"], Request.QueryString["keyword"]);
            }
            else
            {
                return DataProvider.GovPublicApplyDao.GetSelectString(PublishmentSystemId);
            }
        }

        protected override SortMode GetSortMode()
        {
            var isTaxisDesc = true;
            if (Body.IsQueryExists("isTaxisDESC"))
            {
                isTaxisDesc = TranslateUtils.ToBool(Request.QueryString["isTaxisDESC"]);
            }
            return isTaxisDesc ? SortMode.DESC : SortMode.ASC;
        }

        private string _pageUrl;
        protected override string PageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_pageUrl))
                {
                    _pageUrl = PageUtils.GetWcmUrl(nameof(PageGovPublicApplyToAll), new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"isTaxisDESC", ddlTaxis.SelectedValue},
                        {"state", ddlState.SelectedValue},
                        {"keyword", tbKeyword.Text},
                        {"page", TranslateUtils.ToInt(Request.QueryString["page"], 1).ToString()}
                    });
                }
                return _pageUrl;
            }
        }
    }
}
