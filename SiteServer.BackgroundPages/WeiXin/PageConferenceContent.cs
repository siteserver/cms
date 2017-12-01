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
    public class PageConferenceContent : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;

        public Button BtnDelete;
        public Button BtnReturn;

        private int _conferenceId;
        private string _returnUrl;

        public static string GetRedirectUrl(int publishmentSystemId, int conferenceId, string returnUrl)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageConferenceContent), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"conferenceId", conferenceId.ToString()},
                {"returnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            _conferenceId = TranslateUtils.ToInt(Request.QueryString["conferenceID"]);
            _returnUrl = StringUtils.ValueFromUrl(Request.QueryString["returnUrl"]);

            if (!string.IsNullOrEmpty(Request.QueryString["Delete"]))
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        DataProviderWx.ConferenceContentDao.Delete(PublishmentSystemId, list);
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
            SpContents.SelectCommand = DataProviderWx.ConferenceContentDao.GetSelectString(PublishmentSystemId, _conferenceId);
            SpContents.SortField = ConferenceContentAttribute.Id;
            SpContents.SortMode = SortMode.DESC;
            RptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            { 
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdConference, "申请查看", AppManager.WeiXin.Permission.WebSite.Conference);
                SpContents.DataBind();

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemId, _conferenceId, _returnUrl), "Delete", "True");
                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的申请项", "此操作将删除所选申请项，确认吗？"));
                BtnReturn.Attributes.Add("onclick", $"location.href='{_returnUrl}';return false");
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
                var ltlWxOpenId = e.Item.FindControl("ltlWXOpenID") as Literal;
                var ltlIpAddress = e.Item.FindControl("ltlIPAddress") as Literal;
                var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();

                ltlRealName.Text = contentInfo.RealName;
                ltlMobile.Text = contentInfo.Mobile;
                ltlEmail.Text = contentInfo.Email;
                ltlCompany.Text = contentInfo.Company;
                ltlPosition.Text = contentInfo.Position;
                ltlNote.Text = contentInfo.Note;
                ltlWxOpenId.Text = contentInfo.WxOpenId;
                ltlIpAddress.Text = contentInfo.IpAddress;
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(contentInfo.AddDate);
            }
        }
    }
}
