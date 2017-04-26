using System;
using System.Collections.Generic;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;

namespace SiteServer.CMS.Wcm.Model
{
	public class GovPublicContentInfo : ContentInfo
	{
		public GovPublicContentInfo()
		{
            Identifier = string.Empty;
            Description = string.Empty;
            PublishDate = DateTime.Now;
            EffectDate = DateTime.Now;
            IsAbolition = false;
            AbolitionDate = DateTime.Now;
            DocumentNo = string.Empty;
            Publisher = string.Empty;
            Keywords = string.Empty;
            ImageUrl = string.Empty;
            FileUrl = string.Empty;
            IsRecommend = false;
            IsHot = false;
            IsColor = false;
            Content = string.Empty;
            DepartmentId = 0;
            Category1Id = 0;
            Category2Id = 0;
            Category3Id = 0;
            Category4Id = 0;
            Category5Id = 0;
            Category6Id = 0;
		}

        public GovPublicContentInfo(object dataItem)
            : base(dataItem)
		{
		}

        public string Identifier
		{
            get { return GetExtendedAttribute(GovPublicContentAttribute.Identifier); }
            set { SetExtendedAttribute(GovPublicContentAttribute.Identifier, value); }
		}

        public string Description
		{
            get { return GetExtendedAttribute(GovPublicContentAttribute.Description); }
            set { SetExtendedAttribute(GovPublicContentAttribute.Description, value); }
		}

        public DateTime PublishDate
        {
            get { return GetDateTime(GovPublicContentAttribute.PublishDate, DateTime.Now); }
            set { SetExtendedAttribute(GovPublicContentAttribute.PublishDate, DateUtils.GetDateAndTimeString(value)); }
        }

        public DateTime EffectDate
        {
            get { return GetDateTime(GovPublicContentAttribute.EffectDate, DateTime.Now); }
            set { SetExtendedAttribute(GovPublicContentAttribute.EffectDate, DateUtils.GetDateAndTimeString(value)); }
        }

        public bool IsAbolition
        {
            get { return GetBool(GovPublicContentAttribute.IsAbolition, false); }
            set { SetExtendedAttribute(GovPublicContentAttribute.IsAbolition, value.ToString()); }
        }

        public DateTime AbolitionDate
        {
            get { return GetDateTime(GovPublicContentAttribute.AbolitionDate, DateTime.Now); }
            set { SetExtendedAttribute(GovPublicContentAttribute.AbolitionDate, DateUtils.GetDateAndTimeString(value)); }
        }

        public string DocumentNo
		{
            get { return GetExtendedAttribute(GovPublicContentAttribute.DocumentNo); }
            set { SetExtendedAttribute(GovPublicContentAttribute.DocumentNo, value); }
		}

        public string Publisher
        {
            get { return GetExtendedAttribute(GovPublicContentAttribute.Publisher); }
            set { SetExtendedAttribute(GovPublicContentAttribute.Publisher, value); }
        }

        public string Keywords
        {
            get { return GetExtendedAttribute(GovPublicContentAttribute.Keywords); }
            set { SetExtendedAttribute(GovPublicContentAttribute.Keywords, value); }
        }

        public string ImageUrl
        {
            get { return GetExtendedAttribute(GovPublicContentAttribute.ImageUrl); }
            set { SetExtendedAttribute(GovPublicContentAttribute.ImageUrl, value); }
        }

        public string FileUrl
        {
            get { return GetExtendedAttribute(GovPublicContentAttribute.FileUrl); }
            set { SetExtendedAttribute(GovPublicContentAttribute.FileUrl, value); }
        }

        public bool IsRecommend
        {
            get { return GetBool(GovPublicContentAttribute.IsRecommend, false); }
            set { SetExtendedAttribute(GovPublicContentAttribute.IsRecommend, value.ToString()); }
        }

        public bool IsHot
        {
            get { return GetBool(GovPublicContentAttribute.IsHot, false); }
            set { SetExtendedAttribute(GovPublicContentAttribute.IsHot, value.ToString()); }
        }

        public bool IsColor
        {
            get { return GetBool(GovPublicContentAttribute.IsColor, false); }
            set { SetExtendedAttribute(GovPublicContentAttribute.IsColor, value.ToString()); }
        }

        public int DepartmentId
        {
            get { return GetInt(GovPublicContentAttribute.DepartmentId, 0); }
            set { SetExtendedAttribute(GovPublicContentAttribute.DepartmentId, value.ToString()); }
        }

        public int Category1Id
        {
            get { return GetInt(GovPublicContentAttribute.Category1Id, 0); }
            set { SetExtendedAttribute(GovPublicContentAttribute.Category1Id, value.ToString()); }
        }

        public int Category2Id
        {
            get { return GetInt(GovPublicContentAttribute.Category2Id, 0); }
            set { SetExtendedAttribute(GovPublicContentAttribute.Category2Id, value.ToString()); }
        }

        public int Category3Id
        {
            get { return GetInt(GovPublicContentAttribute.Category3Id, 0); }
            set { SetExtendedAttribute(GovPublicContentAttribute.Category3Id, value.ToString()); }
        }

        public int Category4Id
        {
            get { return GetInt(GovPublicContentAttribute.Category4Id, 0); }
            set { SetExtendedAttribute(GovPublicContentAttribute.Category4Id, value.ToString()); }
        }

        public int Category5Id
        {
            get { return GetInt(GovPublicContentAttribute.Category5Id, 0); }
            set { SetExtendedAttribute(GovPublicContentAttribute.Category5Id, value.ToString()); }
        }

        public int Category6Id
        {
            get { return GetInt(GovPublicContentAttribute.Category6Id, 0); }
            set { SetExtendedAttribute(GovPublicContentAttribute.Category6Id, value.ToString()); }
        }

        public string Content
        {
            get { return GetExtendedAttribute(GovPublicContentAttribute.Content); }
            set { SetExtendedAttribute(GovPublicContentAttribute.Content, value); }
        }

        public override List<string> GetDefaultAttributesNames()
        {
            return GovPublicContentAttribute.AllAttributes;
        }
	}
}
