using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageMessageContent : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;

        public Button BtnDelete;
        public Button BtnReturn;

        private int _messageId;
        private string _returnUrl;

        public static string GetRedirectUrl(int publishmentSystemId, int messageId, string returnUrl)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageMessageContent), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"messageId", messageId.ToString()},
                {"returnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            _messageId = TranslateUtils.ToInt(Request.QueryString["messageID"]);
            _returnUrl = StringUtils.ValueFromUrl(Request.QueryString["returnUrl"]);

            if (!string.IsNullOrEmpty(Request.QueryString["Delete"]))
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        DataProviderWx.MessageContentDao.Delete(PublishmentSystemId, list);
                        SuccessMessage("删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "删除失败！");
                    }
                }
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = 30;
            
            SpContents.SelectCommand = DataProviderWx.MessageContentDao.GetSelectString(PublishmentSystemId, _messageId);
            SpContents.SortField = MessageContentAttribute.Id;
            SpContents.SortMode = SortMode.DESC;
            RptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdMessage, "留言查看", AppManager.WeiXin.Permission.WebSite.Message);
                SpContents.DataBind();

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemId, _messageId, _returnUrl), "Delete", "True");
                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的留言项", "此操作将删除所选留言项，确认吗？"));
                BtnReturn.Attributes.Add("onclick", $"location.href='{_returnUrl}';return false");
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
                var ltlWxOpenId = e.Item.FindControl("ltlWXOpenID") as Literal;
                var ltlIpAddress = e.Item.FindControl("ltlIPAddress") as Literal;
                var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();

                ltlDisplayName.Text = contentInfo.DisplayName;
                ltlContent.Text = contentInfo.Content;
                ltlIsReply.Text = contentInfo.IsReply ? "评论" : "留言";
                ltlWxOpenId.Text = contentInfo.WxOpenId;
                ltlIpAddress.Text = contentInfo.IpAddress;
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(contentInfo.AddDate);
            }
        }
    }
}
