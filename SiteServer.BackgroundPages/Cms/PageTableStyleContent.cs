using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Table;
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
            _redirectUrl = GetRedirectUrl(PublishmentSystemId, nodeId);
            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, nodeId);

            if (IsPostBack) return;

            VerifySitePermissions(AppManager.Permissions.WebSite.Configration);

            //删除样式
            if (Body.IsQueryExists("DeleteStyle"))
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

            InfoMessage(
                $"在此编辑内容模型字段,子栏目默认继承父栏目字段设置; 辅助表:{BaiRongDataProvider.TableCollectionDao.GetTableCnName(_tableName)}({_tableName})");
            NodeManager.AddListItems(DdlNodeId.Items, PublishmentSystemInfo, false, true, Body.AdminName);
            ControlUtils.SelectSingleItem(DdlNodeId, nodeId.ToString());

            var styleInfoList = TableStyleManager.GetTableStyleInfoList(_tableName, _relatedIdentities);

            DgContents.DataSource = styleInfoList;
            DgContents.ItemDataBound += DgContents_ItemDataBound;
            DgContents.DataBind();

            BtnAddStyle.Attributes.Add("onclick", ModalTableStyleAdd.GetOpenWindowString(PublishmentSystemId, 0, _relatedIdentities, _tableName, string.Empty, _redirectUrl));
            BtnAddStyles.Attributes.Add("onclick", ModalTableStylesAdd.GetOpenWindowString(PublishmentSystemId, _relatedIdentities, _tableName, _redirectUrl));
            BtnImport.Attributes.Add("onclick", ModalTableStyleImport.GetOpenWindowString(_tableName, PublishmentSystemId, nodeId));
            BtnExport.Attributes.Add("onclick", ModalExportMessage.GetOpenWindowStringToSingleTableStyle(_tableName, PublishmentSystemId, nodeId));
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
            var ltlValidate = (Literal)e.Item.FindControl("ltlValidate");
            var ltlTaxis = (Literal)e.Item.FindControl("ltlTaxis");
            var ltlEditStyle = (Literal)e.Item.FindControl("ltlEditStyle");
            var ltlEditValidate = (Literal)e.Item.FindControl("ltlEditValidate");

            var showPopWinString = ModalTableMetadataView.GetOpenWindowString(_tableName, styleInfo.AttributeName);
            ltlAttributeName.Text =
                $"<a href=\"javascript:void 0;\" onClick=\"{showPopWinString}\">{styleInfo.AttributeName}</a>";

            ltlDisplayName.Text = styleInfo.DisplayName;
            ltlInputType.Text = InputTypeUtils.GetText(InputTypeUtils.GetEnumType(styleInfo.InputType));
            ltlFieldType.Text = TableMetadataManager.IsAttributeNameExists(_tableName, styleInfo.AttributeName) ? $"真实 {TableMetadataManager.GetTableMetadataDataType(_tableName, styleInfo.AttributeName)}" : "虚拟字段";

            ltlValidate.Text = ValidateTypeUtils.GetValidateInfo(styleInfo);

            showPopWinString = ModalTableStyleAdd.GetOpenWindowString(PublishmentSystemId, styleInfo.TableStyleId, _relatedIdentities, _tableName, styleInfo.AttributeName, _redirectUrl);
            var editText = styleInfo.RelatedIdentity == _nodeInfo.NodeId ? "修改" : "添加";
            ltlEditStyle.Text = $@"<a href=""javascript:;"" onclick=""{showPopWinString}"">{editText}</a>";

            showPopWinString = ModalTableStyleValidateAdd.GetOpenWindowString(styleInfo.TableStyleId, _relatedIdentities, _tableName, styleInfo.AttributeName, _redirectUrl);
            ltlEditValidate.Text = $@"<a href=""javascript:;"" onclick=""{showPopWinString}"">设置</a>";

            ltlTaxis.Text = styleInfo.Taxis.ToString();

            if (styleInfo.RelatedIdentity != _nodeInfo.NodeId) return;

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
    }
}
