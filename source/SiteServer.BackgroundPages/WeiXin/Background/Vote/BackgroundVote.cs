using System;
using System.Web.UI.WebControls;
using BaiRong.Controls;
using BaiRong.Core;
using SiteServer.CMS.BackgroundPages;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundVote : BackgroundBasePageWX
    {
        public Repeater rptContents;
        public SqlPager spContents;

        public Button btnAdd;
        public Button btnDelete;

        public static string GetRedirectUrl(int publishmentSystemID)
        {
            return PageUtils.GetWXUrl($"background_vote.aspx?publishmentSystemID={publishmentSystemID}");
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
                        DataProviderWX.VoteDAO.Delete(PublishmentSystemID, list);
                        SuccessMessage("投票删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "投票删除失败！");
                    }
                }
            }

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = 30;
            spContents.ConnectionString = BaiRongDataProvider.ConnectionString;
            spContents.SelectCommand = DataProviderWX.VoteDAO.GetSelectString(PublishmentSystemID);
            spContents.SortField = VoteAttribute.ID;
            spContents.SortMode = SortMode.ASC;
            rptContents.ItemDataBound += new RepeaterItemEventHandler(rptContents_ItemDataBound);

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Vote, "微投票", AppManager.WeiXin.Permission.WebSite.Vote);
                spContents.DataBind();

                var urlAdd = BackgroundVoteAdd.GetRedirectUrl(PublishmentSystemID, 0);
                btnAdd.Attributes.Add("onclick", $"location.href='{urlAdd}';return false");

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemID), "Delete", "True");
                btnDelete.Attributes.Add("onclick", JsUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的投票活动", "此操作将删除所选投票活动，确认吗？"));
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
                var ltlPVCount = e.Item.FindControl("ltlPVCount") as Literal;
                var ltlIsEnabled = e.Item.FindControl("ltlIsEnabled") as Literal;
                var ltlLogUrl = e.Item.FindControl("ltlLogUrl") as Literal;
                var ltlPreviewUrl = e.Item.FindControl("ltlPreviewUrl") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlTitle.Text = voteInfo.Title;
                ltlKeywords.Text = DataProviderWX.KeywordDAO.GetKeywords(voteInfo.KeywordID);
                ltlType.Text = TranslateUtils.ToBool(voteInfo.ContentIsImageOption) ? "图文" : "文字";
                ltlType.Text += TranslateUtils.ToBool(voteInfo.ContentIsCheckBox) ? " 多选" : " 单选";
                ltlStartDate.Text = DateUtils.GetDateAndTimeString(voteInfo.StartDate);
                ltlEndDate.Text = DateUtils.GetDateAndTimeString(voteInfo.EndDate);
                ltlUserCount.Text = voteInfo.UserCount.ToString();
                ltlPVCount.Text = voteInfo.PVCount.ToString();

                ltlIsEnabled.Text = StringUtils.GetTrueOrFalseImageHtml(!voteInfo.IsDisabled);

                var urlLog = BackgroundVoteLog.GetRedirectUrl(PublishmentSystemID, voteInfo.ID, GetRedirectUrl(PublishmentSystemID));
                ltlLogUrl.Text = $@"<a href=""{urlLog}"">投票记录</a>";

                var urlPreview = VoteManager.GetVoteUrl(voteInfo, string.Empty);
                urlPreview = BackgroundPreview.GetRedirectUrlToMobile(urlPreview);
                ltlPreviewUrl.Text = $@"<a href=""{urlPreview}"" target=""_blank"">预览</a>";

                var urlEdit = BackgroundVoteAdd.GetRedirectUrl(PublishmentSystemID, voteInfo.ID);
                ltlEditUrl.Text = $@"<a href=""{urlEdit}"">编辑</a>";
            }
        }
    }
}
