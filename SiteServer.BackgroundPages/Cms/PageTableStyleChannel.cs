using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTableStyleChannel : BasePageCms
    {
        public DropDownList DdlChannelId;
        public Repeater RptContents;
        public Button BtnAddStyle;
        public Button BtnAddStyles;
        public Button BtnImport;
        public Button BtnExport;

        private string _tableName;
        private ChannelInfo _channelInfo;
        private List<int> _relatedIdentities;
        private string _redirectUrl;

        public static string GetRedirectUrl(int siteId, int channelId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageTableStyleChannel), new NameValueCollection
            {
                {"SiteId", siteId.ToString()},
                {"channelId", channelId.ToString()}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _tableName = DataProvider.ChannelDao.TableName;
            var channelId = AuthRequest.GetQueryInt("channelId", SiteId);
            _channelInfo = ChannelManager.GetChannelInfo(SiteId, channelId);
            _redirectUrl = GetRedirectUrl(SiteId, channelId);
            _relatedIdentities = TableStyleManager.GetRelatedIdentities(_channelInfo);

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.WebSitePermissions.Configration);
                
            //删除样式
            if (AuthRequest.IsQueryExists("DeleteStyle"))
            {
                var attributeName = AuthRequest.GetQueryString("AttributeName");
                if (TableStyleManager.IsExists(_channelInfo.Id, _tableName, attributeName))
                {
                    try
                    {
                        DataProvider.TableStyleDao.Delete(_channelInfo.Id, _tableName, attributeName);
                        AuthRequest.AddSiteLog(SiteId, "删除数据表单样式", $"表单:{_tableName},字段:{attributeName}");
                        SuccessDeleteMessage();
                    }
                    catch (Exception ex)
                    {
                        FailDeleteMessage(ex);
                    }
                }
            }

            ChannelManager.AddListItems(DdlChannelId.Items, SiteInfo, false, true, AuthRequest.AdminPermissionsImpl);
            ControlUtils.SelectSingleItem(DdlChannelId, channelId.ToString());

            RptContents.DataSource = TableStyleManager.GetChannelStyleInfoList(_channelInfo);
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            BtnAddStyle.Attributes.Add("onclick", ModalTableStyleAdd.GetOpenWindowString(SiteId, 0, _relatedIdentities, _tableName, string.Empty, _redirectUrl));
            BtnAddStyles.Attributes.Add("onclick", ModalTableStylesAdd.GetOpenWindowString(SiteId, _relatedIdentities, _tableName, _redirectUrl));
            BtnImport.Attributes.Add("onclick", ModalTableStyleImport.GetOpenWindowString(_tableName, SiteId, channelId));
            BtnExport.Attributes.Add("onclick", ModalExportMessage.GetOpenWindowStringToSingleTableStyle(_tableName, SiteId, channelId));
        }

        public void Redirect(object sender, EventArgs e)
        {
            PageUtils.Redirect(GetRedirectUrl(SiteId, TranslateUtils.ToInt(DdlChannelId.SelectedValue)));
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var styleInfo = (TableStyleInfo)e.Item.DataItem;

            var ltlAttributeName = (Literal)e.Item.FindControl("ltlAttributeName");
            var ltlDisplayName = (Literal)e.Item.FindControl("ltlDisplayName");
            var ltlInputType = (Literal)e.Item.FindControl("ltlInputType");
            var ltlValidate = (Literal)e.Item.FindControl("ltlValidate");
            var ltlTaxis = (Literal)e.Item.FindControl("ltlTaxis");
            var ltlEditStyle = (Literal)e.Item.FindControl("ltlEditStyle");
            var ltlEditValidate = (Literal)e.Item.FindControl("ltlEditValidate");

            ltlAttributeName.Text = styleInfo.AttributeName;

            ltlDisplayName.Text = styleInfo.DisplayName;
            ltlInputType.Text = InputTypeUtils.GetText(styleInfo.InputType);

            ltlValidate.Text = TableStyleManager.GetValidateInfo(styleInfo);

            var showPopWinString = ModalTableStyleAdd.GetOpenWindowString(SiteId, styleInfo.Id, _relatedIdentities, _tableName, styleInfo.AttributeName, _redirectUrl);
            var editText = styleInfo.RelatedIdentity == _channelInfo.Id ? "修改" : "添加";
            ltlEditStyle.Text = $@"<a href=""javascript:;"" onclick=""{showPopWinString}"">{editText}</a>";

            showPopWinString = ModalTableStyleValidateAdd.GetOpenWindowString(SiteId, styleInfo.Id, _relatedIdentities, _tableName, styleInfo.AttributeName, _redirectUrl);
            ltlEditValidate.Text = $@"<a href=""javascript:;"" onclick=""{showPopWinString}"">设置</a>";

            ltlTaxis.Text = styleInfo.Taxis.ToString();

            if (styleInfo.RelatedIdentity != _channelInfo.Id) return;

            var urlStyle = PageUtils.GetCmsUrl(SiteId, nameof(PageTableStyleChannel), new NameValueCollection
            {
                {"channelId", _channelInfo.Id.ToString()},
                {"DeleteStyle", true.ToString()},
                {"TableName", _tableName},
                {"AttributeName", styleInfo.AttributeName}
            });
            ltlEditStyle.Text +=
                $@"&nbsp;&nbsp;<a href=""{urlStyle}"" onClick=""javascript:return confirm('此操作将删除对应显示样式，确认吗？');"">删除</a>";
        }
	}
}
