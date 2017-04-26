using BaiRong.Core;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;
using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundStoreCategory : BackgroundBasePageWX
    {
        public Repeater rptContents;

        public Button btnAdd;
        public Button btnDelete;

        private int categoryID;

        public static string GetRedirectUrl(int publishmentSystemID, int categoryID)
        {
            return PageUtils.GetWXUrl(
                $"background_storeCategory.aspx?publishmentSystemID={publishmentSystemID}&categoryID={categoryID}");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            categoryID = TranslateUtils.ToInt(Request.QueryString["categoryID"]);

            if (!string.IsNullOrEmpty(Request.QueryString["Delete"]) && !string.IsNullOrEmpty(Request.QueryString["CategoryIDCollection"]))
            {
                var categoryIDArrayList = TranslateUtils.StringCollectionToIntArrayList(Request.QueryString["CategoryIDCollection"]);
                foreach (int theCategoryID in categoryIDArrayList)
                {
                    DataProviderWX.StoreCategoryDAO.Delete(PublishmentSystemID, theCategoryID);
                }
                SuccessMessage("成功删除所选区域");
            }
            else if (!string.IsNullOrEmpty(Request.QueryString["Subtract"]) || !string.IsNullOrEmpty(Request.QueryString["Add"]))
            {
                var isSubtract = (!string.IsNullOrEmpty(Request.QueryString["Subtract"])) ? true : false;
                DataProviderWX.StoreCategoryDAO.UpdateTaxis(PublishmentSystemID, categoryID, isSubtract);

                PageUtils.Redirect(GetRedirectUrl(PublishmentSystemID, categoryID));
                return;
            }

            BindGrid();

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Store, "门店属性管理", AppManager.WeiXin.Permission.WebSite.Store);
                RegisterClientScriptBlock("NodeTreeScript", CategoryTreeItem.GetScript(PublishmentSystemID, ECategoryLoadingType.Category, null));

                if (categoryID > 0)
                {
                    var onLoadScript = GetScriptOnLoad();
                    if (!string.IsNullOrEmpty(onLoadScript))
                    {
                        Page.RegisterClientScriptBlock("NodeTreeScriptOnLoad", onLoadScript);
                    }
                }

                var arguments = new NameValueCollection();
                var showPopWinString = string.Empty;

                btnAdd.Attributes.Add("onclick", Modal.StoreCategoryAdd.GetOpenWindowStringToAdd(PublishmentSystemID));

                btnDelete.Attributes.Add("onclick", JsUtils.GetRedirectStringWithCheckBoxValueAndAlert(PageUtils.GetWXUrl("background_storeCategory.aspx?Delete=True&publishmentSystemID=" + PublishmentSystemID), "CategoryIDCollection", "CategoryIDCollection", "请选择需要删除的门店属性！", "此操作将删除所选门店属性，确认删除吗？"));

            }

        }

        public string GetScriptOnLoad()
        {
            if (categoryID > 0)
            {
                var categoryInfo = DataProviderWX.StoreCategoryDAO.GetCategoryInfo(categoryID);
                if (categoryInfo != null)
                {
                    var path = string.Empty;
                    if (categoryInfo.ParentsCount <= 1)
                    {
                        path = categoryID.ToString();
                    }
                    else
                    {
                        path = categoryInfo.ParentsPath.Substring(categoryInfo.ParentsPath.IndexOf(",") + 1) + "," + categoryID.ToString();
                    }
                    return CategoryTreeItem.GetScriptOnLoad(path);
                }
            }
            return string.Empty;
        }

        public void BindGrid()
        {
            try
            {
                rptContents.DataSource = DataProviderWX.StoreCategoryDAO.GetCategoryIDListByParentID(PublishmentSystemID, 0);
                rptContents.ItemDataBound += new RepeaterItemEventHandler(rptContents_ItemDataBound);
                rptContents.DataBind();
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage(ex.Message);
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var categoryID = (int)e.Item.DataItem;

            var categoryInfo = DataProviderWX.StoreCategoryDAO.GetCategoryInfo(categoryID);

            var ltlHtml = e.Item.FindControl("ltlHtml") as Literal;

            ltlHtml.Text = GetCategoryRowHtml(PublishmentSystemID, categoryInfo, ECategoryLoadingType.Category, null);
        }

        public static string GetCategoryRowHtml(int publishmentSystemID, StoreCategoryInfo categoryInfo, ECategoryLoadingType loadingType, NameValueCollection additional)
        {
            var treeItem = CategoryTreeItem.CreateInstance(categoryInfo);
            var title = treeItem.GetItemHtml(loadingType, additional, false);

            var rowHtml = string.Empty;

            if (loadingType == ECategoryLoadingType.Category)
            {
                var editUrl = string.Empty;
                var upLink = string.Empty;
                var downLink = string.Empty;
                var checkBoxHtml = string.Empty;


                var urlEdit = Modal.StoreCategoryAdd.GetOpenWindowStringToEdit(publishmentSystemID, categoryInfo.ID);
                editUrl = $@"<a href=""javascript:;"" onclick=""{urlEdit}"">编辑</a>";

                var categoryUrl = GetRedirectUrl(publishmentSystemID, categoryInfo.ID);

                string urlUp = $"{categoryUrl}&Subtract=True";
                upLink = $@"<a href=""{urlUp}""><img src=""../Pic/icon/up.gif"" border=""0"" alt=""上升"" /></a>";

                string urlDown = $"{categoryUrl}&Add=True";
                downLink = $@"<a href=""{urlDown}""><img src=""../Pic/icon/down.gif"" border=""0"" alt=""下降"" /></a>";

                checkBoxHtml = $"<input type='checkbox' name='CategoryIDCollection' value='{categoryInfo.ID}' />";

                rowHtml = $@"
<tr treeItemLevel=""{categoryInfo.ParentsCount + 1}"">
    <td>{title}</td>
    <td class=""center"">{upLink}</td>
    <td class=""center"">{downLink}</td>
    <td class=""center"">{editUrl}</td>
    <td class=""center"">{checkBoxHtml}</td>
</tr>
";
            }
            return rowHtml;
        }

    }
}
