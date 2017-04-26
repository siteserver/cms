using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Wcm;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageTableStyle : BasePageCms
    {
        public DataGrid dgContents;

        public Button btnAddStyle;
        public Button btnAddStyles;
        public Button btnImport;
        public Button btnExport;
        public Button btnReturn;

        private ETableStyle _tableStyle;
        private int _relatedIdentity;
        private List<int> _relatedIdentities;
        private string _tableName;
        private int _itemId;
        private List<string> _attributeNames;

        public static string GetRedirectUrl(int publishmentSystemId, ETableStyle tableStyle, string tableName, int relatedIdentity)
        {
            return PageUtils.GetCmsUrl(nameof(PageTableStyle), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"TableStyle", ETableStyleUtils.GetValue(tableStyle)},
                {"TableName", tableName},
                {"RelatedIdentity", relatedIdentity.ToString()}
            });
        }

        public static string GetRedirectUrl(int publishmentSystemId, ETableStyle tableStyle, string tableName, int relatedIdentity, int itemId)
        {
            return PageUtils.GetCmsUrl(nameof(PageTableStyle), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"TableStyle", ETableStyleUtils.GetValue(tableStyle)},
                {"TableName", tableName},
                {"RelatedIdentity", relatedIdentity.ToString()},
                {"ItemID", itemId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _relatedIdentity = string.IsNullOrEmpty(Body.GetQueryString("RelatedIdentity")) ? PublishmentSystemId : Body.GetQueryInt("RelatedIdentity");
            _tableStyle = ETableStyleUtils.GetEnumType(Body.GetQueryString("TableStyle"));
            _tableName = Body.GetQueryString("TableName");
            _itemId = Body.GetQueryInt("itemID");
            _relatedIdentities = RelatedIdentities.GetRelatedIdentities(_tableStyle, PublishmentSystemId, _relatedIdentity);
            _attributeNames = TableManager.GetAttributeNameList(_tableStyle, _tableName);

            if (IsPostBack) return;

            if (_tableStyle == ETableStyle.InputContent)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdInput, "提交表单管理", AppManager.Cms.Permission.WebSite.Input);
            }
            else if (_tableStyle == ETableStyle.Site)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdConfigration, "站点属性设置",
                    AppManager.Cms.Permission.WebSite.Configration);
            }
            else
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdConfigration, AppManager.Cms.LeftMenu.Configuration.IdConfigurationContentModel, "虚拟字段管理",
                    AppManager.Cms.Permission.WebSite.Configration);
            }

            //删除样式
            if (Body.IsQueryExists("DeleteStyle"))
            {
                DeleteStyle();
            }
            else if (Body.IsQueryExists("SetTaxis"))
            {
                SetTaxis();
            }

            if (_tableStyle == ETableStyle.BackgroundContent)
            {
                var urlModel = PageContentModel.GetRedirectUrl(PublishmentSystemId);
                btnReturn.Attributes.Add("onclick", $"location.href='{urlModel}';return false;");
            }
            else if (_tableStyle == ETableStyle.InputContent)
            {
                btnReturn.Attributes.Add("onclick", $"location.href='{PageInput.GetRedirectUrl(PublishmentSystemId)}';return false;");
            }
            else if (_tableStyle == ETableStyle.GovInteractContent)
            {
                var urlReturn = PageGovInteractListAll.GetRedirectUrl(PublishmentSystemId, 0);
                btnReturn.Attributes.Add("onclick", $"location.href='{urlReturn}';return false;");
            }
            else if (_tableStyle == ETableStyle.Site)
            {
                btnReturn.Attributes.Add("onclick", $"location.href='{PageConfigurationSiteAttributes.GetRedirectUrl(PublishmentSystemId)}';return false;");
            }
            else
            {
                btnReturn.Visible = false;
            }

            var styleInfoList = TableStyleManager.GetTableStyleInfoList(_tableStyle, _tableName, _relatedIdentities);

            dgContents.DataSource = styleInfoList;
            dgContents.ItemDataBound += dgContents_ItemDataBound;
            dgContents.DataBind();

            var redirectUrl = GetRedirectUrl(PublishmentSystemId, _tableStyle, _tableName, _relatedIdentity, _itemId);

            btnAddStyle.Attributes.Add("onclick", ModalTableStyleAdd.GetOpenWindowString(PublishmentSystemId, 0, _relatedIdentities, _tableName, string.Empty, _tableStyle, redirectUrl));
            btnAddStyles.Attributes.Add("onclick", ModalTableStylesAdd.GetOpenWindowString(PublishmentSystemId, _relatedIdentities, _tableName, _tableStyle, redirectUrl));

            btnImport.Attributes.Add("onclick", ModalTableStyleImport.GetOpenWindowString(_tableName, _tableStyle, PublishmentSystemId, _relatedIdentity));
            btnExport.Attributes.Add("onclick", ModalExportMessage.GetOpenWindowStringToSingleTableStyle(_tableStyle, _tableName, PublishmentSystemId, _relatedIdentity));
        }

        private void DeleteStyle()
        {
            var attributeName = Body.GetQueryString("AttributeName");
            if (TableStyleManager.IsExists(_relatedIdentity, _tableName, attributeName))
            {
                try
                {
                    TableStyleManager.Delete(_relatedIdentity, _tableName, attributeName);
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
            if (styleInfo != null && styleInfo.RelatedIdentity == _relatedIdentity)
            {
                var direction = Body.GetQueryString("Direction");

                switch (direction.ToUpper())
                {
                    case "UP":
                        BaiRongDataProvider.TableStyleDao.TaxisUp(tableStyleId);
                        break;
                    case "DOWN":
                        BaiRongDataProvider.TableStyleDao.TaxisDown(tableStyleId); 
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

        void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var styleInfo = (TableStyleInfo)e.Item.DataItem;

                if (_attributeNames.Contains(styleInfo.AttributeName))
                {
                    e.Item.Visible = false;
                    return;
                }

                var ltlAttributeName = (Literal)e.Item.FindControl("ltlAttributeName");
                var ltlDisplayName = (Literal)e.Item.FindControl("ltlDisplayName");
                var ltlInputType = (Literal)e.Item.FindControl("ltlInputType");
                var ltlIsVisible = (Literal)e.Item.FindControl("ltlIsVisible");
                var ltlValidate = (Literal)e.Item.FindControl("ltlValidate");
                var ltlEditStyle = (Literal)e.Item.FindControl("ltlEditStyle");
                var ltlEditValidate = (Literal)e.Item.FindControl("ltlEditValidate");
                var upLinkButton = (HyperLink)e.Item.FindControl("UpLinkButton");
                var downLinkButton = (HyperLink)e.Item.FindControl("DownLinkButton");

                var showPopWinString = Sys.ModalTableMetadataView.GetOpenWindowString(ETableStyleUtils.GetTableType(_tableStyle), _tableName, styleInfo.AttributeName);
                ltlAttributeName.Text =
                    $"<a href=\"javascript:void 0;\" onClick=\"{showPopWinString}\">{styleInfo.AttributeName}</a>";

                ltlDisplayName.Text = styleInfo.DisplayName;
                ltlInputType.Text = EInputTypeUtils.GetText(EInputTypeUtils.GetEnumType(styleInfo.InputType));

                ltlIsVisible.Text = StringUtils.GetTrueOrFalseImageHtml(styleInfo.IsVisible.ToString());
                ltlValidate.Text = EInputValidateTypeUtils.GetValidateInfo(styleInfo);

                var redirectUrl = GetRedirectUrl(PublishmentSystemId, _tableStyle, _tableName, _relatedIdentity, _itemId);
                showPopWinString = ModalTableStyleAdd.GetOpenWindowString(PublishmentSystemId, styleInfo.TableStyleId, _relatedIdentities, _tableName, styleInfo.AttributeName, _tableStyle, redirectUrl);
                var editText = "设置";
                if (styleInfo.RelatedIdentity == _relatedIdentity)//数据库中有样式
                {
                    editText = "修改";
                }
                ltlEditStyle.Text = $"<a href=\"javascript:void 0;\" onClick=\"{showPopWinString}\">{editText}</a>";

                showPopWinString = ModalTableStyleValidateAdd.GetOpenWindowString(styleInfo.TableStyleId, _relatedIdentities, _tableName, styleInfo.AttributeName, _tableStyle, redirectUrl);
                ltlEditValidate.Text = $"<a href=\"javascript:void 0;\" onClick=\"{showPopWinString}\">设置</a>";

                if (styleInfo.RelatedIdentity == _relatedIdentity)//数据库中有样式
                {
                    var urlStyle = PageUtils.GetCmsUrl(nameof(PageTableStyle), new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"TableStyle", ETableStyleUtils.GetValue(_tableStyle)},
                        {"TableName", _tableName},
                        {"RelatedIdentity", _relatedIdentity.ToString()},
                        {"DeleteStyle", true.ToString()},
                        {"AttributeName", styleInfo.AttributeName}
                    });
                    ltlEditStyle.Text +=
                        $@"&nbsp;&nbsp;<a href=""{urlStyle}"" onClick=""javascript:return confirm('此操作将删除对应显示样式，确认吗？');"">删除</a>";
                }

                //if (TableStyleManager.IsMetadata(this.tableStyle, styleInfo.AttributeName) || styleInfo.RelatedIdentity != this.relatedIdentity)
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

                    upLinkButton.NavigateUrl = PageUtils.GetCmsUrl(nameof(PageTableStyle), new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"TableStyle", ETableStyleUtils.GetValue(_tableStyle)},
                        {"TableName", _tableName},
                        {"RelatedIdentity", _relatedIdentity.ToString()},
                        {"SetTaxis", true.ToString()},
                        {"TableStyleID", styleInfo.TableStyleId.ToString()},
                        {"Direction", "UP"},
                        {"TableMetadataId", tableMetadataId.ToString()}
                    });
                    downLinkButton.NavigateUrl = PageUtils.GetCmsUrl(nameof(PageTableStyle), new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"TableStyle", ETableStyleUtils.GetValue(_tableStyle)},
                        {"TableName", _tableName},
                        {"RelatedIdentity", _relatedIdentity.ToString()},
                        {"SetTaxis", true.ToString()},
                        {"TableStyleID", styleInfo.TableStyleId.ToString()},
                        {"Direction", "DOWN"},
                        {"TableMetadataId", tableMetadataId.ToString()}
                    });
                }
            }
        }
    }
}
