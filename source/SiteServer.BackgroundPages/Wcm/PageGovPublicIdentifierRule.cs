using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.GovPublic;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.BackgroundPages.Wcm
{
    public class PageGovPublicIdentifierRule : BasePageGovPublic
    {
        public Literal ltlPreview;
        public DataGrid dgContents;
        public Button AddButton;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetWcmUrl(nameof(PageGovPublicIdentifierRule), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["Delete"] != null)
            {
                var ruleID = TranslateUtils.ToInt(Request.QueryString["RuleID"]);
                try
                {
                    DataProvider.GovPublicIdentifierRuleDao.Delete(ruleID);
                    SuccessMessage("成功删除规则");
                }
                catch (Exception ex)
                {
                    SuccessMessage($"删除规则失败，{ex.Message}");
                }
            }
            else if ((Request.QueryString["Up"] != null || Request.QueryString["Down"] != null) && Request.QueryString["RuleID"] != null)
            {
                var ruleID = TranslateUtils.ToInt(Request.QueryString["RuleID"]);
                var isDown = (Request.QueryString["Down"] != null) ? true : false;
                if (isDown)
                {
                    DataProvider.GovPublicIdentifierRuleDao.UpdateTaxisToUp(ruleID, PublishmentSystemId);
                }
                else
                {
                    DataProvider.GovPublicIdentifierRuleDao.UpdateTaxisToDown(ruleID, PublishmentSystemId);
                }
            }

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovPublic, AppManager.Wcm.LeftMenu.GovPublic.IdGovPublicContentConfiguration, "索引号生成规则", AppManager.Wcm.Permission.WebSite.GovPublicContentConfiguration);

                ltlPreview.Text = GovPublicManager.GetPreviewIdentifier(PublishmentSystemId);

                dgContents.DataSource = DataProvider.GovPublicIdentifierRuleDao.GetRuleInfoArrayList(PublishmentSystemId);
                dgContents.ItemDataBound += dgContents_ItemDataBound;
                dgContents.DataBind();

                AddButton.Attributes.Add("onclick", ModalGovPublicIdentifierRuleAdd.GetOpenWindowStringToAdd(PublishmentSystemId));
            }
        }

        void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ruleInfo = e.Item.DataItem as GovPublicIdentifierRuleInfo;

                var ltlIndex = e.Item.FindControl("ltlIndex") as Literal;
                var ltlRuleName = e.Item.FindControl("ltlRuleName") as Literal;
                var ltlIdentifierType = e.Item.FindControl("ltlIdentifierType") as Literal;
                var ltlMinLength = e.Item.FindControl("ltlMinLength") as Literal;
                var ltlSuffix = e.Item.FindControl("ltlSuffix") as Literal;
                var hlUpLinkButton = e.Item.FindControl("hlUpLinkButton") as HyperLink;
                var hlDownLinkButton = e.Item.FindControl("hlDownLinkButton") as HyperLink;
                var ltlSettingUrl = e.Item.FindControl("ltlSettingUrl") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;
                var ltlDeleteUrl = e.Item.FindControl("ltlDeleteUrl") as Literal;

                ltlIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlRuleName.Text = ruleInfo.RuleName;
                ltlIdentifierType.Text = EGovPublicIdentifierTypeUtils.GetText(ruleInfo.IdentifierType);
                ltlMinLength.Text = ruleInfo.MinLength.ToString();
                ltlSuffix.Text = ruleInfo.Suffix;

                hlUpLinkButton.NavigateUrl = PageUtils.GetWcmUrl(nameof(PageGovPublicIdentifierRule),
                    new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"RuleID", ruleInfo.RuleID.ToString()},
                        {"Up", true.ToString()}
                    });

                hlDownLinkButton.NavigateUrl = PageUtils.GetWcmUrl(nameof(PageGovPublicIdentifierRule),
                    new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"RuleID", ruleInfo.RuleID.ToString()},
                        {"Down", true.ToString()}
                    });

                if (ruleInfo.IdentifierType == EGovPublicIdentifierType.Department)
                {
                    var urlSetting = PageGovPublicDepartment.GetRedirectUrl(PublishmentSystemId);
                    ltlSettingUrl.Text = $@"<a href=""{urlSetting}"">机构分类设置</a>";
                }
                else if (ruleInfo.IdentifierType == EGovPublicIdentifierType.Channel)
                {
                    ltlSettingUrl.Text = $@"<a href=""{PageGovPublicChannel.GetRedirectUrl(PublishmentSystemId)}"">主题分类设置</a>";
                }

                ltlEditUrl.Text =
                    $@"<a href='javascript:;' onclick=""{ModalGovPublicIdentifierRuleAdd.GetOpenWindowStringToEdit(
                        PublishmentSystemId, ruleInfo.RuleID)}"">编辑</a>";

                var urlDelete = PageUtils.GetWcmUrl(nameof(PageGovPublicIdentifierRule),
                    new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"RuleID", ruleInfo.RuleID.ToString()},
                        {"Delete", true.ToString()}
                    });
                ltlDeleteUrl.Text =
                    $@"<a href=""{urlDelete}"" onClick=""javascript:return confirm('此操作将删除规则“{ruleInfo.RuleName}”，确认吗？');"">删除</a>";
            }
        }
    }
}
