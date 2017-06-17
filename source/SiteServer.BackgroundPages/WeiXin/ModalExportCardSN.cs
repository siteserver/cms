using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalExportCardSn : BasePageCms
    {
        public static string GetOpenWindowString(int publishmentSystemId, int cardId)
        {
            return PageUtils.GetOpenWindowString("导出CSV",
                PageUtils.GetWeiXinUrl(nameof(ModalExportCardSn), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"cardId", cardId.ToString()}
                }), 400, 240, true);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            { 
                var cardId = Body.GetQueryInt("cardID");

                var cardSnInfoList = DataProviderWx.CardSnDao.GetCardSnInfoList(PublishmentSystemId, cardId);

                if (cardSnInfoList.Count == 0)
                {
                    FailMessage("暂无数据导出！");
                    return;
                }

                var docFileName = "会员卡名单" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                var filePath = PathUtils.GetTemporaryFilesPath(docFileName);

                ExportCardSncsv(filePath, cardSnInfoList);

                var fileUrl = PageUtils.GetTemporaryFilesUrl(docFileName);
                SuccessMessage($@"成功导出文件，请点击 <a href=""{fileUrl}"">这里</a> 进行下载！");
            }
        }
 
        public void ExportCardSncsv(string filePath, List<CardSnInfo> cardSnInfoList)
        {
            var head = new List<string>();
            head.Add("序号");
            head.Add("卡号");
            head.Add("姓名");
            head.Add("手机");
            head.Add("邮箱");
            head.Add("地址");
            head.Add("金额");
            head.Add("积分");
            head.Add("领卡时间");
            
            //var rows = new List<List<string>>();

            //var index = 1;
            //foreach (var cardSnInfo in cardSnInfoList)
            //{
            //    var userInfo = BaiRongDataProvider.UserDao.GetUserInfoByUserName(cardSnInfo.UserName);
            //    var userContactInfo = BaiRongDataProvider.UserContactDao.GetContactInfo(cardSnInfo.UserName);

            //    var row = new List<string>();

            //    row.Add((index++).ToString());
            //    row.Add(cardSnInfo.Sn);
            //    row.Add(userInfo != null ? userInfo.DisplayName : string.Empty);
            //    row.Add(userInfo != null ? userInfo.Mobile : string.Empty);
            //    row.Add(userInfo != null ? userInfo.Email : string.Empty);
            //    row.Add(userContactInfo != null ? userContactInfo.Address : string.Empty);
            //    row.Add(cardSnInfo.Amount.ToString());
            //    row.Add(userInfo != null ? userInfo.Credits.ToString() : "0");
            //    row.Add(DateUtils.GetDateAndTimeString(cardSnInfo.AddDate));
            //    rows.Add(row);   
            //}

            //CsvUtils.Export(filePath, head, rows);   
        }
    }
}