using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Wcm.GovPublic;

namespace SiteServer.BackgroundPages.Wcm
{
    public class PageGovPublicDepartment : BasePageGovPublic
    {
        public Repeater rptContents;

        public Button AddDepartment;
        public Button Delete;

        private int _currentDepartmentId;
        private readonly NameValueCollection _additional = new NameValueCollection();

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetWcmUrl(nameof(PageGovPublicDepartment), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public static string GetRedirectUrl(int publishmentSystemId, int currentDepartmentId)
        {
            if (currentDepartmentId != 0)
            {
                return PageUtils.GetWcmUrl(nameof(PageGovPublicDepartment), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"CurrentDepartmentID", currentDepartmentId.ToString()}
                });
            }
            return PageUtils.GetWcmUrl(nameof(PageGovPublicDepartment), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (Body.IsQueryExists("Delete") && Body.IsQueryExists("DepartmentIDCollection"))
            {
                var departmentIdArrayList = TranslateUtils.StringCollectionToIntList(Request.QueryString["DepartmentIDCollection"]);
                foreach (var departmentId in departmentIdArrayList)
                {
                    BaiRongDataProvider.DepartmentDao.Delete(departmentId);
                }
                SuccessMessage("成功删除所选部门");
            }
            else if (Body.IsQueryExists("DepartmentID") && (Body.IsQueryExists("Subtract") || Body.IsQueryExists("Add")))
            {
                var departmentId = int.Parse(Request.QueryString["DepartmentID"]);
                var isSubtract = Body.IsQueryExists("Subtract");
                BaiRongDataProvider.DepartmentDao.UpdateTaxis(departmentId, isSubtract);

                PageUtils.Redirect(GetRedirectUrl(departmentId));
                return;
            }

            _additional["PublishmentSystemID"] = PublishmentSystemId.ToString();

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovPublic, AppManager.Wcm.LeftMenu.GovPublic.IdGovPublicContentConfiguration, "机构分类设置", AppManager.Wcm.Permission.WebSite.GovPublicContentConfiguration);

                ClientScriptRegisterClientScriptBlock("NodeTreeScript", DepartmentTreeItem.GetScript(EDepartmentLoadingType.GovPublicDepartment, _additional));

                if (Body.IsQueryExists("CurrentDepartmentID"))
                {
                    _currentDepartmentId = TranslateUtils.ToInt(Request.QueryString["CurrentDepartmentID"]);
                    var onLoadScript = GetScriptOnLoad(_currentDepartmentId);
                    if (!string.IsNullOrEmpty(onLoadScript))
                    {
                        ClientScriptRegisterClientScriptBlock("NodeTreeScriptOnLoad", onLoadScript);
                    }
                }

                AddDepartment.Attributes.Add("onclick", ModalGovPublicDepartmentAdd.GetOpenWindowStringToAdd(PublishmentSystemId, GetRedirectUrl(0)));

                Delete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(PageUtils.GetWcmUrl(nameof(PageGovPublicDepartment), new NameValueCollection
                {
                    {"PublishmentSystemID", PublishmentSystemId.ToString()},
                    {"Delete", true.ToString()}
                }), "DepartmentIDCollection", "DepartmentIDCollection", "请选择需要删除的部门！", "此操作将删除对应部门以及所有下级部门，确认删除吗？"));

                BindGrid();
            }
        }

        public string GetScriptOnLoad(int currentDepartmentID)
        {
            if (currentDepartmentID != 0)
            {
                var departmentInfo = DepartmentManager.GetDepartmentInfo(currentDepartmentID);
                if (departmentInfo != null)
                {
                    var path = string.Empty;
                    if (departmentInfo.ParentsCount <= 1)
                    {
                        path = currentDepartmentID.ToString();
                    }
                    else
                    {
                        path = departmentInfo.ParentsPath.Substring(departmentInfo.ParentsPath.IndexOf(",") + 1) + "," + currentDepartmentID.ToString();
                    }
                    return DepartmentTreeItem.GetScriptOnLoad(path);
                }
            }
            return string.Empty;
        }

        public void BindGrid()
        {
            try
            {
                rptContents.DataSource = GovPublicManager.GetFirstDepartmentIdList(PublishmentSystemInfo);
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
            var departmentID = (int)e.Item.DataItem;

            var departmentInfo = DepartmentManager.GetDepartmentInfo(departmentID);

            var ltlHtml = e.Item.FindControl("ltlHtml") as Literal;

            //ltlHtml.Text = PageDepartment.GetDepartmentRowHtml(departmentInfo, EDepartmentLoadingType.GovPublicDepartment, additional);
        }
    }
}
