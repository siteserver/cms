using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
	public class ModalFunctionSelect : BasePageCms
    {
        public DropDownList DdlKeywordType;
        public TextBox TbTitle;

        public PlaceHolder PhFunction;
        public Repeater RptContents;
        public SqlPager SpContents;

        private string _jsMethod;
        private int _itemIndex;

        public static string GetOpenWindowStringByItemIndex(int publishmentSystemId, string jsMethod, string itemIndex)
        {
            return PageUtils.GetOpenWindowString("选择微功能",
                PageUtils.GetWeiXinUrl(nameof(ModalFunctionSelect), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"jsMethod", jsMethod},
                    {"itemIndex", itemIndex}
                }), true);
        }

        public static string GetOpenWindowString(int publishmentSystemId, string jsMethod)
        {
            return PageUtils.GetOpenWindowString("选择微功能",
                PageUtils.GetWeiXinUrl(nameof(ModalFunctionSelect), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"jsMethod", jsMethod}
                }), true);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemId");
            _jsMethod = Request.QueryString["jsMethod"];
            _itemIndex = Body.GetQueryInt("itemIndex");

			if(!IsPostBack)
            {                
                EKeywordTypeUtils.AddListItemsUrlOnly(DdlKeywordType);
                DdlKeywordType.Items.Insert(0, new ListItem("<选择微功能类型>", string.Empty));

                ReFresh(null, EventArgs.Empty);
			}
		}

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ltlTitle = e.Item.FindControl("ltlTitle") as Literal;

                var functionId = SqlUtils.EvalInt(e.Item.DataItem, "ID");
                var keywordType = EKeywordTypeUtils.GetEnumType(DdlKeywordType.SelectedValue);
                var pageTitle = KeywordManager.GetFunctionName(keywordType, functionId);

                var clickString = string.Empty;

                if (Request.QueryString["itemIndex"] != null)
                {
                    clickString =
                        $@"window.parent.{_jsMethod}({_itemIndex}, '{EKeywordTypeUtils.GetValue(keywordType)}', {functionId}, '{pageTitle}');{PageUtils.HidePopWin}";
                }
                else
                {
                    clickString =
                        $@"window.parent.{_jsMethod}('{EKeywordTypeUtils.GetValue(keywordType)},{functionId},{pageTitle}');{PageUtils.HidePopWin}";
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
                if (!string.IsNullOrEmpty(DdlKeywordType.SelectedValue))
                {
                    SpContents.ControlToPaginate = RptContents;

                    var keywordType = EKeywordTypeUtils.GetEnumType(DdlKeywordType.SelectedValue);

                    SpContents.SelectCommand = KeywordManager.GetFunctionSqlString(PublishmentSystemId, keywordType);

                    SpContents.ItemsPerPage = 50;
                    SpContents.SortField = "ID";
                    SpContents.SortMode = SortMode.DESC;
                    RptContents.ItemDataBound += rptContents_ItemDataBound;

                    SpContents.DataBind();

                    PhFunction.Visible = RptContents.Items.Count > 0;
                }
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage(ex.Message);
            }
        }
	}
}
