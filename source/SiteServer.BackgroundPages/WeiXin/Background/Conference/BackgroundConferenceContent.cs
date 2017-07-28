using System;
using System.Web.UI.WebControls;
using BaiRong.Controls;
using BaiRong.Core;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;


namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundConferenceContent : BackgroundBasePageWX
    {
        public Repeater rptContents;
        public SqlPager spContents;

        public Button btnDelete;
        public Button btnReturn;

        private int conferenceID;
        private string returnUrl;

        public static string GetRedirectUrl(int publishmentSystemID, int conferenceID, string returnUrl)
        {
            return PageUtils.GetWXUrl(
                $"background_conferenceContent.aspx?publishmentSystemID={publishmentSystemID}&conferenceID={conferenceID}&returnUrl={StringUtils.ValueToUrl(returnUrl)}");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            conferenceID = TranslateUtils.ToInt(Request.QueryString["conferenceID"]);
            returnUrl = StringUtils.ValueFromUrl(Request.QueryString["returnUrl"]);

            if (!string.IsNullOrEmpty(Request.QueryString["Delete"]))
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        DataProviderWX.ConferenceContentDAO.Delete(PublishmentSystemID, list);
                        SuccessMessage("删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "删除失败！");
                    }
                }
            }

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = 30;
            spContents.ConnectionString = BaiRongDataProvider.ConnectionString;
            spContents.SelectCommand = DataProviderWX.ConferenceContentDAO.GetSelectString(PublishmentSystemID, conferenceID);
            spContents.SortField = ConferenceContentAttribute.ID;
            spContents.SortMode = SortMode.DESC;
            rptContents.ItemDataBound += new RepeaterItemEventHandler(rptContents_ItemDataBound);

            if (!IsPostBack)
            { 
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Conference, "申请查看", AppManager.WeiXin.Permission.WebSite.Conference);
                spContents.DataBind();

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemID, conferenceID, returnUrl), "Delete", "True");
                btnDelete.Attributes.Add("onclick", JsUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的申请项", "此操作将删除所选申请项，确认吗？"));
                btnReturn.Attributes.Add("onclick", $"location.href='{returnUrl}';return false");
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var contentInfo = new ConferenceContentInfo(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlRealName = e.Item.FindControl("ltlRealName") as Literal;
                var ltlMobile = e.Item.FindControl("ltlMobile") as Literal;
                var ltlEmail = e.Item.FindControl("ltlEmail") as Literal;
                var ltlCompany = e.Item.FindControl("ltlCompany") as Literal;
                var ltlPosition = e.Item.FindControl("ltlPosition") as Literal;
                var ltlNote = e.Item.FindControl("ltlNote") as Literal;
                var ltlWXOpenID = e.Item.FindControl("ltlWXOpenID") as Literal;
                var ltlIPAddress = e.Item.FindControl("ltlIPAddress") as Literal;
                var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();

                ltlRealName.Text = contentInfo.RealName;
                ltlMobile.Text = contentInfo.Mobile;
                ltlEmail.Text = contentInfo.Email;
                ltlCompany.Text = contentInfo.Company;
                ltlPosition.Text = contentInfo.Position;
                ltlNote.Text = contentInfo.Note;
                ltlWXOpenID.Text = contentInfo.WXOpenID;
                ltlIPAddress.Text = contentInfo.IPAddress;
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(contentInfo.AddDate);
            }
        }
    }
}
