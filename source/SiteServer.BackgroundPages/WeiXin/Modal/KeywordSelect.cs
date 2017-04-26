using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Controls;
using BaiRong.Core;
using SiteServer.CMS.BackgroundPages;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;

namespace SiteServer.WeiXin.BackgroundPages.Modal
{
	public class KeywordSelect : BackgroundBasePage
	{
        public DropDownList ddlKeywordType;
        public TextBox tbKeyword;

        public Repeater rptContents;
        public SqlPager spContents;

        private string jsMethod;

        public static string GetOpenWindowString(int publishmentSystemID, string jsMethod)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            arguments.Add("jsMethod", jsMethod);
            return PageUtilityWX.GetOpenWindowString("选择关键词", "modal_keywordSelect.aspx", arguments);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            jsMethod = Request.QueryString["jsMethod"];

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
            spContents.ItemsPerPage = 50;
            spContents.SortField = DataProviderWX.KeywordMatchDAO.GetSortField();
            spContents.SortMode = SortMode.DESC;
            rptContents.ItemDataBound += new RepeaterItemEventHandler(rptContents_ItemDataBound);

			if(!IsPostBack)
            {
                BreadCrumb(AppManager.CMS.LeftMenu.ID_Content, "关键词搜索", string.Empty);

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

                ltlKeyword.Text =
                    $@"<div class=""alert alert-success pull-left"" style=""margin:5px;padding-right:14px; cursor:pointer;"" onclick=""window.parent.{jsMethod}('{keyword}');{JsUtils.OpenWindow.HIDE_POP_WIN}""><strong style=""color: #468847"">{keyword}</strong>&nbsp;({EKeywordTypeUtils
                        .GetText(keywordType)})</div>";
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
                        $"modal_keywordSelect.aspx?publishmentSystemID={PublishmentSystemID}&keywordType={ddlKeywordType.SelectedValue}&keyword={tbKeyword.Text}&jsMethod={jsMethod}";
                }
                return _pageUrl;
            }
        }
	}
}
