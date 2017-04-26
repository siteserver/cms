using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class CardAttribute
    {
        protected CardAttribute()
        {
        }

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string KeywordID = "KeywordID";
        public const string UserCount = "UserCount";
        public const string PVCount = "PVCount";
        public const string IsDisabled = "IsDisabled";
        public const string Title = "Title";
        public const string ImageUrl = "ImageUrl";
        public const string Summary = "Summary"; 
        public const string CardTitle = "CardTitle";
        public const string CardTitleColor = "CardTitleColor";
        public const string CardNoColor = "CardNoColor";
        public const string ContentFrontImageUrl = "ContentFrontImageUrl";
        public const string ContentBackImageUrl = "ContentBackImageUrl";
        public const string ShopName = "ShopName";
        public const string ShopAddress = "ShopAddress";
        public const string ShopTel = "ShopTel";
        public const string ShopPosition = "ShopPosition";
        public const string ShopPassword = "ShopPassword";
        public const string ShopOperatorList = "ShopOperatorList";
          
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
                    allAttributes.Add(KeywordID);
                    allAttributes.Add(UserCount);
                    allAttributes.Add(PVCount);
                    allAttributes.Add(IsDisabled);
                    allAttributes.Add(Title);
                    allAttributes.Add(ImageUrl);
                    allAttributes.Add(Summary);
                    allAttributes.Add(CardTitle);
                    allAttributes.Add(CardTitleColor);
                    allAttributes.Add(CardNoColor);
                    allAttributes.Add(ContentFrontImageUrl);
                    allAttributes.Add(ContentBackImageUrl);
                    allAttributes.Add(ShopName);
                    allAttributes.Add(ShopAddress);
                    allAttributes.Add(ShopTel);
                    allAttributes.Add(ShopPosition);
                    allAttributes.Add(ShopPassword);
                    allAttributes.Add(ShopOperatorList);
                }

                return allAttributes;
            }
        }
    }
    public class CardInfo : BaseInfo
    {
        public CardInfo() { }
        public CardInfo(object dataItem) : base(dataItem) { }
        public CardInfo(NameValueCollection form) : base(form) { }
        public CardInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public int KeywordID { get; set; }
        public int UserCount { get;set;}
        public int PVCount { get; set; }
        public bool IsDisabled { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Summary { get; set; }
        public string CardTitle { get; set; }
        public string CardTitleColor { get; set; }
        public string CardNoColor { get; set; }
        public string ContentFrontImageUrl { get; set; }
        public string ContentBackImageUrl { get; set; }
        public string ShopName { get; set; }
        public string ShopAddress { get; set; }
        public string ShopTel { get; set; }
        public string ShopPosition { get; set; }
        public string ShopPassword { get; set; }
        public string ShopOperatorList { get; set; }
        protected override List<string> AllAttributes
        {
            get
            {
                return CardAttribute.AllAttributes;
            }
        }
    }
}
