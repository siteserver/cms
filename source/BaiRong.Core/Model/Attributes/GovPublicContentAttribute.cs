using System.Collections.Generic;

namespace BaiRong.Core.Model.Attributes
{
	public class GovPublicContentAttribute
	{
        protected GovPublicContentAttribute()
		{
		}

        //system
        public const string ImageUrl = "ImageUrl";                //缩略图
        public const string FileUrl = "FileUrl";                //附件
        public const string Content = "Content";                //内容

        //hidden
        public const string Identifier = "Identifier";              //索引号
        public const string Description = "Description";            //内容概述
        public const string PublishDate = "PublishDate";            //发文日期
        public const string EffectDate = "EffectDate";               //生效日期
        public const string IsAbolition = "IsAbolition";            //是否废止
        public const string AbolitionDate = "AbolitionDate";        //废止日期
        public const string DocumentNo = "DocumentNo";                  //文号
        public const string Publisher = "Publisher";          //发布机构
        public const string Keywords = "Keywords";              //主题词
        public const string DepartmentId = "DepartmentID";      //部门ID
        public const string Category1Id = "Category1ID";
        public const string Category2Id = "Category2ID";
        public const string Category3Id = "Category3ID";
        public const string Category4Id = "Category4ID";
        public const string Category5Id = "Category5ID";
        public const string Category6Id = "Category6ID";

        public const string IsRecommend = "IsRecommend";
        public const string IsHot = "IsHot";
        public const string IsColor = "IsColor";

        public static List<string> AllAttributes
        {
            get
            {
                var arraylist = new List<string>(ContentAttribute.AllAttributes);
                arraylist.AddRange(SystemAttributes);
                return arraylist;
            }
        }

        private static List<string> _systemAttributes;
        public static List<string> SystemAttributes => _systemAttributes ?? (_systemAttributes = new List<string>
        {
            ImageUrl.ToLower(),
            FileUrl.ToLower(),
            Content.ToLower(),
            Identifier.ToLower(),
            Description.ToLower(),
            PublishDate.ToLower(),
            EffectDate.ToLower(),
            IsAbolition.ToLower(),
            AbolitionDate.ToLower(),
            DocumentNo.ToLower(),
            Publisher.ToLower(),
            Keywords.ToLower(),
            DepartmentId.ToLower(),
            Category1Id.ToLower(),
            Category2Id.ToLower(),
            Category3Id.ToLower(),
            Category4Id.ToLower(),
            Category5Id.ToLower(),
            Category6Id.ToLower(),
            IsRecommend.ToLower(),
            IsHot.ToLower(),
            IsColor.ToLower()
        });

	    private static List<string> _excludeAttributes;
        public static List<string> ExcludeAttributes => _excludeAttributes ?? (_excludeAttributes = new List<string>(ContentAttribute.ExcludeAttributes)
        {
            ContentAttribute.Title.ToLower(),
            Identifier.ToLower(),
            Description.ToLower(),
            PublishDate.ToLower(),
            EffectDate.ToLower(),
            IsAbolition.ToLower(),
            AbolitionDate.ToLower(),
            DocumentNo.ToLower(),
            Publisher.ToLower(),
            Keywords.ToLower(),
            DepartmentId.ToLower(),
            Category1Id.ToLower(),
            Category2Id.ToLower(),
            Category3Id.ToLower(),
            Category4Id.ToLower(),
            Category5Id.ToLower(),
            Category6Id.ToLower(),
            IsRecommend.ToLower(),
            IsHot.ToLower(),
            IsColor.ToLower()
        });

	    private static List<string> _checkBoxAttributes;
        public static List<string> CheckBoxAttributes => _checkBoxAttributes ?? (_checkBoxAttributes = new List<string>
        {
            ContentAttribute.IsTop.ToLower(),
            IsRecommend.ToLower(),
            IsHot.ToLower(),
            IsColor.ToLower()
        });
	}
}
