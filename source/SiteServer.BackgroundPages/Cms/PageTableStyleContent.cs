using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Settings;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageTableStyleContent : BasePageCms
    {
        public DropDownList DdlNodeId;
        public DataGrid DgContents;

        public Button BtnAddStyle;
        public Button BtnAddStyles;
        public Button BtnImport;
        public Button BtnExport;

        private NodeInfo _nodeInfo;
        private string _tableName;
        private EAuxiliaryTableType _tableType;
        private ETableStyle _tableStyle;
        private List<int> _relatedIdentities;
        private string _redirectUrl;

        public static string GetRedirectUrl(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetCmsUrl(nameof(PageTableStyleContent), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var nodeId = Body.GetQueryInt("NodeID", PublishmentSystemId);
            _nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);
            _tableName = NodeManager.GetTableName(PublishmentSystemInfo, _nodeInfo);
            _tableType = NodeManager.GetTableType(PublishmentSystemInfo, _nodeInfo);
            _tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, _nodeInfo);
            _redirectUrl = GetRedirectUrl(PublishmentSystemId, nodeId);

            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, nodeId);

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdConfigration, "内容字段管理", AppManager.Permissions.WebSite.Configration);

                //删除样式
                if (Body.IsQueryExists("DeleteStyle"))
                {
                    DeleteStyle();
                }
                else if (Body.IsQueryExists("SetTaxis"))
                {
                    SetTaxis();
                }

                InfoMessage(
                    $"在此编辑内容模型字段,子栏目默认继承父栏目字段设置; 辅助表:{BaiRongDataProvider.TableCollectionDao.GetTableCnName(_tableName)}({_tableName})");
                NodeManager.AddListItems(DdlNodeId.Items, PublishmentSystemInfo, false, true, Body.AdminName);
                ControlUtils.SelectListItems(DdlNodeId, nodeId.ToString());

                var styleInfoList = TableStyleManager.GetTableStyleInfoList(_tableStyle, _tableName, _relatedIdentities);

                DgContents.DataSource = styleInfoList;
                DgContents.ItemDataBound += DgContents_ItemDataBound;
                DgContents.DataBind();

                BtnAddStyle.Attributes.Add("onclick", ModalTableStyleAdd.GetOpenWindowString(PublishmentSystemId, 0, _relatedIdentities, _tableName, string.Empty, _tableStyle, _redirectUrl));
                BtnAddStyles.Attributes.Add("onclick", ModalTableStylesAdd.GetOpenWindowString(PublishmentSystemId, _relatedIdentities, _tableName, _tableStyle, _redirectUrl));
                BtnImport.Attributes.Add("onclick", ModalTableStyleImport.GetOpenWindowString(_tableName, _tableStyle, PublishmentSystemId, nodeId));
                BtnExport.Attributes.Add("onclick", ModalExportMessage.GetOpenWindowStringToSingleTableStyle(_tableStyle, _tableName, PublishmentSystemId, nodeId));
            }
        }

        private void DeleteStyle()
        {
            var attributeName = Body.GetQueryString("AttributeName");
            if (TableStyleManager.IsExists(_nodeInfo.NodeId, _tableName, attributeName))
            {
                try
                {
                    TableStyleManager.Delete(_nodeInfo.NodeId, _tableName, attributeName);
                    Body.AddSiteLog(PublishmentSystemId, "删除数据表单样式", $"表单:{_tableName},字段:{attributeName}");
                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }
        }

        private void SetTaxis()
        {
            var tableStyleId = Body.GetQueryInt("TableStyleID");
            var styleInfo = BaiRongDataProvider.TableStyleDao.GetTableStyleInfo(tableStyleId);
            if (styleInfo != null && styleInfo.RelatedIdentity == _nodeInfo.NodeId)
            {
                var direction = Body.GetQueryString("Direction");

                switch (direction.ToUpper())
                {
                    case "UP":
                        BaiRongDataProvider.TableStyleDao.TaxisDown(tableStyleId);
                        break;
                    case "DOWN":
                        BaiRongDataProvider.TableStyleDao.TaxisUp(tableStyleId);
                        break;
                }
                SuccessMessage("排序成功！");
            }
            else
            {
                var direction = Body.GetQueryString("Direction");
                var tableMetadataId = Body.GetQueryInt("TableMetadataId");
                switch (direction.ToUpper())
                {
                    case "UP":
                        BaiRongDataProvider.TableMetadataDao.TaxisDown(tableMetadataId, _tableName);
                        break;
                    case "DOWN":
                        BaiRongDataProvider.TableMetadataDao.TaxisUp(tableMetadataId, _tableName);
                        break;
                }
                SuccessMessage("排序成功！");
            }
        }

        public void Redirect(object sender, EventArgs e)
        {
            PageUtils.Redirect(GetRedirectUrl(PublishmentSystemId, TranslateUtils.ToInt(DdlNodeId.SelectedValue)));
        }

        private void DgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var styleInfo = (TableStyleInfo)e.Item.DataItem;

            var ltlAttributeName = (Literal)e.Item.FindControl("ltlAttributeName");
            var ltlDisplayName = (Literal)e.Item.FindControl("ltlDisplayName");
            var ltlInputType = (Literal)e.Item.FindControl("ltlInputType");
            var ltlFieldType = (Literal)e.Item.FindControl("ltlFieldType");
            var ltlIsVisible = (Literal)e.Item.FindControl("ltlIsVisible");
            var ltlValidate = (Literal)e.Item.FindControl("ltlValidate");
            var ltlEditStyle = (Literal)e.Item.FindControl("ltlEditStyle");
            var ltlEditValidate = (Literal)e.Item.FindControl("ltlEditValidate");
            var upLinkButton = (HyperLink)e.Item.FindControl("UpLinkButton");
            var downLinkButton = (HyperLink)e.Item.FindControl("DownLinkButton");

            var showPopWinString = ModalTableMetadataView.GetOpenWindowString(_tableType, _tableName, styleInfo.AttributeName);
            ltlAttributeName.Text =
                $"<a href=\"javascript:void 0;\" onClick=\"{showPopWinString}\">{styleInfo.AttributeName}</a>";

            ltlDisplayName.Text = styleInfo.DisplayName;
            ltlInputType.Text = InputTypeUtils.GetText(InputTypeUtils.GetEnumType(styleInfo.InputType));
            ltlFieldType.Text = TableManager.IsAttributeNameExists(_tableStyle, _tableName, styleInfo.AttributeName) ? $"真实 {TableManager.GetTableMetadataDataType(_tableName, styleInfo.AttributeName)}" : "虚拟字段";

            ltlIsVisible.Text = StringUtils.GetTrueOrFalseImageHtml(styleInfo.IsVisible.ToString());
            ltlValidate.Text = ValidateTypeUtils.GetValidateInfo(styleInfo);

            showPopWinString = ModalTableStyleAdd.GetOpenWindowString(PublishmentSystemId, styleInfo.TableStyleId, _relatedIdentities, _tableName, styleInfo.AttributeName, _tableStyle, _redirectUrl);
            var editText = "添加";
            if (styleInfo.RelatedIdentity == _nodeInfo.NodeId)//数据库中有样式
            {
                editText = "修改";
            }
            ltlEditStyle.Text = $"<a href=\"javascript:void 0;\" onClick=\"{showPopWinString}\">{editText}</a>";

            showPopWinString = ModalTableStyleValidateAdd.GetOpenWindowString(styleInfo.TableStyleId, _relatedIdentities, _tableName, styleInfo.AttributeName, _tableStyle, _redirectUrl);
            ltlEditValidate.Text = $"<a href=\"javascript:void 0;\" onClick=\"{showPopWinString}\">设置</a>";

            if (styleInfo.RelatedIdentity == _nodeInfo.NodeId)//数据库中有样式
            {
                var urlStyle = PageUtils.GetCmsUrl(nameof(PageTableStyleContent), new NameValueCollection
                {
                    {"PublishmentSystemID", PublishmentSystemId.ToString()},
                    {"NodeID", _nodeInfo.NodeId.ToString()},
                    {"DeleteStyle", true.ToString()},
                    {"TableName", _tableName},
                    {"AttributeName", styleInfo.AttributeName}
                });
                ltlEditStyle.Text +=
                    $@"&nbsp;&nbsp;<a href=""{urlStyle}"" onClick=""javascript:return confirm('此操作将删除对应显示样式，确认吗？');"">删除</a>";
            }

            //if (TableStyleManager.IsMetadata(this.tableStyle, styleInfo.AttributeName) || styleInfo.RelatedIdentity != this.nodeInfo.NodeID)
            //{
            //    isTaxisVisible = false;
            //}
            //else
            //{
            var isTaxisVisible = !TableStyleManager.IsExistsInParents(_relatedIdentities, _tableName, styleInfo.AttributeName);
            //}

            if (!isTaxisVisible)
            {
                upLinkButton.Visible = downLinkButton.Visible = false;
            }
            else
            {
                var tableMetadataId = BaiRongDataProvider.TableMetadataDao.GetTableMetadataId(styleInfo.TableName, styleInfo.AttributeName);
                upLinkButton.NavigateUrl = PageUtils.GetCmsUrl(nameof(PageTableStyleContent), new NameValueCollection
                {
                    {"PublishmentSystemID", PublishmentSystemId.ToString()},
                    {"NodeID", _nodeInfo.NodeId.ToString()},
                    {"SetTaxis", true.ToString()},
                    {"TableStyleID", styleInfo.TableStyleId.ToString()},
                    {"Direction", "UP"},
                    {"TableMetadataId", tableMetadataId.ToString()}
                });
                downLinkButton.NavigateUrl = PageUtils.GetCmsUrl(nameof(PageTableStyleContent), new NameValueCollection
                {
                    {"PublishmentSystemID", PublishmentSystemId.ToString()},
                    {"NodeID", _nodeInfo.NodeId.ToString()},
                    {"SetTaxis", true.ToString()},
                    {"TableStyleID", styleInfo.TableStyleId.ToString()},
                    {"Direction", "DOWN"},
                    {"TableMetadataId", tableMetadataId.ToString()}
                });
            }
        }
    }
}
