using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageKeywordText : BasePageCms
    {
        public DataGrid DgContents;
        public Button BtnAdd;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageKeywordText), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Request.QueryString["Delete"] != null)
            {
                var keywordId = TranslateUtils.ToInt(Request.QueryString["keywordID"]);

                try
                {
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
                BreadCrumb(AppManager.WeiXin.LeftMenu.IdAccounts, string.Empty, AppManager.WeiXin.Permission.WebSite.TextReply);

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
                    AddWaitAndRedirectScript(GetRedirectUrl(PublishmentSystemId));
                }

                DgContents.DataSource = DataProviderWx.KeywordDao.GetDataSource(PublishmentSystemId, EKeywordType.Text);
                DgContents.ItemDataBound += dgContents_ItemDataBound;
                DgContents.DataBind();

                var showPopWinString = ModalKeywordAddText.GetOpenWindowStringToAdd(PublishmentSystemId);
                BtnAdd.Attributes.Add("onclick", showPopWinString);
            }
        }

        void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var keywordId = SqlUtils.EvalInt(e.Item.DataItem, "KeywordId");
                var keywords = SqlUtils.EvalString(e.Item.DataItem, "Keywords");
                var reply = SqlUtils.EvalString(e.Item.DataItem, "Reply");
                var isDisabled = TranslateUtils.ToBool(SqlUtils.EvalString(e.Item.DataItem, "IsDisabled"));
                var matchType = EMatchTypeUtils.GetEnumType(SqlUtils.EvalString(e.Item.DataItem, "MatchType"));
                var addDate = SqlUtils.EvalDateTime(e.Item.DataItem, "AddDate");

                var ltlReply = e.Item.FindControl("ltlReply") as Literal;
                var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;
                var ltlMatchType = e.Item.FindControl("ltlMatchType") as Literal;
                var ltlIsEnabled = e.Item.FindControl("ltlIsEnabled") as Literal;
                var hlUp = e.Item.FindControl("hlUp") as HyperLink;
                var hlDown = e.Item.FindControl("hlDown") as HyperLink;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;
                var ltlDeleteUrl = e.Item.FindControl("ltlDeleteUrl") as Literal;

                ltlReply.Text = StringUtils.CleanText(reply);
                ltlAddDate.Text = DateUtils.GetDateString(addDate);
                ltlMatchType.Text = EMatchTypeUtils.GetText(matchType);
                ltlIsEnabled.Text = StringUtils.GetTrueOrFalseImageHtml(!isDisabled);

                var redirectUrl = GetRedirectUrl(PublishmentSystemId);

                hlUp.NavigateUrl = $"{redirectUrl}&SetTaxis=True&KeywordId={keywordId}&Direction=UP";
                hlDown.NavigateUrl = $"{redirectUrl}&SetTaxis=True&KeywordId={keywordId}&Direction=DOWN";

                var showPopWinString = ModalKeywordAddText.GetOpenWindowStringToEdit(PublishmentSystemId, keywordId);
                ltlEditUrl.Text = $@"<a href=""javascript:;"" onClick=""{showPopWinString}"">修改</a>";

                ltlDeleteUrl.Text =
                    $@"<a href=""{redirectUrl}&Delete=True&KeywordID={keywordId}"" onClick=""javascript:return confirm('此操作将删除关键字“{keywords}”，确认吗？');"">删除</a>";
            }
        }
    }
}
