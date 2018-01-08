using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.BackgroundPages.Core;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageAdminArea : BasePage
    {
        public Repeater RptContents;

        public Button BtnAdd;
        public Button BtnDelete;

        private int _currentAreaId;

        public static string GetRedirectUrl(int currentAreaId)
        {
            if (currentAreaId > 0)
            {
                return PageUtils.GetSettingsUrl(nameof(PageAdminArea), new NameValueCollection
                {
                    {"CurrentAreaID", currentAreaId.ToString()}
                });
            }
            return PageUtils.GetSettingsUrl(nameof(PageAdminArea), null);
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

            if (IsPostBack) return;

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.Admin);

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

            BtnAdd.Attributes.Add("onclick", ModalAreaAdd.GetOpenWindowStringToAdd(GetRedirectUrl(0)));

            var urlDelete = PageUtils.GetSettingsUrl(nameof(PageAdminArea), new NameValueCollection
            {
                {"Delete", "True"}
            });

            BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "AreaIDCollection", "AreaIDCollection", "请选择需要删除的区域！", "此操作将删除对应区域以及所有下级区域，确认删除吗？"));

            BindGrid();
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
            RptContents.DataSource = BaiRongDataProvider.AreaDao.GetAreaIdListByParentId(0);
            RptContents.ItemDataBound += rptContents_ItemDataBound;
            RptContents.DataBind();
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

                var urlUp = PageUtils.GetSettingsUrl(nameof(PageAdminArea), new NameValueCollection
                {
                    {"Subtract", "True"},
                    {"AreaID", areaInfo.AreaId.ToString()}
                });
                string upLink = $@"<a href=""{urlUp}""><img src=""../Pic/icon/up.gif"" border=""0"" alt=""上升"" /></a>";

                var urlDown = PageUtils.GetSettingsUrl(nameof(PageAdminArea), new NameValueCollection
                {
                    {"Add", "True"},
                    {"AreaID", areaInfo.AreaId.ToString()}
                });
                string downLink = $@"<a href=""{urlDown}""><img src=""../Pic/icon/down.gif"" border=""0"" alt=""下降"" /></a>";

                string checkBoxHtml = $"<input type='checkbox' name='AreaIDCollection' value='{areaInfo.AreaId}' />";

                rowHtml = $@"
<tr treeItemLevel=""{areaInfo.ParentsCount + 1}"">
    <td>{title}</td>
    <td class=""text-center"">{areaInfo.CountOfAdmin}</td>
    <td class=""text-center"">{upLink}</td>
    <td class=""text-center"">{downLink}</td>
    <td class=""text-center"">{editUrl}</td>
    <td class=""text-center"">{checkBoxHtml}</td>
</tr>
";
            }
            return rowHtml;
        }
    }
}
