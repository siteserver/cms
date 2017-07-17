using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.WeiXin.Core;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageStoreCategory : BasePageCms
    {
        public Repeater RptContents;

        public Button BtnAdd;
        public Button BtnDelete;

        private int _categoryId;

        public static string GetRedirectUrl(int publishmentSystemId, int categoryId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageStoreCategory), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"categoryId", categoryId.ToString()}
            });
        }

        public static string GetRedirectUrlOfDelete(int publishmentSystemId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageStoreCategory), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"Delete", true.ToString()},
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _categoryId = TranslateUtils.ToInt(Request.QueryString["categoryID"]);

            if (!string.IsNullOrEmpty(Request.QueryString["Delete"]) && !string.IsNullOrEmpty(Request.QueryString["CategoryIDCollection"]))
            {
                var categoryIdList = TranslateUtils.StringCollectionToIntList(Request.QueryString["CategoryIDCollection"]);
                foreach (var theCategoryId in categoryIdList)
                {
                    DataProviderWx.StoreCategoryDao.Delete(PublishmentSystemId, theCategoryId);
                }
                SuccessMessage("成功删除所选区域");
            }
            else if (!string.IsNullOrEmpty(Request.QueryString["Subtract"]) || !string.IsNullOrEmpty(Request.QueryString["Add"]))
            {
                var isSubtract = (!string.IsNullOrEmpty(Request.QueryString["Subtract"])) ? true : false;
                DataProviderWx.StoreCategoryDao.UpdateTaxis(PublishmentSystemId, _categoryId, isSubtract);

                PageUtils.Redirect(GetRedirectUrl(PublishmentSystemId, _categoryId));
                return;
            }

            BindGrid();

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdStore, "门店属性管理", AppManager.WeiXin.Permission.WebSite.Store);
                RegisterClientScriptBlock("NodeTreeScript", CategoryTreeItem.GetScript(PublishmentSystemId, ECategoryLoadingType.Category, null));

                if (_categoryId > 0)
                {
                    var onLoadScript = GetScriptOnLoad();
                    if (!string.IsNullOrEmpty(onLoadScript))
                    {
                        Page.RegisterClientScriptBlock("NodeTreeScriptOnLoad", onLoadScript);
                    }
                }

                var arguments = new NameValueCollection();
                var showPopWinString = string.Empty;

                BtnAdd.Attributes.Add("onclick", ModalStoreCategoryAdd.GetOpenWindowStringToAdd(PublishmentSystemId));

                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(GetRedirectUrlOfDelete(PublishmentSystemId), "CategoryIDCollection", "CategoryIDCollection", "请选择需要删除的门店属性！", "此操作将删除所选门店属性，确认删除吗？"));

            }

        }

        public string GetScriptOnLoad()
        {
            if (_categoryId > 0)
            {
                var categoryInfo = DataProviderWx.StoreCategoryDao.GetCategoryInfo(_categoryId);
                if (categoryInfo != null)
                {
                    var path = string.Empty;
                    if (categoryInfo.ParentsCount <= 1)
                    {
                        path = _categoryId.ToString();
                    }
                    else
                    {
                        path = categoryInfo.ParentsPath.Substring(categoryInfo.ParentsPath.IndexOf(",") + 1) + "," + _categoryId;
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
                RptContents.DataSource = DataProviderWx.StoreCategoryDao.GetCategoryIdListByParentId(PublishmentSystemId, 0);
                RptContents.ItemDataBound += rptContents_ItemDataBound;
                RptContents.DataBind();
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage(ex.Message);
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var categoryId = (int)e.Item.DataItem;

            var categoryInfo = DataProviderWx.StoreCategoryDao.GetCategoryInfo(categoryId);

            var ltlHtml = e.Item.FindControl("ltlHtml") as Literal;

            ltlHtml.Text = GetCategoryRowHtml(PublishmentSystemId, categoryInfo, ECategoryLoadingType.Category, null);
        }

        public static string GetCategoryRowHtml(int publishmentSystemId, StoreCategoryInfo categoryInfo, ECategoryLoadingType loadingType, NameValueCollection additional)
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


                var urlEdit = ModalStoreCategoryAdd.GetOpenWindowStringToEdit(publishmentSystemId, categoryInfo.Id);
                editUrl = $@"<a href=""javascript:;"" onclick=""{urlEdit}"">编辑</a>";

                var categoryUrl = GetRedirectUrl(publishmentSystemId, categoryInfo.Id);

                string urlUp = $"{categoryUrl}&Subtract=True";
                upLink = $@"<a href=""{urlUp}""><img src=""../Pic/icon/up.gif"" border=""0"" alt=""上升"" /></a>";

                string urlDown = $"{categoryUrl}&Add=True";
                downLink = $@"<a href=""{urlDown}""><img src=""../Pic/icon/down.gif"" border=""0"" alt=""下降"" /></a>";

                checkBoxHtml = $"<input type='checkbox' name='CategoryIDCollection' value='{categoryInfo.Id}' />";

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
