using System;
using System.Web.UI.WebControls;
using BaiRong.Controls;
using BaiRong.Core;
using SiteServer.CMS.BackgroundPages;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;
 

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundSearch : BackgroundBasePageWX
    {
        public Repeater rptContents;
        public SqlPager spContents;

        public Button btnAdd;
        public Button btnDelete;
         
        public static string GetRedirectUrl(int publishmentSystemID)
        {
            return PageUtils.GetWXUrl($"background_search.aspx?PublishmentSystemID={publishmentSystemID}");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
             
            if (!string.IsNullOrEmpty(Request.QueryString["Delete"])) 
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        DataProviderWX.SearchDAO.Delete(PublishmentSystemID, list);

                        SuccessMessage("微搜索删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "微搜索删除失败！");
                    }
                }
            }

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = 30;
            spContents.ConnectionString = BaiRongDataProvider.ConnectionString;
            spContents.SelectCommand = DataProviderWX.SearchDAO.GetSelectString(PublishmentSystemID);
            spContents.SortField = SearchAttribute.ID;
            spContents.SortMode = SortMode.ASC;
            rptContents.ItemDataBound += new RepeaterItemEventHandler(rptContents_ItemDataBound);

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Search, "微搜索", AppManager.WeiXin.Permission.WebSite.Search);
                spContents.DataBind();

                var urlAdd = BackgroundSearchAdd.GetRedirectUrl(PublishmentSystemID, 0);
                
                btnAdd.Attributes.Add("onclick", $"location.href='{urlAdd}';return false");

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemID), "Delete", "True");
                btnDelete.Attributes.Add("onclick", JsUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的微搜索", "此操作将删除所选微搜索，确认吗？"));
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var searchInfo = new SearchInfo(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlTitle = e.Item.FindControl("ltlTitle") as Literal;
                var ltlKeywords = e.Item.FindControl("ltlKeywords") as Literal;
                var ltlPVCount = e.Item.FindControl("ltlPVCount") as Literal;
                var ltlIsEnabled = e.Item.FindControl("ltlIsEnabled") as Literal;
                var ltlPreviewUrl = e.Item.FindControl("ltlPreviewUrl") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlTitle.Text = searchInfo.Title;
                ltlKeywords.Text = DataProviderWX.KeywordDAO.GetKeywords(searchInfo.KeywordID);
                ltlPVCount.Text = searchInfo.PVCount.ToString();

                ltlIsEnabled.Text = StringUtils.GetTrueOrFalseImageHtml(!searchInfo.IsDisabled);

                var urlPreview = SearchManager.GetSearchUrl(searchInfo);
                urlPreview = BackgroundPreview.GetRedirectUrlToMobile(urlPreview);
                ltlPreviewUrl.Text = $@"<a href=""{urlPreview}"" target=""_blank"">预览</a>";

                var urlEdit = BackgroundSearchAdd.GetRedirectUrl(PublishmentSystemID, searchInfo.ID);
              
                ltlEditUrl.Text = $@"<a href=""{urlEdit}"">编辑</a>";
            }
        }
    }
}
