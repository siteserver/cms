using System;
using System.Web.UI.WebControls;

using BaiRong.Core;
using System.Collections.Specialized;
using SiteServer.CMS.BackgroundPages;
using SiteServer.WeiXin.Model;
using SiteServer.WeiXin.Core;
using System.Collections.Generic;
using System.Text;


namespace SiteServer.WeiXin.BackgroundPages.Modal
{
    public class CardOperatorAdd : BackgroundBasePage
    {
        public Literal ltlOperatorItems;

        private int cardID;

        public static string GetOpenWindowStringToAdd(int publishmentSystemID, int cardID)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            arguments.Add("cardID", cardID.ToString());
            return PageUtilityWX.GetOpenWindowString("会员卡操作员", "modal_cardOperatorAdd.aspx", arguments, 450, 450);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            cardID = TranslateUtils.ToInt(GetQueryString("cardID"));

            if (!IsPostBack)
            {
                var cardInfo = DataProviderWX.CardDAO.GetCardInfo(cardID);

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

                    ltlOperatorItems.Text =
                        $@"itemController.itemCount = {operatorInfoList.Count};itemController.items = [{operatorBuilder
                            .ToString()}];";
                }
                else
                {
                    ltlOperatorItems.Text = "itemController.itemCount = 0;itemController.items = [{}];";
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
                        var cardInfo = DataProviderWX.CardDAO.GetCardInfo(cardID);
                        cardInfo.ShopOperatorList = TranslateUtils.ObjectToJson(operatorInfoList);
                        DataProviderWX.CardDAO.Update(cardInfo);

                        JsUtils.OpenWindow.CloseModalPage(Page);
                    }
                }
                catch (Exception ex)
                {
                    FailMessage("保存失败"+ex.ToString());
                }
             }
        }
    }
}