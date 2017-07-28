using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class CardEntitySNAttribute
    {
        protected CardEntitySNAttribute()
        {
        }

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string CardID = "CardID";
        public const string SN = "SN";
        public const string UserName = "UserName";
        public const string Mobile = "Mobile";
        public const string Amount = "Amount";
        public const string Credits = "Credits";
        public const string Email = "Email";
        public const string Address = "Address";
        public const string IsBinding = "IsBinding";
        public const string AddDate = "AddDate";
         
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
                    allAttributes.Add(CardID);
                    allAttributes.Add(SN);
                    allAttributes.Add(UserName);
                    allAttributes.Add(Mobile);
                    allAttributes.Add(Amount);
                    allAttributes.Add(Credits);
                    allAttributes.Add(Email);
                    allAttributes.Add(Address);
                    allAttributes.Add(IsBinding);
                    allAttributes.Add(AddDate);
                     
                 }

                return allAttributes;
            }
        }
    }
    public class CardEntitySNInfo : BaseInfo
    {
        public CardEntitySNInfo() { }
        public CardEntitySNInfo(object dataItem) : base(dataItem) { }
        public CardEntitySNInfo(NameValueCollection form) : base(form) { }
        public CardEntitySNInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public int CardID { get; set; }
        public string SN { get; set; }
        public string UserName { get; set; }
        public string Mobile { get; set; }
        public decimal Amount { get; set; }
        public int Credits { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public bool IsBinding { get; set; }
        public DateTime AddDate { get; set; }
         
        protected override List<string> AllAttributes
        {
            get
            {
                return CardEntitySNAttribute.AllAttributes;
            }
        }
    }
}
