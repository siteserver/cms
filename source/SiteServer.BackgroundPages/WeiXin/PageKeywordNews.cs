using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.IO;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;
using SiteServer.CMS.WeiXin.MP;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageKeywordNews : BasePageCms
    {
        public Repeater RptContents;

        public Button BtnAddSingle;
        public Button BtnAddMultiple;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageKeywordNews), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Request.QueryString["delete"] != null)
            {
                var keywordId = TranslateUtils.ToInt(Request.QueryString["keywordID"]);

                try
                {
                    foreach (var resourceId in DataProviderWx.KeywordResourceDao.GetResourceIdList(keywordId))
                    {
                        FileUtilityWX.DeleteWeiXinContent(PublishmentSystemInfo, keywordId, resourceId);
                    }
                    DataProviderWx.KeywordDao.Delete(keywordId);
                    Body.AddSiteLog(PublishmentSystemId, "删除关键字");
                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }

            if (!IsPostBack)
            {
                BreadCrumb("", string.Empty, AppManager.WeiXin.Permission.WebSite.ImageReply);
                if (Request.QueryString["SetTaxis"] != null)
                {
                    var keywordId = TranslateUtils.ToInt(Request.QueryString["keywordID"]);
                    var direction = Request.QueryString["Direction"];

                    switch (direction.ToUpper())
                    {
                        case "UP":
                            DataProviderWx.KeywordDao.UpdateTaxisToUp(PublishmentSystemId, EKeywordType.Text, keywordId);
                            break;
                        case "DOWN":
                            DataProviderWx.KeywordDao.UpdateTaxisToDown(PublishmentSystemId, EKeywordType.Text, keywordId);
                            break;
                        default:
                            break;
                    }
                    SuccessMessage("排序成功！");
                    AddWaitAndRedirectScript(PageKeywordText.GetRedirectUrl(PublishmentSystemId));
                }

                RptContents.DataSource = DataProviderWx.KeywordDao.GetDataSource(PublishmentSystemId, EKeywordType.News);
                RptContents.ItemDataBound += rptContents_ItemDataBound;
                RptContents.DataBind();

                BtnAddSingle.Attributes.Add("onclick", ModalKeywordAddNews.GetOpenWindowStringToAdd(PublishmentSystemId, true));
                BtnAddMultiple.Attributes.Add("onclick", ModalKeywordAddNews.GetOpenWindowStringToAdd(PublishmentSystemId, false));
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var keywordId = SqlUtils.EvalInt(e.Item.DataItem, "KeywordId");
                var keywords = SqlUtils.EvalString(e.Item.DataItem, "Keywords");
                var isDisabled = TranslateUtils.ToBool(SqlUtils.EvalString(e.Item.DataItem, "IsDisabled"));
                var reply = SqlUtils.EvalString(e.Item.DataItem, "Reply");
                var matchType = EMatchTypeUtils.GetEnumType(SqlUtils.EvalString(e.Item.DataItem, "MatchType"));
                var addDate = SqlUtils.EvalDateTime(e.Item.DataItem, "AddDate");

                var phSingle = e.Item.FindControl("phSingle") as PlaceHolder;
                var phMultiple = e.Item.FindControl("phMultiple") as PlaceHolder;

                var resourceInfoList = DataProviderWx.KeywordResourceDao.GetResourceInfoList(keywordId);

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
                        $@"{keywords + (isDisabled ? "(禁用)" : string.Empty)}&nbsp;<a href=""javascript:;"" onclick=""{ModalKeywordAddNews.GetOpenWindowStringToEdit(PublishmentSystemId, keywordId)}"">修改</a>";
                    ltlSingleAddDate.Text = addDate.ToShortDateString();
                    if (!string.IsNullOrEmpty(resourceInfo.ImageUrl))
                    {
                        ltlSingleImageUrl.Text =
                            $@"<img src=""{PageUtility.ParseNavigationUrl(PublishmentSystemInfo, resourceInfo.ImageUrl)}"" class=""appmsg_thumb"">";
                    }
                    ltlSingleSummary.Text = MPUtils.GetSummary(resourceInfo.Summary, resourceInfo.Content);
                    ltlSingleEditUrl.Text =
                        $@"<a class=""js_edit"" href=""{PageKeywordNewsAdd.GetRedirectUrl(PublishmentSystemId,
                            keywordId, resourceInfo.ResourceId, phSingle.Visible)}""><i class=""icon18_common edit_gray"">编辑</i></a>";
                    ltlSingleDeleteUrl.Text =
                        $@"<a class=""js_del no_extra"" href=""{GetRedirectUrl(
                            PublishmentSystemId)}&delete=true&keywordID={keywordId}"" onclick=""javascript:return confirm('此操作将删除图文回复“{keywords}”，确认吗？');""><i class=""icon18_common del_gray"">删除</i></a>";
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
                        $@"{keywords + (isDisabled ? "(禁用)" : string.Empty)}&nbsp;<a href=""javascript:;"" onclick=""{ModalKeywordAddNews.GetOpenWindowStringToEdit(PublishmentSystemId, keywordId)}"">修改</a>";

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
                        $@"<a class=""js_edit"" href=""{PageKeywordNewsAdd.GetRedirectUrl(PublishmentSystemId,
                            keywordId, resourceInfo.ResourceId, phSingle.Visible)}""><i class=""icon18_common edit_gray"">编辑</i></a>";
                    ltlMultipleDeleteUrl.Text =
                        $@"<a class=""js_del no_extra"" href=""{GetRedirectUrl(
                            PublishmentSystemId)}&delete=true&keywordID={keywordId}"" onclick=""javascript:return confirm('此操作将删除图文回复“{keywords}”，确认吗？');""><i class=""icon18_common del_gray"">删除</i></a>";
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
