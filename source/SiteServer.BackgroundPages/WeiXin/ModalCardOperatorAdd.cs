using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalCardOperatorAdd : BasePageCms
    {
        public Literal LtlOperatorItems;

        private int _cardId;

        public static string GetOpenWindowStringToAdd(int publishmentSystemId, int cardId)
        {
            return PageUtils.GetOpenWindowString("会员卡操作员",
                PageUtils.GetWeiXinUrl(nameof(ModalCardOperatorAdd), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"cardId", cardId.ToString()}
                }), 450, 450);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _cardId = Body.GetQueryInt("cardID");

            if (!IsPostBack)
            {
                var cardInfo = DataProviderWx.CardDao.GetCardInfo(_cardId);

                var operatorInfoList = new List<CardOperatorInfo>();
                operatorInfoList = TranslateUtils.JsonToObject(cardInfo.ShopOperatorList, operatorInfoList) as List<CardOperatorInfo>;
                if (operatorInfoList != null)
                {
                    var operatorBuilder = new StringBuilder();
                    foreach (var operatorInfo in operatorInfoList)
                    {
                        operatorBuilder.AppendFormat(@"{{userName: '{0}', password: '{1}'}},", operatorInfo.UserName, operatorInfo.Password);
                    }
                    if (operatorBuilder.Length > 0) operatorBuilder.Length--;

                    LtlOperatorItems.Text =
                        $@"itemController.itemCount = {operatorInfoList.Count};itemController.items = [{operatorBuilder}];";
                }
                else
                {
                    LtlOperatorItems.Text = "itemController.itemCount = 0;itemController.items = [{}];";
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {

            if (IsPostBack && IsValid)
            {
                var errorMessage = string.Empty;

                try
                {
                    var itemCount = TranslateUtils.ToInt(Request.Form["itemCount"]);
                    var userNameList = TranslateUtils.StringCollectionToStringList(Request.Form["itemUserName"]);
                    var passwordList = TranslateUtils.StringCollectionToStringList(Request.Form["itemPassword"]);
                    var operatorInfoList = new List<CardOperatorInfo>();

                    if ( userNameList.Count ==0)
                    {
                       errorMessage= "保存失败,姓名为空！";
                    }
                    else if (passwordList.Count == 0)
                    {
                        errorMessage = "保存失败,密码为空！";
                    }
                    if (itemCount == userNameList.Count && itemCount == passwordList.Count)
                    {
                        for (var i = 0; i < itemCount; i++)
                        {
                            var userName = userNameList[i];
                            var password = passwordList[i];
                            if (string.IsNullOrEmpty(userName))
                            {
                                errorMessage = "保存失败,姓名为空！";
                                break;
                            }
                            if (string.IsNullOrEmpty(password))
                            {
                                errorMessage = "保存失败,密码为空！";
                                break;
                            }

                            var operatorInfo = new CardOperatorInfo { UserName = userName, Password = password };

                            operatorInfoList.Add(operatorInfo);
                        }
                    }

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        FailMessage(errorMessage);
                    }
                    else
                    {
                        var cardInfo = DataProviderWx.CardDao.GetCardInfo(_cardId);
                        cardInfo.ShopOperatorList = TranslateUtils.ObjectToJson(operatorInfoList);
                        DataProviderWx.CardDao.Update(cardInfo);

                        PageUtils.CloseModalPage(Page);
                    }
                }
                catch (Exception ex)
                {
                    FailMessage("保存失败"+ex);
                }
             }
        }
    }
}