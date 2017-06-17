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
 
        public const string UserName = nameof(CardOperatorInfo.UserName);
        public const string Password = nameof(CardOperatorInfo.Password);
      
        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            UserName,
            Password
        });
    }

    public class CardOperatorInfo : BaseInfo
    {
        public CardOperatorInfo() { }
        public CardOperatorInfo(object dataItem) : base(dataItem) { }
        public CardOperatorInfo(NameValueCollection form) : base(form) { }
        public CardOperatorInfo(IDataReader rdr) : base(rdr) { }
       
        public string UserName { get; set; }
        public string Password { get; set; }

        protected override List<string> AllAttributes => CardOperatorAttribute.AllAttributes;
    }
}
