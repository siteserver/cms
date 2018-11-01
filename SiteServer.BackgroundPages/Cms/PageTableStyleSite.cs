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
    public class PageTableStyleSite : BasePageCms
    {
        public Repeater RptContents;

        public Button BtnAddStyle;
        public Button BtnAddStyles;
        public Button BtnImport;
        public Button BtnExport;
        public Button BtnReturn;

        private List<int> _relatedIdentities;
        private string _tableName;
        private int _itemId;
        private List<string> _attributeNames;
        private string _returnUrl;

        public static string GetRedirectUrl(int siteId, int itemId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageTableStyleSite), new NameValueCollection
            {
                {"ItemID", itemId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _tableName = DataProvider.SiteDao.TableName;
            _itemId = AuthRequest.GetQueryInt("itemID");
            _relatedIdentities = TableStyleManager.GetRelatedIdentities(SiteId);
            _attributeNames = TableColumnManager.GetTableColumnNameList(_tableName);
            _returnUrl = StringUtils.ValueFromUrl(AuthRequest.GetQueryString("ReturnUrl"));

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.WebSitePermissions.Configration);

            //删除样式
            if (AuthRequest.IsQueryExists("DeleteStyle"))
            {
                var attributeName = AuthRequest.GetQueryString("AttributeName");
                if (TableStyleManager.IsExists(SiteId, _tableName, attributeName))
                {
                    try
                    {
                        DataProvider.TableStyleDao.Delete(SiteId, _tableName, attributeName);
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

            RptContents.DataSource = TableStyleManager.GetSiteStyleInfoList(SiteId);
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            var redirectUrl = GetRedirectUrl(SiteId, _itemId, _returnUrl);

            BtnAddStyle.Attributes.Add("onclick", ModalTableStyleAdd.GetOpenWindowString(SiteId, 0, _relatedIdentities, _tableName, string.Empty, redirectUrl));
            BtnAddStyles.Attributes.Add("onclick", ModalTableStylesAdd.GetOpenWindowString(SiteId, _relatedIdentities, _tableName, redirectUrl));

            BtnImport.Attributes.Add("onclick", ModalTableStyleImport.GetOpenWindowString(_tableName, SiteId, SiteId));
            BtnExport.Attributes.Add("onclick", ModalExportMessage.GetOpenWindowStringToSingleTableStyle(_tableName, SiteId, SiteId));
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

            var redirectUrl = GetRedirectUrl(SiteId, _itemId, _returnUrl);
            var showPopWinString = ModalTableStyleAdd.GetOpenWindowString(SiteId, styleInfo.Id, _relatedIdentities, _tableName, styleInfo.AttributeName, redirectUrl);
            ltlEditStyle.Text = $@"<a href=""javascript:;"" onclick=""{showPopWinString}"">修改</a>";

            showPopWinString = ModalTableStyleValidateAdd.GetOpenWindowString(SiteId, styleInfo.Id, _relatedIdentities, _tableName, styleInfo.AttributeName, redirectUrl);
            ltlEditValidate.Text = $@"<a href=""javascript:;"" onclick=""{showPopWinString}"">设置</a>";

            ltlTaxis.Text = styleInfo.Taxis == 0 ? string.Empty : styleInfo.Taxis.ToString();

            var urlStyle = PageUtils.GetCmsUrl(SiteId, nameof(PageTableStyleSite), new NameValueCollection
            {
                {"TableName", _tableName},
                {"RelatedIdentity", SiteId.ToString()},
                {"DeleteStyle", true.ToString()},
                {"AttributeName", styleInfo.AttributeName}
            });
            ltlEditStyle.Text +=
                $@"&nbsp;&nbsp;<a href=""{urlStyle}"" onClick=""javascript:return confirm('此操作将删除对应显示样式，确认吗？');"">删除</a>";
        }
    }
}
