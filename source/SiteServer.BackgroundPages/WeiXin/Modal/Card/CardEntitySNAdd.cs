using System;
using System.Web.UI.WebControls;

using BaiRong.Core;
using System.Collections.Specialized;
using SiteServer.CMS.BackgroundPages;
using SiteServer.WeiXin.Model;
using SiteServer.WeiXin.Core;


namespace SiteServer.WeiXin.BackgroundPages.Modal
{
    public class CardEntitySNAdd : BackgroundBasePage
    {
        public TextBox tbCardSN;
        public TextBox tbUserName;
        public TextBox tbAmount;
        public TextBox tbCredits;
        public TextBox tbEmail;
        public TextBox tbMobile;
        public TextBox tbAddress;

        private int cardID;
        private int cardEntitySNID;

        public static string GetOpenWindowStringToAdd(int publishmentSystemID, int cardID, int cardEntitySNID)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            arguments.Add("cardID", cardID.ToString());
            arguments.Add("cardEntitySNID", cardEntitySNID.ToString());
            return PageUtilityWX.GetOpenWindowString("新增实体卡", "modal_cardEntitySNAdd.aspx", arguments, 450, 450);
        }
        public static string GetOpenWindowStringToEdit(int publishmentSystemID, int cardID, int cardEntitySNID)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            arguments.Add("cardID", cardID.ToString());
            arguments.Add("cardEntitySNID", cardEntitySNID.ToString());
            return PageUtilityWX.GetOpenWindowString("编辑实体卡", "modal_cardEntitySNAdd.aspx", arguments, 450, 450);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            cardID = TranslateUtils.ToInt(GetQueryString("cardID"));
            cardEntitySNID = TranslateUtils.ToInt(GetQueryString("cardEntitySNID"));

            if (!IsPostBack)
            {

                if (cardEntitySNID > 0)
                {
                    var cardEntitySNInfo = DataProviderWX.CardEntitySNDAO.GetCardEntitySNInfo(cardEntitySNID);

                    if (cardEntitySNInfo != null)
                    {
                        tbCardSN.Text = cardEntitySNInfo.SN;
                        tbCardSN.Enabled = false;
                        tbUserName.Text = cardEntitySNInfo.UserName;
                        tbAmount.Text = cardEntitySNInfo.Amount.ToString();
                        tbCredits.Text = cardEntitySNInfo.Credits.ToString();
                        tbEmail.Text = cardEntitySNInfo.Email;
                        tbMobile.Text = cardEntitySNInfo.Mobile;
                        tbAddress.Text = cardEntitySNInfo.Address;
                    }
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {

            if (IsPostBack && IsValid)
            {
                var cardEntitySNInfo = new CardEntitySNInfo();

                if (cardEntitySNID > 0)
                {
                    cardEntitySNInfo = DataProviderWX.CardEntitySNDAO.GetCardEntitySNInfo(cardEntitySNID);
                }

                cardEntitySNInfo.PublishmentSystemID = PublishmentSystemID;
                cardEntitySNInfo.CardID = cardID;
                cardEntitySNInfo.SN = tbCardSN.Text;
                cardEntitySNInfo.UserName = tbUserName.Text;
                cardEntitySNInfo.Amount = TranslateUtils.ToDecimal(tbAmount.Text);
                cardEntitySNInfo.Credits = TranslateUtils.ToInt(tbCredits.Text);
                cardEntitySNInfo.Mobile = tbMobile.Text;
                cardEntitySNInfo.Email = tbEmail.Text;
                cardEntitySNInfo.Address = tbAddress.Text;

                if (cardEntitySNID > 0)
                {
                    try
                    {
                        DataProviderWX.CardEntitySNDAO.Update(cardEntitySNInfo);

                        JsUtils.OpenWindow.CloseModalPage(Page);
                    }
                    catch (Exception ex)
                    {
                        FailMessage($"实体卡修改失败：{ex.ToString()}");
                    }
                }
                else
                {
                    var isExist = DataProviderWX.CardEntitySNDAO.IsExist(PublishmentSystemID, cardID, cardEntitySNInfo.SN);
                    var isExistMobile = DataProviderWX.CardEntitySNDAO.IsExistMobile(PublishmentSystemID, cardID, cardEntitySNInfo.Mobile);
                   
                    if (isExistMobile)
                    {
                        FailMessage("手机号使用，请更换手机号！");
                        return;
                    }

                    if (isExist)
                    {
                        FailMessage($"{cardEntitySNInfo.SN}实体卡号已存在！");
                        return;
                    }
                    else
                    { 
                        try
                        {
                            cardEntitySNInfo.IsBinding = false;
                            cardEntitySNInfo.AddDate = DateTime.Now;
                            DataProviderWX.CardEntitySNDAO.Insert(cardEntitySNInfo);

                            JsUtils.OpenWindow.CloseModalPage(Page);
                        }
                        catch (Exception ex)
                        {
                            FailMessage($"实体卡新增失败：{ex.ToString()}");
                        }
                    }
                }
            }
        }
    }
}