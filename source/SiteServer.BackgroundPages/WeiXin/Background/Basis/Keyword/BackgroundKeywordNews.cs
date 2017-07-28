using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.WeiXin.Core;
using SiteServer.CMS.Core;
using SiteServer.WeiXin.Model;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundKeywordNews : BackgroundBasePageWX
    {
        public Repeater rptContents;

        public Button btnAddSingle;
        public Button btnAddMultiple;

        public static string GetRedirectUrl(int publishmentSystemID)
        {
            return PageUtils.GetWXUrl($"background_keywordNews.aspx?publishmentSystemID={publishmentSystemID}");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Request.QueryString["delete"] != null)
            {
                var keywordID = TranslateUtils.ToInt(Request.QueryString["keywordID"]);

                try
                {
                    foreach (var resourceID in DataProviderWX.KeywordResourceDAO.GetResourceIDList(keywordID))
                    {
                        FileUtilityWX.DeleteWeiXinContent(PublishmentSystemInfo, keywordID, resourceID);
                    }
                    DataProviderWX.KeywordDAO.Delete(keywordID);
                    StringUtility.AddLog(PublishmentSystemID, "删除关键字");
                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Accounts, AppManager.WeiXin.LeftMenu.Function.ID_ImageReply, string.Empty, AppManager.WeiXin.Permission.WebSite.ImageReply);
                if (Request.QueryString["SetTaxis"] != null)
                {
                    var keywordID = TranslateUtils.ToInt(Request.QueryString["keywordID"]);
                    var direction = Request.QueryString["Direction"];

                    switch (direction.ToUpper())
                    {
                        case "UP":
                            DataProviderWX.KeywordDAO.UpdateTaxisToUp(PublishmentSystemID, EKeywordType.Text, keywordID);
                            break;
                        case "DOWN":
                            DataProviderWX.KeywordDAO.UpdateTaxisToDown(PublishmentSystemID, EKeywordType.Text, keywordID);
                            break;
                        default:
                            break;
                    }
                    SuccessMessage("排序成功！");
                    AddWaitAndRedirectScript(BackgroundKeywordText.GetRedirectUrl(PublishmentSystemID));
                }

                rptContents.DataSource = DataProviderWX.KeywordDAO.GetDataSource(PublishmentSystemID, EKeywordType.News);
                rptContents.ItemDataBound += new RepeaterItemEventHandler(rptContents_ItemDataBound);
                rptContents.DataBind();

                btnAddSingle.Attributes.Add("onclick", Modal.KeywordAddNews.GetOpenWindowStringToAdd(PublishmentSystemID, true));
                btnAddMultiple.Attributes.Add("onclick", Modal.KeywordAddNews.GetOpenWindowStringToAdd(PublishmentSystemID, false));
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var keywordID = TranslateUtils.EvalInt(e.Item.DataItem, "KeywordID");
                var keywords = TranslateUtils.EvalString(e.Item.DataItem, "Keywords");
                var isDisabled = TranslateUtils.ToBool(TranslateUtils.EvalString(e.Item.DataItem, "IsDisabled"));
                var reply = TranslateUtils.EvalString(e.Item.DataItem, "Reply");
                var matchType = EMatchTypeUtils.GetEnumType(TranslateUtils.EvalString(e.Item.DataItem, "MatchType"));
                var addDate = TranslateUtils.EvalDateTime(e.Item.DataItem, "AddDate");

                var phSingle = e.Item.FindControl("phSingle") as PlaceHolder;
                var phMultiple = e.Item.FindControl("phMultiple") as PlaceHolder;

                var resourceInfoList = DataProviderWX.KeywordResourceDAO.GetResourceInfoList(keywordID);

                phMultiple.Visible = resourceInfoList.Count > 1;
                phSingle.Visible = !phMultiple.Visible;

                if (phSingle.Visible)
                {
                    var resourceInfo = new KeywordResourceInfo();
                    if (resourceInfoList.Count > 0)
                    {
                        resourceInfo = resourceInfoList[0];
                    }

                    var ltlSingleTitle = e.Item.FindControl("ltlSingleTitle") as Literal;
                    var ltlSingleKeywords = e.Item.FindControl("ltlSingleKeywords") as Literal;
                    var ltlSingleAddDate = e.Item.FindControl("ltlSingleAddDate") as Literal;
                    var ltlSingleImageUrl = e.Item.FindControl("ltlSingleImageUrl") as Literal;
                    var ltlSingleSummary = e.Item.FindControl("ltlSingleSummary") as Literal;
                    var ltlSingleEditUrl = e.Item.FindControl("ltlSingleEditUrl") as Literal;
                    var ltlSingleDeleteUrl = e.Item.FindControl("ltlSingleDeleteUrl") as Literal;

                    ltlSingleTitle.Text = $@"<a href=""{"javascript:;"}"" target=""_blank"">{resourceInfo.Title}</a>";
                    ltlSingleKeywords.Text =
                        $@"{keywords + (isDisabled ? "(禁用)" : string.Empty)}&nbsp;<a href=""javascript:;"" onclick=""{Modal
                            .KeywordAddNews.GetOpenWindowStringToEdit(PublishmentSystemID, keywordID)}"">修改</a>";
                    ltlSingleAddDate.Text = addDate.ToShortDateString();
                    if (!string.IsNullOrEmpty(resourceInfo.ImageUrl))
                    {
                        ltlSingleImageUrl.Text =
                            $@"<img src=""{PageUtility.ParseNavigationUrl(PublishmentSystemInfo, resourceInfo.ImageUrl)}"" class=""appmsg_thumb"">";
                    }
                    ltlSingleSummary.Text = MPUtils.GetSummary(resourceInfo.Summary, resourceInfo.Content);
                    ltlSingleEditUrl.Text =
                        $@"<a class=""js_edit"" href=""{BackgroundKeywordNewsAdd.GetRedirectUrl(PublishmentSystemID,
                            keywordID, resourceInfo.ResourceID, phSingle.Visible)}""><i class=""icon18_common edit_gray"">编辑</i></a>";
                    ltlSingleDeleteUrl.Text =
                        $@"<a class=""js_del no_extra"" href=""{GetRedirectUrl(
                            PublishmentSystemID)}&delete=true&keywordID={keywordID}"" onclick=""javascript:return confirm('此操作将删除图文回复“{keywords}”，确认吗？');""><i class=""icon18_common del_gray"">删除</i></a>";
                }
                else
                {
                    var resourceInfo = resourceInfoList[0];
                    resourceInfoList.Remove(resourceInfo);

                    var ltlMultipleKeywords = e.Item.FindControl("ltlMultipleKeywords") as Literal;
                    var ltlMultipleAddDate = e.Item.FindControl("ltlMultipleAddDate") as Literal;
                    var ltlMultipleTitle = e.Item.FindControl("ltlMultipleTitle") as Literal;
                    var ltlMultipleImageUrl = e.Item.FindControl("ltlMultipleImageUrl") as Literal;
                    var rptMultipleContents = e.Item.FindControl("rptMultipleContents") as Repeater;
                    var ltlMultipleEditUrl = e.Item.FindControl("ltlMultipleEditUrl") as Literal;
                    var ltlMultipleDeleteUrl = e.Item.FindControl("ltlMultipleDeleteUrl") as Literal;

                    ltlMultipleKeywords.Text =
                        $@"{keywords + (isDisabled ? "(禁用)" : string.Empty)}&nbsp;<a href=""javascript:;"" onclick=""{Modal
                            .KeywordAddNews.GetOpenWindowStringToEdit(PublishmentSystemID, keywordID)}"">修改</a>";

                    ltlMultipleAddDate.Text = addDate.ToShortDateString();
                    ltlMultipleTitle.Text = $@"<a href=""{"javascript:;"}"" target=""_blank"">{resourceInfo.Title}</a>";
                    if (!string.IsNullOrEmpty(resourceInfo.ImageUrl))
                    {
                        ltlMultipleImageUrl.Text =
                            $@"<img src=""{PageUtility.ParseNavigationUrl(PublishmentSystemInfo, resourceInfo.ImageUrl)}"" class=""appmsg_thumb"">";
                    }

                    rptMultipleContents.DataSource = resourceInfoList;
                    rptMultipleContents.ItemDataBound += rptMultipleContents_ItemDataBound;
                    rptMultipleContents.DataBind();

                    ltlMultipleEditUrl.Text =
                        $@"<a class=""js_edit"" href=""{BackgroundKeywordNewsAdd.GetRedirectUrl(PublishmentSystemID,
                            keywordID, resourceInfo.ResourceID, phSingle.Visible)}""><i class=""icon18_common edit_gray"">编辑</i></a>";
                    ltlMultipleDeleteUrl.Text =
                        $@"<a class=""js_del no_extra"" href=""{GetRedirectUrl(
                            PublishmentSystemID)}&delete=true&keywordID={keywordID}"" onclick=""javascript:return confirm('此操作将删除图文回复“{keywords}”，确认吗？');""><i class=""icon18_common del_gray"">删除</i></a>";
                }
            }
        }

        void rptMultipleContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var resourceInfo = e.Item.DataItem as KeywordResourceInfo;

            var ltlImageUrl = e.Item.FindControl("ltlImageUrl") as Literal;
            var ltlTitle = e.Item.FindControl("ltlTitle") as Literal;

            if (string.IsNullOrEmpty(resourceInfo.ImageUrl))
            {
                ltlImageUrl.Text = @"<i class=""appmsg_thumb default"">缩略图</i>";
            }
            else
            {
                ltlImageUrl.Text =
                    $@"<img class=""appmsg_thumb"" style=""max-width:78px;max-height:78px;display:block"" src=""{PageUtility
                        .ParseNavigationUrl(PublishmentSystemInfo, resourceInfo.ImageUrl)}"">";
            }
            ltlTitle.Text = $@"<a href=""javascript:;"">{resourceInfo.Title}</a>";
        }
    }
}
