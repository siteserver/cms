using System;
using System.Web.UI.WebControls;
using BaiRong.Controls;
using BaiRong.Core;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundMessageContent : BackgroundBasePageWX
    {
        public Repeater rptContents;
        public SqlPager spContents;

        public Button btnDelete;
        public Button btnReturn;

        private int messageID;
        private string returnUrl;

        public static string GetRedirectUrl(int publishmentSystemID, int messageID, string returnUrl)
        {
            return PageUtils.GetWXUrl(
                $"background_messageContent.aspx?publishmentSystemID={publishmentSystemID}&messageID={messageID}&returnUrl={StringUtils.ValueToUrl(returnUrl)}");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            messageID = TranslateUtils.ToInt(Request.QueryString["messageID"]);
            returnUrl = StringUtils.ValueFromUrl(Request.QueryString["returnUrl"]);

            if (!string.IsNullOrEmpty(Request.QueryString["Delete"]))
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        DataProviderWX.MessageContentDAO.Delete(PublishmentSystemID, list);
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
            spContents.SelectCommand = DataProviderWX.MessageContentDAO.GetSelectString(PublishmentSystemID, messageID);
            spContents.SortField = MessageContentAttribute.ID;
            spContents.SortMode = SortMode.DESC;
            rptContents.ItemDataBound += new RepeaterItemEventHandler(rptContents_ItemDataBound);

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Message, "留言查看", AppManager.WeiXin.Permission.WebSite.Message);
                spContents.DataBind();

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemID, messageID, returnUrl), "Delete", "True");
                btnDelete.Attributes.Add("onclick", JsUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的留言项", "此操作将删除所选留言项，确认吗？"));
                btnReturn.Attributes.Add("onclick", $"location.href='{returnUrl}';return false");
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var contentInfo = new MessageContentInfo(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlDisplayName = e.Item.FindControl("ltlDisplayName") as Literal;
                var ltlContent = e.Item.FindControl("ltlContent") as Literal;
                var ltlIsReply = e.Item.FindControl("ltlIsReply") as Literal;
                var ltlWXOpenID = e.Item.FindControl("ltlWXOpenID") as Literal;
                var ltlIPAddress = e.Item.FindControl("ltlIPAddress") as Literal;
                var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();

                ltlDisplayName.Text = contentInfo.DisplayName;
                ltlContent.Text = contentInfo.Content;
                ltlIsReply.Text = contentInfo.IsReply ? "评论" : "留言";
                ltlWXOpenID.Text = contentInfo.WXOpenID;
                ltlIPAddress.Text = contentInfo.IPAddress;
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(contentInfo.AddDate);
            }
        }
    }
}
