using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class LotteryAttribute
    {
        protected LotteryAttribute()
        {
        }

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string LotteryType = "LotteryType";
        public const string KeywordID = "KeywordID";
        public const string IsDisabled = "IsDisabled";
        public const string UserCount = "UserCount";
        public const string PVCount = "PVCount";
        public const string StartDate = "StartDate";
        public const string EndDate = "EndDate";
        public const string Title = "Title";
        public const string ImageUrl = "ImageUrl";
        public const string Summary = "Summary";
        public const string ContentImageUrl = "ContentImageUrl";
        public const string ContentAwardImageUrl = "ContentAwardImageUrl";
        public const string ContentUsage = "ContentUsage";
        public const string AwardImageUrl = "AwardImageUrl";
        public const string AwardUsage = "AwardUsage";
        public const string IsAwardTotalNum = "IsAwardTotalNum";
        public const string AwardMaxCount = "AwardMaxCount";
        public const string AwardMaxDailyCount = "AwardMaxDailyCount";
        public const string AwardCode = "AwardCode";
        public const string IsFormRealName = "IsFormRealName";
        public const string FormRealNameTitle = "FormRealNameTitle";
        public const string IsFormMobile = "IsFormMobile";
        public const string FormMobileTitle = "FormMobileTitle";
        public const string IsFormEmail = "IsFormEmail";
        public const string FormEmailTitle = "FormEmailTitle";
        public const string IsFormAddress = "IsFormAddress";
        public const string FormAddressTitle = "FormAddressTitle";
        public const string EndTitle = "EndTitle";
        public const string EndImageUrl = "EndImageUrl";
        public const string EndSummary = "EndSummary";

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
                    allAttributes.Add(KeywordID);
                    allAttributes.Add(IsDisabled);
                    allAttributes.Add(UserCount);
                    allAttributes.Add(PVCount);
                    allAttributes.Add(StartDate);
                    allAttributes.Add(EndDate);
                    allAttributes.Add(Title);
                    allAttributes.Add(ImageUrl);
                    allAttributes.Add(Summary);
                    allAttributes.Add(ContentImageUrl);
                    allAttributes.Add(ContentAwardImageUrl);
                    allAttributes.Add(ContentUsage);
                    allAttributes.Add(AwardImageUrl);
                    allAttributes.Add(AwardUsage);
                    allAttributes.Add(IsAwardTotalNum);
                    allAttributes.Add(AwardMaxCount);
                    allAttributes.Add(AwardMaxDailyCount);
                    allAttributes.Add(AwardCode);
                    allAttributes.Add(IsFormRealName);
                    allAttributes.Add(FormRealNameTitle);
                    allAttributes.Add(IsFormMobile);
                    allAttributes.Add(FormMobileTitle);
                    allAttributes.Add(IsFormEmail);
                    allAttributes.Add(FormEmailTitle);
                    allAttributes.Add(IsFormAddress);
                    allAttributes.Add(FormAddressTitle);
                    allAttributes.Add(EndTitle);
                    allAttributes.Add(EndImageUrl);
                    allAttributes.Add(EndSummary);
                }

                return allAttributes;
            }
        }
    }
    public class LotteryInfo : BaseInfo
    {
        public LotteryInfo() { }
        public LotteryInfo(object dataItem) : base(dataItem) { }
        public LotteryInfo(NameValueCollection form) : base(form) { }
        public LotteryInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public string LotteryType { get; set; }
        public int KeywordID { get; set; }
        public bool IsDisabled { get; set; }
        public int UserCount { get; set; }
        public int PVCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Summary { get; set; }
        public string ContentImageUrl { get; set; }
        public string ContentAwardImageUrl { get; set; }
        public string ContentUsage { get; set; }
        public string AwardImageUrl { get; set; }
        public string AwardUsage { get; set; }
        public bool IsAwardTotalNum { get; set; }
        public int AwardMaxCount { get; set; }
        public int AwardMaxDailyCount { get; set; }
        public string AwardCode { get; set; }
        public bool IsFormRealName { get; set; }
        public string FormRealNameTitle { get; set; }
        public bool IsFormMobile { get; set; }
        public string FormMobileTitle { get; set; }
        public bool IsFormEmail { get; set; }
        public string FormEmailTitle { get; set; }
        public bool IsFormAddress { get; set; }
        public string FormAddressTitle { get; set; }
        public string EndTitle { get; set; }
        public string EndImageUrl { get; set; }
        public string EndSummary { get; set; }

        protected override List<string> AllAttributes
        {
            get
            {
                return LotteryAttribute.AllAttributes;
            }
        }
    }
}
