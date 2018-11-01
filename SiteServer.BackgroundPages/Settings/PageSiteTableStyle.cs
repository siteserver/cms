using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageSiteTableStyle : BasePage
    {
        public Repeater RptContents;

        public Button BtnAddStyle;
        public Button BtnAddStyles;
        public Button BtnImport;
        public Button BtnExport;

        private string _tableName;
        private string _redirectUrl;

        public static string GetRedirectUrl(string tableName)
        {
            return PageUtils.GetSettingsUrl(nameof(PageSiteTableStyle), new NameValueCollection
            {
                {"tableName", tableName}
            });
        }

        public string GetReturnUrl()
        {
            return string.Empty;
            //return PageSiteAuxiliaryTable.GetRedirectUrl();
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _tableName = AuthRequest.GetQueryString("tableName");
            _redirectUrl = GetRedirectUrl(_tableName);

            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Site);

            if (AuthRequest.IsQueryExists("DeleteStyle"))
            {
                var attributeName = AuthRequest.GetQueryString("AttributeName");
                if (TableStyleManager.IsExists(0, _tableName, attributeName))
                {
                    DataProvider.TableStyleDao.Delete(0, _tableName, attributeName);
                    AuthRequest.AddAdminLog("删除数据表单样式", $"表单:{_tableName},字段:{attributeName}");
                    SuccessDeleteMessage();
                }
            }

            var styleInfoList = TableStyleManager.GetStyleInfoList(_tableName, TableStyleManager.EmptyRelatedIdentities);

            RptContents.DataSource = styleInfoList;
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            BtnAddStyle.Attributes.Add("onclick", ModalTableStyleAdd.GetOpenWindowString(0, 0, new List<int> { 0 }, _tableName, string.Empty, _redirectUrl));
            BtnAddStyles.Attributes.Add("onclick",
                ModalTableStylesAdd.GetOpenWindowString(0, new List<int> {0}, _tableName, _redirectUrl));
            BtnImport.Attributes.Add("onclick", ModalTableStyleImport.GetOpenWindowString(_tableName, 0, 0));
            BtnExport.Attributes.Add("onclick", ModalExportMessage.GetOpenWindowStringToSingleTableStyle(_tableName));
        }

        public void Redirect(object sender, EventArgs e)
        {
            PageUtils.Redirect(GetRedirectUrl(_tableName));
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

            var columnInfo = TableColumnManager.GetTableColumnInfo(_tableName, styleInfo.AttributeName);

            ltlFieldType.Text = columnInfo != null ? $"真实 {DataTypeUtils.GetText(columnInfo.DataType)}" : "虚拟字段";

            ltlValidate.Text = TableStyleManager.GetValidateInfo(styleInfo);

            var showPopWinString = ModalTableStyleAdd.GetOpenWindowString(0, styleInfo.Id, new List<int>{0}, _tableName, styleInfo.AttributeName, _redirectUrl);
            var editText = styleInfo.Id != 0 ? "修改" : "添加";
            ltlEditStyle.Text = $@"<a href=""javascript:;"" onclick=""{showPopWinString}"">{editText}</a>";

            showPopWinString = ModalTableStyleValidateAdd.GetOpenWindowString(0, styleInfo.Id, new List<int> { 0 }, _tableName, styleInfo.AttributeName, _redirectUrl);
            ltlEditValidate.Text = $@"<a href=""javascript:;"" onclick=""{showPopWinString}"">设置</a>";

            ltlTaxis.Text = styleInfo.Taxis.ToString();

            if (styleInfo.Id == 0) return;

            ltlEditStyle.Text +=
                $@"&nbsp;&nbsp;<a href=""{PageUtils.GetSettingsUrl(nameof(PageSiteTableStyle), new NameValueCollection
                {
                    {"tableName", _tableName},
                    {"DeleteStyle", true.ToString()},
                    {"AttributeName", styleInfo.AttributeName}
                })}"" onClick=""javascript:return confirm('此操作将删除对应显示样式，确认吗？');"">删除</a>";
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(string.Empty);
        }
    }
}
