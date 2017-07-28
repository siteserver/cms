using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTagStyleMailSMS : BasePageCms
    {
        public RadioButtonList rblIsSMS;
        public PlaceHolder phSMS;
        public RadioButtonList rblSMSReceiver;
        public PlaceHolder phSMSTo;
        public TextBox tbSMSTo;
        public PlaceHolder phSMSFiledName;
        public DropDownList ddlSMSFiledName;
        public RadioButtonList rblIsSMSTemplate;
        public PlaceHolder phSMSTemplate;
        public TextBox tbSMSContent;
        public Literal ltlTips2;

        private int _styleId;
        private ETableStyle _tableStyle;
        private ITagStyleMailSMSBaseInfo _mailSmsInfo;

        public static string GetRedirectUrl(int publishmentSystemId, int styleId, ETableStyle tableStyle, int relatedIdentity)
        {
            return PageUtils.GetCmsUrl(nameof(PageTagStyleMailSMS), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"StyleID", styleId.ToString()},
                {"TableStyle", ETableStyleUtils.GetValue(tableStyle)},
                {"RelatedIdentity", relatedIdentity.ToString()}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _styleId = Body.GetQueryInt("StyleID");
            _tableStyle = ETableStyleUtils.GetEnumType(Body.GetQueryString("TableStyle"));
            var relatedIdentity = Body.GetQueryInt("RelatedIdentity");
            var tagStyleInfo = DataProvider.TagStyleDao.GetTagStyleInfo(_styleId);
            if (_tableStyle == ETableStyle.GovInteractContent)
            {
                _mailSmsInfo = new TagStyleGovInteractApplyInfo(tagStyleInfo.SettingsXML);
            }

			if (!IsPostBack)
			{
                ltlTips2.Text =
                    $"[{ContentAttribute.AddDate}]代表提交时间，[{GovInteractContentAttribute.QueryCode}]代表查询码，";

                var styleInfoList = RelatedIdentities.GetTableStyleInfoList(PublishmentSystemInfo, _tableStyle, relatedIdentity);
                foreach (var styleInfo in styleInfoList)
                {
                    if (styleInfo.IsVisible)
                    {
                        ltlTips2.Text += $@"[{styleInfo.AttributeName}]代表{styleInfo.DisplayName}，";
                    }
                }

                ltlTips2.Text = ltlTips2.Text.TrimEnd('，');

                //短信

                ControlUtils.SelectListItemsIgnoreCase(rblIsSMS, _mailSmsInfo.IsSMS.ToString());
                rblIsSMS_SelectedIndexChanged(null, EventArgs.Empty);

                ControlUtils.SelectListItemsIgnoreCase(rblSMSReceiver, ETriStateUtils.GetValue(_mailSmsInfo.SMSReceiver));
                rblSMSReceiver_SelectedIndexChanged(null, EventArgs.Empty);

                tbSMSTo.Text = _mailSmsInfo.SMSTo;

                foreach (var styleInfo in styleInfoList)
                {
                    if (styleInfo.IsVisible)
                    {
                        var listItem = new ListItem(styleInfo.DisplayName + "(" + styleInfo.AttributeName + ")", styleInfo.AttributeName);
                        if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, _mailSmsInfo.SMSFiledName))
                        {
                            listItem.Selected = true;
                        }
                        ddlSMSFiledName.Items.Add(listItem);
                    }
                }

                ControlUtils.SelectListItemsIgnoreCase(rblIsSMSTemplate, _mailSmsInfo.IsSMSTemplate.ToString());
                rblIsSMSTemplate_SelectedIndexChanged(null, EventArgs.Empty);

                tbSMSContent.Text = _mailSmsInfo.SMSContent;

                if (string.IsNullOrEmpty(tbSMSContent.Text))
                {
                    tbSMSContent.Text = MessageManager.GetSMSContent(styleInfoList);
                }
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                _mailSmsInfo.IsSMS = TranslateUtils.ToBool(rblIsSMS.SelectedValue);
                _mailSmsInfo.SMSReceiver = ETriStateUtils.GetEnumType(rblSMSReceiver.SelectedValue);
                _mailSmsInfo.SMSTo = tbSMSTo.Text;
                _mailSmsInfo.SMSFiledName = ddlSMSFiledName.SelectedValue;
                _mailSmsInfo.IsSMSTemplate = TranslateUtils.ToBool(rblIsSMSTemplate.SelectedValue);
                _mailSmsInfo.SMSContent = tbSMSContent.Text;

                try
                {
                    var tagStyleInfo = DataProvider.TagStyleDao.GetTagStyleInfo(_styleId);
                    tagStyleInfo.SettingsXML = _mailSmsInfo.ToString();
                    DataProvider.TagStyleDao.Update(tagStyleInfo);
                    SuccessMessage("邮件/短信发送设置修改成功！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "邮件/短信发送设置修改失败," + ex.Message);
                }
            }
        }

        public void rblSMSReceiver_SelectedIndexChanged(object sender, EventArgs e)
        {
            phSMSTo.Visible = phSMSFiledName.Visible = false;

            var smsReceiver = ETriStateUtils.GetEnumType(rblSMSReceiver.SelectedValue);
            if (smsReceiver == ETriState.True)
            {
                phSMSTo.Visible = true;
            }
            else if (smsReceiver == ETriState.False)
            {
                phSMSFiledName.Visible = true;
            }
            else if (smsReceiver == ETriState.All)
            {
                phSMSTo.Visible = phSMSFiledName.Visible = true;
            }
        }

        public void rblIsSMS_SelectedIndexChanged(object sender, EventArgs e)
        {
            phSMS.Visible = TranslateUtils.ToBool(rblIsSMS.SelectedValue);
        }

        public void rblIsSMSTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            phSMSTemplate.Visible = TranslateUtils.ToBool(rblIsSMSTemplate.SelectedValue);
        }
	}
}
