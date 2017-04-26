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
	public class FunctionSelect : BackgroundBasePage
	{
        public DropDownList ddlKeywordType;
        public TextBox tbTitle;

        public PlaceHolder phFunction;
        public Repeater rptContents;
        public SqlPager spContents;

        private string jsMethod;
        private int itemIndex;

        public static string GetOpenWindowStringByItemIndex(int publishmentSystemID, string jsMethod, string itemIndex)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            arguments.Add("jsMethod", jsMethod);
            arguments.Add("itemIndex", itemIndex);
            return PageUtilityWX.GetOpenWindowString("选择微功能", "modal_functionSelect.aspx", arguments, true);
        }

        public static string GetOpenWindowString(int publishmentSystemID, string jsMethod)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            arguments.Add("jsMethod", jsMethod);
            return PageUtilityWX.GetOpenWindowString("选择微功能", "modal_functionSelect.aspx", arguments, true);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            jsMethod = Request.QueryString["jsMethod"];
            itemIndex = TranslateUtils.ToInt(GetQueryString("itemIndex"));

			if(!IsPostBack)
            {
                BreadCrumb(AppManager.CMS.LeftMenu.ID_Content, "微功能搜索", string.Empty);
                 
                EKeywordTypeUtils.AddListItemsUrlOnly(ddlKeywordType);
                ddlKeywordType.Items.Insert(0, new ListItem("<选择微功能类型>", string.Empty));

                ReFresh(null, EventArgs.Empty);
			}
		}

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ltlTitle = e.Item.FindControl("ltlTitle") as Literal;

                var functionID = TranslateUtils.EvalInt(e.Item.DataItem, "ID");
                var keywordType = EKeywordTypeUtils.GetEnumType(ddlKeywordType.SelectedValue);
                var pageTitle = KeywordManager.GetFunctionName(keywordType, functionID);

                var clickString = string.Empty;

                if (Request.QueryString["itemIndex"] != null)
                {
                    clickString =
                        $@"window.parent.{jsMethod}({itemIndex}, '{EKeywordTypeUtils.GetValue(keywordType)}', {functionID}, '{pageTitle}');{JsUtils
                            .OpenWindow.HIDE_POP_WIN}";
                }
                else
                {
                    clickString =
                        $@"window.parent.{jsMethod}('{EKeywordTypeUtils.GetValue(keywordType)},{functionID},{pageTitle}');{JsUtils
                            .OpenWindow.HIDE_POP_WIN}";
                }

                ltlTitle.Text = $@"
<div class=""alert alert-success pull-left"" style=""margin:5px;padding-right:14px; cursor:pointer;"" onclick=""{clickString}"">
    <strong style=""color: #468847"">{pageTitle}</strong>
</div>";
                 
            }
        }

        public void ReFresh(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(ddlKeywordType.SelectedValue))
                {
                    spContents.ControlToPaginate = rptContents;
                    spContents.ConnectionString = BaiRongDataProvider.ConnectionString;

                    var keywordType = EKeywordTypeUtils.GetEnumType(ddlKeywordType.SelectedValue);

                    spContents.SelectCommand = KeywordManager.GetFunctionSqlString(PublishmentSystemID, keywordType);

                    spContents.ItemsPerPage = 50;
                    spContents.SortField = "ID";
                    spContents.SortMode = SortMode.DESC;
                    rptContents.ItemDataBound += new RepeaterItemEventHandler(rptContents_ItemDataBound);

                    spContents.DataBind();

                    phFunction.Visible = rptContents.Items.Count > 0;
                }
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage(ex.Message);
            }
        }
	}
}
