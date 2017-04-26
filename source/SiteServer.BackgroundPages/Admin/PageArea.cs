using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.BackgroundPages.Core;

namespace SiteServer.BackgroundPages.Admin
{
    public class PageArea : BasePage
    {
        public Repeater rptContents;

        public Button btnAdd;
        public Button btnDelete;

        private int _currentAreaId;

        public static string GetRedirectUrl(int currentAreaId)
        {
            if (currentAreaId > 0)
            {
                return PageUtils.GetAdminUrl(nameof(PageArea), new NameValueCollection
                {
                    {"CurrentAreaID", currentAreaId.ToString()}
                });
            }
            return PageUtils.GetAdminUrl(nameof(PageArea), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Body.IsQueryExists("Delete") && Body.IsQueryExists("AreaIDCollection"))
            {
                var areaIdArrayList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("AreaIDCollection"));
                foreach (var areaId in areaIdArrayList)
                {
                    BaiRongDataProvider.AreaDao.Delete(areaId);
                }
                SuccessMessage("成功删除所选区域");
            }
            else if (Body.IsQueryExists("AreaID") && (Body.IsQueryExists("Subtract") || Body.IsQueryExists("Add")))
            {
                var areaId = int.Parse(Body.GetQueryString("AreaID"));
                var isSubtract = Body.IsQueryExists("Subtract");
                BaiRongDataProvider.AreaDao.UpdateTaxis(areaId, isSubtract);

                PageUtils.Redirect(GetRedirectUrl(areaId));
                return;
            }

            if (!IsPostBack)
            {
                BreadCrumbAdmin(AppManager.Admin.LeftMenu.AdminConfiguration, "所在区域管理", AppManager.Admin.Permission.AdminConfiguration);

                ClientScriptRegisterClientScriptBlock("NodeTreeScript", AreaTreeItem.GetScript(EAreaLoadingType.Management, null));

                if (Body.IsQueryExists("CurrentAreaID"))
                {
                    _currentAreaId = Body.GetQueryInt("CurrentAreaID");
                    var onLoadScript = GetScriptOnLoad(_currentAreaId);
                    if (!string.IsNullOrEmpty(onLoadScript))
                    {
                        ClientScriptRegisterClientScriptBlock("NodeTreeScriptOnLoad", onLoadScript);
                    }
                }

                btnAdd.Attributes.Add("onclick", ModalAreaAdd.GetOpenWindowStringToAdd(GetRedirectUrl(0)));

                var urlDelete = PageUtils.GetAdminUrl(nameof(PageArea), new NameValueCollection
                {
                    {"Delete", "True"}
                });

                btnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "AreaIDCollection", "AreaIDCollection", "请选择需要删除的区域！", "此操作将删除对应区域以及所有下级区域，确认删除吗？"));

                BindGrid();
            }
        }

        public string GetScriptOnLoad(int currentAreaId)
        {
            if (currentAreaId == 0) return string.Empty;

            var areaInfo = AreaManager.GetAreaInfo(currentAreaId);
            if (areaInfo == null) return string.Empty;

            string path;
            if (areaInfo.ParentsCount <= 1)
            {
                path = currentAreaId.ToString();
            }
            else
            {
                path = areaInfo.ParentsPath.Substring(areaInfo.ParentsPath.IndexOf(",", StringComparison.Ordinal) + 1) + "," + currentAreaId;
            }
            return AreaTreeItem.GetScriptOnLoad(path);
        }

        public void BindGrid()
        {
            try
            {
                rptContents.DataSource = BaiRongDataProvider.AreaDao.GetAreaIdListByParentId(0);
                rptContents.ItemDataBound += rptContents_ItemDataBound;
                rptContents.DataBind();
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage(ex.Message);
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var areaId = (int)e.Item.DataItem;

            var areaInfo = AreaManager.GetAreaInfo(areaId);

            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");

            ltlHtml.Text = GetAreaRowHtml(areaInfo, EAreaLoadingType.Management, null);
        }

        public static string GetAreaRowHtml(AreaInfo areaInfo, EAreaLoadingType loadingType, NameValueCollection additional)
        {
            var treeItem = AreaTreeItem.CreateInstance(areaInfo);
            var title = treeItem.GetItemHtml(loadingType, additional, false);

            var rowHtml = string.Empty;

            if (loadingType == EAreaLoadingType.Management)
            {
                string editUrl = $@"<a href=""javascript:;"" onclick=""{ModalAreaAdd.GetOpenWindowStringToEdit(areaInfo.AreaId,
                    GetRedirectUrl(areaInfo.AreaId))}"">编辑</a>";

                var urlUp = PageUtils.GetAdminUrl(nameof(PageArea), new NameValueCollection
                {
                    {"Subtract", "True"},
                    {"AreaID", areaInfo.AreaId.ToString()}
                });
                string upLink = $@"<a href=""{urlUp}""><img src=""../Pic/icon/up.gif"" border=""0"" alt=""上升"" /></a>";

                var urlDown = PageUtils.GetAdminUrl(nameof(PageArea), new NameValueCollection
                {
                    {"Add", "True"},
                    {"AreaID", areaInfo.AreaId.ToString()}
                });
                string downLink = $@"<a href=""{urlDown}""><img src=""../Pic/icon/down.gif"" border=""0"" alt=""下降"" /></a>";

                string checkBoxHtml = $"<input type='checkbox' name='AreaIDCollection' value='{areaInfo.AreaId}' />";

                rowHtml = $@"
<tr treeItemLevel=""{areaInfo.ParentsCount + 1}"">
    <td>{title}</td>
    <td class=""center"">{areaInfo.CountOfAdmin}</td>
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
