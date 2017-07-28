using System;
using System.Collections;
using System.Collections.Specialized;

using BaiRong.Core;
using SiteServer.CMS.BackgroundPages;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;
using System.Web.UI.HtmlControls;
using BaiRong.Core.IO;

namespace SiteServer.WeiXin.BackgroundPages.Modal
{
    public class CardEntitySNImport : BackgroundBasePage
    { 
        public HtmlInputFile hifUpload;

        private int cardID;
       
        public static string GetOpenUploadWindowString(int publishmentSystemID, int cardID )
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            arguments.Add("cardID", cardID.ToString());
            
            return PageUtilityWX.GetOpenWindowString("导入实体卡", "modal_cardEntitySNImport.aspx", arguments, 400, 300);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            cardID = TranslateUtils.ToInt(GetQueryString("cardID"));
           
            if (!IsPostBack)
            {
                
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {
                var filePath = PathUtils.GetTemporaryFilesPath("cardsn-example.csv");
                FileUtils.DeleteFileIfExists(filePath);
                hifUpload.PostedFile.SaveAs(filePath);

                try
                { 
                    var cardEntitySNInfoArrayList = GetCardEntitySNInfoArrayList(filePath);
                    if (cardEntitySNInfoArrayList.Count > 0)
                    {
                        isChanged = true;
                        var errorMessage = string.Empty;
                        for (var i = 0; i < cardEntitySNInfoArrayList.Count; i++)
                        {
                            var cardEntitySNInfo = cardEntitySNInfoArrayList[i] as CardEntitySNInfo;

                            var isExist=DataProviderWX.CardEntitySNDAO.IsExist(PublishmentSystemID,cardID,cardEntitySNInfo.SN);
                            if (!isExist)
                            {
                                DataProviderWX.CardEntitySNDAO.Insert(cardEntitySNInfo);
                            }
                       }
                    }
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "失败：" + ex.Message);
                }

            }
            catch (Exception ex)
            {
                FailMessage(ex, "失败：" + ex.Message);
            }

            if (isChanged)
            {
                JsUtils.OpenWindow.CloseModalPage(Page);
            }
        }

        public ArrayList GetCardEntitySNInfoArrayList(string filePath)
        {
            var index = 0;
            var cardEntitySNInfoArrayList = new ArrayList();

            var cardEntitySNInfoList = CSVUtils.Import(filePath);
            if (cardEntitySNInfoList.Count > 0)
            {
                foreach (var info in cardEntitySNInfoList)
                {
                    index++;
                    if (index == 1) continue;
                    if (string.IsNullOrEmpty(info)) continue;
                    var value = info.Split(',');

                    var cardEntitySNInfo = new CardEntitySNInfo();

                    cardEntitySNInfo.PublishmentSystemID = PublishmentSystemID;
                    cardEntitySNInfo.CardID = cardID;
                    cardEntitySNInfo.SN = value[0];
                    cardEntitySNInfo.UserName = value[1];
                    cardEntitySNInfo.Mobile = value[2];
                    cardEntitySNInfo.Email = value[3];
                    cardEntitySNInfo.Address = value[4];
                    cardEntitySNInfo.Amount = TranslateUtils.ToDecimal(value[5]);
                    cardEntitySNInfo.Credits = TranslateUtils.ToInt(value[6]);
                    cardEntitySNInfo.IsBinding = false;
                    cardEntitySNInfo.AddDate = DateTime.Now;
                    cardEntitySNInfoArrayList.Add(cardEntitySNInfo);
                 }
             }
          return cardEntitySNInfoArrayList;
        }
   }

}
