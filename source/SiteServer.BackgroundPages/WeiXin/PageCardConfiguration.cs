using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageCardConfiguration : BasePageCms
    {
        public RadioButtonList IsClaimCardCredits;
	    public Control ClaimCardCreditsRow;
        public TextBox TbClaimCardCredits;

        public RadioButtonList IsGiveConsumeCredits;
        public Control GiveConsumeCreditsRow;
        public TextBox TbConsumeAmount;
        public TextBox TbGivCredits;
        public RadioButtonList IsBinding;
        public RadioButtonList IsExchange;
        public Control ExchangeProportionRow;
        public TextBox TbExchangeProportion;

        public RadioButtonList IsSign;
        public Control SignCreditsRow;
        public Literal LtlScript;
        private List<string> _configureList = new List<string>();

        public string GetSignDayFrom(int itemIndex)
        {
            if (_configureList == null || _configureList.Count <= itemIndex) return string.Empty;
            var configureInfo = _configureList[itemIndex] as string;
            if (!configureInfo.Contains("&")) return string.Empty;
            var signDayFrom = configureInfo.Split('&')[0];
            return signDayFrom;
        }
        public string GetSignDayTo(int itemIndex)
        {
            if (_configureList == null || _configureList.Count <= itemIndex) return string.Empty;
            var configureInfo = _configureList[itemIndex] as string;
            if (!configureInfo.Contains("&")) return string.Empty;
            var signDayTo = configureInfo.Split('&')[1];
            return signDayTo;
        }

        public string GetSignCredits(int itemIndex)
        {
            if (_configureList == null || _configureList.Count <= itemIndex) return string.Empty;
            var configureInfo = _configureList[itemIndex] as string;
            if (!configureInfo.Contains("&")) return string.Empty;
            var signCredits = configureInfo.Split('&')[2];
            return signCredits;
        }
		 
		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			PageUtils.CheckRequestParameter("PublishmentSystemId");

            if (Body.GetQueryString("successMessage") != null)
            {
                SuccessMessage("会员卡设置修改成功！");
            }
         
			if (!IsPostBack)
            {
               
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdCard, "会员卡设置", AppManager.WeiXin.Permission.WebSite.Card);
                EBooleanUtils.AddListItems(IsClaimCardCredits, "是", "否");
                ControlUtils.SelectListItemsIgnoreCase(IsClaimCardCredits, PublishmentSystemInfo.Additional.WxCardIsClaimCardCredits.ToString());
                TbClaimCardCredits.Text = PublishmentSystemInfo.Additional.WxCardClaimCardCredits.ToString();

                EBooleanUtils.AddListItems(IsGiveConsumeCredits, "是", "否");
                ControlUtils.SelectListItemsIgnoreCase(IsGiveConsumeCredits, PublishmentSystemInfo.Additional.WxCardIsGiveConsumeCredits.ToString());

                TbConsumeAmount.Text = PublishmentSystemInfo.Additional.WxCardConsumeAmount.ToString();
                TbGivCredits.Text = PublishmentSystemInfo.Additional.WxCardGiveCredits.ToString();
                EBooleanUtils.AddListItems(IsBinding, "是", "否");
                ControlUtils.SelectListItemsIgnoreCase(IsBinding, PublishmentSystemInfo.Additional.WxCardIsBinding.ToString());
                EBooleanUtils.AddListItems(IsExchange, "是", "否");
                ControlUtils.SelectListItemsIgnoreCase(IsExchange, PublishmentSystemInfo.Additional.WxCardIsExchange.ToString());
                TbExchangeProportion.Text = PublishmentSystemInfo.Additional.WxCardExchangeProportion.ToString();

                EBooleanUtils.AddListItems(IsSign, "是", "否");
                ControlUtils.SelectListItemsIgnoreCase(IsSign, PublishmentSystemInfo.Additional.WxCardIsSign.ToString());
                 
                if (TranslateUtils.ToBool(IsClaimCardCredits.SelectedValue))
                {
                    ClaimCardCreditsRow.Visible = true;
                }
                else
                {
                    ClaimCardCreditsRow.Visible = false;
                }

                if (TranslateUtils.ToBool(IsGiveConsumeCredits.SelectedValue))
                {
                    GiveConsumeCreditsRow.Visible = true;
                }
                else
                {
                    GiveConsumeCreditsRow.Visible = false;
                }

                if (TranslateUtils.ToBool(IsExchange.SelectedValue))
                {
                    ExchangeProportionRow.Visible = true;
                }
                else
                {
                    ExchangeProportionRow.Visible = false;
                }
                if (TranslateUtils.ToBool(IsSign.SelectedValue))
                {
                    SignCreditsRow.Visible = true;
                }
                else
                {
                    SignCreditsRow.Visible = false;
                }
                 
			}

            var signCreditsConfigure = PublishmentSystemInfo.Additional.WxCardSignCreditsConfigure;
            if (!string.IsNullOrEmpty(signCreditsConfigure))
            {
                _configureList = TranslateUtils.StringCollectionToStringList(signCreditsConfigure);
            }

            var script = string.Empty;

            for (var i = 2; i < _configureList.Count; i++)
            {
                var configureInfo = _configureList[i] as string;
                if (string.IsNullOrEmpty(configureInfo)) continue;
                script +=
                    $"addItem('{configureInfo.Split('&')[0]}','{configureInfo.Split('&')[1]}','{configureInfo.Split('&')[2]}');";
            }
            if (!string.IsNullOrEmpty(script))
            {
                LtlScript.Text = $@"<script>{script}</script>";
            }
		}
         
        public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                PublishmentSystemInfo.Additional.WxCardIsClaimCardCredits = TranslateUtils.ToBool(IsClaimCardCredits.SelectedValue);
                PublishmentSystemInfo.Additional.WxCardClaimCardCredits = TranslateUtils.ToInt(TbClaimCardCredits.Text);

                PublishmentSystemInfo.Additional.WxCardIsGiveConsumeCredits = TranslateUtils.ToBool(IsGiveConsumeCredits.SelectedValue);
                PublishmentSystemInfo.Additional.WxCardConsumeAmount = TranslateUtils.ToDecimal(TbConsumeAmount.Text);
                PublishmentSystemInfo.Additional.WxCardGiveCredits = TranslateUtils.ToInt(TbGivCredits.Text);
                PublishmentSystemInfo.Additional.WxCardIsBinding = TranslateUtils.ToBool(IsBinding.SelectedValue);
                PublishmentSystemInfo.Additional.WxCardIsExchange = TranslateUtils.ToBool(IsExchange.SelectedValue);
                PublishmentSystemInfo.Additional.WxCardExchangeProportion = TranslateUtils.ToDecimal(TbExchangeProportion.Text);

                PublishmentSystemInfo.Additional.WxCardIsSign = TranslateUtils.ToBool(IsSign.SelectedValue);
                var itemCount = TranslateUtils.ToInt(Request.Form["itemCount"]);
                var signCreditsConfigure = new StringBuilder();
                for (var i = 0; i < itemCount; i++)
                {
                    var optionConfigure = string.Empty;
                    var dayFrom = Request.Form["optionsDayFrom[" + i + "]"];
                    var dayTo = Request.Form["optionsDayTo[" + i + "]"];
                    var credits = Request.Form["optionsSignCredits[" + i + "]"];
                    if (!string.IsNullOrEmpty(dayFrom) && !string.IsNullOrEmpty(dayTo) && !string.IsNullOrEmpty(credits))
                    {
                        optionConfigure = $"{dayFrom}&{dayTo}&{credits}";
                    }
                    signCreditsConfigure.AppendFormat("{0},", optionConfigure);
                }

                PublishmentSystemInfo.Additional.WxCardSignCreditsConfigure = signCreditsConfigure.ToString(); ;
				
				try
				{
                    DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);
                    Body.AddSiteLog(PublishmentSystemId, "修改会员卡设置");
                    SuccessMessage("会员卡设置修改成功！");
                }
				catch(Exception ex)
				{
                    FailMessage(ex, "会员卡设置修改失败！");
                }
			}
		}

		public void  Refrush(object sender, EventArgs e)
		{
            if (TranslateUtils.ToBool(IsClaimCardCredits.SelectedValue))
            {
                ClaimCardCreditsRow.Visible = true;
            }
            else
            {
                ClaimCardCreditsRow.Visible = false;
            }

            if (TranslateUtils.ToBool(IsGiveConsumeCredits.SelectedValue))
            {
                GiveConsumeCreditsRow.Visible = true;
            }
            else
            {
                GiveConsumeCreditsRow.Visible = false;
            }
            if (TranslateUtils.ToBool(IsExchange.SelectedValue))
            {
                ExchangeProportionRow.Visible = true;
            }
            else
            {
                ExchangeProportionRow.Visible = false;
            }
            if (TranslateUtils.ToBool(IsSign.SelectedValue))
            {
                SignCreditsRow.Visible = true;
            }
            else
            {
                SignCreditsRow.Visible = false;
            }
		}
	}
}
