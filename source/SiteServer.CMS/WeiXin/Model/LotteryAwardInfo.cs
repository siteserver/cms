using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class LotteryAwardAttribute
    {
        protected LotteryAwardAttribute()
        {
        }

        public const string Id = nameof(LotteryAwardInfo.Id);
        public const string PublishmentSystemId = nameof(LotteryAwardInfo.PublishmentSystemId);
        public const string LotteryId = nameof(LotteryAwardInfo.LotteryId);
        public const string AwardName = nameof(LotteryAwardInfo.AwardName);
        public const string Title = nameof(LotteryAwardInfo.Title);
        public const string TotalNum = nameof(LotteryAwardInfo.TotalNum);
        public const string Probability = nameof(LotteryAwardInfo.Probability);
        public const string WonNum = nameof(LotteryAwardInfo.WonNum);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            LotteryId,
            AwardName,
            Title,
            TotalNum,
            Probability,
            WonNum
        });
    }

    public class LotteryAwardInfo : BaseInfo
    {
        public LotteryAwardInfo() { }
        public LotteryAwardInfo(object dataItem) : base(dataItem) { }
        public LotteryAwardInfo(NameValueCollection form) : base(form) { }
        public LotteryAwardInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public int LotteryId { get; set; }
        public string AwardName { get; set; }
        public string Title { get; set; }
        public int TotalNum { get; set; }
        public decimal Probability { get; set; }
        public int WonNum { get; set; }

        protected override List<string> AllAttributes => LotteryAwardAttribute.AllAttributes;
    }
}
