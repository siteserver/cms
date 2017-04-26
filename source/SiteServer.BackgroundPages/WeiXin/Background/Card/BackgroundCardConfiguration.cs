using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Core;
using System.Collections;
using System.Text;
using BaiRong.Core.Model.Enumerations;


namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundCardConfiguration : BackgroundBasePageWX
	{
        public RadioButtonList IsClaimCardCredits;
	    public Control ClaimCardCreditsRow;
        public TextBox tbClaimCardCredits;

        public RadioButtonList IsGiveConsumeCredits;
        public Control GiveConsumeCreditsRow;
        public TextBox tbConsumeAmount;
        public TextBox tbGivCredits;
        public RadioButtonList IsBinding;
        public RadioButtonList IsExchange;
        public Control ExchangeProportionRow;
        public TextBox tbExchangeProportion;

        public RadioButtonList IsSign;
        public Control SignCreditsRow;
        public Literal ltlScript;
        private ArrayList configureInfoArrayList = new ArrayList();

        public string GetSignDayFrom(int itemIndex)
        {
            if (configureInfoArrayList == null || configureInfoArrayList.Count <= itemIndex) return string.Empty;
            var configureInfo = configureInfoArrayList[itemIndex] as string;
            if (!configureInfo.Contains("&")) return string.Empty;
            var signDayFrom = configureInfo.Split('&')[0];
            return signDayFrom;
        }
        public string GetSignDayTo(int itemIndex)
        {
            if (configureInfoArrayList == null || configureInfoArrayList.Count <= itemIndex) return string.Empty;
            var configureInfo = configureInfoArrayList[itemIndex] as string;
            if (!configureInfo.Contains("&")) return string.Empty;
            var signDayTo = configureInfo.Split('&')[1];
            return signDayTo;
        }

        public string GetSignCredits(int itemIndex)
        {
            if (configureInfoArrayList == null || configureInfoArrayList.Count <= itemIndex) return string.Empty;
            var configureInfo = configureInfoArrayList[itemIndex] as string;
            if (!configureInfo.Contains("&")) return string.Empty;
            var signCredits = configureInfo.Split('&')[2];
            return signCredits;
        }
		 
		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (GetQueryString("successMessage") != null)
            {
                SuccessMessage("会员卡设置修改成功！");
            }
         
			if (!IsPostBack)
            {
               
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Card, "会员卡设置", AppManager.WeiXin.Permission.WebSite.Card);
                EBooleanUtils.AddListItems(IsClaimCardCredits, "是", "否");
                ControlUtils.SelectListItemsIgnoreCase(IsClaimCardCredits, PublishmentSystemInfo.Additional.Card_IsClaimCardCredits.ToString());
                tbClaimCardCredits.Text = PublishmentSystemInfo.Additional.Card_ClaimCardCredits.ToString();

                EBooleanUtils.AddListItems(IsGiveConsumeCredits, "是", "否");
                ControlUtils.SelectListItemsIgnoreCase(IsGiveConsumeCredits, PublishmentSystemInfo.Additional.Card_IsGiveConsumeCredits.ToString());

                tbConsumeAmount.Text = PublishmentSystemInfo.Additional.Card_ConsumeAmount.ToString();
                tbGivCredits.Text = PublishmentSystemInfo.Additional.Card_GiveCredits.ToString();
                EBooleanUtils.AddListItems(IsBinding, "是", "否");
                ControlUtils.SelectListItemsIgnoreCase(IsBinding, PublishmentSystemInfo.Additional.Card_IsBinding.ToString());
                EBooleanUtils.AddListItems(IsExchange, "是", "否");
                ControlUtils.SelectListItemsIgnoreCase(IsExchange, PublishmentSystemInfo.Additional.Card_IsExchange.ToString());
                tbExchangeProportion.Text = PublishmentSystemInfo.Additional.Card_ExchangeProportion.ToString();

                EBooleanUtils.AddListItems(IsSign, "是", "否");
                ControlUtils.SelectListItemsIgnoreCase(IsSign, PublishmentSystemInfo.Additional.Card_IsSign.ToString());
                 
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

            var signCreditsConfigure = PublishmentSystemInfo.Additional.Card_SignCreditsConfigure;
            if (!string.IsNullOrEmpty(signCreditsConfigure))
            {
                configureInfoArrayList = TranslateUtils.StringCollectionToArrayList(signCreditsConfigure);
            }

            var script = string.Empty;

            for (var i = 2; i < configureInfoArrayList.Count; i++)
            {
                var configureInfo = configureInfoArrayList[i] as string;
                if (string.IsNullOrEmpty(configureInfo)) continue;
                script +=
                    $"addItem('{configureInfo.Split('&')[0]}','{configureInfo.Split('&')[1]}','{configureInfo.Split('&')[2]}');";
            }
            if (!string.IsNullOrEmpty(script))
            {
                ltlScript.Text = $@"<script>{script}</script>";
            }
		}
         
        public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                PublishmentSystemInfo.Additional.Card_IsClaimCardCredits = TranslateUtils.ToBool(IsClaimCardCredits.SelectedValue);
                PublishmentSystemInfo.Additional.Card_ClaimCardCredits = TranslateUtils.ToInt(tbClaimCardCredits.Text);

                PublishmentSystemInfo.Additional.Card_IsGiveConsumeCredits = TranslateUtils.ToBool(IsGiveConsumeCredits.SelectedValue);
                PublishmentSystemInfo.Additional.Card_ConsumeAmount = TranslateUtils.ToDecimal(tbConsumeAmount.Text);
                PublishmentSystemInfo.Additional.Card_GiveCredits = TranslateUtils.ToInt(tbGivCredits.Text);
                PublishmentSystemInfo.Additional.Card_IsBinding = TranslateUtils.ToBool(IsBinding.SelectedValue);
                PublishmentSystemInfo.Additional.Card_IsExchange = TranslateUtils.ToBool(IsExchange.SelectedValue);
                PublishmentSystemInfo.Additional.Card_ExchangeProportion = TranslateUtils.ToDecimal(tbExchangeProportion.Text);

                PublishmentSystemInfo.Additional.Card_IsSign = TranslateUtils.ToBool(IsSign.SelectedValue);
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

                PublishmentSystemInfo.Additional.Card_SignCreditsConfigure = signCreditsConfigure.ToString(); ;
				
				try
				{
                    DataProvider.PublishmentSystemDAO.Update(PublishmentSystemInfo);
                    StringUtility.AddLog(PublishmentSystemID, "修改会员卡设置");
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
