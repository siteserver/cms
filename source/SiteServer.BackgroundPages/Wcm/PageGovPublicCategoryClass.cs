using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.BackgroundPages.Admin;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Wcm
{
    public class PageGovPublicCategoryClass : BasePageGovPublic
    {
        public DataGrid dgContents;
        public Button AddButton;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetWcmUrl(nameof(PageGovPublicCategoryClass), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["Delete"] != null && Request.QueryString["ClassCode"] != null)
            {
                var classCode = Request.QueryString["ClassCode"];
                try
                {
                    DataProvider.GovPublicCategoryClassDao.Delete(classCode, PublishmentSystemId);
                    SuccessMessage("成功删除分类法");
                }
                catch (Exception ex)
                {
                    SuccessMessage($"删除分类法失败，{ex.Message}");
                }
            }
            else if ((Request.QueryString["Up"] != null || Request.QueryString["Down"] != null) && Request.QueryString["ClassCode"] != null)
            {
                var classCode = Request.QueryString["ClassCode"];
                var isDown = (Request.QueryString["Down"] != null) ? true : false;
                if (isDown)
                {
                    DataProvider.GovPublicCategoryClassDao.UpdateTaxisToUp(classCode, PublishmentSystemId);
                }
                else
                {
                    DataProvider.GovPublicCategoryClassDao.UpdateTaxisToDown(classCode, PublishmentSystemId);
                }
            }

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovPublic, AppManager.Wcm.LeftMenu.GovPublic.IdGovPublicContentConfiguration, "分类法管理", AppManager.Wcm.Permission.WebSite.GovPublicContentConfiguration);

                BindGrid();

                AddButton.Attributes.Add("onclick", ModalGovPublicCategoryClassAdd.GetOpenWindowStringToAdd(PublishmentSystemId));
            }
        }

        public void BindGrid()
        {
            dgContents.DataSource = DataProvider.GovPublicCategoryClassDao.GetDataSource(PublishmentSystemId);
            dgContents.ItemDataBound += dgContents_ItemDataBound;
            dgContents.DataBind();
        }

        void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var classCode = SqlUtils.EvalString(e.Item.DataItem, "ClassCode");
                var className = SqlUtils.EvalString(e.Item.DataItem, "ClassName");
                var isSystem = TranslateUtils.ToBool(SqlUtils.EvalString(e.Item.DataItem, "IsSystem"));
                var isEnabled = TranslateUtils.ToBool(SqlUtils.EvalString(e.Item.DataItem, "IsEnabled"));

                var ltlClassName = e.Item.FindControl("ltlClassName") as Literal;
                var ltlClassCode = e.Item.FindControl("ltlClassCode") as Literal;
                var hlUpLinkButton = e.Item.FindControl("hlUpLinkButton") as HyperLink;
                var hlDownLinkButton = e.Item.FindControl("hlDownLinkButton") as HyperLink;
                var ltlIsEnabled = e.Item.FindControl("ltlIsEnabled") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;
                var ltlDeleteUrl = e.Item.FindControl("ltlDeleteUrl") as Literal;

                if (!isSystem)
                {
                    ltlClassName.Text = $@"<a href=""{PageGovPublicCategory.GetRedirectUrl(PublishmentSystemId, classCode)}"" target=""category"">{className}分类</a>";
                }
                else if (EGovPublicCategoryClassTypeUtils.Equals(EGovPublicCategoryClassType.Channel, classCode))
                {
                    ltlClassName.Text = $@"<a href=""{PageGovPublicChannel.GetRedirectUrl(PublishmentSystemId)}"" target=""category"">{className}分类</a>";
                }
                else if (EGovPublicCategoryClassTypeUtils.Equals(EGovPublicCategoryClassType.Department, classCode))
                {
                    ltlClassName.Text = $@"<a href=""{PageDepartment.GetRedirectUrl(0)}"" target=""category"">{className}分类</a>";
                }
                
                ltlClassCode.Text = classCode;
                ltlIsEnabled.Text = StringUtils.GetTrueOrFalseImageHtml(isEnabled);

                hlUpLinkButton.NavigateUrl = PageUtils.GetWcmUrl(nameof(PageGovPublicCategoryClass),
                    new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"ClassCode", classCode},
                        {"Up", true.ToString()}
                    });

                hlDownLinkButton.NavigateUrl = PageUtils.GetWcmUrl(nameof(PageGovPublicCategoryClass),
                    new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"ClassCode", classCode},
                        {"Down", true.ToString()}
                    });

                ltlEditUrl.Text =
                    $@"<a href='javascript:;' onclick=""{ModalGovPublicCategoryClassAdd.GetOpenWindowStringToEdit(
                        PublishmentSystemId, classCode)}"">编辑</a>";

                if (!isSystem)
                {
                    var urlDelete = PageUtils.GetWcmUrl(nameof(PageGovPublicCategoryClass), new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"ClassCode", classCode},
                        {"Delete", true.ToString()}
                    });
                    ltlDeleteUrl.Text =
                        $@"<a href=""{urlDelete}"" onClick=""javascript:return confirm('此操作将删除分类法“{className}”及其分类项，确认吗？');"">删除</a>";
                }
            }
        }
    }
}
