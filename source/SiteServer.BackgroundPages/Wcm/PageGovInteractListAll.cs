using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Wcm
{
    public class PageGovInteractListAll : BasePageGovInteractList
    {
        public DropDownList ddlTaxis;
        public DropDownList ddlState;
        public DateTimeTextBox tbDateFrom;
        public DateTimeTextBox tbDateTo;
        public TextBox tbKeyword;

        public static string GetRedirectUrl(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetWcmUrl(nameof(PageGovInteractListAll), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovInteract, "所有办件", AppManager.Wcm.Permission.WebSite.GovInteract);

                EBooleanUtils.AddListItems(ddlTaxis, "倒序", "正序");
                if (Body.IsQueryExists("isTaxisDESC"))
                {
                    var isTaxisDesc = TranslateUtils.ToBool(Request.QueryString["isTaxisDESC"]);
                    ControlUtils.SelectListItemsIgnoreCase(ddlTaxis, isTaxisDesc.ToString());
                }
                var listItem = new ListItem("全部", string.Empty);
                ddlState.Items.Add(listItem);
                EGovInteractStateUtils.AddListItems(ddlState);
                if (Body.IsQueryExists("state"))
                {
                    ControlUtils.SelectListItemsIgnoreCase(ddlState, Request.QueryString["state"]);
                }
                tbDateFrom.Text = Request.QueryString["dateFrom"];
                tbDateTo.Text = Request.QueryString["dateTo"];
                tbKeyword.Text = Request.QueryString["keyword"];
            }
        }

        public void Search_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageUrl);
        }

        protected override string GetSelectString()
        {
            if (Body.IsQueryExists("state") || Body.IsQueryExists("dateFrom") || Body.IsQueryExists("dateTo") || Body.IsQueryExists("keyword"))
            {
                return DataProvider.GovInteractContentDao.GetSelectString(PublishmentSystemInfo, nodeID, Request.QueryString["state"], Request.QueryString["dateFrom"], Request.QueryString["dateTo"], Request.QueryString["keyword"]);
            }
            else
            {
                return DataProvider.GovInteractContentDao.GetSelectString(PublishmentSystemInfo, nodeID);
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
                    _pageUrl = PageUtils.GetWcmUrl(nameof(PageGovInteractListAll), new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"NodeID", nodeID.ToString()},
                        {"isTaxisDESC", ddlTaxis.SelectedValue},
                        {"state", ddlState.SelectedValue},
                        {"dateFrom", tbDateFrom.Text},
                        {"dateTo", tbDateTo.Text},
                        {"keyword", tbKeyword.Text},
                        {"page", TranslateUtils.ToInt(Request.QueryString["page"], 1).ToString()}
                    });
                }
                return _pageUrl;
            }
        }
    }
}
