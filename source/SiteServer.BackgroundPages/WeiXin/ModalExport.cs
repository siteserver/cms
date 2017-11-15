using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core;
using BaiRong.Core.IO;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalExport : BasePageCms
    {
        public static string GetOpenWindowStringByLottery(int publishmentSystemId, ELotteryType lotteryType, int lotteryId)
        {
            return PageUtils.GetOpenWindowString("导出CSV",
                PageUtils.GetWeiXinUrl(nameof(ModalExport), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"lotteryType", ELotteryTypeUtils.GetValue(lotteryType)},
                    {"lotteryId", lotteryId.ToString()}
                }), 400, 240, true);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var lotteryType = ELotteryTypeUtils.GetEnumType(Body.GetQueryString("lotteryType"));
                var lotteryId = Body.GetQueryInt("lotteryID");

                var winnerInfoList = DataProviderWx.LotteryWinnerDao.GetWinnerInfoList(PublishmentSystemId, lotteryType, lotteryId);

                if (winnerInfoList.Count == 0)
                {
                    FailMessage("暂无数据导出！");
                    return;
                }

                var docFileName = "获奖名单" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                var filePath = PathUtils.GetTemporaryFilesPath(docFileName);

                ExportLotteryCsv(filePath, winnerInfoList);

                var fileUrl = PageUtils.GetTemporaryFilesUrl(docFileName);
                SuccessMessage($@"成功导出文件，请点击 <a href=""{fileUrl}"">这里</a> 进行下载！");
            }
        }


        private Dictionary<int, LotteryAwardInfo> _awardInfoMap = new Dictionary<int, LotteryAwardInfo>();

        public void ExportLotteryCsv(string filePath, List<LotteryWinnerInfo> winnerInfoList)
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
                if (_awardInfoMap.ContainsKey(winnerInfo.AwardId))
                {
                    awardInfo = _awardInfoMap[winnerInfo.AwardId];
                }
                else
                {
                    awardInfo = DataProviderWx.LotteryAwardDao.GetAwardInfo(winnerInfo.AwardId);
                    _awardInfoMap.Add(winnerInfo.AwardId, awardInfo);
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
                row.Add(winnerInfo.CashSn);
                row.Add(DateUtils.GetDateAndTimeString(winnerInfo.CashDate));

                rows.Add(row);
            }

            CsvUtils.Export(filePath, head, rows);   
        }
    }
}