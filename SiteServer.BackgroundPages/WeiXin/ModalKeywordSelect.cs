using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
	public class ModalKeywordSelect : BasePageCms
    {
        public DropDownList DdlKeywordType;
        public TextBox TbKeyword;

        public Repeater RptContents;
        public SqlPager SpContents;

        private string _jsMethod;

        public static string GetOpenWindowString(int publishmentSystemId, string jsMethod)
        {
            return PageUtils.GetOpenWindowString("选择关键词",
                PageUtils.GetWeiXinUrl(nameof(ModalKeywordSelect), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"jsMethod", jsMethod}
                }));
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemId");
            _jsMethod = Request.QueryString["jsMethod"];

            SpContents.ControlToPaginate = RptContents;
            if (Request.QueryString["keywordType"] == null)
            {
                SpContents.SelectCommand = DataProviderWx.KeywordMatchDao.GetSelectString(PublishmentSystemId);
            }
            else
            {
                var keywordType = Request.QueryString["keywordType"];
                var keyword = Request.QueryString["keyword"];
                SpContents.SelectCommand = DataProviderWx.KeywordMatchDao.GetSelectString(PublishmentSystemId, keywordType, keyword);
            }
            SpContents.ItemsPerPage = 50;
            SpContents.SortField = DataProviderWx.KeywordMatchDao.GetSortField();
            SpContents.SortMode = SortMode.DESC;
            RptContents.ItemDataBound += rptContents_ItemDataBound;

			if(!IsPostBack)
            {
                var listItem = new ListItem("所有类型", string.Empty);
                DdlKeywordType.Items.Add(listItem);

                EKeywordTypeUtils.AddListItems(DdlKeywordType);

                if (Request.QueryString["keywordType"] != null)
                {
                    var keywordType = Request.QueryString["keywordType"];
                    var keyword = Request.QueryString["keyword"];

                    ControlUtils.SelectListItems(DdlKeywordType, keywordType);
                    TbKeyword.Text = keyword;
                }

                SpContents.DataBind();                
			}
		}

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var keywordId = SqlUtils.EvalInt(e.Item.DataItem, "KeywordId");
                var keyword = SqlUtils.EvalString(e.Item.DataItem, "Keyword");
                var keywordType = EKeywordTypeUtils.GetEnumType(SqlUtils.EvalString(e.Item.DataItem, "KeywordType"));

                var ltlKeyword = e.Item.FindControl("ltlKeyword") as Literal;

                ltlKeyword.Text =
                    $@"<div class=""alert alert-success pull-left"" style=""margin:5px;padding-right:14px; cursor:pointer;"" onclick=""window.parent.{_jsMethod}('{keyword}');;window.parent.closeWindow();""><strong style=""color: #468847"">{keyword}</strong>&nbsp;({EKeywordTypeUtils.GetText(keywordType)})</div>";
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
                    _pageUrl = PageUtils.GetWeiXinUrl(nameof(ModalKeywordSelect), new NameValueCollection
                    {
                        {"publishmentSystemId", PublishmentSystemId.ToString()},
                        {"keywordType", DdlKeywordType.SelectedValue},
                        {"keyword", TbKeyword.Text},
                        {"jsMethod", _jsMethod}
                    });
                }
                return _pageUrl;
            }
        }
	}
}
