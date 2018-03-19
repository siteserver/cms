using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageTableStyleSite : BasePageCms
    {
        public Repeater RptContents;

        public Button BtnAddStyle;
        public Button BtnAddStyles;
        public Button BtnImport;
        public Button BtnExport;
        public Button BtnReturn;

        private int _relatedIdentity;
        private List<int> _relatedIdentities;
        private string _tableName;
        private int _itemId;
        private List<string> _attributeNames;
        private string _returnUrl;

        public static string GetRedirectUrl(int siteId, int relatedIdentity, string returnUrl)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageTableStyleSite), new NameValueCollection
            {
                {"RelatedIdentity", relatedIdentity.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public static string GetRedirectUrl(int siteId, int relatedIdentity, int itemId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageTableStyleSite), new NameValueCollection
            {
                {"RelatedIdentity", relatedIdentity.ToString()},
                {"ItemID", itemId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _relatedIdentity = string.IsNullOrEmpty(AuthRequest.GetQueryString("RelatedIdentity")) ? SiteId : AuthRequest.GetQueryInt("RelatedIdentity");
            _tableName = DataProvider.SiteDao.TableName;
            _itemId = AuthRequest.GetQueryInt("itemID");
            _relatedIdentities = RelatedIdentities.GetRelatedIdentities(SiteId, _relatedIdentity);
            _attributeNames = TableMetadataManager.GetAttributeNameList(_tableName);
            _returnUrl = StringUtils.ValueFromUrl(AuthRequest.GetQueryString("ReturnUrl"));

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.WebSitePermissions.Configration);

            //删除样式
            if (AuthRequest.IsQueryExists("DeleteStyle"))
            {
                var attributeName = AuthRequest.GetQueryString("AttributeName");
                if (TableStyleManager.IsExists(_relatedIdentity, _tableName, attributeName))
                {
                    try
                    {
                        TableStyleManager.Delete(_relatedIdentity, _tableName, attributeName);
                        AuthRequest.AddSiteLog(SiteId, "删除数据表单样式", $"表单:{_tableName},字段:{attributeName}");
                        SuccessDeleteMessage();
                    }
                    catch (Exception ex)
                    {
                        FailDeleteMessage(ex);
                    }
                }
            }

            if (!string.IsNullOrEmpty(_returnUrl))
            {
                BtnReturn.Attributes.Add("onclick", $"location.href='{_returnUrl}';return false;");
            }
            else
            {
                BtnReturn.Visible = false;
            }

            RptContents.DataSource = TableStyleManager.GetTableStyleInfoList(_tableName, _relatedIdentities);
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            var redirectUrl = GetRedirectUrl(SiteId, _relatedIdentity, _itemId, _returnUrl);

            BtnAddStyle.Attributes.Add("onclick", ModalTableStyleAdd.GetOpenWindowString(SiteId, 0, _relatedIdentities, _tableName, string.Empty, redirectUrl));
            BtnAddStyles.Attributes.Add("onclick", ModalTableStylesAdd.GetOpenWindowString(SiteId, _relatedIdentities, _tableName, redirectUrl));

            BtnImport.Attributes.Add("onclick", ModalTableStyleImport.GetOpenWindowString(_tableName, SiteId, _relatedIdentity));
            BtnExport.Attributes.Add("onclick", ModalExportMessage.GetOpenWindowStringToSingleTableStyle(_tableName, SiteId, _relatedIdentity));
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var styleInfo = (TableStyleInfo)e.Item.DataItem;

            if (_attributeNames.Contains(styleInfo.AttributeName))
            {
                e.Item.Visible = false;
                return;
            }

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

            var redirectUrl = GetRedirectUrl(SiteId, _relatedIdentity, _itemId, _returnUrl);
            var showPopWinString = ModalTableStyleAdd.GetOpenWindowString(SiteId, styleInfo.Id, _relatedIdentities, _tableName, styleInfo.AttributeName, redirectUrl);
            var editText = styleInfo.RelatedIdentity == _relatedIdentity ? "修改" : "设置";
            ltlEditStyle.Text = $@"<a href=""javascript:;"" onclick=""{showPopWinString}"">{editText}</a>";

            showPopWinString = ModalTableStyleValidateAdd.GetOpenWindowString(SiteId, styleInfo.Id, _relatedIdentities, _tableName, styleInfo.AttributeName, redirectUrl);
            ltlEditValidate.Text = $@"<a href=""javascript:;"" onclick=""{showPopWinString}"">设置</a>";

            ltlTaxis.Text = styleInfo.Taxis.ToString();

            if (styleInfo.RelatedIdentity != _relatedIdentity) return;

            var urlStyle = PageUtils.GetCmsUrl(SiteId, nameof(PageTableStyleSite), new NameValueCollection
            {
                {"TableName", _tableName},
                {"RelatedIdentity", _relatedIdentity.ToString()},
                {"DeleteStyle", true.ToString()},
                {"AttributeName", styleInfo.AttributeName}
            });
            ltlEditStyle.Text +=
                $@"&nbsp;&nbsp;<a href=""{urlStyle}"" onClick=""javascript:return confirm('此操作将删除对应显示样式，确认吗？');"">删除</a>";
        }
    }
}
