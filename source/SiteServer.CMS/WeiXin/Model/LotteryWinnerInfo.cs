using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class LotteryWinnerAttribute
    {
        protected LotteryWinnerAttribute()
        {
        }

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string LotteryType = "LotteryType";
        public const string LotteryID = "LotteryID";
        public const string AwardID = "AwardID";
        public const string Status = "Status";
        public const string CookieSN = "CookieSN";
        public const string WXOpenID = "WXOpenID";
        public const string UserName = "UserName";
        public const string RealName = "RealName";
        public const string Mobile = "Mobile";
        public const string Email = "Email";
        public const string Address = "Address";
        public const string AddDate = "AddDate";
        public const string CashSN = "CashSN";
        public const string CashDate = "CashDate";
        

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
                    allAttributes.Add(LotteryType);
                    allAttributes.Add(LotteryID);
                    allAttributes.Add(AwardID);
                    allAttributes.Add(Status);
                    allAttributes.Add(CookieSN);
                    allAttributes.Add(WXOpenID);
                    allAttributes.Add(UserName);
                    allAttributes.Add(RealName);
                    allAttributes.Add(Mobile);
                    allAttributes.Add(Email);
                    allAttributes.Add(Address);
                    allAttributes.Add(AddDate);
                    allAttributes.Add(CashSN);
                    allAttributes.Add(CashDate);
                }

                return allAttributes;
            }
        }
    }
    public class LotteryWinnerInfo : BaseInfo
    {
        public LotteryWinnerInfo() { }
        public LotteryWinnerInfo(object dataItem) : base(dataItem) { }
        public LotteryWinnerInfo(NameValueCollection form) : base(form) { }
        public LotteryWinnerInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public string LotteryType { get; set; }
        public int LotteryID { get; set; }
        public int AwardID { get; set; }
        public string Status { get; set; }
        public string CookieSN { get; set; }
        public string WXOpenID { get; set; }
        public string UserName { get; set; }
        public string RealName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime AddDate { get; set; }
        public string CashSN { get; set; }
        public DateTime CashDate { get; set; }

        protected override List<string> AllAttributes
        {
            get
            {
                return LotteryWinnerAttribute.AllAttributes;
            }
        }
    }
}
