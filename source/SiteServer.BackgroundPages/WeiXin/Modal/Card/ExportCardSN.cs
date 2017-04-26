using System;
using System.Collections.Specialized;
using BaiRong.Core;
using SiteServer.CMS.BackgroundPages;
using System.Collections.Generic;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;
using BaiRong.Core.IO;

namespace SiteServer.WeiXin.BackgroundPages.Modal
{
    public class ExportCardSN : BackgroundBasePage
    {
        public static string GetOpenWindowString(int publishmentSystemID, int cardID)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            arguments.Add("cardID", cardID.ToString());

            return JsUtils.OpenWindow.GetOpenWindowString("导出CSV", "modal_exportCardSN.aspx", arguments, 400, 240, true);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            { 
                var cardID = TranslateUtils.ToInt(GetQueryString("cardID"));

                var cardSNInfoList = DataProviderWX.CardSNDAO.GetCardSNInfoList(PublishmentSystemID, cardID);

                if (cardSNInfoList.Count == 0)
                {
                    FailMessage("暂无数据导出！");
                    return;
                }

                var docFileName = "会员卡名单" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                var filePath = PathUtils.GetTemporaryFilesPath(docFileName);

                ExportCardSNCSV(filePath, cardSNInfoList);

                var fileUrl = PageUtils.GetTemporaryFilesUrl(docFileName);
                SuccessMessage($@"成功导出文件，请点击 <a href=""{fileUrl}"">这里</a> 进行下载！");
            }
        }
 
        public void ExportCardSNCSV(string filePath, List<CardSNInfo> cardSNInfoList)
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
            
            var rows = new List<List<string>>();

            var index = 1;
            foreach (var cardSNInfo in cardSNInfoList)
            {
                var userInfo = BaiRongDataProvider.UserDao.GetUserInfoByUserName(cardSNInfo.UserName);
                var userContactInfo = BaiRongDataProvider.UserContactDao.GetContactInfo(cardSNInfo.UserName);

                var row = new List<string>();

                row.Add((index++).ToString());
                row.Add(cardSNInfo.SN);
                row.Add(userInfo != null ? userInfo.DisplayName : string.Empty);
                row.Add(userInfo != null ? userInfo.Mobile : string.Empty);
                row.Add(userInfo != null ? userInfo.Email : string.Empty);
                row.Add(userContactInfo != null ? userContactInfo.Address : string.Empty);
                row.Add(cardSNInfo.Amount.ToString());
                row.Add(userInfo != null ? userInfo.Credits.ToString() : "0");
                row.Add(DateUtils.GetDateAndTimeString(cardSNInfo.AddDate));
                rows.Add(row);   
            }

            CSVUtils.Export(filePath, head, rows);   
        }
    }
}