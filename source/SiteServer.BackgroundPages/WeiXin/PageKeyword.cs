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
    public class PageKeyword : BasePageCms
    {
        public DropDownList DdlKeywordType;
        public TextBox TbKeyword;

        public Repeater RptContents;
        public SqlPager SpContents;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageKeyword), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (Request.QueryString["delete"] != null)
			{
                var keywordId = TranslateUtils.ToInt(Request.QueryString["deleteKeywordId"]);
                var keyword = Request.QueryString["deleteKeyword"];
			
				try
				{
                    var keywordInfo = DataProviderWx.KeywordDao.GetKeywordInfo(keywordId);
                    var keywordList = TranslateUtils.StringCollectionToStringList(keywordInfo.Keywords, ' ');
                    if (keywordList.Remove(keyword))
                    {
                        keywordInfo.Keywords = TranslateUtils.ObjectCollectionToString(keywordList, " ");
                        DataProviderWx.KeywordDao.Update(keywordInfo);
                        SuccessDeleteMessage();
                    }
				}
				catch(Exception ex)
				{
                    FailDeleteMessage(ex);
				}
			}

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
            SpContents.ItemsPerPage = 60;
            SpContents.SortField = "KeywordId";
            SpContents.SortMode = SortMode.DESC;
            RptContents.ItemDataBound += rptContents_ItemDataBound;

			if (!IsPostBack)
            {
                //BreadCrumb(AppManager.Cms.LeftMenu.IdContent, AppManager.Cms.LeftMenu.Content.Id_Category, "关键字管理", AppManager.Cms.Permission.WebSite.Category);

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

                string urlDelete =
                    $@"{GetRedirectUrl(PublishmentSystemId)}&delete=true&deleteKeywordId={keywordId}&deleteKeyword={keyword}";

                ltlKeyword.Text =
                    $@"<div class=""alert alert-success pull-left"" style=""margin:5px;padding-right:14px;""><strong style=""color: #468847"">{keyword}</strong>&nbsp;({EKeywordTypeUtils
                        .GetText(keywordType)})&nbsp;<a href=""javascript:;"" onclick=""{ModalKeywordEdit
                        .GetOpenWindowString(PublishmentSystemId, keywordId, keyword)}""><i class=""icon-edit""></i></a>&nbsp;<a href=""{urlDelete}"" onclick=""javascript:return confirm('此操作将删除关键字“{keyword}”，确认吗？');""><i class=""icon-remove""></i></a></div>";
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
                    _pageUrl = PageUtils.GetWeiXinUrl(nameof(PageKeyword), new NameValueCollection
                    {
                        {"publishmentSystemId", PublishmentSystemId.ToString()},
                        {"keywordType", DdlKeywordType.SelectedValue},
                        {"keyword", TbKeyword.Text}
                    });
                }
                return _pageUrl;
            }
        }
	}
}
