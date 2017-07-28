using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.BackgroundPages.Wcm
{
	public class ModalGovPublicIdentifierRuleAdd : BasePageCms
	{
        public TextBox tbRuleName;
        public DropDownList ddlIdentifierType;
        public PlaceHolder phAttributeName;
        public DropDownList ddlAttributeName;
        public PlaceHolder phMinLength;
        public TextBox tbMinLength;
        public PlaceHolder phFormatString;
        public TextBox tbFormatString;
        public TextBox tbSuffix;
        public PlaceHolder phSequence;
        public TextBox tbSequence;
        public RadioButtonList rblIsSequenceChannelZero;
        public RadioButtonList rblIsSequenceDepartmentZero;
        public RadioButtonList rblIsSequenceYearZero;

        private int _ruleId;

        public static string GetOpenWindowStringToAdd(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowString("添加规则", PageUtils.GetWcmUrl(nameof(ModalGovPublicIdentifierRuleAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            }), 520, 460);
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemId, int ruleId)
        {
            return PageUtils.GetOpenWindowString("修改规则", PageUtils.GetWcmUrl(nameof(ModalGovPublicIdentifierRuleAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"RuleID", ruleId.ToString()}
            }), 520, 460);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _ruleId = TranslateUtils.ToInt(Request.QueryString["RuleID"]);

			if (!IsPostBack)
			{
                EGovPublicIdentifierTypeUtils.AddListItems(ddlIdentifierType);

                var styleInfoList = TableStyleManager.GetTableStyleInfoList(ETableStyle.GovPublicContent, PublishmentSystemInfo.AuxiliaryTableForGovPublic, null);
                foreach (var tableStyleInfo in styleInfoList)
                {
                    if (tableStyleInfo.AttributeName == ContentAttribute.Title || tableStyleInfo.AttributeName == GovPublicContentAttribute.Content || tableStyleInfo.AttributeName == GovPublicContentAttribute.DepartmentId || tableStyleInfo.AttributeName == GovPublicContentAttribute.Description || tableStyleInfo.AttributeName == GovPublicContentAttribute.ImageUrl || tableStyleInfo.AttributeName == GovPublicContentAttribute.FileUrl || tableStyleInfo.AttributeName == GovPublicContentAttribute.Identifier || tableStyleInfo.AttributeName == GovPublicContentAttribute.Keywords || tableStyleInfo.AttributeName == GovPublicContentAttribute.DocumentNo || tableStyleInfo.AttributeName == GovPublicContentAttribute.Publisher) continue;
                    ddlAttributeName.Items.Add(new ListItem(tableStyleInfo.DisplayName + "(" + tableStyleInfo.AttributeName + ")", tableStyleInfo.AttributeName));
                }
                EBooleanUtils.AddListItems(rblIsSequenceChannelZero);
                EBooleanUtils.AddListItems(rblIsSequenceDepartmentZero);
                EBooleanUtils.AddListItems(rblIsSequenceYearZero);

                ControlUtils.SelectListItemsIgnoreCase(rblIsSequenceChannelZero, true.ToString());
                ControlUtils.SelectListItemsIgnoreCase(rblIsSequenceDepartmentZero, false.ToString());
                ControlUtils.SelectListItemsIgnoreCase(rblIsSequenceYearZero, true.ToString());

                if (_ruleId > 0)
                {
                    var ruleInfo = DataProvider.GovPublicIdentifierRuleDao.GetIdentifierRuleInfo(_ruleId);
                    if (ruleInfo != null)
                    {
                        tbRuleName.Text = ruleInfo.RuleName;
                        ControlUtils.SelectListItems(ddlIdentifierType, EGovPublicIdentifierTypeUtils.GetValue(ruleInfo.IdentifierType));
                        ControlUtils.SelectListItems(ddlAttributeName, ruleInfo.AttributeName);
                        tbMinLength.Text = ruleInfo.MinLength.ToString();
                        tbFormatString.Text = ruleInfo.FormatString;
                        tbSuffix.Text = ruleInfo.Suffix;
                        tbSequence.Text = ruleInfo.Sequence.ToString();

                        ControlUtils.SelectListItemsIgnoreCase(rblIsSequenceChannelZero, ruleInfo.Additional.IsSequenceChannelZero.ToString());
                        ControlUtils.SelectListItemsIgnoreCase(rblIsSequenceDepartmentZero, ruleInfo.Additional.IsSequenceDepartmentZero.ToString());
                        ControlUtils.SelectListItemsIgnoreCase(rblIsSequenceYearZero, ruleInfo.Additional.IsSequenceYearZero.ToString());
                    }
                }

                ddlIdentifierType.SelectedIndexChanged += ddlIdentifierType_SelectedIndexChanged;
                ddlIdentifierType_SelectedIndexChanged(null, EventArgs.Empty);
			}
		}

        public void ddlIdentifierType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var identifierType = EGovPublicIdentifierTypeUtils.GetEnumType(ddlIdentifierType.SelectedValue);
            if (identifierType == EGovPublicIdentifierType.Department || identifierType == EGovPublicIdentifierType.Channel)
            {
                phAttributeName.Visible = false;
                phFormatString.Visible = false;
                phMinLength.Visible = true;
                phSequence.Visible = false;
            }
            else if (identifierType == EGovPublicIdentifierType.Sequence)
            {
                phAttributeName.Visible = false;
                phFormatString.Visible = false;
                phMinLength.Visible = true;
                phSequence.Visible = true;
            }
            else if (identifierType == EGovPublicIdentifierType.Attribute)
            {
                phAttributeName.Visible = true;
                phFormatString.Visible = true;
                phMinLength.Visible = true;
                phSequence.Visible = false;
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;
            var ruleInfoArrayList = DataProvider.GovPublicIdentifierRuleDao.GetRuleInfoArrayList(PublishmentSystemId);
				
			if (_ruleId > 0)
			{
				try
				{
                    var ruleInfo = DataProvider.GovPublicIdentifierRuleDao.GetIdentifierRuleInfo(_ruleId);
                    ruleInfo.RuleName = tbRuleName.Text;
                    ruleInfo.IdentifierType = EGovPublicIdentifierTypeUtils.GetEnumType(ddlIdentifierType.SelectedValue);
                    ruleInfo.MinLength = TranslateUtils.ToInt(tbMinLength.Text);
                    ruleInfo.Suffix = tbSuffix.Text;
                    ruleInfo.FormatString = tbFormatString.Text;
                    ruleInfo.AttributeName = ddlAttributeName.SelectedValue;
                    ruleInfo.Sequence = TranslateUtils.ToInt(tbSequence.Text);

                    if (ruleInfo.IdentifierType == EGovPublicIdentifierType.Sequence)
                    {
                        ruleInfo.Additional.IsSequenceChannelZero = TranslateUtils.ToBool(rblIsSequenceChannelZero.SelectedValue);
                        ruleInfo.Additional.IsSequenceDepartmentZero = TranslateUtils.ToBool(rblIsSequenceDepartmentZero.SelectedValue);
                        ruleInfo.Additional.IsSequenceYearZero = TranslateUtils.ToBool(rblIsSequenceYearZero.SelectedValue);
                    }

                    foreach (GovPublicIdentifierRuleInfo identifierRuleInfo in ruleInfoArrayList)
                    {
                        if (identifierRuleInfo.RuleID == ruleInfo.RuleID) continue;
                        if (identifierRuleInfo.IdentifierType != EGovPublicIdentifierType.Attribute && identifierRuleInfo.IdentifierType == ruleInfo.IdentifierType)
                        {
                            FailMessage("规则修改失败，本类型规则只能添加一次！");
                            return;
                        }
                        if (identifierRuleInfo.RuleName == tbRuleName.Text)
                        {
                            FailMessage("规则修改失败，规则名称已存在！");
                            return;
                        }
                    }

                    DataProvider.GovPublicIdentifierRuleDao.Update(ruleInfo);

                    Body.AddSiteLog(PublishmentSystemId, "修改规则", $"规则:{ruleInfo.RuleName}");

					isChanged = true;
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "规则修改失败！");
				}
			}
			else
			{
                var identifierType = EGovPublicIdentifierTypeUtils.GetEnumType(ddlIdentifierType.SelectedValue);

                foreach (GovPublicIdentifierRuleInfo ruleInfo in ruleInfoArrayList)
                {
                    if (ruleInfo.IdentifierType != EGovPublicIdentifierType.Attribute && ruleInfo.IdentifierType == identifierType)
                    {
                        FailMessage("规则添加失败，本类型规则只能添加一次！");
                        return;
                    }
                    if (ruleInfo.RuleName == tbRuleName.Text)
                    {
                        FailMessage("规则添加失败，规则名称已存在！");
                        return;
                    }
                }

                try
                {
                    var ruleInfo = new GovPublicIdentifierRuleInfo(0, tbRuleName.Text, PublishmentSystemId, identifierType, TranslateUtils.ToInt(tbMinLength.Text), tbSuffix.Text, tbFormatString.Text, ddlAttributeName.SelectedValue, TranslateUtils.ToInt(tbSequence.Text), 0, string.Empty);

                    if (ruleInfo.IdentifierType == EGovPublicIdentifierType.Sequence)
                    {
                        ruleInfo.Additional.IsSequenceChannelZero = TranslateUtils.ToBool(rblIsSequenceChannelZero.SelectedValue);
                        ruleInfo.Additional.IsSequenceDepartmentZero = TranslateUtils.ToBool(rblIsSequenceDepartmentZero.SelectedValue);
                        ruleInfo.Additional.IsSequenceYearZero = TranslateUtils.ToBool(rblIsSequenceYearZero.SelectedValue);
                    }

                    DataProvider.GovPublicIdentifierRuleDao.Insert(ruleInfo);

                    Body.AddSiteLog(PublishmentSystemId, "添加规则", $"规则:{ruleInfo.RuleName}");

                    isChanged = true;
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "规则添加失败！");
                }
			}

			if (isChanged)
			{
                PageUtils.CloseModalPageAndRedirect(Page, PageGovPublicIdentifierRule.GetRedirectUrl(PublishmentSystemId));
			}
		}
	}
}
