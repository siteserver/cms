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

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string LotteryID = "LotteryID";
        public const string AwardName = "AwardName";
        public const string Title = "Title";
        public const string TotalNum = "TotalNum";
        public const string Probability = "Probability";
        public const string WonNum = "WonNum";

        private static List<string> allAttributes;
        public static List<string> AllAttributes
        {
            get
            {
                if (allAttributes == null)
                {
                    allAttributes = new List<string>();
                    allAttributes.Add(ID);
                    allAttributes.Add(PublishmentSystemID);
                    allAttributes.Add(LotteryID);
                    allAttributes.Add(AwardName);
                    allAttributes.Add(Title);
                    allAttributes.Add(TotalNum);
                    allAttributes.Add(Probability);
                    allAttributes.Add(WonNum);
                }

                return allAttributes;
            }
        }
    }
    public class LotteryAwardInfo : BaseInfo
    {
        public LotteryAwardInfo() { }
        public LotteryAwardInfo(object dataItem) : base(dataItem) { }
        public LotteryAwardInfo(NameValueCollection form) : base(form) { }
        public LotteryAwardInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public int LotteryID { get; set; }
        public string AwardName { get; set; }
        public string Title { get; set; }
        public int TotalNum { get; set; }
        public decimal Probability { get; set; }
        public int WonNum { get; set; }

        protected override List<string> AllAttributes
        {
            get
            {
                return LotteryAwardAttribute.AllAttributes;
            }
        }
    }
}
