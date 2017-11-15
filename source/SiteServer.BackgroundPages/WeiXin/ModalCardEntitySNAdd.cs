using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalCardEntitySnAdd : BasePageCms
    {
        public TextBox TbCardSn;
        public TextBox TbUserName;
        public TextBox TbAmount;
        public TextBox TbCredits;
        public TextBox TbEmail;
        public TextBox TbMobile;
        public TextBox TbAddress;

        private int _cardId;
        private int _cardEntitySnid;

        public static string GetOpenWindowStringToAdd(int publishmentSystemId, int cardId, int cardEntitySnid)
        {
            return PageUtils.GetOpenWindowString("新增实体卡",
                PageUtils.GetWeiXinUrl(nameof(ModalCardEntitySnAdd), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"cardId", cardId.ToString()},
                    {"cardEntitySnid", cardEntitySnid.ToString()}
                }), 450, 450);
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemId, int cardId, int cardEntitySnid)
        {
            return PageUtils.GetOpenWindowString("编辑实体卡",
                   PageUtils.GetWeiXinUrl(nameof(ModalCardEntitySnAdd), new NameValueCollection
                   {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"cardId", cardId.ToString()},
                    {"cardEntitySnid", cardEntitySnid.ToString()}
                   }), 450, 450);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _cardId = Body.GetQueryInt("cardID");
            _cardEntitySnid = Body.GetQueryInt("cardEntitySNID");

            if (!IsPostBack)
            {

                if (_cardEntitySnid > 0)
                {
                    var cardEntitySnInfo = DataProviderWx.CardEntitySnDao.GetCardEntitySnInfo(_cardEntitySnid);

                    if (cardEntitySnInfo != null)
                    {
                        TbCardSn.Text = cardEntitySnInfo.Sn;
                        TbCardSn.Enabled = false;
                        TbUserName.Text = cardEntitySnInfo.UserName;
                        TbAmount.Text = cardEntitySnInfo.Amount.ToString();
                        TbCredits.Text = cardEntitySnInfo.Credits.ToString();
                        TbEmail.Text = cardEntitySnInfo.Email;
                        TbMobile.Text = cardEntitySnInfo.Mobile;
                        TbAddress.Text = cardEntitySnInfo.Address;
                    }
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {

            if (IsPostBack && IsValid)
            {
                var cardEntitySnInfo = new CardEntitySnInfo();

                if (_cardEntitySnid > 0)
                {
                    cardEntitySnInfo = DataProviderWx.CardEntitySnDao.GetCardEntitySnInfo(_cardEntitySnid);
                }

                cardEntitySnInfo.PublishmentSystemId = PublishmentSystemId;
                cardEntitySnInfo.CardId = _cardId;
                cardEntitySnInfo.Sn = TbCardSn.Text;
                cardEntitySnInfo.UserName = TbUserName.Text;
                cardEntitySnInfo.Amount = TranslateUtils.ToDecimal(TbAmount.Text);
                cardEntitySnInfo.Credits = TranslateUtils.ToInt(TbCredits.Text);
                cardEntitySnInfo.Mobile = TbMobile.Text;
                cardEntitySnInfo.Email = TbEmail.Text;
                cardEntitySnInfo.Address = TbAddress.Text;

                if (_cardEntitySnid > 0)
                {
                    try
                    {
                        DataProviderWx.CardEntitySnDao.Update(cardEntitySnInfo);

                        PageUtils.CloseModalPage(Page);
                    }
                    catch (Exception ex)
                    {
                        FailMessage($"实体卡修改失败：{ex}");
                    }
                }
                else
                {
                    var isExist = DataProviderWx.CardEntitySnDao.IsExist(PublishmentSystemId, _cardId, cardEntitySnInfo.Sn);
                    var isExistMobile = DataProviderWx.CardEntitySnDao.IsExistMobile(PublishmentSystemId, _cardId, cardEntitySnInfo.Mobile);
                   
                    if (isExistMobile)
                    {
                        FailMessage("手机号使用，请更换手机号！");
                        return;
                    }

                    if (isExist)
                    {
                        FailMessage($"{cardEntitySnInfo.Sn}实体卡号已存在！");
                        return;
                    }
                    else
                    { 
                        try
                        {
                            cardEntitySnInfo.IsBinding = false;
                            cardEntitySnInfo.AddDate = DateTime.Now;
                            DataProviderWx.CardEntitySnDao.Insert(cardEntitySnInfo);

                            PageUtils.CloseModalPage(Page);
                        }
                        catch (Exception ex)
                        {
                            FailMessage($"实体卡新增失败：{ex}");
                        }
                    }
                }
            }
        }
    }
}