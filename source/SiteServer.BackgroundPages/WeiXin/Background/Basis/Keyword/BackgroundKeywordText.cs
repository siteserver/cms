using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.WeiXin.Core;
using SiteServer.CMS.Core;
using SiteServer.WeiXin.Model;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundKeywordText : BackgroundBasePageWX
    {
        public DataGrid dgContents;
        public Button btnAdd;

        public static string GetRedirectUrl(int publishmentSystemID)
        {
            return PageUtils.GetWXUrl($"background_keywordText.aspx?publishmentSystemID={publishmentSystemID}");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Request.QueryString["Delete"] != null)
            {
                var keywordID = TranslateUtils.ToInt(Request.QueryString["keywordID"]);

                try
                {
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
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Accounts, AppManager.WeiXin.LeftMenu.Function.ID_TextReply, string.Empty, AppManager.WeiXin.Permission.WebSite.TextReply);

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
                    AddWaitAndRedirectScript(GetRedirectUrl(PublishmentSystemID));
                }

                dgContents.DataSource = DataProviderWX.KeywordDAO.GetDataSource(PublishmentSystemID, EKeywordType.Text);
                dgContents.ItemDataBound += new DataGridItemEventHandler(dgContents_ItemDataBound);
                dgContents.DataBind();

                var showPopWinString = Modal.KeywordAddText.GetOpenWindowStringToAdd(PublishmentSystemID);
                btnAdd.Attributes.Add("onclick", showPopWinString);
            }
        }

        void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var keywordID = TranslateUtils.EvalInt(e.Item.DataItem, "KeywordID");
                var keywords = TranslateUtils.EvalString(e.Item.DataItem, "Keywords");
                var reply = TranslateUtils.EvalString(e.Item.DataItem, "Reply");
                var isDisabled = TranslateUtils.ToBool(TranslateUtils.EvalString(e.Item.DataItem, "IsDisabled"));
                var matchType = EMatchTypeUtils.GetEnumType(TranslateUtils.EvalString(e.Item.DataItem, "MatchType"));
                var addDate = TranslateUtils.EvalDateTime(e.Item.DataItem, "AddDate");

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

                var redirectUrl = GetRedirectUrl(PublishmentSystemID);

                hlUp.NavigateUrl = $"{redirectUrl}&SetTaxis=True&KeywordID={keywordID}&Direction=UP";
                hlDown.NavigateUrl = $"{redirectUrl}&SetTaxis=True&KeywordID={keywordID}&Direction=DOWN";

                var showPopWinString = Modal.KeywordAddText.GetOpenWindowStringToEdit(PublishmentSystemID, keywordID);
                ltlEditUrl.Text = $@"<a href=""javascript:;"" onClick=""{showPopWinString}"">修改</a>";

                ltlDeleteUrl.Text =
                    $@"<a href=""{redirectUrl}&Delete=True&KeywordID={keywordID}"" onClick=""javascript:return confirm('此操作将删除关键字“{keywords}”，确认吗？');"">删除</a>";
            }
        }
    }
}
