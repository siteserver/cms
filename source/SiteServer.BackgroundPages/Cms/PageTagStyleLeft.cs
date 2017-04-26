using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Wcm;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Wcm.GovInteract;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageTagStyleLeft : BasePageCms
    {
        public DataGrid dgContents;

        private ETableStyle tableStyle = ETableStyle.GovInteractContent;
        private string type = string.Empty;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            tableStyle = ETableStyleUtils.GetEnumType(Body.GetQueryString("tableStyle"));
            type = Body.GetQueryString("type");

            if (!IsPostBack)
            {
                var pageTitle = string.Empty;
                if (tableStyle == ETableStyle.GovInteractContent)
                {
                    if (StringUtils.EqualsIgnoreCase(type, "DepartmentSelect"))
                    {
                        pageTitle = "负责部门设置";
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, "AdministratorSelect"))
                    {
                        pageTitle = "负责人员设置";
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, "Attributes"))
                    {
                        pageTitle = "办件字段管理";
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, "Apply"))
                    {
                        pageTitle = "办件提交样式";
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, "Query"))
                    {
                        pageTitle = "办件查询样式";
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, "MailSMS"))
                    {
                        pageTitle = " 邮件/短信发送管理";
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, "InteractType"))
                    {
                        pageTitle = "办件类型管理";
                    }
                }

                InfoMessage(pageTitle);

                BindGrid();
            }
        }

        public void BindGrid()
        {
            if (tableStyle == ETableStyle.GovInteractContent)
            {
                var nodeInfoList = GovInteractManager.GetNodeInfoList(PublishmentSystemInfo);
                dgContents.DataSource = nodeInfoList;
            }
            
            dgContents.ItemDataBound += dgContents_ItemDataBound;
            dgContents.DataBind();
        }

        void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ltlName = e.Item.FindControl("ltlName") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;

                if (tableStyle == ETableStyle.GovInteractContent)
                {
                    var nodeInfo = e.Item.DataItem as NodeInfo;
                    ltlName.Text = nodeInfo.NodeName;
                    if (StringUtils.EqualsIgnoreCase(type, "DepartmentSelect"))
                    {
                        ltlEditUrl.Text =
                            $@"<a target='management' href=""{PageGovInteractDepartmentSelect.GetRedirectUrl(
                                PublishmentSystemId, nodeInfo.NodeId)}"">负责部门设置</a>";
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, "AdministratorSelect"))
                    {
                        ltlEditUrl.Text =
                            $@"<a target='management' href=""{PageGovInteractPermissions.GetRedirectUrl(
                                PublishmentSystemId, nodeInfo.NodeId)}"">负责人员设置</a>";
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, "Attributes"))
                    {
                        ltlEditUrl.Text =
                            $@"<a target='management' href=""{PageTableStyle.GetRedirectUrl(PublishmentSystemId,
                                ETableStyle.GovInteractContent,
                                PublishmentSystemInfo.AuxiliaryTableForGovInteract, nodeInfo.NodeId)}"">自定义字段</a>";
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, "Apply"))
                    {
                        var applyStyleId = DataProvider.GovInteractChannelDao.GetApplyStyleId(nodeInfo.PublishmentSystemId, nodeInfo.NodeId);
                        ltlEditUrl.Text = $@"<a target='management' href=""{PageTagStyleTemplate.GetRedirectUrl(PublishmentSystemId, applyStyleId, string.Empty)}"">自定义提交模板</a>&nbsp;&nbsp;&nbsp;&nbsp;<a target='management' href=""{PageTagStylePreview.GetRedirectUrl(PublishmentSystemId, applyStyleId, string.Empty)}"">预览 </a>";
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, "Query"))
                    {
                        var queryStyleId = DataProvider.GovInteractChannelDao.GetQueryStyleId(nodeInfo.PublishmentSystemId, nodeInfo.NodeId);
                        ltlEditUrl.Text = $@"<a target='management' href=""{PageTagStyleTemplate.GetRedirectUrl(PublishmentSystemId, queryStyleId, string.Empty)}"">自定义查询模板</a>&nbsp;&nbsp;&nbsp;&nbsp;<a target='management' href=""{PageTagStylePreview.GetRedirectUrl(PublishmentSystemId, queryStyleId, string.Empty)}"">预览 </a>";
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, "MailSMS"))
                    {
                        var styleID = DataProvider.GovInteractChannelDao.GetApplyStyleId(nodeInfo.PublishmentSystemId, nodeInfo.NodeId);
                        ltlEditUrl.Text =
                            $@"<a target='management' href=""{PageTagStyleMailSMS.GetRedirectUrl(
                                PublishmentSystemId, styleID, tableStyle, nodeInfo.NodeId)}"">邮件/短信发送</a>";
                    }
                    else if (StringUtils.EqualsIgnoreCase(type, "InteractType"))
                    {
                        ltlEditUrl.Text = $@"<a target='management' href=""{PageGovInteractType.GetRedirectUrl(PublishmentSystemId, nodeInfo.NodeId)}"">办件类型管理</a>";
                    }
                }
            }
        }
    }
}
