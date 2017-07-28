using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.BackgroundPages.Wcm
{
    public class PageGovPublicCategory : BasePageGovPublic
    {
        public Repeater RptContents;
        public Button BtnAddChannel;
        public Button BtnDelete;

        private GovPublicCategoryClassInfo _categoryClassInfo;
        private int _currentCategoryId;

        public static string GetRedirectUrl(int publishmentSystemId, string classCode)
        {
            return PageUtils.GetWcmUrl(nameof(PageGovPublicCategory), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"ClassCode", classCode}
            });
        }

        public static string GetRedirectUrl(int publishmentSystemId, string classCode, int currentCategoryId)
        {
            if (currentCategoryId != 0)
            {
                return PageUtils.GetWcmUrl(nameof(PageGovPublicCategory), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"ClassCode", classCode},
                    {"CurrentCategoryID", currentCategoryId.ToString()},
                });
            }
            return PageUtils.GetWcmUrl(nameof(PageGovPublicCategory), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"ClassCode", classCode}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            var classCode = Body.GetQueryString("ClassCode");
            _categoryClassInfo = DataProvider.GovPublicCategoryClassDao.GetCategoryClassInfo(classCode, PublishmentSystemId);

            if (Body.IsQueryExists("Delete") && Body.IsQueryExists("CategoryIDCollection"))
            {
                var categoryIdList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("CategoryIDCollection"));
                foreach (var categoryId in categoryIdList)
                {
                    DataProvider.GovPublicCategoryDao.Delete(categoryId);
                }
                SuccessMessage("成功删除所选节点");
            }
            else if (Body.IsQueryExists("CategoryID") && (Body.IsQueryExists("Subtract") || Body.IsQueryExists("Add")))
            {
                var categoryId = Body.GetQueryInt("CategoryID");
                var isSubtract = Body.IsQueryExists("Subtract");
                DataProvider.GovPublicCategoryDao.UpdateTaxis(_categoryClassInfo.ClassCode, PublishmentSystemId, categoryId, isSubtract);

                PageUtils.Redirect(GetRedirectUrl(PublishmentSystemId, _categoryClassInfo.ClassCode, categoryId));
                return;
            }

            if (!IsPostBack)
            {
                BreadCrumbWithItemTitle(AppManager.Wcm.LeftMenu.IdGovPublic, AppManager.Wcm.LeftMenu.GovPublic.IdGovPublicContentConfiguration, "分类法管理", ClassName + "分类", AppManager.Wcm.Permission.WebSite.GovPublicContentConfiguration);

                ClientScriptRegisterClientScriptBlock("NodeTreeScript", GovPublicCategoryTreeItem.GetScript(_categoryClassInfo.ClassCode, PublishmentSystemId, EGovPublicCategoryLoadingType.List, null));

                if (Body.IsQueryExists("CurrentCategoryID"))
                {
                    _currentCategoryId = TranslateUtils.ToInt(Request.QueryString["CurrentCategoryID"]);
                    var onLoadScript = GetScriptOnLoad(_currentCategoryId);
                    if (!string.IsNullOrEmpty(onLoadScript))
                    {
                        ClientScriptRegisterClientScriptBlock("NodeTreeScriptOnLoad", onLoadScript);
                    }
                }

                BtnAddChannel.Attributes.Add("onclick", ModalGovPublicCategoryAdd.GetOpenWindowStringToAdd(_categoryClassInfo.ClassCode, PublishmentSystemId, GetRedirectUrl(PublishmentSystemId, _categoryClassInfo.ClassCode, 0)));

                BtnDelete.Attributes.Add("onclick",
                    PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                        PageUtils.GetWcmUrl(nameof(PageGovPublicCategory), new NameValueCollection
                        {
                            {"PublishmentSystemID", PublishmentSystemId.ToString()},
                            {"ClassCode", _categoryClassInfo.ClassCode},
                            {"Delete", true.ToString()},
                        }), "CategoryIDCollection", "CategoryIDCollection", "请选择需要删除的节点！", "此操作将删除对应节点以及所有下级节点，确认删除吗？"));

                BindGrid();
            }
        }

        public string ClassName => _categoryClassInfo != null ? _categoryClassInfo.ClassName : string.Empty;

        public string GetScriptOnLoad(int currentCategoryId)
        {
            if (currentCategoryId == 0) return string.Empty;
            var categoryInfo = DataProvider.GovPublicCategoryDao.GetCategoryInfo(currentCategoryId);
            if (categoryInfo == null) return string.Empty;
            string path;
            if (categoryInfo.ParentsCount <= 1)
            {
                path = currentCategoryId.ToString();
            }
            else
            {
                path = categoryInfo.ParentsPath.Substring(categoryInfo.ParentsPath.IndexOf(",", StringComparison.Ordinal) + 1) + "," + currentCategoryId;
            }
            return GovPublicCategoryTreeItem.GetScriptOnLoad(path);
        }

        public void BindGrid()
        {
            try
            {
                RptContents.DataSource = DataProvider.GovPublicCategoryDao.GetCategoryIdArrayListByParentId(_categoryClassInfo.ClassCode, PublishmentSystemId, 0);
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

            var categoryInfo = DataProvider.GovPublicCategoryDao.GetCategoryInfo(categoryId);

            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");

            ltlHtml.Text = GetCategoryRowHtml(categoryInfo, true, EGovPublicCategoryLoadingType.List, null);
        }

        public static string GetCategoryRowHtml(GovPublicCategoryInfo categoryInfo, bool enabled, EGovPublicCategoryLoadingType loadingType, NameValueCollection additional)
        {
            var treeItem = GovPublicCategoryTreeItem.CreateInstance(categoryInfo, enabled);
            var title = treeItem.GetItemHtml(loadingType);

            var rowHtml = string.Empty;

            if (loadingType == EGovPublicCategoryLoadingType.Tree || loadingType == EGovPublicCategoryLoadingType.Select)
            {
                rowHtml = $@"
<tr treeItemLevel=""{categoryInfo.ParentsCount + 1}"">
	<td nowrap>
		{title}
	</td>
</tr>
";
            }
            else if (loadingType == EGovPublicCategoryLoadingType.List)
            {
                var editUrl = string.Empty;
                var upLink = string.Empty;
                var downLink = string.Empty;
                var checkBoxHtml = string.Empty;

                if (enabled)
                {
                    editUrl =
                        $@"<a href=""javascript:;"" onclick=""{ModalGovPublicCategoryAdd.GetOpenWindowStringToEdit(
                            categoryInfo.ClassCode, categoryInfo.PublishmentSystemID, categoryInfo.CategoryID,
                            GetRedirectUrl(categoryInfo.PublishmentSystemID, categoryInfo.ClassCode, categoryInfo.CategoryID))}"">编辑</a>";

                    var urlUp = PageUtils.GetWcmUrl(nameof(PageGovPublicCategory), new NameValueCollection
                    {
                        {"PublishmentSystemID", categoryInfo.PublishmentSystemID.ToString()},
                        {"ClassCode", categoryInfo.ClassCode},
                        {"Subtract", true.ToString()},
                        {"CategoryID", categoryInfo.CategoryID.ToString()}
                    });
                    upLink = $@"<a href=""{urlUp}""><img src=""../Pic/icon/up.gif"" border=""0"" alt=""上升"" /></a>";

                    var urlDown = PageUtils.GetWcmUrl(nameof(PageGovPublicCategory), new NameValueCollection
                    {
                        {"PublishmentSystemID", categoryInfo.PublishmentSystemID.ToString()},
                        {"ClassCode", categoryInfo.ClassCode},
                        {"Add", true.ToString()},
                        {"CategoryID", categoryInfo.CategoryID.ToString()}
                    });
                    downLink =
                        $@"<a href=""{urlDown}""><img src=""../Pic/icon/down.gif"" border=""0"" alt=""下降"" /></a>";

                    checkBoxHtml =
                        $"<input type='checkbox' name='CategoryIDCollection' value='{categoryInfo.CategoryID}' />";
                }

                rowHtml = $@"
<tr treeItemLevel=""{categoryInfo.ParentsCount + 1}"">
    <td>{title}</td>
    <td>{categoryInfo.CategoryCode}</td>
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
