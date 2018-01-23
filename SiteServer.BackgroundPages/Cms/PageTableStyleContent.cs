using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageTableStyleContent : BasePageCms
    {
        public DropDownList DdlNodeId;
        public Repeater RptContents;

        public Button BtnAddStyle;
        public Button BtnAddStyles;
        public Button BtnImport;
        public Button BtnExport;

        private ChannelInfo _nodeInfo;
        private string _tableName;
        private List<int> _relatedIdentities;
        private string _redirectUrl;

        public static string GetRedirectUrl(int siteId, int nodeId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageTableStyleContent), new NameValueCollection
            {
                {"NodeID", nodeId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var nodeId = Body.GetQueryInt("NodeID", SiteId);
            _nodeInfo = ChannelManager.GetChannelInfo(SiteId, nodeId);
            _tableName = ChannelManager.GetTableName(SiteInfo, _nodeInfo);
            _redirectUrl = GetRedirectUrl(SiteId, nodeId);
            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(SiteId, nodeId);

            if (IsPostBack) return;

            VerifySitePermissions(AppManager.Permissions.WebSite.Configration);

            //删除样式
            if (Body.IsQueryExists("DeleteStyle"))
            {
                var attributeName = Body.GetQueryString("AttributeName");
                if (TableStyleManager.IsExists(_nodeInfo.Id, _tableName, attributeName))
                {
                    try
                    {
                        TableStyleManager.Delete(_nodeInfo.Id, _tableName, attributeName);
                        Body.AddSiteLog(SiteId, "删除数据表单样式", $"表单:{_tableName},字段:{attributeName}");
                        SuccessDeleteMessage();
                    }
                    catch (Exception ex)
                    {
                        FailDeleteMessage(ex);
                    }
                }
            }

            InfoMessage(
                $"在此编辑内容模型字段,子栏目默认继承父栏目字段设置; 辅助表:{DataProvider.TableDao.GetDisplayName(_tableName)}({_tableName})");
            ChannelManager.AddListItems(DdlNodeId.Items, SiteInfo, false, true, Body.AdminName);
            ControlUtils.SelectSingleItem(DdlNodeId, nodeId.ToString());

            RptContents.DataSource = TableStyleManager.GetTableStyleInfoList(_tableName, _relatedIdentities);
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            BtnAddStyle.Attributes.Add("onclick", ModalTableStyleAdd.GetOpenWindowString(SiteId, 0, _relatedIdentities, _tableName, string.Empty, _redirectUrl));
            BtnAddStyles.Attributes.Add("onclick", ModalTableStylesAdd.GetOpenWindowString(SiteId, _relatedIdentities, _tableName, _redirectUrl));
            BtnImport.Attributes.Add("onclick", ModalTableStyleImport.GetOpenWindowString(_tableName, SiteId, nodeId));
            BtnExport.Attributes.Add("onclick", ModalExportMessage.GetOpenWindowStringToSingleTableStyle(_tableName, SiteId, nodeId));
        }

        public void Redirect(object sender, EventArgs e)
        {
            PageUtils.Redirect(GetRedirectUrl(SiteId, TranslateUtils.ToInt(DdlNodeId.SelectedValue)));
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
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

            ltlAttributeName.Text = styleInfo.AttributeName;

            ltlDisplayName.Text = styleInfo.DisplayName;
            ltlInputType.Text = InputTypeUtils.GetText(styleInfo.InputType);
            ltlFieldType.Text = TableMetadataManager.IsAttributeNameExists(_tableName, styleInfo.AttributeName) ? $"真实 {TableMetadataManager.GetTableMetadataDataType(_tableName, styleInfo.AttributeName)}" : "虚拟字段";

            ltlValidate.Text = TableStyleManager.GetValidateInfo(styleInfo);

            var showPopWinString = ModalTableStyleAdd.GetOpenWindowString(SiteId, styleInfo.Id, _relatedIdentities, _tableName, styleInfo.AttributeName, _redirectUrl);
            var editText = styleInfo.RelatedIdentity == _nodeInfo.Id ? "修改" : "添加";
            ltlEditStyle.Text = $@"<a href=""javascript:;"" onclick=""{showPopWinString}"">{editText}</a>";

            showPopWinString = ModalTableStyleValidateAdd.GetOpenWindowString(SiteId, styleInfo.Id, _relatedIdentities, _tableName, styleInfo.AttributeName, _redirectUrl);
            ltlEditValidate.Text = $@"<a href=""javascript:;"" onclick=""{showPopWinString}"">设置</a>";

            ltlTaxis.Text = styleInfo.Taxis.ToString();

            if (styleInfo.RelatedIdentity != _nodeInfo.Id) return;

            var urlStyle = PageUtils.GetCmsUrl(SiteId, nameof(PageTableStyleContent), new NameValueCollection
            {
                {"NodeID", _nodeInfo.Id.ToString()},
                {"DeleteStyle", true.ToString()},
                {"TableName", _tableName},
                {"AttributeName", styleInfo.AttributeName}
            });
            ltlEditStyle.Text +=
                $@"&nbsp;&nbsp;<a href=""{urlStyle}"" onClick=""javascript:return confirm('此操作将删除对应显示样式，确认吗？');"">删除</a>";
        }
    }
}
