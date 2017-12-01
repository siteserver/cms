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
    public class PageVote : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;

        public Button BtnAdd;
        public Button BtnDelete;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageVote), new NameValueCollection
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
                        DataProviderWx.VoteDao.Delete(PublishmentSystemId, list);
                        SuccessMessage("投票删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "投票删除失败！");
                    }
                }
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = 30;
            
            SpContents.SelectCommand = DataProviderWx.VoteDao.GetSelectString(PublishmentSystemId);
            SpContents.SortField = VoteAttribute.Id;
            SpContents.SortMode = SortMode.ASC;
            RptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdVote, "微投票", AppManager.WeiXin.Permission.WebSite.Vote);
                SpContents.DataBind();

                var urlAdd = PageVoteAdd.GetRedirectUrl(PublishmentSystemId, 0);
                BtnAdd.Attributes.Add("onclick", $"location.href='{urlAdd}';return false");

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemId), "Delete", "True");
                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的投票活动", "此操作将删除所选投票活动，确认吗？"));
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var voteInfo = new VoteInfo(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlTitle = e.Item.FindControl("ltlTitle") as Literal;
                var ltlKeywords = e.Item.FindControl("ltlKeywords") as Literal;
                var ltlType = e.Item.FindControl("ltlType") as Literal;
                var ltlStartDate = e.Item.FindControl("ltlStartDate") as Literal;
                var ltlEndDate = e.Item.FindControl("ltlEndDate") as Literal;
                var ltlUserCount = e.Item.FindControl("ltlUserCount") as Literal;
                var ltlPvCount = e.Item.FindControl("ltlPVCount") as Literal;
                var ltlIsEnabled = e.Item.FindControl("ltlIsEnabled") as Literal;
                var ltlLogUrl = e.Item.FindControl("ltlLogUrl") as Literal;
                var ltlPreviewUrl = e.Item.FindControl("ltlPreviewUrl") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlTitle.Text = voteInfo.Title;
                ltlKeywords.Text = DataProviderWx.KeywordDao.GetKeywords(voteInfo.KeywordId);
                ltlType.Text = TranslateUtils.ToBool(voteInfo.ContentIsImageOption) ? "图文" : "文字";
                ltlType.Text += TranslateUtils.ToBool(voteInfo.ContentIsCheckBox) ? " 多选" : " 单选";
                ltlStartDate.Text = DateUtils.GetDateAndTimeString(voteInfo.StartDate);
                ltlEndDate.Text = DateUtils.GetDateAndTimeString(voteInfo.EndDate);
                ltlUserCount.Text = voteInfo.UserCount.ToString();
                ltlPvCount.Text = voteInfo.PvCount.ToString();

                ltlIsEnabled.Text = StringUtils.GetTrueOrFalseImageHtml(!voteInfo.IsDisabled);

                var urlLog = PageVoteLog.GetRedirectUrl(PublishmentSystemId, voteInfo.Id, GetRedirectUrl(PublishmentSystemId));
                ltlLogUrl.Text = $@"<a href=""{urlLog}"">投票记录</a>";

                //var urlPreview = VoteManager.GetVoteUrl(PublishmentSystemInfo, voteInfo, string.Empty);
                //urlPreview = BackgroundPreview.GetRedirectUrlToMobile(urlPreview);
                //ltlPreviewUrl.Text = $@"<a href=""{urlPreview}"" target=""_blank"">预览</a>";

                var urlEdit = PageVoteAdd.GetRedirectUrl(PublishmentSystemId, voteInfo.Id);
                ltlEditUrl.Text = $@"<a href=""{urlEdit}"">编辑</a>";
            }
        }
    }
}
