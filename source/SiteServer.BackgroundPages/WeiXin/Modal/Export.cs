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
    public class Export : BackgroundBasePage
    {
        public static string GetOpenWindowStringByLottery(int publishmentSystemID, ELotteryType lotteryType, int lotteryID)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            arguments.Add("lotteryType", ELotteryTypeUtils.GetValue(lotteryType));
            arguments.Add("lotteryID", lotteryID.ToString());

            return JsUtils.OpenWindow.GetOpenWindowString("导出CSV", "modal_export.aspx", arguments, 400, 240, true);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var lotteryType = ELotteryTypeUtils.GetEnumType(GetQueryString("lotteryType"));
                var lotteryID = TranslateUtils.ToInt(GetQueryString("lotteryID"));

                var winnerInfoList = DataProviderWX.LotteryWinnerDAO.GetWinnerInfoList(PublishmentSystemID, lotteryType, lotteryID);

                if (winnerInfoList.Count == 0)
                {
                    FailMessage("暂无数据导出！");
                    return;
                }

                var docFileName = "获奖名单" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                var filePath = PathUtils.GetTemporaryFilesPath(docFileName);

                ExportLotteryCSV(filePath, winnerInfoList);

                var fileUrl = PageUtils.GetTemporaryFilesUrl(docFileName);
                SuccessMessage($@"成功导出文件，请点击 <a href=""{fileUrl}"">这里</a> 进行下载！");
            }
        }


        private Dictionary<int, LotteryAwardInfo> awardInfoMap = new Dictionary<int, LotteryAwardInfo>();

        public void ExportLotteryCSV(string filePath, List<LotteryWinnerInfo> winnerInfoList)
        {
            var head = new List<string>();
            head.Add("序号");
            head.Add("奖项");
            head.Add("姓名");
            head.Add("手机");
            head.Add("邮箱");
            head.Add("地址");
            head.Add("状态");
            head.Add("中奖时间");
            head.Add("兑奖码");
            head.Add("兑奖时间");

            var rows = new List<List<string>>();

            var index = 1;
            foreach (var winnerInfo in winnerInfoList)
            {
                LotteryAwardInfo awardInfo = null;
                if (awardInfoMap.ContainsKey(winnerInfo.AwardID))
                {
                    awardInfo = awardInfoMap[winnerInfo.AwardID];
                }
                else
                {
                    awardInfo = DataProviderWX.LotteryAwardDAO.GetAwardInfo(winnerInfo.AwardID);
                    awardInfoMap.Add(winnerInfo.AwardID, awardInfo);
                }

                var award = string.Empty;
                if (awardInfo != null)
                {
                    award = awardInfo.AwardName + "：" + awardInfo.Title;
                }

                var row = new List<string>();

                row.Add((index++).ToString());
                row.Add(award);
                row.Add(winnerInfo.RealName);
                row.Add(winnerInfo.Mobile);
                row.Add(winnerInfo.Email);
                row.Add(winnerInfo.Address);
                row.Add(EWinStatusUtils.GetText(EWinStatusUtils.GetEnumType(winnerInfo.Status)));
                row.Add(DateUtils.GetDateAndTimeString(winnerInfo.AddDate));
                row.Add(winnerInfo.CashSN);
                row.Add(DateUtils.GetDateAndTimeString(winnerInfo.CashDate));

                rows.Add(row);
            }

            CSVUtils.Export(filePath, head, rows);   
        }
    }
}