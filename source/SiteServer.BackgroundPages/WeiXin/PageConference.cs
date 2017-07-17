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
    public class PageConference : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;

        public Button BtnAdd;
        public Button BtnDelete;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageConference), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!string.IsNullOrEmpty(Request.QueryString["Delete"]))
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        DataProviderWx.ConferenceDao.Delete(PublishmentSystemId, list);
                        SuccessMessage("会议（活动）删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "会议（活动）删除失败！");
                    }
                }
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = 30;
            SpContents.SelectCommand = DataProviderWx.ConferenceDao.GetSelectString(PublishmentSystemId);
            SpContents.SortField = ConferenceAttribute.Id;
            SpContents.SortMode = SortMode.ASC;
            RptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdConference, "微会议（活动）", AppManager.WeiXin.Permission.WebSite.Conference);
                SpContents.DataBind();

                var urlAdd = PageConferenceAdd.GetRedirectUrl(PublishmentSystemId, 0);
                BtnAdd.Attributes.Add("onclick", $"location.href='{urlAdd}';return false");

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemId), "Delete", "True");
                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的会议（活动）活动", "此操作将删除所选会议（活动）活动，确认吗？"));
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var conferenceInfo = new ConferenceInfo(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlTitle = e.Item.FindControl("ltlTitle") as Literal;
                var ltlKeywords = e.Item.FindControl("ltlKeywords") as Literal;
                var ltlStartDate = e.Item.FindControl("ltlStartDate") as Literal;
                var ltlEndDate = e.Item.FindControl("ltlEndDate") as Literal;
                var ltlUserCount = e.Item.FindControl("ltlUserCount") as Literal;
                var ltlPvCount = e.Item.FindControl("ltlPVCount") as Literal;
                var ltlIsEnabled = e.Item.FindControl("ltlIsEnabled") as Literal;
                var ltlLogUrl = e.Item.FindControl("ltlLogUrl") as Literal;
                var ltlPreviewUrl = e.Item.FindControl("ltlPreviewUrl") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlTitle.Text = conferenceInfo.Title;
                ltlKeywords.Text = DataProviderWx.KeywordDao.GetKeywords(conferenceInfo.KeywordId);
                ltlStartDate.Text = DateUtils.GetDateAndTimeString(conferenceInfo.StartDate);
                ltlEndDate.Text = DateUtils.GetDateAndTimeString(conferenceInfo.EndDate);
                ltlUserCount.Text = conferenceInfo.UserCount.ToString();
                ltlPvCount.Text = conferenceInfo.PvCount.ToString();

                ltlIsEnabled.Text = StringUtils.GetTrueOrFalseImageHtml(!conferenceInfo.IsDisabled);

                var urlContent = PageConferenceContent.GetRedirectUrl(PublishmentSystemId, conferenceInfo.Id, GetRedirectUrl(PublishmentSystemId));
                ltlLogUrl.Text = $@"<a href=""{urlContent}"">申请参会列表</a>";

                //var urlPreview = ConferenceManager.GetConferenceUrl(conferenceInfo, string.Empty);
                //urlPreview = BackgroundPreview.GetRedirectUrlToMobile(urlPreview);
                //ltlPreviewUrl.Text = $@"<a href=""{urlPreview}"" target=""_blank"">预览</a>";

                var urlEdit = PageConferenceAdd.GetRedirectUrl(PublishmentSystemId, conferenceInfo.Id);
                ltlEditUrl.Text = $@"<a href=""{urlEdit}"">编辑</a>";
            }
        }
    }
}
