using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class CardOperatorAttribute
    {
        protected CardOperatorAttribute()
        {
        }
 
        public const string UserName = "UserName";
        public const string Password = "Password";
        
      
        private static List<string> allAttributes;
        public static List<string> AllAttributes
        {
            get
            {
                if (allAttributes == null)
                {
                    allAttributes = new List<string>();
                   
                    allAttributes.Add(UserName);
                    allAttributes.Add(Password);
                  }

                return allAttributes;
            }
        }
    }
    public class CardOperatorInfo : BaseInfo
    {
        public CardOperatorInfo() { }
        public CardOperatorInfo(object dataItem) : base(dataItem) { }
        public CardOperatorInfo(NameValueCollection form) : base(form) { }
        public CardOperatorInfo(IDataReader rdr) : base(rdr) { }
       
        public string UserName { get; set; }
        public string Password { get; set; }
         
        protected override List<string> AllAttributes
        {
            get
            {
                return CardOperatorAttribute.AllAttributes;
            }
        }
    }
}
