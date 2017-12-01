using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using BaiRong.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalCardEntitySnImport : BasePageCms
    { 
        public HtmlInputFile HifUpload;

        private int _cardId;
       
        public static string GetOpenUploadWindowString(int publishmentSystemId, int cardId )
        {
            return PageUtils.GetOpenWindowString("导入实体卡",
                PageUtils.GetWeiXinUrl(nameof(ModalCardEntitySnImport), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"cardId", cardId.ToString()}
                }), 400, 300);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _cardId = Body.GetQueryInt("cardID");
           
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
                HifUpload.PostedFile.SaveAs(filePath);

                try
                { 
                    var cardEntitySnInfoArrayList = GetCardEntitySnInfoArrayList(filePath);
                    if (cardEntitySnInfoArrayList.Count > 0)
                    {
                        isChanged = true;
                        var errorMessage = string.Empty;
                        for (var i = 0; i < cardEntitySnInfoArrayList.Count; i++)
                        {
                            var cardEntitySnInfo = cardEntitySnInfoArrayList[i] as CardEntitySnInfo;

                            var isExist= DataProviderWx.CardEntitySnDao.IsExist(PublishmentSystemId,_cardId,cardEntitySnInfo.Sn);
                            if (!isExist)
                            {
                                DataProviderWx.CardEntitySnDao.Insert(cardEntitySnInfo);
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
                PageUtils.CloseModalPage(Page);
            }
        }

        public ArrayList GetCardEntitySnInfoArrayList(string filePath)
        {
            var index = 0;
            var cardEntitySnInfoArrayList = new ArrayList();

            //var cardEntitySnInfoList = CsvUtils.Import(filePath);
            //if (cardEntitySnInfoList.Count > 0)
            //{
            //    foreach (var info in cardEntitySnInfoList)
            //    {
            //        index++;
            //        if (index == 1) continue;
            //        if (string.IsNullOrEmpty(info)) continue;
            //        var value = info.Split(',');

            //        var cardEntitySnInfo = new CardEntitySnInfo();

            //        cardEntitySnInfo.PublishmentSystemId = PublishmentSystemId;
            //        cardEntitySnInfo.CardId = _cardId;
            //        cardEntitySnInfo.Sn = value[0];
            //        cardEntitySnInfo.UserName = value[1];
            //        cardEntitySnInfo.Mobile = value[2];
            //        cardEntitySnInfo.Email = value[3];
            //        cardEntitySnInfo.Address = value[4];
            //        cardEntitySnInfo.Amount = TranslateUtils.ToDecimal(value[5]);
            //        cardEntitySnInfo.Credits = TranslateUtils.ToInt(value[6]);
            //        cardEntitySnInfo.IsBinding = false;
            //        cardEntitySnInfo.AddDate = DateTime.Now;
            //        cardEntitySnInfoArrayList.Add(cardEntitySnInfo);
            //     }
            // }
          return cardEntitySnInfoArrayList;
        }
   }

}
