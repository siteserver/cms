using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Wcm
{
    public class PageGovInteractType : BasePageGovInteract
    {
        public DataGrid dgContents;
        public Button AddButton;

        private int _nodeId;

        public static string GetRedirectUrl(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetWcmUrl(nameof(PageGovInteractType), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            _nodeId = TranslateUtils.ToInt(Request.QueryString["NodeID"]);

            if (Request.QueryString["Delete"] != null && Request.QueryString["TypeID"] != null)
            {
                var typeId = TranslateUtils.ToInt(Request.QueryString["TypeID"]);
                try
                {
                    DataProvider.GovInteractTypeDao.Delete(typeId);
                    SuccessMessage("成功删除办件类型");
                }
                catch (Exception ex)
                {
                    SuccessMessage($"删除办件类型失败，{ex.Message}");
                }
            }
            else if ((Request.QueryString["Up"] != null || Request.QueryString["Down"] != null) && Request.QueryString["TypeID"] != null)
            {
                var typeId = TranslateUtils.ToInt(Request.QueryString["TypeID"]);
                var isDown = Request.QueryString["Down"] != null;
                if (isDown)
                {
                    DataProvider.GovInteractTypeDao.UpdateTaxisToUp(typeId, _nodeId);
                }
                else
                {
                    DataProvider.GovInteractTypeDao.UpdateTaxisToDown(typeId, _nodeId);
                }
            }

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovInteract, AppManager.Wcm.LeftMenu.GovInteract.IdGovInteractConfiguration, "办件类型管理", AppManager.Wcm.Permission.WebSite.GovInteractConfiguration);

                dgContents.DataSource = DataProvider.GovInteractTypeDao.GetDataSource(_nodeId);
                dgContents.ItemDataBound += dgContents_ItemDataBound;
                dgContents.DataBind();

                AddButton.Attributes.Add("onclick", ModalGovInteractTypeAdd.GetOpenWindowStringToAdd(PublishmentSystemId, _nodeId));
            }
        }

        void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var typeId = SqlUtils.EvalInt(e.Item.DataItem, "TypeID");
                var typeName = SqlUtils.EvalString(e.Item.DataItem, "TypeName");

                var ltlTypeName = e.Item.FindControl("ltlTypeName") as Literal;
                var hlUpLinkButton = e.Item.FindControl("hlUpLinkButton") as HyperLink;
                var hlDownLinkButton = e.Item.FindControl("hlDownLinkButton") as HyperLink;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;
                var ltlDeleteUrl = e.Item.FindControl("ltlDeleteUrl") as Literal;

                ltlTypeName.Text = typeName;

                hlUpLinkButton.NavigateUrl = PageUtils.GetWcmUrl(nameof(PageGovInteractType), new NameValueCollection
                {
                    {"PublishmentSystemID", PublishmentSystemId.ToString()},
                    {"NodeID", _nodeId.ToString()},
                    {"TypeID", typeId.ToString()},
                    {"Up", true.ToString()}
                });

                hlDownLinkButton.NavigateUrl = PageUtils.GetWcmUrl(nameof(PageGovInteractType), new NameValueCollection
                {
                    {"PublishmentSystemID", PublishmentSystemId.ToString()},
                    {"NodeID", _nodeId.ToString()},
                    {"TypeID", typeId.ToString()},
                    {"Down", true.ToString()}
                });

                ltlEditUrl.Text =
                    $@"<a href='javascript:;' onclick=""{ModalGovInteractTypeAdd.GetOpenWindowStringToEdit(
                        PublishmentSystemId, _nodeId, typeId)}"">编辑</a>";

                var urlDelete = PageUtils.GetWcmUrl(nameof(PageGovInteractType), new NameValueCollection
                {
                    {"PublishmentSystemID", PublishmentSystemId.ToString()},
                    {"NodeID", _nodeId.ToString()},
                    {"TypeID", typeId.ToString()},
                    {"Delete", true.ToString()}
                });
                ltlDeleteUrl.Text =
                    $@"<a href=""{urlDelete}"" onClick=""javascript:return confirm('此操作将删除办件类型“{typeName}”，确认吗？');"">删除</a>";
            }
        }
    }
}
