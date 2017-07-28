using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;
using BaiRong.Controls;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundKeyword : BackgroundBasePageWX
	{
        public DropDownList ddlKeywordType;
        public TextBox tbKeyword;

        public Repeater rptContents;
        public SqlPager spContents;

        public static string GetRedirectUrl(int publishmentSystemID)
        {
            return PageUtils.GetWXUrl($"background_keyword.aspx?publishmentSystemID={publishmentSystemID}");
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (Request.QueryString["delete"] != null)
			{
                var keywordID = TranslateUtils.ToInt(Request.QueryString["deleteKeywordID"]);
                var keyword = Request.QueryString["deleteKeyword"];
			
				try
				{
                    var keywordInfo = DataProviderWX.KeywordDAO.GetKeywordInfo(keywordID);
                    var keywordList = TranslateUtils.StringCollectionToStringList(keywordInfo.Keywords, ' ');
                    if (keywordList.Remove(keyword))
                    {
                        keywordInfo.Keywords = TranslateUtils.ObjectCollectionToString(keywordList, " ");
                        DataProviderWX.KeywordDAO.Update(keywordInfo);
                        SuccessDeleteMessage();
                    }
				}
				catch(Exception ex)
				{
                    FailDeleteMessage(ex);
				}
			}

            spContents.ControlToPaginate = rptContents;
            spContents.ConnectionString = BaiRongDataProvider.ConnectionString;
            if (Request.QueryString["keywordType"] == null)
            {
                spContents.SelectCommand = DataProviderWX.KeywordMatchDAO.GetSelectString(PublishmentSystemID);
            }
            else
            {
                var keywordType = Request.QueryString["keywordType"];
                var keyword = Request.QueryString["keyword"];
                spContents.SelectCommand = DataProviderWX.KeywordMatchDAO.GetSelectString(PublishmentSystemID, keywordType, keyword);
            }
            spContents.ItemsPerPage = 60;
            spContents.SortField = "KeywordID";
            spContents.SortMode = SortMode.DESC;
            rptContents.ItemDataBound += new RepeaterItemEventHandler(rptContents_ItemDataBound);

			if (!IsPostBack)
            {
                BreadCrumb(AppManager.CMS.LeftMenu.ID_Content, AppManager.CMS.LeftMenu.Content.ID_Category, "关键字管理", AppManager.CMS.Permission.WebSite.Category);

                var listItem = new ListItem("所有类型", string.Empty);
                ddlKeywordType.Items.Add(listItem);

                EKeywordTypeUtils.AddListItems(ddlKeywordType);

                if (Request.QueryString["keywordType"] != null)
                {
                    var keywordType = Request.QueryString["keywordType"];
                    var keyword = Request.QueryString["keyword"];

                    ControlUtils.SelectListItems(ddlKeywordType, keywordType);
                    tbKeyword.Text = keyword;
                }

                spContents.DataBind();                
			}
		}

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var keywordID = TranslateUtils.EvalInt(e.Item.DataItem, "KeywordID");
                var keyword = TranslateUtils.EvalString(e.Item.DataItem, "Keyword");
                var keywordType = EKeywordTypeUtils.GetEnumType(TranslateUtils.EvalString(e.Item.DataItem, "KeywordType"));

                var ltlKeyword = e.Item.FindControl("ltlKeyword") as Literal;

                string urlDelete =
                    $@"{GetRedirectUrl(PublishmentSystemID)}&delete=true&deleteKeywordID={keywordID}&deleteKeyword={keyword}";

                ltlKeyword.Text =
                    $@"<div class=""alert alert-success pull-left"" style=""margin:5px;padding-right:14px;""><strong style=""color: #468847"">{keyword}</strong>&nbsp;({EKeywordTypeUtils
                        .GetText(keywordType)})&nbsp;<a href=""javascript:;"" onclick=""{Modal.KeywordEdit
                        .GetOpenWindowString(PublishmentSystemID, keywordID, keyword)}""><i class=""icon-edit""></i></a>&nbsp;<a href=""{urlDelete}"" onclick=""javascript:return confirm('此操作将删除关键字“{keyword}”，确认吗？');""><i class=""icon-remove""></i></a></div>";
            }
        }

        public void Search_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(PageUrl, true);
        }

        private string _pageUrl;
        private string PageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_pageUrl))
                {
                    _pageUrl =
                        $"background_keyword.aspx?publishmentSystemID={PublishmentSystemID}&keywordType={ddlKeywordType.SelectedValue}&keyword={tbKeyword.Text}";
                }
                return _pageUrl;
            }
        }
	}
}
