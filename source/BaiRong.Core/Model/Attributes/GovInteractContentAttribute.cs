using System.Collections.Generic;

namespace BaiRong.Core.Model.Attributes
{
    public class GovInteractContentAttribute
    {
        protected GovInteractContentAttribute()
        {
        }

        //hidden
        public const string DepartmentName = "DepartmentName";
        public const string QueryCode = "QueryCode";
        public const string State = "State";
        public const string IpAddress = "IPAddress";
        public const string IsRecommend = "IsRecommend";        //是否推荐

        //basic
        public const string RealName = "RealName";
        public const string Organization = "Organization";
        public const string CardType = "CardType";
        public const string CardNo = "CardNo";
        public const string Phone = "Phone";
        public const string PostCode = "PostCode";
        public const string Address = "Address";
        public const string Email = "Email";
        public const string Fax = "Fax";

        public const string TypeId = "TypeID";
        public const string IsPublic = "IsPublic";
        public const string Title = "Title";
        public const string Content = "Content";
        public const string FileUrl = "FileUrl";
        public const string DepartmentId = "DepartmentID";

        //extend
        public const string TranslateFromNodeId = "TranslateFromNodeID";

        //不存在
        public const string Reply = "Reply";
        public const string ReplyDepartment = "ReplyDepartment";
        public const string ReplyUserName = "ReplyUserName";
        public const string ReplyDate = "ReplyDate";
        public const string ReplyFileUrl = "ReplyFileUrl";
        public const string NavigationUrl = "NavigationUrl";
        public const string CountOfPhotos = "CountOfPhotos";			//商品图片数

        public static List<string> AllAttributes
        {
            get
            {
                var arraylist = new List<string>();
                arraylist.AddRange(HiddenAttributes);
                arraylist.AddRange(SystemAttributes);
                return arraylist;
            }
        }

        private static List<string> _hiddenAttributes;
        public static List<string> HiddenAttributes => _hiddenAttributes ?? (_hiddenAttributes = new List<string>(ContentAttribute.HiddenAttributes)
        {
            DepartmentName.ToLower(),
            QueryCode,
            State.ToLower(),
            IpAddress.ToLower()
        });

        private static List<string> _systemAttributes;
        public static List<string> SystemAttributes => _systemAttributes ?? (_systemAttributes = new List<string>(ContentAttribute.SystemAttributes)
        {
            RealName.ToLower(),
            Organization.ToLower(),
            CardType.ToLower(),
            CardNo.ToLower(),
            Phone.ToLower(),
            PostCode.ToLower(),
            Address.ToLower(),
            Email.ToLower(),
            Fax.ToLower(),
            TypeId.ToLower(),
            IsPublic.ToLower(),
            Title.ToLower(),
            Content.ToLower(),
            FileUrl.ToLower(),
            DepartmentId.ToLower()
        });

        private static List<string> _excludeAttributes;
        public static List<string> ExcludeAttributes => _excludeAttributes ?? (_excludeAttributes = new List<string>());
    }
}
