using System.Collections;
using System.Collections.Generic;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;

namespace SiteServer.CMS.Model
{
	public class BackgroundContentInfo : ContentInfo
	{
		public BackgroundContentInfo()
		{
            SubTitle = string.Empty;
            ImageUrl = string.Empty;
            VideoUrl = string.Empty;
            FileUrl = string.Empty;
            LinkUrl = string.Empty;
            Summary = string.Empty;
            Author = string.Empty;
            Source = string.Empty;
            IsRecommend = false;
            IsHot = false;
            IsColor = false;
            IsTop = false;
            Content = string.Empty;          
		}

        public BackgroundContentInfo(object dataItem)
            : base(dataItem)
		{
		}

        public string SubTitle
		{
            get { return GetExtendedAttribute(BackgroundContentAttribute.SubTitle); }
            set { SetExtendedAttribute(BackgroundContentAttribute.SubTitle, value); }
		}

        public string ImageUrl
		{
            get { return GetExtendedAttribute(BackgroundContentAttribute.ImageUrl); }
            set { SetExtendedAttribute(BackgroundContentAttribute.ImageUrl, value); }
		}

        public string VideoUrl
        {
            get { return GetExtendedAttribute(BackgroundContentAttribute.VideoUrl); }
            set { SetExtendedAttribute(BackgroundContentAttribute.VideoUrl, value); }
        }

        public string FileUrl
        {
            get { return GetExtendedAttribute(BackgroundContentAttribute.FileUrl); }
            set { SetExtendedAttribute(BackgroundContentAttribute.FileUrl, value); }
        }

        public string LinkUrl
		{
            get { return GetExtendedAttribute(BackgroundContentAttribute.LinkUrl); }
            set { SetExtendedAttribute(BackgroundContentAttribute.LinkUrl, value); }
		}

        public string Summary
        {
            get { return GetExtendedAttribute(BackgroundContentAttribute.Summary); }
            set { SetExtendedAttribute(BackgroundContentAttribute.Summary, value); }
        }

        public string Author
		{
            get { return GetExtendedAttribute(BackgroundContentAttribute.Author); }
            set { SetExtendedAttribute(BackgroundContentAttribute.Author, value); }
		}

        public string Source
		{
            get { return GetExtendedAttribute(BackgroundContentAttribute.Source); }
            set { SetExtendedAttribute(BackgroundContentAttribute.Source, value); }
		}

        public bool IsRecommend
		{
            get { return GetBool(BackgroundContentAttribute.IsRecommend, false); }
            set { SetExtendedAttribute(BackgroundContentAttribute.IsRecommend, value.ToString()); }
		}

        public bool IsHot
		{
            get { return GetBool(BackgroundContentAttribute.IsHot, false); }
            set { SetExtendedAttribute(BackgroundContentAttribute.IsHot, value.ToString()); }
		}

        public bool IsColor
		{
            get { return GetBool(BackgroundContentAttribute.IsColor, false); }
            set { SetExtendedAttribute(BackgroundContentAttribute.IsColor, value.ToString()); }
		}

        public string Content
		{
            get { return GetExtendedAttribute(BackgroundContentAttribute.Content); }
            set { SetExtendedAttribute(BackgroundContentAttribute.Content, value); }
		}

        public override List<string> GetDefaultAttributesNames()
        {
            return BackgroundContentAttribute.AllAttributes;
        }
	}
}
